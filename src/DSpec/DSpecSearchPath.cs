using System.Text.Json.Serialization;

namespace DPMGallery.DSpec
{
	public class DSpecSearchPath
	{
		public DSpecSearchPath()
		{

		}

		[JsonPropertyName("path")]
		public string Path { get; set; }


	}
}
