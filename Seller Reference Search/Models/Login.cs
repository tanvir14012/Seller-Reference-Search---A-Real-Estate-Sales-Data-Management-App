using System.ComponentModel.DataAnnotations;

namespace Seller_Reference_Search.Models
{
    public class Login
    {
        [Required]
        [MaxLength(254, ErrorMessage = "Email address must not exceed 254 characters")]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "The provided email address is not valid")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [MaxLength(128, ErrorMessage = "Password  must not exceed 128 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,128}$", ErrorMessage = "The provided password does not meet the criteria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
