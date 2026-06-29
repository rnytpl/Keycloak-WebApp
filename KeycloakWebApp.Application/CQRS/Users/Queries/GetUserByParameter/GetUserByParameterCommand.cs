using KeycloakWebApp.Application.Common.Models;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Queries.GetUserByParameter;

public record GetUserByParameterCommand(string Parameter) : IRequest<UserDto>;
