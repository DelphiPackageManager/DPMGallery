using System.Text.Json.Serialization;

namespace DPMGallery.DSpec
{
	public class DSpecMetaData
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("version")]
		public string Version { get; set; }

		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("authors")]
		public string Authors { get; set; }

		[JsonPropertyName("license")]
		public string License { get; set; }

		[JsonPropertyName("icon")]
		public string Icon { get; set; }

		[JsonPropertyName("iconUrl")]
		public string IconUrl { get; set; }

		[JsonPropertyName("copyright")]
		public string Copyright { get; set; }

		[JsonPropertyName("tags")]
		public string Tags { get; set; }

		[JsonPropertyName("isCommercial")]
		public bool IsCommercial { get; set; }

		[JsonPropertyName("isTrial")]
		public bool IsTrial { get; set; }
	}
}
