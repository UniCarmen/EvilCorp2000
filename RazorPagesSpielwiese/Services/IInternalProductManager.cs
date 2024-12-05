﻿using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Services
{
    public interface IInternalProductManager
    {
        Task<List<ProductForInternalUseDTO>> GetProductsForInternalUse();
        Task<List<CategoryDTO>> GetCategories();
        Task SaveProductToStore(ProductToStoreDTO productToStore);
        Task UpdateProductToStore(ProductToStoreDTO productToStore);
        Task AddDiscount(DiscountDTO discount, ProductToStoreDTO productToStore);


    }
}