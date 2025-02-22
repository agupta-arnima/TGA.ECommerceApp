using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGA.ECommerceApp.Product.Domain.Models;

namespace TGA.ECommerceApp.Product.Domain.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<ProductInfo> GetProducts();
        ProductInfo GetProductById(int id);
    }
}
