using DataAccess.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Mappings
{
    public class CategoryMappings
    {
        public CategoryDTO CategoryToCategoryDto(Category category)
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


        //TODO1: generische Methoden für die Nullprüfung in einem Sharedprojekt anlegen? So kann ich alles auslagern
        //TODO1: evtl diese Schreibweise testen / verwenden: ArgumentNullException.ThrowIfNull(category, nameof(category));

        //INFO: <T>: sagt, dass ein generischer verwende wird
        //INFO: where T: class: sagt, dass nur Referenztypen (class) erlaubt sind // struct für Werttypen
        //INFO: bei Werttypen steht T? in den Parametern

        //INFO: für Reftypen: string, eigene Objekte
        private static T ThrowExceptionWhenNull<T> (T value, string parameterName) where T: class
        {
            //INFO: gibt entweder den Eingangwert zurück oder wirft einen Fehler, falls es keinen gibt = null
            return value ?? throw new ArgumentNullException(nameof(parameterName));
        }

        //INFO: für Werttypen: int, Guid, decimal
        private static T EnsureNotNull<T>(T? value, string paramName) where T : struct
        {
            return value ?? throw new ArgumentNullException(paramName);
        }
    }
}
