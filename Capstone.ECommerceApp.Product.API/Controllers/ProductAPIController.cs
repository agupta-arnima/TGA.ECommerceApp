using Microsoft.AspNetCore.Mvc;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Application.Interfaces;
using Capstone.ECommerceApp.Product.Application.Queries;
using MediatR;

namespace Capstone.ECommerceApp.Product.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductService productService;
        private ResponseDto responseDto;
        private readonly ISender sender;

        public ProductAPIController(IProductService productService,
                                    ISender sender)
        {
            this.productService = productService;
            responseDto = new ResponseDto();
            this.sender = sender;
        }
        //Get all the products
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            var responseDto = new ResponseDto();
            try
            {
                responseDto.Result = await sender.Send(new GetProductsQuery(pageNumber,pageSize));
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