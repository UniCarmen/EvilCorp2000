namespace RazorPagesSpielwiese.Mappings
{
    public class CategoryMappings
    {
        public Models.CategoryDTO CategoryEntityToCategoryModel (Entities.Category category)
        {
            return new Models.CategoryDTO ()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }
    }
}
