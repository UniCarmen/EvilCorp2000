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
        /// <summary>
        /// Determines whether the provided value is a valid start date (today or a future date).
        /// </summary>
        public class StartDateValidationAttribute : BaseValidationAttribute
        {
            //INFO: mit :base(errorMessage -> Konstruktorverkettung. Der Konstruktor der Basisklasse wird aufgerufen, um die KLasse zu initialisieren
            public StartDateValidationAttribute(string errorMessage) : base(errorMessage) { }

            //Ausdrucksbasierte Methode. Anstatt KLammern ein => und kein return Keyword nötig
            protected override ValidationResult? IsValid(object? value, ValidationContext _) =>
                value is not DateTime date || date < DateTime.Today ? Fail() : SuccessResult;
        }

        /// <summary>
        /// Determines whether the provided end date is valid (after the start date and today or later).
        /// </summary>
        public class EndDateValidationAttribute : BaseValidationAttribute
        {
            public EndDateValidationAttribute(string errorMessage) : base(errorMessage) { }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var startDateProp = validationContext.ObjectType.GetProperty("StartDate");
                var startDateValue = startDateProp.GetValue(validationContext.ObjectInstance) as DateTime?;

                var endDate = value as DateTime?;

                return (startDateValue == null || endDate == null || endDate <= startDateValue || endDate <= DateTime.Today)
                    ? Fail()
                    : SuccessResult;
            }
        }

        /// <summary>
        /// Determines whether the provided discount percentage is valid (greater than 0).
        /// </summary>
        public class DiscountPercentageAttribute : BaseValidationAttribute
        {
            public DiscountPercentageAttribute(string errorMessage) : base(errorMessage) { }

            protected override ValidationResult? IsValid(object? value, ValidationContext _) =>
                //dem Attribut wird automatisch der Wert des Propertires DiscountPercentage übergeben
                value is not double percentage || percentage <= 0.0 ? Fail() : SuccessResult;
        }

    }
}
