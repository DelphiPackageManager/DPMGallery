using System.Data;
using System.Data.Common;

namespace DPMGallery.Data
{
	public interface IDbConnectionFactory
	{
		DbConnection Create();
	}
}
