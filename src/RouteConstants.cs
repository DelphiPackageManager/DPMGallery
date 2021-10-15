using System;
using System.Collections.Generic;
using System.Text;

namespace DPMGallery
{
	public partial class Constants
	{
		public static class RouteNames
		{

			public const string IndexRouteName = "index";
			public const string SearchRouteName = "search";
			public const string SearchIdsRouteName = "searchbyids";
			public const string ListRouteName = "list";
			public const string UploadPackageRouteName = "uploadpackage";
			public const string DelistRouteName = "delistpackage";

			public const string PackageDetailsRouteName = "package-details";
			public const string PackageVersionsRouteName = "package-versions";
			public const string PackageVersionsWithDepsRouteName = "package-versions-deps";
			public const string PackageDownloadRouteName = "package-download";
			public const string PackageInfoRouteName = "package-info";

			public const string PackageDownloadMetadataRouteName = "package-download-manifest";
			public const string PackageDownloadReadmeRouteName = "package-download-readme";
			public const string PackageDownloadIconRouteName = "package-download-icon";
		}

		//service index resource ids
		public static class ResourceNames
        {
			public const string ServiceIndes	= "api/v1/index.json";
			public const string PackagePublish	= "PackagePublish";
			public const string PackageList		= "PackageList";
			public const string PackageSearch	= "PackageSearch";
			public const string PackageSearchIds = "PackageSearchIds";
			public const string PackageVersions = "PackageVersions";
			public const string PackageDownload = "PackageDownload";
			public const string PackageMetadata = "PackageMetadata";
			public const string PackageIcon		= "PackageIcon";
			public const string PackageReadMe	= "PackageReadme";
			public const string PackageInfo		= "PackageInfo";
			public const string PackageVersionsWithDeps = "PackageVersionsWithDeps";
		}

		//service index resource uri
		public static class ResourceUri
        {
			public const string PackagePublish	= "api/v1/package";
			public const string PackageDownload = "api/v1/package";
			public const string PackageList		= "api/v1/list";
			public const string PackageSearch	= "api/v1/search";
			public const string PackageSearchIds = "api/v1/searchbyids";
			public const string PackageVersions = "api/v1/package";
			public const string PackageMetadata = "api/v1/package";
			public const string PackageIcon		= "api/v1/package";
			public const string PackageReadme	= "api/v1/package";
			public const string PackageInfo		= "api/v1/package";
			public const string PackageVersionsWithDeps = "api/v1/package";

		}

		//used by the serviceindex in comment
		public static class RouteTemplates
        {
			public const string PackageDetails			= "packages/{id}/{compilerVersion}/{platform}/{version}";
			public const string PackageDelist			= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/delist";
			public const string PackageVersions			= "api/v1/package/{id}/{compilerVersion}/{platform}/versions";
			public const string PackageVersionsWithDeps = "api/v1/package/{id}/{compilerVersion}/{platform}/{versionRange}/versionswithdependencies";
			public const string PackageDownload			= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/dpkg";
			public const string PackageMetadata			= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/dspec";
			public const string PackageReadme			= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/readme";
			public const string PackageIcon				= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/icon";
			public const string PackageInfo				= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/info";
		}

		//used for asp.net routing
		public static class RoutePatterns
        {
			public const string PackageDelist			= RouteTemplates.PackageDelist;
			public const string PackagePublish			= ResourceUri.PackagePublish;
			public const string PackageInfo				= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/info";

			//sharing route  across multiple resources.
			public const string PackageDownloadFile		= "api/v1/package/{id}/{compilerVersion}/{platform}/{version}/{fileType}";
            public const string PackageDetails			= RouteTemplates.PackageDetails;
			public const string PackageVersions			= RouteTemplates.PackageVersions;
			public const string PackageVersionsWithDeps = RouteTemplates.PackageVersionsWithDeps;

			public const string PackageList				= ResourceUri.PackageList;
			public const string PackageSearch			= ResourceUri.PackageSearch;
			public const string PackageSearchIds		= ResourceUri.PackageSearchIds;
		}
	}
}
