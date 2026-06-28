using MediatR;

namespace KeycloakWebApp.Application.CQRS.Identity.Commands.GetAccessToken;

public record GetAccessTokenCommand(string? grantType, string? clientId, string? clientSecret, string? username, string? password) : IRequest;
