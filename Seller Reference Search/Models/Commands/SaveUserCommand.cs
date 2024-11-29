using MediatR;

namespace Seller_Reference_Search.Models.Commands
{
    public class SaveUserCommand: IRequest<AppUserDto>
    {
        public AppUserDto AppUserDto { get; set; }
        public SaveUserCommand(AppUserDto model)
        {
            AppUserDto = model;
        }
    }
}
