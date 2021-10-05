using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace DPMGallery.Data
{
	public class DbContextFactory : IDbContextFactory
	{
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly ILogger _logger;

		public DbContextFactory(IDbConnectionFactory connectionFactory, ILogger logger)
		{
			_connectionFactory = connectionFactory;
			_logger = logger;
		}

		public IDbContext CreateDbContext()
		{
			return new DbContext(_connectionFactory, _logger);
		}
	}
}
