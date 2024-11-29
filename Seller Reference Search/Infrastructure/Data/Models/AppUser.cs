using Microsoft.AspNetCore.Identity;
using Seller_Reference_Search.Infrastructure.Interfaces;

namespace Seller_Reference_Search.Infrastructure.Data.Models
{
    public class AppUser: IdentityUser<int>, IAggregateRoot
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? CountryCode { get; set; }
        public string? PostalCode { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FileUpload> FileUploads { get; set; }
    }
}
