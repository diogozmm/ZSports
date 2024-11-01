using Microsoft.Extensions.Options;
using ZSports.Keycloak.Client;
using ZSports.Keycloak.Options;

namespace Api.DependencyInjection
{
    public static class ClientsDependencyInjectionExtension
    {
        public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IKeycloakClient, KeycloakClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                if (string.IsNullOrEmpty(options.Url))
                    throw new InvalidOperationException("Keycloak URL is not configured.");

                client.BaseAddress = new Uri(options.Url);
            });

            services.AddScoped<IKeycloakClient, KeycloakClient>();

            return services;
        }
    }
}
