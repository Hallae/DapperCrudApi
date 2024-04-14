using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace DapperCrudApi.Models
{
    //<summary>
    // Database Connections Strings
    //</summary>
    
    public class DatabaseContext
    {
        private readonly string _connectionString;
        private readonly string _connectionStringApp;

        public DatabaseContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _connectionStringApp = configuration.GetConnectionString("ApplicationConnection");
        }

        //Responsible for the Patients postgres connectionstring
        public IDbConnection DefaultConnection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }
        //Responsible for the Application postgres connectionstring
        public IDbConnection ApplicationConnection
        {
            get
            {
                return new NpgsqlConnection(_connectionStringApp);
            }
        }
    }
}
