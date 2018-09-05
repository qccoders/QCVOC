// <copyright file="Account.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.Model
{
    using System;
    using QCVOC.Api.Security;

    /// <summary>
    ///     A user Account.
    /// </summary>
    public class Account : IEquatable<Account>
    {
        /// <summary>
        ///     Gets the name of the user which created the Account.
        /// </summary>
        public string CreationBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which created the Account.
        /// </summary>
        public Guid CreationById { get; set; }

        /// <summary>
        ///     Gets or sets the Account creation date.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     Gets or sets the Account id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets the name of the user which performed the last update.
        /// </summary>
        public string LastUpdateBy { get; }

        /// <summary>
        ///     Gets or sets the id of the user which performed the last update.
        /// </summary>
        public Guid LastUpdateById { get; set; }

        /// <summary>
        ///     Gets or sets the date on which the Account was last updated.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        ///     Gets or sets the Account name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Account password hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a password reset is required.
        /// </summary>
        public bool PasswordResetRequired { get; set; }

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
            && this.PasswordResetRequired == account.PasswordResetRequired
            && this.Role == account.Role
            && this.CreationDate == account.CreationDate
            && this.CreationById == account.CreationById
            && this.CreationBy == account.CreationBy
            && this.LastUpdateDate == account.LastUpdateDate
            && this.LastUpdateById == account.LastUpdateById
            && this.LastUpdateBy == account.LastUpdateBy;
        }
    }
}