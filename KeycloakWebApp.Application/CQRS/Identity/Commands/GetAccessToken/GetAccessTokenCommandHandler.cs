using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace KeycloakWebApp.Application.CQRS.Identity.Commands.GetAccessToken;

public class GetAccessTokenCommandHandler(IIdentityService keycloakService, IConfiguration configuration) : IRequestHandler<GetAccessTokenCommand>
{
    public async Task Handle(GetAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var dto = new AccessTokenDto("password",
            configuration.GetSection("Keycloak:AdminClientId").Value,
            configuration.GetSection("Keycloak:AdminClientSecret").Value,
            request.username, request.password);


        //var result = await keycloakService.GetAccessTokenAsync();

        throw new NotImplementedException();
    }
}
