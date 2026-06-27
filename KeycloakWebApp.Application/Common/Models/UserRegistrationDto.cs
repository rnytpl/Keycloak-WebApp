namespace KeycloakWebApp.Application.Common.Models;

public record UserRegistrationDto(
    string Username, 
    string Email, 
    string FirstName, 
    string LastName, 
    string Password);
