using Npgsql;
using System.Data;

namespace DapperCrudApi.Models
{
    public class DatabaseContext
    {

        private readonly string _connectionString;

        public DatabaseContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }
    }
}
