using KeycloakWebApp.Application.Common.Models;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;
