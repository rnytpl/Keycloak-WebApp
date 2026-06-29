using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Interfaces;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IIdentityService identityService) : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{

    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await identityService.GetUsersAsync();
    }
}
