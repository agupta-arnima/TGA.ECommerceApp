using AutoMapper;
using Capstone.ECommerceApp.ShoppingCart.Application.Dto;
using Capstone.ECommerceApp.ShoppingCart.Domain.Models;

namespace Capstone.ECommerceApp.ShoppingCart.Application
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