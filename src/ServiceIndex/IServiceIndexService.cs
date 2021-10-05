using DPMGallery.DTO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Services
{
	public interface IServiceIndexService
	{
		/// <summary>
		/// Get the resources available on this package feed.
		/// </summary>
		/// <returns>The resources available on this package feed.</returns>
		Task<ServiceIndexResponseDTO> GetAsync(CancellationToken cancellationToken = default);
	}
}
