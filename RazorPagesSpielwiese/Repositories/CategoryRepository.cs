﻿using Microsoft.EntityFrameworkCore;
using RazorPagesSpielwiese.DBContexts;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EvilCorp2000Context _context;

        public CategoryRepository(EvilCorp2000Context context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Category.AsNoTracking().ToListAsync ();
        }

        public async Task SaveNewCategory(Category category)
        {
            if (category == null) { throw new ArgumentNullException("keine Productclass"); }
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category newCategory)
        {
            if (newCategory == null) { throw new ArgumentNullException("keine Productclass"); }

            var oldCategory = await _context.Category.FirstAsync(p => p.CategoryId == newCategory.CategoryId);

            if (oldCategory == null)
            {
                throw new ArgumentNullException("keine alte Klasse vorhanden");
            }
            _context.Category.Remove(oldCategory);
            _context.Add(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategory(Category categoryToDelete)
        {
            if (categoryToDelete == null) { throw new ArgumentNullException("keine Productclass"); }

            var oldClass = await _context.Category.FirstAsync(p => p.CategoryId == categoryToDelete.CategoryId);

            if (oldClass == null)
            {
                throw new ArgumentNullException("keine alte Klasse vorhanden");
            }
            _context.Category.Remove(oldClass);
            await _context.SaveChangesAsync();
        }

        public List<Category> AttachCategoriesIfNeeded(List<Category> categories)
        {

            //foreach (var category in categories)
            //{
            //    var trackedCategory = _context.ChangeTracker.Entries<Category>()
            //        .FirstOrDefault(e => e.Entity.CategoryId == category.CategoryId);

            //    if (trackedCategory == null)
            //    {
            //        // Nur attachen, wenn nicht bereits getrackt
            //        _context.Attach(category); // Verhindert das Hinzufügen neuer Kategorien in der Datenbank
            //    }
            //    //_context.Attach(category); 
            //}
            //return categories;

            var attachedCategories = new List<Category>();

            foreach (var category in categories)
            {
                var existingCategory = _context.Category
                    .Local
                    .FirstOrDefault(c => c.CategoryId == category.CategoryId);

                if (existingCategory == null)
                {
                    // Füge hinzu, falls nicht lokal
                    _context.Attach(category);
                    attachedCategories.Add(category);
                }
                else
                {
                    // Nutze die bestehende Instanz
                    attachedCategories.Add(existingCategory);
                }
            }

            return attachedCategories;
        }
    }
}
