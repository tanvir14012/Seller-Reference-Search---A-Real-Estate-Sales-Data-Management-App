using Seller_Reference_Search.Infrastructure.Interfaces;

namespace Seller_Reference_Search.Infrastructure.Data.Models
{
    public class County: BaseEntity, IAggregateRoot
    {
        public County(int Id, string CountyName, string StateCode)
        {
            this.Id = Id;
            this.CountyName = CountyName;
            this.StateCode = StateCode;
        }
        public string CountyName { get; set; }
        public string StateCode { get; set; }
    }
}
