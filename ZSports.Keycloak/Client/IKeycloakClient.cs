using ZSports.Keycloak.Request;
using ZSports.Keycloak.Response;

namespace ZSports.Keycloak.Client
{
    public interface IKeycloakClient
    {
        Task<bool> RegisterUserAsync(KeycloakRegisterUserRequest request);
        Task<KeycloakAccessTokenResponse> LoginUserAsync(KeycloakLoginUserRequest request);
        Task<KeycloakUser> GetUserByEmailAsync(string email);
    }
}
