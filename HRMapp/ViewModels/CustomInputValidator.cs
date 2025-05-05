using InputKit.Shared.Validations;

namespace HRMapp.ViewModels
{
    public class NullableNumericValidator : IValidation
    {
        public string? Message { get; set; } = "Input must be numeric";

        public bool Validate(object value)
        {
            if (value == null)
                return true;

            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return true; // empty input is allowed

                return double.TryParse(str, out _);
            }

            return false;
        }
    }
}