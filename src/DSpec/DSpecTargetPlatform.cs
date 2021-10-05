using DPMGallery.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DPMGallery.DSpec
{
	public class DSpecTargetPlatform
	{
		[JsonPropertyName("compiler")]
		public CompilerVersion Compiler { get; set; }

		[JsonPropertyName("platforms")]
		public Platform Platforms { get; set; }

		[JsonPropertyName("dependencies")]
		public IList<DSpecDependency> Dependencies { get; set; }

		[JsonPropertyName("searchPaths")]
		public IList<DSpecSearchPath> SearchPaths { get; set; }
	}
}
