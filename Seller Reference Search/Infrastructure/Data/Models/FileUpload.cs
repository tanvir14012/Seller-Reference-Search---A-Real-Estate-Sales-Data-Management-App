using Seller_Reference_Search.Infrastructure.Interfaces;

namespace Seller_Reference_Search.Infrastructure.Data.Models
{
    public class FileUpload: BaseEntity, IAggregateRoot
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string Status { get; set; }
        public int UploadedByUserId { get; set; }
        public string? Description { get; set; }
        public string UploadPath { get; set; }
        public string? Checksum { get; set; }
        public TimeSpan? UploadDuration { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public int? RowsFound { get; set; }
        public int? RowsInserted { get; set; }
        public int? RowsUpdated { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}
