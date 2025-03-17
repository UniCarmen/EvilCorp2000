using BusinessLayer.Models;
using EvilCorp2000.UIModels;

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
