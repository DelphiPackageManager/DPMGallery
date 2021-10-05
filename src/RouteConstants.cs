using System;
using System.Collections.Generic;
using System.Text;

namespace DPMGallery
{
	public class RouteConstants
	{
		public const string IndexRouteName = "index";
		public const string SearchRouteName = "query";
		public const string UploadPackageRouteName = "uploadpackage";
		public const string DelistRouteName = "delistpackage";

		public const string PackageDetailsRouteName = "package-details";
		public const string PackageVersionsRouteName = "package-versions";
		public const string PackageVersionsWithDepsRouteName = "package-versions-deps";
		public const string PackageDownloadRouteName = "package-download";
		
		public const string PackageDownloadMetadataRouteName = "package-download-manifest";
		public const string PackageDownloadReadmeRouteName = "package-download-readme";
		public const string PackageDownloadIconRouteName = "package-download-icon";


		public static class Templates
        {
			public const string PackageDetailsTemplate = "packages/{id}/{compilerVersion}/{platform}/{version}";
			public const string PackageDelistTemplate	= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/delist";
			public const string PackageVersionsTemplate = "api/v1/package/{id}/{compilerVersion}/{platform}/versions";
			public const string PackageVersionsWithDepsTemplate = "api/v1/package/{id}/{compilerVersion}/{platform}/versionswithdependencies";
			public const string PackageDownloadTemplate = "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/dpkg";
			public const string PackageMetadataTemplate = "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/dspec";
			public const string PackageReadmeTemplate	= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/readme";
			public const string PackageIconTemplate		= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/icon";
		}

		public static class Patterns
        {
			public const string PackageDownloadFile = "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/{fileType}";
            public const string PackageDetails		= Templates.PackageDetailsTemplate;
        }

	}
}
