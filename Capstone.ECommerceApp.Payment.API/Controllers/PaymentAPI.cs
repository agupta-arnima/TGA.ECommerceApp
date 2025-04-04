using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Capstone.ECommerceApp.Payment.Application.Dto;


namespace Capstone.ECommerceApp.Payment.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, ADMIN")]
    public class PaymentAPI : ControllerBase
    {
        private ResponseDto responseDto;

        public PaymentAPI()
        {
            responseDto = new ResponseDto();
        }

        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] OrderHeaderDto order)
        {
            //TODO
            responseDto.Message = "Payment Processed.";
            responseDto.IsSuccess = true;
            return Ok(responseDto);
        }

        [HttpPost("refund")]
        public IActionResult RefundPayment([FromBody] OrderHeaderDto order)
        {
            //TODO
            responseDto.Message = "Refund Processed.";
            responseDto.IsSuccess = true;
            return Ok(responseDto);
        }
    }
}
