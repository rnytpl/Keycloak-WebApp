using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Commands.AssignRole;

public record AssignRoleCommand(string UserId, string RoleName) : IRequest;
