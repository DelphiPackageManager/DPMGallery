using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Data
{
	public interface IDbContextFactory
	{
		IDbContext CreateDbContext();
	}
}
