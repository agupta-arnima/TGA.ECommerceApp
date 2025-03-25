using AutoMapper;
using TGA.ECommerceApp.Product.Application.Dto;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Application
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductInfo, ProductDto>()
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            });
            return config;
        }
    }
}
