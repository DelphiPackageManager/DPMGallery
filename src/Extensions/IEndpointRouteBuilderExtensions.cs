using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Text;

namespace DPMGallery.Extensions
{
    public static class IEndpointRouteBuilderExtensions
    {
        public static void MapApiRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: RouteConstants.IndexRouteName,
                pattern: "api/v1/index.json",
                defaults: new { controller = "ServiceIndex", action = "Get" });

            endpoints.MapControllerRoute(
                name: RouteConstants.UploadPackageRouteName,
                pattern: "api/v1/package",
                defaults: new { controller = "PackagePublish", action = "PushPackage" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("PUT") });

            endpoints.MapControllerRoute(
                name: RouteConstants.SearchRouteName,
                pattern: "api/v1/search",
                defaults: new { controller = "Search", action = "Search" });

            endpoints.MapControllerRoute(
                name: RouteConstants.DelistRouteName,
                pattern: RouteConstants.Templates.PackageDelistTemplate,
                defaults: new { controller = "PackagePublish", action = "Delist" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("DELETE") });

            endpoints.MapControllerRoute(
                name: RouteConstants.PackageVersionsRouteName,
                pattern: RouteConstants.Templates.PackageVersionsTemplate,
                defaults: new { controller = "PackageContent", action = "GetPackageVersions" });

            endpoints.MapControllerRoute(
                            name: RouteConstants.PackageVersionsWithDepsRouteName,
                            pattern: RouteConstants.Templates.PackageVersionsWithDepsTemplate,
                            defaults: new { controller = "PackageContent", action = "GetPackageVersionsWithDependencies" });

            endpoints.MapControllerRoute(
                name: RouteConstants.PackageDownloadRouteName,
                pattern: RouteConstants.Patterns.PackageDownloadFile,
                defaults: new { controller = "PackageContent", action = "DownloadFile" });

            endpoints.MapControllerRoute(
                name: RouteConstants.PackageDetailsRouteName,
                pattern: RouteConstants.Patterns.PackageDetails,
                defaults: new { controller = "Packages", action = "Details" });

        }

    }
}
