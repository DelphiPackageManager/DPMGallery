using Microsoft.Extensions.DependencyInjection;

namespace DPMGallery.Storage.BunnyCDN
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBunnyCDNStorageService(this IServiceCollection services)
        {
            services.AddTransient<BunnyCDNStorageService>();
            return services;
        }

    }
}
