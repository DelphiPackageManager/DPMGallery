using System.Text.Json.Serialization;

namespace DPMGallery.DSpec
{
	public class DSpecDependency
	{

		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("version")]
		public string Version { get; set; }
	}
}
