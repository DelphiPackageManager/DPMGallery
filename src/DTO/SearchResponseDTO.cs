using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DPMGallery.DTO
{
	/// <summary>
	/// The response to a search!
	/// </summary>
	public class SearchResponseDTO
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
		public IList<SearchResultDTO> Results { get; set; }

		public SearchResponseDTO()
		{
			Results = new List<SearchResultDTO>();
		}
	}
}
