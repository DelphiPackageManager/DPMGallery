﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


namespace DPMGallery.Storage.Amazon
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddS3StorageService(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<ServerConfig>().Storage.S3Storage;

                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region)
                };

                if (options.UseInstanceProfile)
                {
                    var credentials = FallbackCredentialsFactory.GetCredentials();
                    return new AmazonS3Client(credentials, config);
                }

                if (!string.IsNullOrEmpty(options.AssumeRoleArn))
                {
                    var credentials = FallbackCredentialsFactory.GetCredentials();
                    var assumedCredentials = AwsIamHelper
                        .AssumeRoleAsync(credentials, options.AssumeRoleArn, $"DPM-Session-{Guid.NewGuid()}").GetAwaiter().GetResult();

                    return new AmazonS3Client(assumedCredentials, config);
                }


                if (!string.IsNullOrEmpty(options.AccessKey))
                    return new AmazonS3Client(new BasicAWSCredentials(options.AccessKey, options.SecretKey), config);

                return new AmazonS3Client(config);
            });

            services.AddTransient<S3StorageService>();

            return services;
        }
    }
}
