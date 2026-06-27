using KeycloakWebApp.Application.Interfaces;
using MediatR;

namespace KeycloakWebApp.Application.CQRS.Users.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = new Common.Models.UserRegistrationDto(
            request.Username, 
            request.Email, 
            request.FirstName, 
            request.LastName, 
            request.Password);

        return await _identityService.RegisterUserAsync(dto);
    }
}
