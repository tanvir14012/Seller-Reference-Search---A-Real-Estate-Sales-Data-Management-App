namespace Seller_Reference_Search.Models
{
    public class SaleDto
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string? OwnerName { get; set; }
        public string ParcelNumber { get; set; }
        public double LotAcreage { get; set; }
        public string OfferPrice { get; set; }
        public string? OfferPPA { get; set; }
        public string? RealPPA { get; set; }
        public string? PPACalc { get; set; }
        public string? Profit { get; set; }
        public string? RetailValue { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string? ZipCode { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
