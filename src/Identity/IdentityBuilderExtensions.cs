using DPMGallery.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace DPMGallery.Identity
{
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        /// Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder)
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType);
            //var options = GetDefaultOptions();
            //dbProviderOptionsAction?.Invoke(options);
            //builder.Services.AddSingleton(options);

            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType)
        {
            if (userType != typeof(User))
                throw new InvalidOperationException($"{nameof(AddDapperStores)} can only be called with a user that is of type {nameof(User)}.");

            if (roleType != null)
            {
                if (roleType != typeof(Role))
                    throw new InvalidOperationException($"{nameof(AddDapperStores)} can only be called with a role that is of type {nameof(Role)}.");

                services.TryAddScoped<IEmailSender, EmailSender>();
                services.TryAddScoped<IRoleStore<Role>, RoleStore>();
                services.TryAddScoped<IUserStore<User>, UserStore>();
                services.TryAddScoped<IUserEmailStore<User>, UserStore>();

                services.TryAddTransient<UserManager<User>>();
            }
        }

    }
}
