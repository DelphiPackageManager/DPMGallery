using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DPMGallery.Storage.Google
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleCloudStorageService(this IServiceCollection services)
        {
            services.AddTransient<GoogleCloudStorageService>();
            return services;
        }
    }
}
