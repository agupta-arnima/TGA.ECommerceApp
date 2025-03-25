using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.Order.Application.Dto;
using TGA.ECommerceApp.Order.Domain.Models;

namespace TGA.ECommerceApp.Order.Application
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(dest => dest.OrderTotal, u => u.MapFrom(src => src.CartTotal)).ReverseMap();

                cfg.CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));  //No reverse map here

                cfg.CreateMap<OrderDetailsDto, CartDetailsDto>(); //While mapping from OrderDetailsDto to CartDetailsDto, we will not map ProductName and Price

                cfg.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                cfg.CreateMap<OrderDetails, OrderDetailsDto>().ReverseMap();

            });
            return config;
        }
    }
}
