using KeycloakWebApp.Application.Common.Models;

namespace KeycloakWebApp.Application.Interfaces;

public interface IIdentityService
{
    Task<string> RegisterUserAsync(UserRegistrationDto userDto);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task AssignRoleAsync(string userId, string roleName);
    Task RemoveRoleAsync(string userId, string roleName);
}
