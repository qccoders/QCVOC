// <copyright file="NpgsqlDbConnectionFactory.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.ConnectionFactory
{
    using System.Data;
    using Npgsql;

    public class NpgsqlDbConnectionFactory : IDbConnectionFactory
    {
        #region Public Constructors

        public NpgsqlDbConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion Public Constructors

        #region Private Properties

        private string ConnectionString { get; set; }

        #endregion Private Properties

        #region Public Methods

        public IDbConnection CreateConnection()
        {
            var connection = NpgsqlFactory.Instance.CreateConnection();
            connection.ConnectionString = ConnectionString;

            return connection;
        }

        #endregion Public Methods
    }
}