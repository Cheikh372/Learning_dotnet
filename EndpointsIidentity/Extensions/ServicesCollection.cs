using EndpointsIidentity.Infrastructures;
using EndpointsIidentity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;

namespace EndpointsIidentity.Extensions
{
    public static class ServicesCollection
    {

       public static IServiceCollection  AddServicesExtension(this IServiceCollection services , IConfiguration configuration) 
       {
            // Add the authorization services
            services.AddAuthorization();

            services.AddDbContext<EndpointsIidentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // add identity services
            services
                .AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<EndpointsIidentityDbContext>();

            

            return services;

       }
    }
}
