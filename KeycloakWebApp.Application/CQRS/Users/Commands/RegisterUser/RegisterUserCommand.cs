using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string Username, 
    string Email, 
    string FirstName, 
    string LastName, 
    string Password) : IRequest<string>;
