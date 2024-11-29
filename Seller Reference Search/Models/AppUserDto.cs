using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Seller_Reference_Search.Models
{
    public class AppUserDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("FirstName")]
        [Required]
        [MinLength(2, ErrorMessage = "First name must be at least two characters long")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Only letters and whitespaces are allowed")]
        [MaxLength(50, ErrorMessage = "First name must not exceed 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        [Required]
        [MinLength(2, ErrorMessage = "Last name must be at least two characters long")]
        [MaxLength(50, ErrorMessage = "Last name must not exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z ]+$", ErrorMessage = "Only letters and whitespaces are allowed")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        [Required]
        [MaxLength(254, ErrorMessage = "Email address must not exceed 254 characters")]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "The provided email address is not valid")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [JsonProperty("Password")]
        [Required]
        [MaxLength(128, ErrorMessage = "Password  must not exceed 128 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,128}$", ErrorMessage = "The provided password does not meet the criteria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [JsonProperty("ConfirmPassword")]
        [Required]
        [MaxLength(128, ErrorMessage = "Password  must not exceed 128 characters")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,128}$", ErrorMessage = "The provided password does not meet the criteria")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
