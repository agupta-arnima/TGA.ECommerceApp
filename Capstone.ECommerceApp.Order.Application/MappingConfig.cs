using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.ECommerceApp.Infra.Common;
using Capstone.ECommerceApp.Order.Domain.Models;

namespace Capstone.ECommerceApp.Order.Application;

public class MappingConfig
{

    public static MapperConfiguration RegisterMaps()
    {
        MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CartHeaderDto, OrderHeaderDto>()
            .ForMember(dest => dest.OrderTotal, u => u.MapFrom(src => src.CartTotal))
            .ReverseMap();

            cfg.CreateMap<CartDetailsDto, OrderDetailsDto>()
            .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

            cfg.CreateMap<OrderDetailsDto, CartDetailsDto>();

            cfg.CreateMap<OrderHeader, OrderHeaderDto>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
            .ReverseMap();

            cfg.CreateMap<OrderDetails, OrderDetailsDto>()
            .ReverseMap();
        });

        return config;
    }

}
