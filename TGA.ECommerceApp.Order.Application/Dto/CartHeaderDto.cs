namespace TGA.ECommerceApp.Order.Application.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; } //Coupon code is at CartHeader level
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        //Below 2 properties are calculated properties based on CartDetails, Product.Price and Count, Coupon.DiscountAmount
        public double CartTotal { get; set; }
        public double Discount { get; set; }
    }
}
