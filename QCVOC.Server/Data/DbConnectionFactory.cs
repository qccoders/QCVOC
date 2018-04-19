using Npgsql;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace QCVOC.Server.Data
{
    public class DbConnectionFactory
    {
        #region Public Methods

        public IDbConnection GetDbConnection<T>(string connectionString) where T : IDbConnection
        {
            if (typeof(T) == typeof(SqlConnection))
            {
                return new SqlConnection(connectionString);
            }
            else if (typeof(T) == typeof(NpgsqlConnection))
            {
                return new NpgsqlConnection(connectionString);
            }
            else if (typeof(T) == typeof(SQLiteConnection))
            {
                return new SQLiteConnection(connectionString);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion Public Methods
    }
}