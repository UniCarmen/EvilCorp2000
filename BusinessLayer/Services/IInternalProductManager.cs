﻿using BusinessLayer.Models;
using static Shared.Utilities.Utilities;

namespace BusinessLayer.Services
{
    public interface IInternalProductManager
    {
        Task<ProductListReturn<ProductManagementProductDTO>> GetProductsForInternalUse(GetProductsParameters parameters);
        Task<ProductManagementProductDTO> GetProductForInternalUse(Guid id);
        Task<List<CategoryDTO>> GetCategories();
        Task SaveProductToStore(ProductManagementProductDTO productToStore);
        Task UpdateProductToStore(ProductManagementProductDTO productToStore);
        Task AddDiscount(DiscountDTO discount, ProductManagementProductDTO productToStore);
        Task SaveProductPicture(Guid productId, string encodedPicture);
        Task DeleteProductPicture(Guid productId);
        Task DeleteProduct(Guid productId);
    }
}