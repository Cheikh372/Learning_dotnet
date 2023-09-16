using MauiApp1.View;
using MauiApp1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    /// <summary>
    /// Extension of IServicsCollection called to add services in MauiProgram.cs file
    /// </summary>
    public static class ServiesCollectionExtension
    {
        /// <summary>
        /// Adds the main page.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonServices(this IServiceCollection services)
        {

            services.AddSingleton<MainPage>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IConnectivity>(Connectivity.Current);

            return services;

        }

        /// <summary>
        /// Adds the routes.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddRoutesTransient(this IServiceCollection services)
        {

            services.AddTransient<DetailPageView>();
            services.AddTransient<DetailPageViewModel>();


            return services;
        }
    }
}
