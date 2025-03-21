using AutoMapper;
using TGA.ECommerceApp.ShoppingCart.Application.Dto;
using TGA.ECommerceApp.ShoppingCart.Domain.Models;

namespace TGA.ECommerceApp.ShoppingCart.Application
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                cfg.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
            });
            return config;
        }
    }
}