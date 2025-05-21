using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Domain.Models;
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
        private readonly CartsMetrics cartsMetrics;

        public CartAPIController(ICartService cartService, CartsMetrics cartsMetrics)
        {
            this.cartService = cartService;
            this.responseDto = new ResponseDto();
            this.cartsMetrics = cartsMetrics;
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
                cartDto.CartDetails?.ToList().ForEach(CartDetail => CartDetail.OriginalCount = CartDetail.Count); //Preserve Original Count
                await cartService.CartUpsert(cartDto);
                cartDto.CartDetails?.ToList().ForEach(cartDetail => cartsMetrics.IncreaseCarts(cartDetail.Product?.Name, cartDetail.OriginalCount));                
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
                if (!isDeleted)
                {
                    responseDto.Result = isDeleted;
                    responseDto.Message = "Key does not exist!";
                }
                else
                {
                    responseDto.Result = isDeleted;
                    responseDto.Message = "Key deleted successfully!";
                }
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