// <copyright file="IDbConnectionFactory.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.ConnectionFactory
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        #region Public Methods

        IDbConnection CreateConnection();

        #endregion Public Methods
    }
}