namespace KeycloakWebApp.Application.CQRS.Products.DTOs.Auth;

public record AccessTokenDto(string? grantType, string? clientId, string? clientSecret, string username, string password);