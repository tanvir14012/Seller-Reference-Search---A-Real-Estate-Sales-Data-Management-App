using System.ComponentModel.DataAnnotations;

namespace Seller_Reference_Search.Models.Validators
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            this.extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value == null)
                {
                    return ValidationResult.Success;
                }
                var spreadsheet = value as IFormFile;
                var extension = Path.GetExtension(spreadsheet.FileName).Replace(".", "").ToLower();
                if (spreadsheet.FileName.Length > 255)
                {
                    return new ValidationResult("The file name is too big, max length is 255");
                }

                if (this.extensions.Contains(extension))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("The provided file type is not allowed");
            }
            catch (Exception ex)
            {
                return new ValidationResult("Something is wrong with the file");
            }

        }
    }
}