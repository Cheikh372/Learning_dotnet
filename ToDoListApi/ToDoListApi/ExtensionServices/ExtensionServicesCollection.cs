using Microsoft.EntityFrameworkCore;
using ToDoListApi.Infrastructure;
using ToDoListApi.Services;

namespace ToDoListApi.ExtensionServices
{
    public static class ExtensionServicesCollection
    {
        /// <summary>
        /// Adds the database service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddDbService(this IServiceCollection services, IConfiguration configuration) 
        {
            // Get ConnectionString to link it at DbContext
            var conn = configuration.GetConnectionString("Default");

            // Register AppDbContext to using SqlServer
            services.AddDbContext<ToDoListDbContext>(db => db.UseSqlServer(conn));

            return services;
        }


        /// <summary>
        /// Adds the application service.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddAppService(this IServiceCollection services)
        {
            //services.AddScoped(sp => new HttpClient { });

            services.AddScoped<ITaskServices, TaskServices>();
            // add other appService here

            return services;
        }


        public static IServiceCollection AddMapService(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingService));

            return services;
        }
    }
}
