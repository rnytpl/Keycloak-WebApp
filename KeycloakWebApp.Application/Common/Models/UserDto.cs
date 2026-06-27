namespace KeycloakWebApp.Application.Common.Models;

public record UserDto(
    string Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    bool Enabled,
    string[] Roles);
