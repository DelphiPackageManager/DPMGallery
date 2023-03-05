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
                name: Constants.RouteNames.IndexRouteName,
                pattern: "api/v1/index.json",
                defaults: new { controller = "ServiceIndex", action = "Get" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.UploadPackageRouteName,
                pattern: Constants.RoutePatterns.PackagePublish,
                defaults: new { controller = "PackagePublish", action = "PushPackage" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("PUT") });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.DelistRouteName,
                pattern: Constants.RoutePatterns.PackageDelist,
                defaults: new { controller = "PackagePublish", action = "Delist" },
                constraints: new { httpMethod = new HttpMethodRouteConstraint("DELETE") });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.SearchRouteName,
                pattern: Constants.RoutePatterns.PackageSearch,
                defaults: new { controller = "Search", action = "Search" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.SearchIdsRouteName,
                pattern: Constants.RoutePatterns.PackageSearchIds,
                defaults: new { controller = "Search", action = "SearchByIds" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.SearchIdsRouteName,
                pattern: Constants.RoutePatterns.PackageFind,
                defaults: new { controller = "Search", action = "Find" });


            endpoints.MapControllerRoute(
                name: Constants.RouteNames.ListRouteName,
                pattern: Constants.RoutePatterns.PackageList,
                defaults: new { controller = "Search", action = "List" });


            endpoints.MapControllerRoute(
                name: Constants.RouteNames.PackageVersionsRouteName,
                pattern: Constants.RoutePatterns.PackageVersions,
                defaults: new { controller = "PackageContent", action = "GetPackageVersions" });

            endpoints.MapControllerRoute(
                            name: Constants.RouteNames.PackageVersionsWithDepsRouteName,
                            pattern: Constants.RoutePatterns.PackageVersionsWithDeps,
                            defaults: new { controller = "PackageContent", action = "GetPackageVersionsWithDependencies" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.PackageDownloadRouteName,
                pattern: Constants.RoutePatterns.PackageInfo,
                defaults: new { controller = "PackageContent", action = "GetPackageInfo" });


            endpoints.MapControllerRoute(
                name: Constants.RouteNames.PackageDownloadRouteName,
                pattern: Constants.RoutePatterns.PackageDownloadFile,
                defaults: new { controller = "PackageContent", action = "DownloadFile" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.PackageDetailsRouteName,
                pattern: Constants.RoutePatterns.PackageDetails,
                defaults: new { controller = "Packages", action = "Details" });

            endpoints.MapControllerRoute(
                name: Constants.RouteNames.PackageReportRouteName,
                pattern: Constants.RoutePatterns.PackageReport,
                defaults: new { controller = "Packages", action = "Report" });

        }

    }
}
