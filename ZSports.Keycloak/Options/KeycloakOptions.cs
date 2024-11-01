namespace ZSports.Keycloak.Options
{
    public class KeycloakOptions
    {
        public string Url { get; set; } = default!;
        public string Realm { get; set; } = default!;
        public string Client { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string AdminUsername { get; set; } = default!;
        public string AdminPassword { get; set; } = default!;
    }

}
