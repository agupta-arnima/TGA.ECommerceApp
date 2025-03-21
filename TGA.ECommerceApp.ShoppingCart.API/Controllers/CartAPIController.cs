using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGA.ECommerceApp.ShoppingCart.Application.Dto;
using TGA.ECommerceApp.ShoppingCart.Application.Interfaces;

namespace TGA.ECommerceApp.ShoppingCart.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                await cartService.RemoveCart(cartDetailId);
                responseDto.Result = true;
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