using System.Text.Json.Serialization;

namespace Capstone.ECommerceApp.Order.Application.Dto;

public class OrderHeaderDto
{
    public int OrderHeaderId { get; set; }
    public string? UserId { get; set; }
    public string? CouponCode { get; set; }
    public double Discount { get; set; }
    public double OrderTotal { get; set; }
    [JsonIgnore]
    public string? Name { get; set; }
    [JsonIgnore]
    public string? Phone { get; set; }
    [JsonIgnore]
    public string? Email { get; set; }
    public DateTime OrderTime { get; set; }
    public string? Status { get; set; } //Order Status like Pending, Approved
    public string? PaymentIntentId { get; set; }
    public string? StripeSessionId { get; set; }
    public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }
}
