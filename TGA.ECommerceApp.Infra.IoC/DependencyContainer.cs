using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TGA.ECommerceApp.Auth.Application.Interfaces;
using TGA.ECommerceApp.Auth.Application.Services;
using TGA.ECommerceApp.Auth.Data.Repository;
using TGA.ECommerceApp.Auth.Domain.Interfaces;
using TGA.ECommerceApp.Product.Application;
using TGA.ECommerceApp.Product.Application.Interfaces;
using TGA.ECommerceApp.Product.Application.Services;
using TGA.ECommerceApp.Product.Data.Context;
using TGA.ECommerceApp.Product.Data.Repository;
using TGA.ECommerceApp.Product.Domain.Interfaces;

namespace TGA.ECommerceApp.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {

            //Application Services
            
            
            
            //Data
            
                  
        }
    }
}