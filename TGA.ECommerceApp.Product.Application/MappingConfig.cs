using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.Product.Application.Dto;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Application
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductDto, ProductInfo>().ReverseMap();
            });
            return config;
        }
    }
}
