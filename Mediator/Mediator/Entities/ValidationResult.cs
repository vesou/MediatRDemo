namespace Mediator.Entities
{
    public class ValidationResult
    {
        public ValidationResult(string validationError)
        {
            ValidationError = validationError;
            ValidationPassed = false;
        }

        public ValidationResult(bool validationPassed)
        {
            ValidationPassed = validationPassed;
            ValidationError = null;
        }

        public string ValidationError { get; }
        public bool ValidationPassed { get; }
    }
}