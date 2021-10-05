
using System.Text.Json.Serialization;

namespace DPMGallery.DSpec
{
	/// <summary>
	/// .net implementation of the dspec - needed for extraction
	/// and validation when uploading packages. 
	/// </summary>
	public class DSpec
	{
		[JsonPropertyName("specVersion")]
		public string SpecVersion { get; set; }

		[JsonPropertyName("metadata")]
		public DSpecMetaData MetaData { get; set; }
	}
}
