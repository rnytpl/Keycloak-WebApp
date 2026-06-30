using KeycloakWebApp.Application.Common;
using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.CQRS.Identity.Commands.ResetPassword;

namespace KeycloakWebApp.Application.Interfaces;

public interface IIdentityService
{
    Task<string> RegisterUserAsync(UserRegistrationDto userDto);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<Result<UserDto>> GetUserByParameterAsync(string parameter);
    Task AssignRoleAsync(string userId, string roleName);
    Task RemoveRoleAsync(string userId, string roleName);
    Task<Result> ResetPasswordEmailAsync(string email);
}
