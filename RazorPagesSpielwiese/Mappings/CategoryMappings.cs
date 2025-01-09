using EvilCorp2000.Entities;
using EvilCorp2000.Models;

namespace EvilCorp2000.Mappings
{
    public class CategoryMappings
    {
        public CategoryDTO CategoryEntityToCategoryModel(Category category)
        {
            return new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }

        public Category CategoryDtoToCategory(CategoryDTO category)
        {
            return new Category()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }
    }
}
