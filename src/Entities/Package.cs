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

        //although it's really package versions that are listed
        //we need this to stop the package showing when a package
        //is first published and hasn't finished av scan and uploading
        //first package version will set this to true.
        [Column("active")]
        public bool Active { get; set; }
	}
}
