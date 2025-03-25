using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGA.ECommerceApp.Product.Application.Dto;
using TGA.ECommerceApp.Product.Application.Interfaces;

namespace TGA.ECommerceApp.Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, ADMIN")]
    public class InventoryAPIController : ControllerBase
    {
        private readonly IProductService _productService;
        private ResponseDto responseDto;

        public InventoryAPIController(IProductService productService)
        {
            _productService = productService;
            responseDto = new ResponseDto();
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _productService.AddProduct(productDto);
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        // POST: api/InventoryAPI/
        [HttpDelete("{productId:int}")]
        public IActionResult Delete(int productId)
        {
            try
            {
                var product = _productService.GetProductById(productId);
                if (product == null)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Product not found";
                    return Ok(responseDto);
                }
                _productService.DeleteProduct(product.ProductId);
                responseDto.IsSuccess = true;
                responseDto.Message = "Product Deleted Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }
    }
}
