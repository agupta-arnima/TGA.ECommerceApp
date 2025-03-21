using AutoMapper;
using TGA.ECommerceApp.ShoppingCart.Application.Dto;
using TGA.ECommerceApp.ShoppingCart.Application.Interfaces;
using TGA.ECommerceApp.ShoppingCart.Domain.Interfaces;
using TGA.ECommerceApp.ShoppingCart.Domain.Models;

namespace TGA.ECommerceApp.ShoppingCart.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository shoppingCartRepository;
        private readonly IMapper mapper;
        private readonly IProductService productService;

        public CartService(ICartRepository shoppingCartRepository, IMapper mapper, IProductService productService)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.mapper = mapper;
            this.productService = productService;
        }

        public async Task<CartDto> GetCart(string userId)
        {
            var cartHeader = await shoppingCartRepository.GetCartHeader(userId);
            if (cartHeader == null)
            {
                return new CartDto();
            }
            CartDto cartDto = new()
            {
                CartHeader = mapper.Map<CartHeaderDto>(cartHeader)
            };
            cartDto.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(shoppingCartRepository.GetCartDetails(cartDto.CartHeader.CartHeaderId));

            var productDtos = await productService.GetProducts();
            //cartDto.CartDetails.ToList().ForEach(u => u.Product = productDtos.FirstOrDefault(p => p.ProductId == u.ProductId));
            //cartDto.CartDetails.ToList().ForEach(u => cartDto.CartHeader.CartTotal += u.Product.Price * u.Count);
            foreach (var item in cartDto.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);
                cartDto.CartHeader.CartTotal += item.Product.Price * item.Count;
            }
            //if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            //{
            //    var coupon = await _couponService.GetCouponByCode(cartDto.CartHeader.CouponCode);
            //    if (cartDto.CartHeader.CartTotal > coupon.MinAmount)
            //    {
            //        cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
            //        cartDto.CartHeader.Discount = coupon.DiscountAmount;
            //    }
            //}
            return cartDto;
        }

        public async Task CartUpsert(CartDto cartDto)
        {
            var cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
            var cartDetails = mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails);

            await shoppingCartRepository.CartUpsert(cartHeader, cartDetails);
        }

        public async Task RemoveCart(int cartDetailId)
        {
            await shoppingCartRepository.RemoveCart(cartDetailId);
        }
    }
}