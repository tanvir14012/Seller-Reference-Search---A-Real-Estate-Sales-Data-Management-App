using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models;
using Seller_Reference_Search.Models.Commands;
using System.Text;

namespace PrivilegePro.Features
{
    public class UserSaveRequestHandler : IRequestHandler<SaveUserCommand, AppUserDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserSaveRequestHandler(UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<AppUserDto> Handle(SaveUserCommand request, CancellationToken cancellationToken)
        {
            var appUser = _mapper.Map<AppUser>(request.AppUserDto);
            var isCreated = false;
            AppUser appUserEntity = null;

            if (appUser.Id == 0)
            {
                // Create the new user
                var password = appUser.PasswordHash;
                appUser.PasswordHash = null;
                appUser.UserName = await GenerateUniqueUsernameAsync(appUser.FirstName);
                var result = await _userManager.CreateAsync(appUser, password);
                if (!result.Succeeded)
                {
                    throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                isCreated = true;
            }
            else
            {
                appUserEntity = await _userManager.FindByIdAsync(appUser.Id.ToString());
                if (appUserEntity != null)
                {
                    var passwordHash = appUserEntity.PasswordHash;
                    _mapper.Map(request.AppUserDto, appUserEntity);
                    var password = request.AppUserDto.Password;
                    appUserEntity.PasswordHash = passwordHash;
                    var result = await _userManager.UpdateAsync(appUserEntity);
                    if (!result.Succeeded)
                    {
                        throw new Exception("User update failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                    var token = await _userManager.GeneratePasswordResetTokenAsync(appUserEntity);
                    var passwordChangeResult = await _userManager.ResetPasswordAsync(appUserEntity, token, password);
                    if (!passwordChangeResult.Succeeded)
                        throw new Exception("Password could not be reset!");
                }
                else
                {
                    throw new Exception("App user is not found");
                }
            }

            // Map back to DTO
            var appUserDto = _mapper.Map<AppUserDto>(isCreated ? appUser : appUserEntity);
            return appUserDto;
        }

        protected async Task<string> GenerateUniqueUsernameAsync(string firstName)
        {
            string baseUsername = CapitalizeName(firstName);
            string username = baseUsername;
            int counter = 1;

            while (await _userManager.FindByNameAsync(username) != null)
            {
                username = $"{baseUsername}_{counter}";
                counter++;
            }

            return username;
        }

        protected static string CapitalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "User";
            }

            name = name.Replace(" ", "_");

            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }


    }
}
