using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
	[Table(Constants.Database.TableNames.UserTokens)]
	public class UserToken
	{
		[Column("user_id")]
		public int UserId { get; set; }

		[Column("login_provider")]
		public string LoginProvider { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("value")]
		public string Value { get; set; }
	}
}
