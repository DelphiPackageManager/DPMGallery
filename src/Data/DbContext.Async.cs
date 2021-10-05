using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DPMGallery.Data
{
    public partial class DbContext //.async
    {

        public Task<T> GetAsync<T>(dynamic id, int? commandTimeout = null) where T : class
        {
            CreateOrReuseConnection();
            //return _connection.GetAsync<T>(id, _transaction, commandTimeout);
            return SqlMapperExtensions.GetAsync<T>(_connection, id, _transaction, commandTimeout);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>(int? commandTimeout = null) where T : class
        {
            CreateOrReuseConnection();
            return _connection.GetAllAsync<T>(_transaction, commandTimeout);
        }

        public Task<bool> DeleteAllAsync<T>(int? commandTimeout = null) where T : class
        {
            CreateOrReuseConnection();
            return _connection.DeleteAllAsync<T>(_transaction, commandTimeout);
        }

        public Task<bool> DeleteAsync<T>(T entityToDelete, int? commandTimeout = null) where T : class
        {
            CreateOrReuseConnection();
            return _connection.DeleteAsync<T>(entityToDelete, _transaction, commandTimeout);
        }

        public Task<int> InsertAsync<T>(T entityToInsert, int? commandTimeout = null, ISqlAdapter sqlAdapter = null) where T : class
        {
            CreateOrReuseConnection();
            return _connection.InsertAsync<T>(entityToInsert, _transaction, commandTimeout, sqlAdapter);
        }

        public Task<bool> UpdateAsync<T>(T entityToUpdate, int? commandTimeout = null) where T : class
        {
            CreateOrReuseConnection();
            return _connection.UpdateAsync<T>(entityToUpdate, _transaction, commandTimeout);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.ExecuteAsync(commandDef);
        }



        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <see cref="object"/>.</returns>
        public Task<object> ExecuteScalarAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.ExecuteScalarAsync(commandDef);
        }

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
        /// <param name="commandType">Is it a stored proc or a batch?</param>
        /// <returns>The first cell returned, as <typeparamref name="T"/>.</returns>
        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.ExecuteScalarAsync<T>(commandDef);
        }


        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <remarks>Note: each row can be accessed via "dynamic", or by casting to an IDictionary&lt;string,object&gt;</remarks>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryAsync(commandDef);
        }

        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of <typeparamref name="T"/>; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryAsync<T>(commandDef);

        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<T> QueryFirstAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryFirstAsync<T>(commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryFirstOrDefaultAsync<T>(commandDef);
        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<T> QuerySingleAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleAsync<T>(commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleOrDefaultAsync<T>(commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<dynamic> QueryFirstAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryFirstAsync(commandDef);
        }

        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);

            return _connection.QueryFirstOrDefaultAsync(commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<dynamic> QuerySingleAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleAsync(commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleOrDefaultAsync(commandDef);
        }
        /// <summary>
        /// Execute a query asynchronously using Task.
        /// </summary>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public Task<IEnumerable<object>> QueryAsync(Type type, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);

            return _connection.QueryAsync(type, commandDef);
        }


        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public Task<object> QueryFirstAsync(Type type, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryFirstAsync(type, commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public Task<object> QueryFirstOrDefaultAsync(Type type, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QueryFirstOrDefaultAsync(type, commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public Task<object> QuerySingleAsync(Type type, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleAsync(type, commandDef);
        }
        /// <summary>
        /// Execute a single-row query asynchronously using Task.
        /// </summary>
        /// <param name="type">The type to return.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <c>null</c>.</exception>
        public Task<object> QuerySingleOrDefaultAsync(Type type, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            CreateOrReuseConnection();
            var commandDef = new CommandDefinition(sql, param, _transaction, commandTimeout, commandType, CommandFlags.Buffered, cancellationToken);
            return _connection.QuerySingleOrDefaultAsync(type, commandDef);
        }     
    }
}
