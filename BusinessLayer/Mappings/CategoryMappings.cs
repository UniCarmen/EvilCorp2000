using DataAccess.Entities;
using BusinessLayer.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Utilities;

namespace BusinessLayer.Mappings
{
    public class CategoryMappings
    {
        public CategoryDTO CategoryToCategoryDto(Category category)
        {
            category = Utilities.ReturnValueOrThrowExceptionWhenNull(category, "Category is null.");
            return new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }

        public Category CategoryDtoToCategory(CategoryDTO categoryDto)
        {
            categoryDto = Utilities.ReturnValueOrThrowExceptionWhenNull(categoryDto, "CategoryDto is null.");

            return new Category()
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
            };
        }        
    }
}
