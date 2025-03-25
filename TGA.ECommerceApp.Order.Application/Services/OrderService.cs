using AutoMapper;
using Stripe.Checkout;
using TGA.ECommerceApp.Order.Application.Dto;
using TGA.ECommerceApp.Order.Application.Interfaces;
using TGA.ECommerceApp.Order.Domain.Interfaces;
using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<OrderHeaderDto> CreateOrder(CartDto cartDto)
        {
            var orderHeaderDto = mapper.Map<OrderHeaderDto>(cartDto.CartHeader);//mapping from CartHeaderDto to OrderHeaderDto
            orderHeaderDto.OrderTime = DateTime.Now;
            orderHeaderDto.Status = SD.Status_Pending;
            orderHeaderDto.OrderDetails = mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);//mapping from CartDetailsDto to OrderDetailsDto

            var orderHeaderId = await orderRepository.CreateOrder(mapper.Map<OrderHeader>(orderHeaderDto));
            orderHeaderDto.OrderHeaderId = orderHeaderId;
            return orderHeaderDto;
        }

        public async Task<StripeRequestDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = stripeRequestDto.ApprovedUrl,
                CancelUrl = stripeRequestDto.CanceledUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099 cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Name,
                            Images = new List<string> { item.Product.ImageUrl }
                        }
                    },
                    Quantity = item.Count
                };

                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options); //This is not .Net session, this is Stripe session
            stripeRequestDto.StripeSessionUrl = session.Url;

            await orderRepository.CreateStripeSession(stripeRequestDto.OrderHeader.OrderHeaderId, session.Id);
            return stripeRequestDto;
        }
    }
}
