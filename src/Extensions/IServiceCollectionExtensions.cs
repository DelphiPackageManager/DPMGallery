using DPMGallery.Antivirus;
using DPMGallery.BackgroundServices;
using DPMGallery.Configuration;
using DPMGallery.Data;
using DPMGallery.Repositories;
using DPMGallery.Services;
using DPMGallery.Storage;
using DPMGallery.Storage.Amazon;
using DPMGallery.Storage.Google;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DPMGallery.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDPMServices(this IServiceCollection services, ServerConfig configuration)
        {
            //our dapper wrapper
            services.AddSingleton<IDbConnectionFactory>(s => new DbConnectionFactory(configuration.Database.ConnectionString));
            services.AddSingleton<IDbContextFactory, DbContextFactory>();

            //scoped means single instance per request. That makes transactions across repos possible.            
            services.AddScoped<IDbContext>((s) =>
            {
                var factory = s.GetRequiredService<IDbContextFactory>();
                return factory.CreateDbContext();
            });

            services.AddScoped<IUnitOfWork>((s) =>
            {
                return s.GetRequiredService<IDbContext>() as IUnitOfWork;
            });

            //
            services.AddScoped<ApiKeyRepository>();
            services.AddScoped<PackageRepository>();
            services.AddScoped<TargetPlatformRepository>();
            services.AddScoped<PackageVersionRepository>();
            services.AddScoped<PackageOwnerRepository>();
            services.AddScoped<PackageVersionProcessRepository>();
            services.AddScoped<ReservedPrefixRepository>();
            services.AddScoped<OrganisationRepository>();
            services.AddScoped<SearchRepository>();


            services.AddScoped<IAntivirusService, ClamAVService>();
            services.AddScoped<IAntivirusService, VirusTotalService>();

            services.AddScoped<IServiceIndexService, ServiceIndexService>();
            services.AddScoped<IPackageContentService, PackageContentService>();
            services.AddScoped<IPackageIndexService, PackageIndexService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IUIService, UIService>();

            services.AddTransient<FileStorageService>();

            services.AddS3StorageService();
            services.AddGoogleCloudStorageService();

            services.AddTransient<IStorageService>(provider =>
            {
                var config = provider.GetRequiredService<ServerConfig>();

                return config.Storage.StorageType switch
                {
                    StorageType.FileSystem => provider.GetRequiredService<FileStorageService>(),
                    StorageType.AwsS3 => provider.GetRequiredService<S3StorageService>(),
                    StorageType.GoogleCloudStorage => provider.GetRequiredService<GoogleCloudStorageService>(),
                    _ => throw new InvalidOperationException($"Unsupported storage service: {config.Storage.StorageType}"),
                };
            });

            services.AddHostedService<PackageIndexBackgroundService>();


            return services;
        }
    }
}
