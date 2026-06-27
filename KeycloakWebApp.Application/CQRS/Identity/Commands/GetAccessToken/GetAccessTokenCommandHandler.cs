using MediatR;

namespace KeycloakWebApp.Application.CQRS.Identity.Commands.GetAccessToken;

public class GetAccessTokenCommandHandler : IRequestHandler<GetAccessTokenCommand, int>
{
    public async Task<int> Handle(GetAccessTokenCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
