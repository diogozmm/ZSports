using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ZSports.Keycloak.Options;
using Microsoft.Extensions.Options;
using ZSports.Keycloak.Request;
using ZSports.Keycloak.Response;
using System.Net.Http.Json;

namespace ZSports.Keycloak.Client
{
    public class KeycloakClient : IKeycloakClient
    {
        private readonly HttpClient _httpClient;
        private readonly KeycloakOptions _options;

        public KeycloakClient(HttpClient httpClient, IOptions<KeycloakOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress = new Uri(options.Value.Url);
        }

        public async Task<string> GetAdminAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.Url}/realms/master/protocol/openid-connect/token");

            request.Content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", "admin-cli"),
            new KeyValuePair<string, string>("username", _options.AdminUsername),
            new KeyValuePair<string, string>("password", _options.AdminPassword),
            new KeyValuePair<string, string>("grant_type", "password")
        });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

            return tokenResponse.GetProperty("access_token").GetString()!;
        }

        public async Task<bool> RegisterUserAsync(KeycloakRegisterUserRequest request)
        {
            var accessToken = await GetAdminAccessTokenAsync();

            var userPayload = new
            {
                username = request.Username,
                email = request.Email,
                enabled = true,
                requiredActions = new string[0],
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = request.Password,
                        temporary = false
                    }
                }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_options.Url}/admin/realms/{_options.Realm}/users");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(userPayload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            return response.IsSuccessStatusCode;
        }


        public async Task<KeycloakAccessTokenResponse> LoginUserAsync(KeycloakLoginUserRequest request)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("username", request.Username),
                new KeyValuePair<string, string>("password", request.Password),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var response = await _httpClient.PostAsync($"{_options.Url}/realms/{_options.Realm}/protocol/openid-connect/token", formContent);
            if (!response.IsSuccessStatusCode)
            {
                return null!;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<KeycloakAccessTokenResponse>(responseBody)!;
        }

        public async Task<KeycloakUser> GetUserByEmailAsync(string email)
        {
            var token = await GetAdminAccessTokenAsync();
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Failed to retrieve admin token.");

            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Keycloak API endpoint to get users by email
            var response = await _httpClient.GetAsync($"admin/realms/{_options.Realm}/users?email={email}");

            if (!response.IsSuccessStatusCode)
                return null!;

            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUser>>();
            return users?.FirstOrDefault()!;
        }
    }
}
