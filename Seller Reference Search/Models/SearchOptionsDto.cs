namespace Seller_Reference_Search.Models
{
    public class SearchOptionsDto
    {
        //public double MaxAcres { get; set; }
        //public decimal OfferPriceMax { get; set; }
        //public DateOnly ClosingDateMax { get; set; }

        //public IEnumerable<string> States{ get; set; }

        //public IEnumerable<string> Counties { get; set; }

        public string json { get; set; }

        public bool Errored { get; set; }
        public string? Message { get; set; }
    }
}
