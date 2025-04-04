﻿using AutoMapper;
using Capstone.ECommerceApp.Product.Application.Dto;
using Capstone.ECommerceApp.Product.Application.Interfaces;
using Capstone.ECommerceApp.Product.Domain.Interfaces;
using Capstone.ECommerceApp.Product.Domain.Models;

namespace Capstone.ECommerceApp.Product.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly IMapper mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        this.productRepository = productRepository;
        this.mapper = mapper;
    }

    public IEnumerable<ProductDto> GetProducts()
    {
        return mapper.Map<IEnumerable<ProductDto>>(productRepository.GetProducts());
    }

    public ProductDto GetProductById(int id)
    {
        return mapper.Map<ProductDto>(productRepository.GetProductById(id));
    }

    public bool DeleteProduct(int id) { 
       return productRepository.DeleteProduct(id);
    }

    public ProductDto AddProduct(ProductDto product)
    {
        Category category = new Category();
        Supplier supplier = new Supplier();

        if (product.CategotyId != null)
        {
            category = productRepository.GetCategoryById(product.CategotyId);
        }
        if (product.SupplierId != null)
        {
            supplier = productRepository.GetSupplierById(product.SupplierId);
        }

        var productInfo = new ProductInfo
        {
            CategoryId = category.Id,
            SupplierId = supplier.Id,
            Description = product.Description,
            Name = product.Name,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Stock = product.Stock
        };

        return mapper.Map<ProductDto>(productRepository.CreateProduct(productInfo));
    }

    public ProductDto UpdateProduct(ProductDto product)
    {
        throw new NotImplementedException();
    }

    public SupplierDto SupplierById(int id)
    {
        throw new NotImplementedException();
    }

    public SupplierDto AddSupplier(SupplierDto supplierDto)
    {
        var supplier = mapper.Map<Supplier>(supplierDto);
        var createdSupplier = productRepository.CreateSupplier(supplier);
        return mapper.Map<SupplierDto>(createdSupplier);
    }

    public ProductDto UpdateSupplier(SupplierDto supplier)
    {
        throw new NotImplementedException();
    }

    public ProductDto DeleteSupplier(int id)
    {
        throw new NotImplementedException();
    }

    public CategoryDto CategoryById(int id)
    {
        throw new NotImplementedException();
    }

    public CategoryDto AddCategory(CategoryDto categoryDto)
    {
        var category = mapper.Map<Category>(categoryDto);
        var createdCategory = productRepository.CreateCategory(category);
        return mapper.Map<CategoryDto>(createdCategory);
    }

    public CategoryDto UpdateCategory(CategoryDto category)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ReleaseInventory(OrderHeaderDto order)
    {
        foreach (var item in order.OrderDetails)
        {
            var product = productRepository.GetProductById(item.ProductId);
            if (product == null || product.Stock < item.Count)
            {
                return false; // Not enough stock
            }
            product.Stock += item.Count;
            await productRepository.UpdateProduct(product);
        }
        return true;
    }

    public async Task<bool> ReserveInventory(OrderHeaderDto order)
    {
        foreach (var item in order.OrderDetails)
        {
            var product = productRepository.GetProductById(item.ProductId);
            if (product == null || product.Stock < item.Count)
            {
                return false; // Not enough stock
            }
            product.Stock -= item.Count;
            await productRepository.UpdateProduct(product);
        }
        return true;
    }
}