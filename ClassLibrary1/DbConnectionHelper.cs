using ClassLibrary1.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class DbConnectionHelper : IDbConnectionHelper
    {
        private readonly IExceptionHandling exceptionHandling;
        private readonly IConfiguration configuration;

        public DbConnectionHelper(IExceptionHandling exception,
            IConfiguration config)
        {
            exceptionHandling = exception;
            configuration = config;
        }

        public void CloseConnection(IDbConnection connection)
        {
            connection.Close();
        }

        public IDbConnection GetConnection(string connectionName)
        {
            string connectionString = configuration.GetConnectionString(connectionName);
            var connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
            }
            catch (Exception ex)
            {
                CloseConnection(connection);
                exceptionHandling.LogException(ex, ExceptionPolicy.Web_Exception);
                throw new Exception("Unable to open connection to database");
            }
            return connection;
        }

        public IDbDataAdapter GetDbDataAdapter(IDbCommand command)
        {
            IDbDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }
    }
}
