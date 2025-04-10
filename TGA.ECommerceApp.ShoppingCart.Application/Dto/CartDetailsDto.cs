﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TGA.ECommerceApp.ShoppingCart.Application.Dto;

public class CartDetailsDto
{
    public int ProductId { get; set; }
    public int Count { get; set; }
    public ProductDto? Product { get; set; }
    public CartHeaderDto? CartHeader { get; set; }
}
