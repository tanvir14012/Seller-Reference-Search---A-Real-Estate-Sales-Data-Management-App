using MediatR;

namespace Seller_Reference_Search.Models.Commands
{
    public class PrepareSearchCommand: IRequest<SearchOptionsDto>
    {
        public SearchOptionsDto SearchOptionsDto { get; }
        public PrepareSearchCommand(SearchOptionsDto dto)
        {
            SearchOptionsDto = dto;
        }

    }
}
