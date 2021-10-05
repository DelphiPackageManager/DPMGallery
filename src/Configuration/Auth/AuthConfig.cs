using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DPMGallery.Configuration
{
	public class AuthConfig
	{

		public GoogleAuthConfig Google { get; set; } = new GoogleAuthConfig();

		public MicrosoftAuthConfig Microsoft { get; set; } = new MicrosoftAuthConfig();

		public GitHubAuthConfig GitHub { get; set; } = new GitHubAuthConfig();
	}
}
