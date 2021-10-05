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
						ResourceType = "PackagePublish",
						ResourceUrl = GenerateInternalUrl("api/v1/package"),
						Comment = "The url for publishing DPM packages."
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "SearchService",
						ResourceUrl = GenerateInternalUrl("api/v1/search"),
						Comment = "Search endpoint of DPM Search service"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "SearchAutocompleteService",
						ResourceUrl = GenerateInternalUrl("api/v1/autocomplete"),
						Comment = "Autocomplete endpoint of DPM Search service"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageDetailsTemplate",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageDetailsTemplate),
						Comment = $"Template URL of where the package details can be viewed on the server, in the format https://delphipm.org/{RouteConstants.Templates.PackageDetailsTemplate}"
					},

					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageDownloadTemplate",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageDownloadTemplate),
						Comment = $"Template URL of where DPM package content is stored, in the format https://delphipm.org/{RouteConstants.Templates.PackageDownloadTemplate}"
					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageMetaDataTemplate",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageMetadataTemplate),
						Comment = $"Base URL of where DPM package metadata is stored, in the format https://delphipm.org/{RouteConstants.Templates.PackageMetadataTemplate}"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageReadMeTemplate",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageReadmeTemplate),
						Comment = $"Base URL of where DPM package readme is stored, in the format https://delphipm.org/{RouteConstants.Templates.PackageReadmeTemplate}"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageIconTemplate",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageIconTemplate),
						Comment = $"Template URL of where DPM package icon is stored, in the format https://delphipm.org/{RouteConstants.Templates.PackageIconTemplate}"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageIcon",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageIconTemplate),
						Comment = $"Template URL of where DPM package icon is stored, in the format https://delphipm.org/{RouteConstants.Templates.PackageIconTemplate}"

					},
					new ServiceIndexItemDTO()
					{
						ResourceType = "PackageVersions",
						ResourceUrl = GenerateInternalUrl(RouteConstants.Templates.PackageVersionsTemplate),
						Comment = $"Template Url to get a list of known package versions, in the format https://delphipm.org/{RouteConstants.Templates.PackageVersionsTemplate}"
					},
				}
			};

			_cached = response;
			return Task.FromResult(response);
		}
	}
}
