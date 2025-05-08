namespace Capstone.ECommerceApp.Infra.Common;

public class OrderMessage
{
    public OrderHeaderDto? order { get; set; }
    public DateTime Timestamp { get; set; }
}
