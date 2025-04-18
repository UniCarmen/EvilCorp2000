﻿using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class ProductManagementProductDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CategoryDTO> Categories { get; set; } = [];
        public List<DiscountDTO> Discounts { get; set; } = [];
        public string? ProductPicture { get; set; }

        public decimal Price { get; set; }
        public int AmountOnStock { get; set; }
        public double? Rating { get; set; }
        //TODO: Status: Aktiv / nicht aktiv
    }
}