using ZSports.Core.Interfaces.Repositories;
using ZSports.Repository;
using ZSports.Repository.Repositories;

namespace Api.DependencyInjection
{
    public static class RepositoryDependencyInjectionExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
