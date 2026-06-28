namespace KeycloakWebApp.Application.Common.Models;

public record AccessTokenDto(string? grantType, string? clientId, string? clientSecret, string username, string password);