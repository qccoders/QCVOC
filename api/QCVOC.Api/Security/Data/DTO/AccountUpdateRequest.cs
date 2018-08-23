// <copyright file="AccountUpdateRequest.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Security;

    /// <summary>
    ///     DTO containing updated Account details for an Account update request.
    /// </summary>
    public class AccountUpdateRequest
    {
        /// <summary>
        ///     Gets or sets the updated name for the Account.
        /// </summary>
        [StringLength(maximumLength: 256, MinimumLength = 2)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets updated Role for the Account.
        /// </summary>
        public Role? Role { get; set; }

        /// <summary>
        ///     Gets or sets the updated password for the Account.
        /// </summary>
        [StringLength(maximumLength: 256, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
