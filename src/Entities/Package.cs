using Dapper.Contrib.Extensions;
using DPMGallery.Data;
using System;

namespace DPMGallery.Entities
{
	[Table(Constants.Database.TableNames.Package)]
	public class Package
	{
		[Column("id")]
		public int Id { get; set; }

		[Column("packageid")]
		public string PackageId { get; set; }

		// all downloads for this package.
		[Column("downloads")]
		public Int64 Downloads { get; set; }

		//Note, don't be tempted to add a package owner here.
		//there might be multiple owners

	}
}
