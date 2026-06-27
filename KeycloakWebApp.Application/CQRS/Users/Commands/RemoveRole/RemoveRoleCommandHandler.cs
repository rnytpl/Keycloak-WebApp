using KeycloakWebApp.Application.Interfaces;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Commands.RemoveRole;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand>
{
    private readonly IIdentityService _identityService;

    public RemoveRoleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        await _identityService.RemoveRoleAsync(request.UserId, request.RoleName);
    }
}
