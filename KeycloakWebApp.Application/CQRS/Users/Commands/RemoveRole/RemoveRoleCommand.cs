using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Commands.RemoveRole;

public record RemoveRoleCommand(string UserId, string RoleName) : IRequest;
