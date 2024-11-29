using Seller_Reference_Search.Infrastructure.Interfaces;

namespace Seller_Reference_Search.Infrastructure.Data.Models
{
    public class Sale: BaseEntity, IAggregateRoot
    {
        public string Reference { get; set; }
        public string? OwnerName { get; set; }
        public string ParcelNumber { get; set; }
        public double LotAcreage { get; set; }
        public decimal OfferPrice { get; set; }
        public decimal? OfferPPA { get; set; }
        public decimal? RealPPA { get; set; }
        public decimal? PPACalc { get; set; }
        public decimal? Profit { get; set; }
        public decimal? RetailValue { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string? ZipCode { get; set; }
        public DateTime? ClosingDate { get; set; }

        public int FileUploadId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }

        public virtual FileUpload FileUpload { get; set; }

    }
}
