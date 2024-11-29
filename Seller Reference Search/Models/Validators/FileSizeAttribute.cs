namespace Seller_Reference_Search.Models.Validators
{
    using System.ComponentModel.DataAnnotations;

    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxFileSize;

        public FileSizeAttribute(long maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            var maxFileSizeInMB = (double)_maxFileSize / (1024 * 1024);
            return $"Maximum allowed file size is {maxFileSizeInMB} MB.";
        }
    }

}
