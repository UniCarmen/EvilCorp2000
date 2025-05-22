using BusinessLayer.Models;
using EvilCorp2000.UIModels;
using static Shared.Utilities.Utilities;

namespace EvilCorp2000.Pages.Utilities

{
    public static class Utilities
    {
        public static ValidatedProduct CreateValidatedProduct(ProductManagementProductDTO selectedProduct, List<Guid> categoryIds)
        {
            return new ()
            {
                ProductId = selectedProduct.ProductId,
                ProductPicture = selectedProduct.ProductPicture,
                ProductName = selectedProduct.ProductName,
                AmountOnStock = selectedProduct.AmountOnStock,
                SelectedCategoryIds = categoryIds,
                Description = selectedProduct.Description,
                Discounts = selectedProduct.Discounts,
                Price = selectedProduct.Price
            };
        }


        public static ProductManagementProductDTO CreateProductToStoreDTO(ValidatedProduct validatedProduct, List<CategoryDTO> categories)
        {
            return new ()
            {
                ProductName = validatedProduct.ProductName,
                ProductPicture = validatedProduct.ProductPicture,
                AmountOnStock = validatedProduct.AmountOnStock ?? 0,
                Description = validatedProduct.Description ?? "",
                Categories = categories,
                Discounts = validatedProduct.Discounts,
                Price = validatedProduct.Price ?? 0.0m,
                ProductId = validatedProduct.ProductId,
            };
        }


        //Es wird immer Euro verwendet
        public static string FormatAsEuro(decimal price) =>
            price.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));


        private static readonly Dictionary<string, ProductSortOrder> _sortDisplayToEnum = new()
        {
            [/*"Default"*/ProductSortOrder.Default.ToString()] = ProductSortOrder.Default,
            [/*"Price Ascending"*/ ProductSortOrder.PriceAsc.ToString()] = ProductSortOrder.PriceAsc,
            [/*"Price Descending"*/ ProductSortOrder.PriceDesc.ToString()] = ProductSortOrder.PriceDesc,
            [/*"Discount Descending"*/ ProductSortOrder.DiscountDesc.ToString()] = ProductSortOrder.DiscountDesc,
            [/*"Discount Ascending"*/ ProductSortOrder.DiscountAsc.ToString()] = ProductSortOrder.DiscountAsc,
            [/*"Rating Descending"*/ ProductSortOrder.RatingDesc.ToString()] = ProductSortOrder.RatingDesc,
            [/*"Rating Ascending"*/ ProductSortOrder.RatingAsc.ToString()] = ProductSortOrder.RatingAsc,
            [/*"Name Ascending"*/ ProductSortOrder.NameAsc.ToString()] = ProductSortOrder.NameAsc,
            [/*"Name Descending"*/ ProductSortOrder.NameDesc.ToString()] = ProductSortOrder.NameDesc,
            [/*"On Stock Ascending"*/ ProductSortOrder.StockAsc.ToString()] = ProductSortOrder.StockAsc,
            [/*"On Stock Descending"*/ ProductSortOrder.StockDesc.ToString()] = ProductSortOrder.StockDesc
        };

        static public ProductSortOrder MapSortOrderString(string? displayName)
        {
            if (displayName == null)
                return ProductSortOrder.Default;

            return _sortDisplayToEnum.TryGetValue(displayName, out var sortOrder)
                ? sortOrder
                : ProductSortOrder.Default;
        }


        //TODO Edit Discount
        //public IActionResult OnPostEditDiscount(Guid discountId)
        //{
        //    // hier muss ein Dialog zum Ändern geöffnet werden in der UI / sowas wie zur Eingabe, aber irgendwie unter dem Discount, man kann keine vergangenen Discounts ändern

        //    // Discount bearbeiten (Logik zur Anzeige der Edit-Werte)

        //    //Prüfen, ob Überlappung

        //    //Save Product incl CreateProductToStoreDTO(ValidatedProduct, selectedCategories)

        //    //neues Product mit allem Drum und dran laden.
        //    return Page();
        //}
    }
}
