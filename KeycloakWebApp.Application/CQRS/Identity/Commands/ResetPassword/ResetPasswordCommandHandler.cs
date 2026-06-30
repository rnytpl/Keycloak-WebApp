using KeycloakWebApp.Application.Interfaces;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IIdentityService keycloakService) : IRequestHandler<ResetPasswordCommand>
{
    public Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return keycloakService.ResetPasswordEmailAsync(request.Email);
    }
}
