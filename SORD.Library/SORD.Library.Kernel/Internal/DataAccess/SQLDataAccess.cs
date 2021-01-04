using System;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace SORD.Library.Kernel
{
    internal class SQLDataAccess
    {
        public string GetConnectionString(string name)
        {
            string con = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            return con;
        }

        public List<T> LoadData<T,U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            IDbConnection connection = new SqlConnection(connectionString);

            List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

            return rows;            
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            IDbConnection connection = new SqlConnection(connectionString);

            connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

        }
    }
}
