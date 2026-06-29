using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Interfaces;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Queries.GetUserByParameter;

public class GetUserByParameterCommandHandler(IIdentityService identityService) : IRequestHandler<GetUserByParameterCommand, UserDto>
{
    public Task<UserDto> Handle(GetUserByParameterCommand request, CancellationToken cancellationToken)
    {
        return identityService.GetUserByParameterAsync(request.Parameter);
    }
}
