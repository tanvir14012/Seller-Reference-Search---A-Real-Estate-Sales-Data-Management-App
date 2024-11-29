using MediatR;

namespace Seller_Reference_Search.Models.Commands
{

    public class DeleteUploadsCommand : IRequest<bool>
    {
        public int[] Ids { get; }

        public DeleteUploadsCommand(int[] ids)
        {
            Ids = ids;
        }
    }
}
