using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Application.Events;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ECommerceApp.Order.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService orderService;
        private ResponseDto response;
        private readonly IEventBus messageBus;
        private readonly IConfiguration configuration;
        private readonly CheckoutsMetrics checkoutsMetrics;

        public OrderAPIController(IOrderService orderService, IEventBus messageBus, IConfiguration configuration, CheckoutsMetrics checkoutsMetrics)
        {
            this.orderService = orderService;
            this.response = new ResponseDto();
            this.messageBus = messageBus;
            this.configuration = configuration;
            this.checkoutsMetrics = checkoutsMetrics;
        }

        [Authorize]
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(CartDto cartDto)
        {
            try
            {
                // Check product availability
                var unavailableProducts = await orderService.CheckProductAvailability(cartDto);
                if (unavailableProducts.Any())
                {
                    response.Result = false;
                    response.Message = $"The following products are unavailable or insufficient in stock: {string.Join(", ", unavailableProducts)}";
                    return BadRequest(response);
                }
                var orderHeaderDto = await orderService.CreateOrder(cartDto);
                response.Result = orderHeaderDto;
                response.Message = "Order was created successfully";
                if (response.Result != null)
                {
                    string token = "";
                    if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                    {
                        token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", "").Trim();
                    }
                    await messageBus.PublishMessageAsync(new OrderCreatedEvent(orderHeaderDto),
                        GetQueueName(configuration.GetValue<string>("MessageBrokerType")), token);
                }
                cartDto.CartDetails?.ToList().ForEach(cartDetail => checkoutsMetrics.IncreaseCheckouts(cartDetail.Product?.Name, cartDetail.Count));
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Result = false;
                response.Message = $"Order was not created. Error: {e.Message}";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("cancelOrder")]
        public async Task<IActionResult> CancelOrder(OrderHeaderDto cartDto)
        {
            try
            {
                var orderDto = await orderService.CancelOrder(cartDto);
                if (orderDto)
                {
                    response.Result = orderDto;
                    response.Message = "Order has been cancelled.";
                    return Ok(response);
                }
                else
                {
                    response.Result = false;
                    response.Message = "Please check the order details.";
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                response.Result = false;
                response.Message = $"Order has been not cancelled, Error: {e.Message}";
                return StatusCode(StatusCodes.Status417ExpectationFailed, response);
            }
        }


        [Authorize]
        [HttpPost("createStripeSession")]
        public async Task<IActionResult> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var stripeRequest = await orderService.CreateStripeSession(stripeRequestDto);
                response.Result = stripeRequest;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        private string? GetQueueName(string messageBroker)
        {
            return messageBroker switch
            {
                "RabbitMQ" => configuration.GetValue<string>($"{KeyVaultConfig.SecretPrefix}:ApiSettings:RabbitMQ:TopicAndQueueNames:OrderQueue"),
                "EventHub" => configuration.GetValue<string>($"{KeyVaultConfig.SecretPrefix}:ApiSettings:EventHub:EventHubName"),
                "ServiceBus" => string.Empty,
                _ => throw new ArgumentException("Invalid broker type")
            };
        }
    }
}