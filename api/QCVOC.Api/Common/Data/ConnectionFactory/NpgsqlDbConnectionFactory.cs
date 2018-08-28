// <copyright file="NpgsqlDbConnectionFactory.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.ConnectionFactory
{
    using System.Data;
    using Npgsql;

    /// <summary>
    ///     A connection factory for Postgresql database connections.
    /// </summary>
    public class NpgsqlDbConnectionFactory : IDbConnectionFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NpgsqlDbConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public NpgsqlDbConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private string ConnectionString { get; set; }

        /// <summary>
        ///     Constructs and returns a Postgresql database connection.
        /// </summary>
        /// <returns>A Postgresql database connection.</returns>
        public IDbConnection CreateConnection()
        {
            var connection = NpgsqlFactory.Instance.CreateConnection();
            connection.ConnectionString = ConnectionString;

            return connection;
        }
    }
}