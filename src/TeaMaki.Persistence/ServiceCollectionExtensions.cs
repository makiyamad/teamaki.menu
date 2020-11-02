using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeaMaki.Menu;

namespace TeaMaki.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistenceLibrary(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(RavenDbRepository<>));
            services.AddSingleton<IRavenDbContext, RavenDbContext>();

            services.Configure<PersistenceSettings>(configuration);

            return services;
        }
    }
}
