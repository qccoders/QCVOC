// <copyright file="Account.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.Model
{
    using System;
    using QCVOC.Api.Security;

    /// <summary>
    ///     A user account.
    /// </summary>
    public class Account
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
        ///     Gets or sets the Account password hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        ///     Gets or sets the Account Role.
        /// </summary>
        public Role Role { get; set; }
    }
}