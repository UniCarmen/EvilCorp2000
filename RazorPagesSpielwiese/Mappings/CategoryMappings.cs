namespace RazorPagesSpielwiese.Mappings
{
    public class CategoryMappings
    {
        public Models.CategoryDTO CategoryEntityToCategoryModel(Entities.Category category)
        {
            return new Models.CategoryDTO()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }

        public Entities.Category CategoryDtoToCategory(Models.CategoryDTO category)
        {
            return new Entities.Category()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }
    }
}
