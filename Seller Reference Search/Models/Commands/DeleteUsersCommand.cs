using MediatR;

namespace Seller_Reference_Search.Models.Commands
{
    public class DeleteUsersCommand : IRequest<bool>
    {
        public int[] Ids { get; }

        public DeleteUsersCommand(int[] ids)
        {
            Ids = ids;
        }
    }

}
