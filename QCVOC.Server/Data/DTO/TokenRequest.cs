// <copyright file="TokenRequest.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing user credentials for a Token request.
    /// </summary>
    public class TokenRequest
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Usernames must be between 2 and 256 characters in length.")]
        [RegularExpression("(?!\0)")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        [Required]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Passwords must be between 6 and 256 characters in length.")]
        [RegularExpression("(?!\0)")]
        public string Password { get; set; }

        #endregion Public Properties
    }
}