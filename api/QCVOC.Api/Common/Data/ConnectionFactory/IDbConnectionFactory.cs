﻿// <copyright file="IDbConnectionFactory.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.ConnectionFactory
{
    using System.Data;

    /// <summary>
    ///     A connection factory for database connections.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        ///     Constructs and returns a database connection.
        /// </summary>
        /// <returns>A database connection.</returns>
        IDbConnection CreateConnection();
    }
}