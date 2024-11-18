﻿using System.ComponentModel.DataAnnotations;

namespace RazorPagesSpielwiese.Models
{
    public class NewProductViewModelValidations1
    {
        //überschreibt die IsValid-Methode der Basislöasse ValidationAttribute
        //public class GuidListValidationAttribute : ValidationAttribute
        //{
        //    //Nullable, weil Validation.Success null repräsentiert... zeigt einfach an, dass die Validierung erfolgreich war
        //    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        //    {
        //        var list = value as IList<Guid>;
        //        if (list == null || !list.Any())
        //        {
        //            return new ValidationResult(ErrorMessage); // ?? "Please select at least one category."
        //        }
        //        return ValidationResult.Success;
        //    }
        //}

        public class DecimalValidationAttribute : ValidationAttribute
        {
            private readonly decimal _min;

            public DecimalValidationAttribute(double min, string errorMessage)
            {
                _min = (decimal)min;
                ErrorMessage = errorMessage;//$"The value must be higher than {_min}.";
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value == null || value is string str && string.IsNullOrWhiteSpace(str))
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (value is decimal decimalValue)
                {
                    if (decimalValue < _min)
                    {
                        return new ValidationResult($"Minimum Price is {_min}");
                    }
                    return ValidationResult.Success;
                }
                return new ValidationResult("Invalid value.");
            }
        }


        public class IntValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value == null || value is string str && string.IsNullOrWhiteSpace(str))
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (value is int intValue)
                {
                    if (intValue <= 0)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                    return ValidationResult.Success;
                }
                return new ValidationResult("Invalid value.");
            }
        }

    }
}
