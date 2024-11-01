namespace ZSports.Keycloak.Request
{
    public class KeycloakLoginUserRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

}
