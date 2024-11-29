using MediatR;
using Seller_Reference_Search.Models.DataTable;

namespace Seller_Reference_Search.Models.Commands
{
    public class SearchSaleCommand: IRequest<string>
    {
        public DtParameters<SaleDto> Model { get; }
        public IFormCollection Form { get; }

        public SearchSaleCommand(DtParameters<SaleDto> model, IFormCollection form)
        {
            Model = model;
            Form = form;
        }

    }
}
