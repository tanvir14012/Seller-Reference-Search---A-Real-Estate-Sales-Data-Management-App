using MediatR;

namespace Seller_Reference_Search.Models.Commands
{
    public class GetSaleDetailCommand: IRequest<SaleDto>
    {
        public int Id { get; }
        public GetSaleDetailCommand(int id)
        {
            Id = id;
        }
    }
}
