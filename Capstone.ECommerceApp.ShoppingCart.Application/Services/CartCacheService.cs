using AutoMapper;
using Capstone.ECommerceApp.Domain.Core.Cache;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;
using Capstone.ECommerceApp.ShoppingCart.Domain.Models;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Services;

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
        var cartHeader = await cacheService.GetCacheValueAsync<CartHeader>(cartCacheKey, "CartHeader");
        if (cartHeader == null)
        {
            return new CartDto();
        }

        CartDto cartDto = new()
        {
            CartHeader = mapper.Map<CartHeaderDto>(cartHeader)
        };

        var cartDetails = new List<CartDetailsDto>();
        foreach (var item in cartHeader.CartDetails)
        {
            var cartDetail = await cacheService.GetCacheValueAsync<CartDetailsDto>(cartCacheKey, item.ProductId.ToString());
            if (cartDetail != null)
            {
                cartDetails.Add(cartDetail);
            }
        }
        cartDto.CartDetails = cartDetails;

        cartDto.CartHeader.CartTotal = 0;
        foreach (var item in cartDto.CartDetails)
        {
            var productDtos = await productService.GetProductsById(item.ProductId);
            item.Product = productDtos;
            cartDto.CartHeader.CartTotal += item.Product.Price * item.Count;
        }

        return cartDto;
    }

    public async Task CartUpsert(CartDto cartDto)
    {
        var cartCacheKey = $"Cart:{cartDto.CartHeader.UserId}";

        if (!cartDto.CartDetails.Any())
            throw new Exception("Please add some product item to be added.");

        foreach (var item in cartDto.CartDetails)
        {
            if (item.Count <= 0)
            {
                throw new ArgumentException("Count must be greater than zero.");
            }

            if (!productService.IsProductAvailable(item.ProductId, item.Count))
            {
                throw new InvalidOperationException("Product is not available in the requested quantity.");
            }

            // Check if the product already exists in the cart
            var existingItem = await cacheService.GetCacheValueAsync<CartDetailsDto>(cartCacheKey, item.ProductId.ToString());
            if (existingItem != null)
            {
                // Append the count if the product already exists
                item.Count += existingItem.Count;
            }

            // Update the cache with the new or updated item
            await cacheService.SetCacheValueAsync(cartCacheKey, item.ProductId.ToString(), item);
        }

        var cartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);
        cartHeader.CartDetails = mapper.Map<IEnumerable<CartDetails>>(cartDto.CartDetails);
        await cacheService.SetCacheValueAsync(cartCacheKey, "CartHeader", cartHeader);
    }

        public async Task<bool> ClearCart(string userId)
        {
            var cartCacheKey = $"Cart:{userId}";
            return await cacheService.DeleteKeyAsync(cartCacheKey);
        }
    }
}