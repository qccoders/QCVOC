// <copyright file="AccountResponse.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.DTO
{
    using System;
    using QCVOC.Api.Security;

    /// <summary>
    ///     DTO containing Account details.
    /// </summary>
    public class AccountResponse
    {
        /// <summary>
        ///     Gets or sets the Account id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the Account name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Role for the account.
        /// </summary>
        public Role Role { get; set; }
    }
}