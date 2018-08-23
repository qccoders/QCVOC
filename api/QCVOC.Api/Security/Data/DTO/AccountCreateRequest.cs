// <copyright file="AccountCreateRequest.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Security;

    /// <summary>
    ///     DTO containing new Account details for an Account creation request.
    /// </summary>
    public class AccountCreateRequest
    {
        /// <summary>
        ///     Gets or sets the name for the new Account.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 2)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Role for the new Account.
        /// </summary>
        [Required]
        public Role Role { get; set; }

        /// <summary>
        ///     Gets or sets the password for the new Account.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
