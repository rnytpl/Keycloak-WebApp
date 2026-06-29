using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KeycloakWebApp.Infrastructure.Identity;

public class KeycloakAuthHeaderHandler(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IMemoryCache cache) : DelegatingHandler
{
    private const string TokenCacheKey = "KeycloakAdminToken";
    private string realm = configuration["Keycloak:Realm"]!;
    private string clientId = configuration["Keycloak:AdminClientId"]!;
    private string clientSecret = configuration["Keycloak:AdminClientSecret"]!;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await GetValidTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetValidTokenAsync(CancellationToken cancellationToken)
    {

        // Retrieve token cache from in memory cache
        // if exists
        if (
            cache.TryGetValue(TokenCacheKey, out string? cachedToken) 
            && !string.IsNullOrEmpty(cachedToken)
            )
        {
            return cachedToken;
        }

        // if not fetch token
        var client = httpClientFactory.CreateClient("KeycloakTokenClient");

        var tokenRequest = new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        using var response = await client.PostAsync(string.Empty, tokenRequest, cancellationToken);

        response.EnsureSuccessStatusCode();

        // if successful
        var tokenResult = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

        var accessToken = tokenResult.GetProperty("access_token").GetString()!;

        // Cache the token, expiring it slightly before the actual expiration time to account for clock skew

        var expiresInSeconds = tokenResult.GetProperty("expires_in").GetInt32();
        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expiresInSeconds - 10));

        cache.Set(TokenCacheKey, accessToken, cacheOptions);

        return accessToken;
    }

}
