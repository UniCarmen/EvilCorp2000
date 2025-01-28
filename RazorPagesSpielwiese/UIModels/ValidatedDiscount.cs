using DataAccess.Entities;
using System.ComponentModel.DataAnnotations;
using static EvilCorp2000.UIModels.DiscountValidation;

namespace EvilCorp2000.UIModels
{
    public class ValidatedDiscount
    {
        [StartDateValidation("Start Date Required (min: today)")]
        public DateTime? StartDate { get; set; }

        [EndDateValidation("End Date Required (must be after Start Date)")]
        public DateTime? EndDate { get; set; }

        [DiscountPercentage("Discount Percentage required must be over 0 %")]
        public double? DiscountPercentage { get; set; }
    }


    public class DiscountValidation
    {
        public class StartDateValidationAttribute : ValidationAttribute
        {
            public StartDateValidationAttribute(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                //wenn Date null oder vor dem aktuellen Datum
                if (value == null || value is DateTime date && date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage);
                }

                else return ValidationResult.Success;
            }
        }


        public class EndDateValidationAttribute : ValidationAttribute
        {
            public EndDateValidationAttribute(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var startDateProp = validationContext.ObjectType.GetProperty("StartDate");
                var startDateValue = startDateProp.GetValue(validationContext.ObjectInstance) as DateTime?;

                var endDate = value as DateTime?;

                if (startDateValue == null && (endDate == null || endDate <= DateTime.UtcNow))
                    return new ValidationResult(ErrorMessage);

                if (endDate <= startDateValue || endDate == null || endDate < DateTime.Today)
                    return new ValidationResult(ErrorMessage);

                else return ValidationResult.Success;
            }
        }


        public class DiscountPercentageAttribute : ValidationAttribute
        {
            public DiscountPercentageAttribute(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var percentageProperty = validationContext.ObjectType.GetProperty("DiscountPercentage");
                var percentage = percentageProperty.GetValue(validationContext.ObjectInstance) as double?;

                if (percentage == null || percentage <= 0.0)
                    return new ValidationResult(ErrorMessage);

                else return ValidationResult.Success;
            }
        }

    }
}
