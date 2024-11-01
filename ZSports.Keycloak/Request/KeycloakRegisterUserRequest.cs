namespace ZSports.Keycloak.Request
{
    public class KeycloakRegisterUserRequest
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

}
