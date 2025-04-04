using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Application.Interfaces;

namespace Capstone.ECommerceApp.Product.API.Controllers
{
    [Route("api/inventory")]
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

        [HttpPost("releaseinventory")]
        public IActionResult ReleaseInventory([FromBody] OrderHeaderDto order)
        {
            try
            {
                var releseInventory = _productService.ReleaseInventory(order);
                responseDto.Result = true;
                responseDto.Result = releseInventory;
                responseDto.Message = $"Order has been released Successfullty";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }
        [HttpPost("reserveinventory")]
        public IActionResult ReserveInventory([FromBody] OrderHeaderDto order)
        {
            try
            {
                var reserveInventory = _productService.ReserveInventory(order);
                responseDto.Result = true;
                responseDto.Result = reserveInventory;
                responseDto.Message = $"Order has been reserved Successfullty";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        [HttpPost("product")]
        public IActionResult CreateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _productService.AddProduct(productDto);
                responseDto.Result = product;
                responseDto.IsSuccess = true;
                responseDto.Message = $"{product.Name} added Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        // POST: api/InventoryAPI/
        [HttpDelete("product/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
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

        [HttpPut("product/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Product not found";
                    return Ok(responseDto);
                }
                _productService.UpdateProduct(productDto);
                responseDto.IsSuccess = true;
                responseDto.Message = "Product Updated Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        [HttpPost("supplier")]
        public IActionResult CreateSupplier([FromBody] SupplierDto supplierDto)
        {
            try
            {
                var supplier = _productService.AddSupplier(supplierDto);
                if (supplier == null) { 
                    responseDto.IsSuccess = false;
                }
                responseDto.Result = supplier;
                responseDto.IsSuccess = true;
                responseDto.Message = "Supplier Added Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        [HttpPut("supplier/{id}")]
        public IActionResult UpdateSupplier(int id, [FromBody] SupplierDto supplierDto)
        {
            return Ok();
        }

        [HttpDelete("supplier/{id}")]
        public IActionResult DeleteSupplier(int id)
        {
            try
            {
                var supplier = _productService.SupplierById(id);
                if (supplier == null)
                {
                    responseDto.IsSuccess = false;
                    responseDto.Message = "Supplier not found";
                    return Ok(responseDto);
                }
                _productService.DeleteSupplier(supplier.SupplierId);
                responseDto.IsSuccess = true;
                responseDto.Message = "Supplier Deleted Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        // CRUD operations for Category

        [HttpPost("category")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto category)
        {
            try
            {
                var categoryInfo = _productService.AddCategory(category);
                if (categoryInfo == null)
                {
                    responseDto.IsSuccess = false;
                }
                responseDto.Result = categoryInfo;
                responseDto.IsSuccess = true;
                responseDto.Message = "Category Added Successfully.";
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.Message = ex.Message;
            }
            return Ok(responseDto);
        }

        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto category)
        {
            return Ok();
        }

        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return Ok();
        }
    }
}
