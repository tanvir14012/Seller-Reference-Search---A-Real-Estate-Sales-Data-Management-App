using Seller_Reference_Search.Models.Validators;
using System.ComponentModel.DataAnnotations;

namespace Seller_Reference_Search.Models
{
    public class FileUploadDto
    {
        [Required]
        [FileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { "xlsx", "xls" })]
        [Display(Name = "Excel file")]
        public IFormFile File { get; set; }

        public bool Errored { get; set; }

        public string? Message { get; set; }

    }
}
