using Microsoft.AspNetCore.Mvc;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Application.Interfaces;

namespace Capstone.ECommerceApp.Product.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductService productService;
        private ResponseDto responseDto;

        public ProductAPIController(IProductService productService)
        {
            this.productService = productService;
            responseDto = new ResponseDto();
        }
        //Get all the products
        [HttpGet]
        public IActionResult Get()
        {
            var responseDto = new ResponseDto();
            try
            {
                responseDto.Result = productService.GetProducts();
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        //Get a product by id
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                responseDto.Result = productService.GetProductById(id);
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