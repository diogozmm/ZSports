using ZSports.Core.Interfaces.Repositories;
using ZSports.Repository.Repositories;
using ZSports.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.DependencyInjection
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtToken(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["JwtSettings:Issuer"];
                    options.Audience = configuration["JwtSettings:Audience"]; // Client ID created in Keycloak
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtSettings:Audience"],
                        ValidateLifetime = true
                    };
                });

            return services;
        }
    }
}
