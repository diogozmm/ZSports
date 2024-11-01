using ZSports.Core.Interfaces.Services;
using ZSports.Services;

namespace Api.DependencyInjection
{
    public static class ServicesDependencyInjectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
