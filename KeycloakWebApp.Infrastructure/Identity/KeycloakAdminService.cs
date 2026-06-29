using KeycloakWebApp.Application.Common.Models;
using KeycloakWebApp.Application.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace KeycloakWebApp.Infrastructure.Identity;

public class KeycloakAdminService(HttpClient httpClient) : IIdentityService
{
    public async Task<string> RegisterUserAsync(UserRegistrationDto userDto)
    {
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

        using var response = await httpClient.PostAsJsonAsync("users", userPayload);

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

            using var actionResponse = await httpClient.PutAsJsonAsync(actionsUrl, new string[] { "VERIFY_EMAIL" });

            actionResponse.EnsureSuccessStatusCode();
        }

        return "User Created Successfully";
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {

        using var response = await httpClient.GetAsync("users");

        response.EnsureSuccessStatusCode();

        var usersArray = await response.Content.ReadFromJsonAsync<JsonElement>();

        var userTasks = usersArray.EnumerateArray().Select(async user =>
        {
            var userId = user.GetProperty("id").GetString()!;

            using var roleResponse = await httpClient.GetAsync($"users/{userId}/role-mappings/realm");

            var roles = new List<string>();

            if (roleResponse.IsSuccessStatusCode)
            {
                var roleArray = await roleResponse.Content.ReadFromJsonAsync<JsonElement>();

                foreach (var role in roleArray.EnumerateArray())
                {
                    roles.Add(role.GetProperty("name").GetString()!);
                }
            }

            return new UserDto(
                userId,
                user.GetProperty("username").GetString()!,
                user.TryGetProperty("email", out var email) ? email.GetString()! : "",
                user.TryGetProperty("firstName", out var fn) ? fn.GetString()! : "",
                user.TryGetProperty("lastName", out var ln) ? ln.GetString()! : "",
                user.GetProperty("enabled").GetBoolean(),
                roles.ToArray()
            );
        });

        var result = (await Task.WhenAll(userTasks)).ToList();

        return result;
    }

    public async Task<UserDto> GetUserByParameterAsync(string parameter)
    {
        using var result = await httpClient.GetAsync($"users/{parameter}");

        if (!result.IsSuccessStatusCode)
            throw new Exception("User not found");


        var user = await result.Content.ReadFromJsonAsync<JsonElement>();

        // 2. Extract properties safely. 
        // We use TryGetProperty for things like Email or Names because Keycloak 
        // might omit them entirely from the JSON if they are empty.
        var id = user.GetProperty("id").GetString()!;

        var username = user.GetProperty("username").GetString()!;
        var email = user.TryGetProperty("email", out var e) ? e.GetString()! : string.Empty;
        var firstName = user.TryGetProperty("firstName", out var fn) ? fn.GetString()! : string.Empty;
        var lastName = user.TryGetProperty("lastName", out var ln) ? ln.GetString()! : string.Empty;
        var enabled = user.GetProperty("enabled").GetBoolean();

        // 3. Handle Roles (Important Keycloak Note below)
        var roles = new List<string>();

        using var roleResponse = await httpClient.GetAsync($"users/{id}/role-mappings/realm");
        if (roleResponse.IsSuccessStatusCode)
        {
            var roleArray = await roleResponse.Content.ReadFromJsonAsync<JsonElement>();
            foreach (var role in roleArray.EnumerateArray())
            {
                roles.Add(role.GetProperty("name").GetString()!);
            }
        }

        // 4. Return the mapped DTO
        return new UserDto(id, username, email, firstName, lastName, enabled, roles.ToArray());
    }

    public async Task AssignRoleAsync(string userId, string roleName)
    {

        using var roleResponse = await httpClient.GetAsync($"roles/{roleName}");

        if (!roleResponse.IsSuccessStatusCode)
            throw new Exception("Role not found");

        var roleJson = await roleResponse.Content.ReadFromJsonAsync<JsonElement>();

        var assignEndpoint = $"users/{userId}/role-mappings/realm";

        using var content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json");

        using var assignResponse = await httpClient.PostAsync(assignEndpoint, content, default);

        assignResponse.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoleAsync(string userId, string roleName)
    {

        using var roleResponse = await httpClient.GetAsync($"roles/{roleName}");

        if (!roleResponse.IsSuccessStatusCode)
            throw new Exception("Role not found");

        var removeEndpoint = $"users/{userId}/role-mappings/realm";

        var roleJson = await roleResponse.Content.ReadAsStringAsync();

        using var removeRequest = new HttpRequestMessage(HttpMethod.Delete, removeEndpoint)
        {
            Content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json")
        };

        using var removeResponse = await httpClient.SendAsync(removeRequest);

        removeResponse.EnsureSuccessStatusCode();
    }
}
