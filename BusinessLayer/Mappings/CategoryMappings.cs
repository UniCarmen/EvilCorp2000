﻿using DataAccess.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Mappings
{
    public class CategoryMappings
    {
        public CategoryDTO CategoryEntityToCategoryModel(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            return new CategoryDTO()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }

        public Category CategoryDtoToCategory(CategoryDTO category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            return new Category()
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
            };
        }
    }
}
