using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Configuration
{
	public class DatabaseConfig
	{
		public string ConnectionString { get; set; }
		public bool AutoMigrate { get; set; } = true;

	}
}
