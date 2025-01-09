using System.ComponentModel.DataAnnotations;

namespace EvilCorp2000.Models
{

    public class NewProductValidations
    {
        //überschreibt die IsValid-Methode der Basisklasse ValidationAttribute
        public class GuidListValidationAttribute : ValidationAttribute
        {
            //Nullable, weil Validation.Success null repräsentiert... zeigt einfach an, dass die Validierung erfolgreich war
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var list = value as IList<Guid>;
                if (list == null || !list.Any())
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
        }

    }
}
