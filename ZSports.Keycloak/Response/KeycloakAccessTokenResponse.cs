using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ZSports.Keycloak.Response
{
    public class KeycloakAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;
        public string TokenType { get; set; } = default!;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = default!;
        public int RefreshExpiresIn { get; set; }
    }

}
