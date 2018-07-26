// <copyright file="AccountResponse.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server.Data.DTO
{
    using System;
    using QCVOC.Server.Data.Model;
    using QCVOC.Server.Data.Model.Security;

    public class AccountResponse
    {
        #region Public Properties

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }

        #endregion Public Properties
    }
}