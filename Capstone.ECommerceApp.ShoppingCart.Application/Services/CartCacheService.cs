using AutoMapper;
using Capstone.ECommerceApp.ShoppingCart.Application.Dto;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Domain.Models;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Services
{
    public class CartCacheService : ICartService
    {
        private readonly IRedisCacheService cacheService;
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromDays(30);

        public CartCacheService(IRedisCacheService cacheService, IMapper mapper, IProductService productService)
        {
            this.cacheService = cacheService;
            this.mapper = mapper;
            this.productService = productService;
        }

        public async Task<CartDto> GetCart(string userId)
        {
            var cartCacheKey = $"Cart:{userId}";
            var cartHeader = await cacheService.GetCacheValueAsync<CartHeader>(cartCacheKey);
            if (cartHeader == null)
            {
                return new CartDto();
            }
            CartDto cartDto = new()
            {
                CartHeader = mapper.Map<CartHeaderDto>(cartHeader)
            };
            cartDto.CartDetails = mapper.Map<IEnumerable<CartDetailsDto>>(cartHeader.CartDetails);

            var productDtos = await productService.GetProducts();
            //cartDto.CartDetails.ToList().ForEach(u => u.Product = productDtos.FirstOrDefault(p => p.ProductId == u.ProductId));
            //cartDto.CartDetails.ToList().ForEach(u => cartDto.CartHeader.CartTotal += u.Product.Price * u.Count);
            cartDto.CartHeader.CartTotal = 0;
            foreach (var item in cartDto.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(p => p.ProductId == item.ProductId);
                cartDto.CartHeader.CartTotal += item.Product.Price * item.Count;
            }
            return cartDto;
        }

        public async Task CartUpsert(CartDto cartDto)
        {
            var cartCacheKey = $"Cart:{cartDto.CartHeader.UserId}";
            var cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
            cartHeader.CartDetails = mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails);
            await cacheService.SetCacheValueAsync(cartCacheKey, cartHeader, CacheExpiration);
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartCacheKey = $"Cart:{userId}";
            return await cacheService.DeleteKeyAsync(cartCacheKey);
        }
    }
}