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
    public class Account : IEquatable<Account>
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

        /// <summary>
        ///     Compares two Account instances.
        /// </summary>
        /// <param name="account">The Account to which to compare.</param>
        /// <returns>A value indicating whether the compared instances are equal.</returns>
        public bool Equals(Account account)
        {
            if (account == null)
            {
                return this == null;
            }

            return this.Id == account.Id
            && this.Name == account.Name
            && this.PasswordHash == account.PasswordHash
            && this.Role == account.Role;
        }
    }
}