using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
	[Table(Constants.Database.TableNames.UserLogins)]
	public class UserLogin
	{
		[Column("user_id")]
		public int UserId { get; set; }


		[Column("login_provider")]
		public string LoginProvider { get; set; }
		//
		// Summary:
		//     Gets or sets the unique provider identifier for this login.
		[Column("provider_key")]
		public string ProviderKey { get; set; }
		//
		// Summary:
		//     Gets or sets the friendly name used in a UI for this login.
		[Column("provider_display_name")]
		public string ProviderDisplayName { get; set; }
		//
		// Summary:
		//     Gets or sets the primary key of the user associated with this login.

	}
}
