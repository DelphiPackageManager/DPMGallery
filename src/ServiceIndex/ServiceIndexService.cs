using DPMGallery.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Services
{
	public class ServiceIndexService : IServiceIndexService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public ServiceIndexService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}


		private string GenerateInternalUrl(string relativePath)
		{
			var request = _httpContextAccessor.HttpContext.Request;

			return string.Concat(
				request.Scheme,
				"://",
				request.Host.ToUriComponent(),
				request.PathBase.ToUriComponent(),
				"/",
				relativePath);
		}

		private static ServiceIndexResponseDTO _cached = null;

		public Task<ServiceIndexResponseDTO> GetAsync(CancellationToken cancellationToken = default)
		{
			if (_cached != null)
				return Task.FromResult(_cached); //saves a few ms!

			ServiceIndexResponseDTO response = new ServiceIndexResponseDTO()
			{
				Version = "1.0.0",
				Resources = new List<ServiceIndexItemDTO>()
				{
					new ServiceIndexItemDTO ()
					{
						ResourceType = Constants.ResourceNames.PackagePublish,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackagePublish),
						Comment = "The url for publishing DPM packages (PUT)."
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageList,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageList),
						Comment = "List endpoint of DPM Search service (GET)"
					},

					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageSearch,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageSearch),
						Comment = "Search endpoint of DPM Search service (GET)"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageVersions,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageVersions),
						Comment = $"Get a list of package versions in the format {Constants.DefaultSiteUrl}/{Constants.RouteTemplates.PackageVersions} (GET)"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageDownload,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageDownload),
						Comment = $"Base url of where DPM package files are stored, in the format {Constants.DefaultSiteUrl}/{Constants.RouteTemplates.PackageDownload} (GET)"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageMetadata,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageMetadata),
						Comment = $"Base URL of where DPM package metadata is stored, in the format {Constants.DefaultSiteUrl}/{Constants.RouteTemplates.PackageMetadata} (GET)"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageReadMe,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageReadme),
						Comment = $"Base URL of where DPM package readme is stored, in the format {Constants.DefaultSiteUrl}/{Constants.RouteTemplates.PackageReadme} (GET)"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = Constants.ResourceNames.PackageIcon,
						ResourceUrl = GenerateInternalUrl(Constants.ResourceUri.PackageIcon),
						Comment = $"Template URL of where DPM package icon is stored, in the format {Constants.DefaultSiteUrl}/{Constants.RouteTemplates.PackageIcon} (GET)"
					},
				}
			};

			_cached = response;
			return Task.FromResult(response);
		}
	}
}
