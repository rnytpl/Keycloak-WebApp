using MediatR;

namespace KeycloakWebApp.Application.CQRS.Identity.Commands.ResetPassword;

public record ResetPasswordCommand(string Email) : IRequest;

