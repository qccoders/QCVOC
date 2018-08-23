// <copyright file="RefreshToken.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.Model
{
    using System;

    /// <summary>
    ///     A Refresh Token record.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        ///     Gets or sets the id of the <see cref="Account"/> associated with the token.
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        ///     Gets or sets the time at which the token expires.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        ///     Gets or sets the time at which the token was issued.
        /// </summary>
        public DateTime Issued { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the token.
        /// </summary>
        public Guid Id { get; set; }
    }
}