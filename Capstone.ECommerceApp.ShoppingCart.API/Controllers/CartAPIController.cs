using Capstone.ECommerceApp.ShoppingCart.Application.Dto;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.ECommerceApp.ShoppingCart.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartService cartService;
        private ResponseDto responseDto;

        public CartAPIController(ICartService cartService)
        {
            this.cartService = cartService;
            this.responseDto = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCart(string userId)
        {
            try
            {
                responseDto.Result = await cartService.GetCart(userId);
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message.ToString();
                responseDto.IsSuccess = false;
            }
            return Ok(responseDto);
        }


        [HttpPost("CartUpsert")]
        [Authorize]
        public async Task<ResponseDto> CartUpsert([FromBody] CartDto cartDto)
        {
            try
            {
                await cartService.CartUpsert(cartDto);
                responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message.ToString();
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }

        [HttpDelete("RemoveCart/{userId}")]
        [Authorize]
        public async Task<ResponseDto> RemoveCart(string userId)
        {
            try
            {
                var isDeleted = await cartService.ClearCart(userId);
                responseDto.Result = isDeleted;
                responseDto.Message = "Key deleted";
            }
            catch (Exception ex)
            {
                responseDto.Message = ex.Message.ToString();
                responseDto.IsSuccess = false;
            }
            return responseDto;
        }
    }
}