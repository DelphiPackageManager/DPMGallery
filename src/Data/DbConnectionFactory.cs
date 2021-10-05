using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DPMGallery.Data
{
	public class DbConnectionFactory : IDbConnectionFactory
	{
		private readonly DbProviderFactory _provider;
		private readonly string _connectionString;

		//for autofac use. 
		public DbConnectionFactory()
		{
		}

		/// <summary>
		/// Creates a new DbConnectionFactory instance.
		/// </summary>
		/// <param name="connectionString">the connectionstring.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionStringName"/> is a null value.4</exception>
		/// <exception cref="ConfigurationErrorsException">Thrown if <paramref name="connectionStringName"/> is not found in any app/web.config file available to the application.</exception>
		public DbConnectionFactory(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

			_provider = NpgsqlFactory.Instance;
		}

		/// <summary>
		/// Creates a new instance of <see cref="IDbConnection"/>.
		/// </summary>
		/// <exception cref="ConfigurationErrorsException">Thrown if the connectionstring entry in the app/web.config file is missing information, contains errors or is missing entirely.</exception>
		/// <returns></returns>
		public DbConnection Create()
		{
			var connection = _provider.CreateConnection();
			if (connection == null)
				throw new Exception("Failed to create a connection using the connection supplied");

			connection.ConnectionString = _connectionString;
			return connection;
		}


	}
}
