using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGA.ECommerceApp.Order.Application.Dto;
using TGA.ECommerceApp.Order.Application.Interfaces;

namespace TGA.ECommerceApp.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService orderService;
        private ResponseDto response;

        public OrderAPIController(IOrderService orderService)
        {
            this.orderService = orderService;
            this.response = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CartDto cartDto)
        {
            try
            {
                var orderHeaderDto = await orderService.CreateOrder(cartDto);
                response.Result = orderHeaderDto;
                response.Message = "Order was created successfully";
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
        [HttpPost("CreateStripeSession")]
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