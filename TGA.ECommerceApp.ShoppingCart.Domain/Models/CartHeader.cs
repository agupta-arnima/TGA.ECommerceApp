﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TGA.ECommerceApp.ShoppingCart.Domain.Models
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        [NotMapped]
        public double CartTotal { get; set; }

        [NotMapped]
        public double Discount { get; set; }
    }
}
