using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace KeycloakWebApp.Infrastructure.Identity;

public class KeycloakAdminService(HttpClient httpClient, IConfiguration configuration) : IIdentityService
{
    /// <summary>
    /// This method is responsible for obtaining an access token from Keycloak using the client credentials flow. 
    /// It constructs a request to the token endpoint of Keycloak, including the necessary parameters such as grant type, client ID, and client secret. 
    /// The method then sends the request and processes the response to extract and return the access token, 
    /// which is used for authenticating subsequent API calls to Keycloak's admin endpoints.
    /// </summary>
    /// <param name="grant_type">determines the flow of authentication</param>
    /// <param name="client_id">the client ID for authentication</param>
    /// <param name="client_secret">the client secret for authentication</param>
    /// <param name="authority">the base URL of the Keycloak server</param>
    /// <param name="grant_type">determines the flow of authentication</param>
    /// <param name="client_credentials">is the type of authentication flow where backend, a "machine" is trying to authenticate itself with Keycloak</param>
    /// <param name="standard_flow">is used for user authentication where users interact with Keycloak's login page</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>

    private async Task<string> GetAccessTokenAsync()
    {
        var authority = configuration["Keycloak:Authority"]!;
        var tokenEndpoint = $"{authority}/protocol/openid-connect/token1";

        var tokenRequest = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", configuration["Keycloak:AdminClientId"]!),
            new KeyValuePair<string, string>("client_secret", configuration["Keycloak:AdminClientSecret"]!)
        });

        var tokenResponse = await httpClient.PostAsync(tokenEndpoint, tokenRequest);

        var result = tokenResponse.EnsureSuccessStatusCode();

        var tokenResult = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();


        return tokenResult.GetProperty("access_token").GetString()!;
    }
    /// <summary>
    /// This method is responsible for obtaining an access token from Keycloak using the client credentials flow. 
    /// It constructs a request to the token endpoint of Keycloak, including the necessary parameters such as grant type, client ID, and client secret. 
    /// The method then sends the request and processes the response to extract and return the access token, 
    /// which is used for authenticating subsequent API calls to Keycloak's admin endpoints.
    /// </summary>
    /// <param name="grant_type">determines the flow of authentication</param>
    /// <param name="client_id">the client ID for authentication</param>
    /// <param name="client_secret">the client secret for authentication</param>
    /// <param name="authority">the base URL of the Keycloak server</param>
    /// <param name="grant_type">determines the flow of authentication</param>
    /// <param name="client_credentials">is the type of authentication flow where backend, a "machine" is trying to authenticate itself with Keycloak</param>
    /// <param name="standard_flow">is used for user authentication where users interact with Keycloak's login page</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> RegisterUserAsync(UserRegistrationDto userDto)
    {
        // Invoke the GetAccessTokenAsync method to obtain an access token for Keycloak admin API
        var authority = configuration["Keycloak:Authority"]!;
        var accessToken = await GetAccessTokenAsync();
        var adminApiUrl = authority.Replace("/realms/", "/admin/realms/") + "/users";

        // Construct the user payload to be sent to Keycloak for user creation
        var userPayload = new
        {
            username = userDto.Username,
            email = userDto.Email,
            firstName = userDto.FirstName,
            lastName = userDto.LastName,
            enabled = true,
            emailVerified = false,
            requiredActions = new[] { "VERIFY_EMAIL" },
            credentials = new[]
            {
                new { type = "password", value = userDto.Password, temporary = false }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, adminApiUrl);

        // Set the Authorization header with the obtained access token
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // Set the request content with the user payload serialized as JSON
        request.Content = new StringContent(JsonSerializer.Serialize(userPayload), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception($"Failed to create user in Keycloak: {response.StatusCode} - {error}");
        }

        var location = response.Headers.Location?.ToString();

        // Redirect the user to the email verification page if the location header is present
        if (location != null)
        {
            var redirectUri = Uri.EscapeDataString("http://localhost:3000/api/auth/signin");
            var clientId = "nextjs-client";
            var actionsUrl = $"{location}/execute-actions-email?redirect_uri={redirectUri}&client_id={clientId}";
            using var actionRequest = new HttpRequestMessage(HttpMethod.Put, actionsUrl);
            actionRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            actionRequest.Content = new StringContent(JsonSerializer.Serialize(new[] { "VERIFY_EMAIL" }), Encoding.UTF8, "application/json");
            await httpClient.SendAsync(actionRequest);
        }

        return "User Created Successfully";
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var authority = configuration["Keycloak:Authority"]!;
        var accessToken = await GetAccessTokenAsync();
        var adminApiUrl = authority.Replace("/realms/", "/admin/realms/") + "/users";

        using var request = new HttpRequestMessage(HttpMethod.Get, adminApiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var usersArray = await response.Content.ReadFromJsonAsync<JsonElement>();
        var result = new List<UserDto>();

        foreach (var user in usersArray.EnumerateArray())
        {
            var userId = user.GetProperty("id").GetString()!;

            using var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"{adminApiUrl}/{userId}/role-mappings/realm");

            roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var roleResponse = await httpClient.SendAsync(roleRequest);

            var roles = new List<string>();

            if (roleResponse.IsSuccessStatusCode)
            {
                var roleArray = await roleResponse.Content.ReadFromJsonAsync<JsonElement>();
                foreach (var role in roleArray.EnumerateArray())
                {
                    roles.Add(role.GetProperty("name").GetString()!);
                }
            }

            result.Add(new UserDto(
                userId,
                user.GetProperty("username").GetString()!,
                user.TryGetProperty("email", out var email) ? email.GetString()! : "",
                user.TryGetProperty("firstName", out var fn) ? fn.GetString()! : "",
                user.TryGetProperty("lastName", out var ln) ? ln.GetString()! : "",
                user.GetProperty("enabled").GetBoolean(),
                roles.ToArray()
            ));
        }

        return result;
    }

    public async Task<UserDto> GetUserByParameter(string parameter) {

        var authority = configuration["Keycloak:Authority"];
        //{ { baseUrl} }/ admin / realms /{ { realm} }/ users /{ { user_id} }
        var url = authority.Replace("/realms/", "/admin/realms/users/" + parameter);
        var accessToken = GetAccessTokenAsync();


        throw new Exception();
    }

    public async Task AssignRoleAsync(string userId, string roleName)
    {
        var authority = configuration["Keycloak:Authority"]!;

        var accessToken = await GetAccessTokenAsync();

        var realmRolesUrl = authority.Replace("/realms/", "/admin/realms/") + $"/roles/{roleName}";

        using var roleRequest = new HttpRequestMessage(HttpMethod.Get, realmRolesUrl);

        roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var roleResponse = await httpClient.SendAsync(roleRequest);

        if (!roleResponse.IsSuccessStatusCode)
            throw new Exception("Role not found");

        var roleJson = await roleResponse.Content.ReadAsStringAsync();

        var assignUrl = authority.Replace("/realms/", "/admin/realms/") + $"/users/{userId}/role-mappings/realm";

        using var assignRequest = new HttpRequestMessage(HttpMethod.Post, assignUrl);

        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        assignRequest.Content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json");

        var assignResponse = await httpClient.SendAsync(assignRequest);

        assignResponse.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoleAsync(string userId, string roleName)
    {
        var authority = configuration["Keycloak:Authority"]!;
        var accessToken = await GetAccessTokenAsync();
        var realmRolesUrl = authority.Replace("/realms/", "/admin/realms/") + $"/roles/{roleName}";

        using var roleRequest = new HttpRequestMessage(HttpMethod.Get, realmRolesUrl);
        roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var roleResponse = await httpClient.SendAsync(roleRequest);
        if (!roleResponse.IsSuccessStatusCode) throw new Exception("Role not found");
        var roleJson = await roleResponse.Content.ReadAsStringAsync();

        var removeUrl = authority.Replace("/realms/", "/admin/realms/") + $"/users/{userId}/role-mappings/realm";
        using var removeRequest = new HttpRequestMessage(HttpMethod.Delete, removeUrl);
        removeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        removeRequest.Content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json");
        var removeResponse = await httpClient.SendAsync(removeRequest);
        removeResponse.EnsureSuccessStatusCode();
    }


}
