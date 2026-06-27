namespace KeycloakWebApp.Application.Interfaces;

public interface IKeycloakService
{
    /* -------- ACCESS TOKEN -------- */
    Task<string> GetAccessTokenAsync();

    /* -------- USERS -------- */
    Task RegisterUserAsync();
    Task GetUsersAsync();
    Task GetUserByIdAsync(Guid id);
    Task DeleteUserAsync();

    Task AddRealmRoleAsync();
    Task AddClientRoleAsync();
    Task UpdateUserAsyns();

    /* -------- REALMS --------  */
    Task CreateRealmRole();
    Task DeleteRealmRole();

    /* -------- AUTH -------- */
    Task UserLogin();

    /* -------- ROLES -------- */

    // Realms
    Task GetRealmRolesAsync();
    Task CreateRealmRoleAsync();

    // Clients
    Task GetClientRolesAsync();
    Task CreateClientRoleAsync();

}
