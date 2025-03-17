using System.ComponentModel.DataAnnotations;

namespace EvilCorp2000.UIModels
{

    public abstract class BaseValidationAttribute : ValidationAttribute
    {
        protected BaseValidationAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        protected ValidationResult Fail() => new(ErrorMessage);
        protected ValidationResult SuccessResult => ValidationResult.Success;
    }
}
