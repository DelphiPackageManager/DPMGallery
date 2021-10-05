using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Entities
{
	[Table(Constants.Database.TableNames.UserClaims)]
	public class UserClaim
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("user_id")]
		public int UserId { get; set; }

		[Column("claim_type")]
		public string ClaimType { get; set; }

		[Column("claim_value")]
		public string ClaimValue { get; set; }
	}
}
