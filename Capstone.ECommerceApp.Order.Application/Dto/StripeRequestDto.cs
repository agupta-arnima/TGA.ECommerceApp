namespace Capstone.ECommerceApp.Order.Application.Dto;

public class StripeRequestDto
{
    public string? StripeSessionId { get; set; }
    public string? StripeSessionUrl { get; set; }
    public string ApprovedUrl { get; set; }
    public string CanceledUrl { get; set; }
    public OrderHeaderDto OrderHeader { get; set; }
}
