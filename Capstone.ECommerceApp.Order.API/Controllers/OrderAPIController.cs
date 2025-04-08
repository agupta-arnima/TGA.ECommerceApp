using Capstone.ECommerceApp.Domain.Core.Bus;
using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Capstone.ECommerceApp.Order.Application.Events;
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

        public OrderAPIController(IOrderService orderService, IEventBus messageBus, IConfiguration configuration)
        {
            this.orderService = orderService;
            this.response = new ResponseDto();
            this.messageBus = messageBus;
            this.configuration = configuration;
        }

        [Authorize]
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(CartDto cartDto)
        {
            try
            {
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
                        configuration.GetValue<string>("ApiSettings:RabbitMQ:TopicAndQueueNames:OrderQueue"), token);
                }
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
    }
}