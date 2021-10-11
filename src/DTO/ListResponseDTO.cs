using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.DTO
{
    public class ListResponseDTO
    {
		/// <summary>
		/// The total number of matches - needed for paging
		/// </summary>
		[JsonPropertyName("totalHits")]
		public long TotalHits { get; set; }

		/// <summary>
		/// The packages that matched the search query.
		/// </summary>
		[JsonPropertyName("results")]
		public IList<ListResultDTO> Results { get; set; }

		public ListResponseDTO()
		{
			Results = new List<ListResultDTO>();
		}


	}
}
