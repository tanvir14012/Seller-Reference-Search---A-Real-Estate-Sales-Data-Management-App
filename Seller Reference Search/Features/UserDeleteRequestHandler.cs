using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models.Commands;

namespace PrivilegePro.Features
{
    public class UserDeleteRequestHandler : IRequestHandler<DeleteUsersCommand, bool>
    {
        private readonly UserManager<AppUser> _userManager;

        public UserDeleteRequestHandler(
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(DeleteUsersCommand request, CancellationToken cancellationToken)
        {
            if(request.Ids.Length == 1 && request.Ids[0] == -1)
            {
                // Get all users
                var users = _userManager.Users.ToList();

                // Find users who are in the "Admin" role
                var adminRoleUsers = new List<AppUser>();
                foreach (var user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        adminRoleUsers.Add(user);
                    }
                }

                // Find users who are not in the "Admin" role
                var nonAdminUsers = users.Except(adminRoleUsers).ToList();

                if (nonAdminUsers.Count == 0)
                    throw new Exception("Administrator users can not be deleted.");

                // Delete non-admin users
                foreach (var user in nonAdminUsers)
                {
                    await _userManager.DeleteAsync(user);

                }
            }
            else
            {

                var usersToDelete = new List<AppUser>();

                foreach (var userId in request.Ids)
                {
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                    {
                        continue;
                    }

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        continue; // Skip admin users
                    }

                    usersToDelete.Add(user);
                }

                if (usersToDelete.Count == 0)
                    throw new Exception("Administrator users can not be deleted.");

                foreach (var user in usersToDelete)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            return true;
        }
    }
}
