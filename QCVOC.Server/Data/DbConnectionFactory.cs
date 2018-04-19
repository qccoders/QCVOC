using System;
using System.Data;
using System.Data.SqlClient;

namespace QCVOC.Server.Data
{
    public class DbConnectionFactory
    {
        #region Public Methods

        public IDbConnection GetDbConnection<T>(string connectionString)
        {
            if (typeof(T) == typeof(SqlConnection))
            {
                return new SqlConnection(connectionString);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion Public Methods
    }
}