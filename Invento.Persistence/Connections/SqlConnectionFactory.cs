using Invento.Application.Common.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Invento.Persistence.Connections
{
    public class SqlConnectionFactory
        : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(
            IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string 'DefaultConnection' was not found.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}