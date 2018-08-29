// <copyright file="Extensions.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    ///     Miscellaneous extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Returns the value of the <see cref="ClaimTypes.NameIdentifier"/> claim for the specified <paramref name="user"/>
        /// </summary>
        /// <param name="user">The ClaimsPrincipal instance from which to retrieve the id.</param>
        /// <returns>The retrieved id.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the supplied <see cref="ClaimsPrincipal"/> doesn't contain a <see cref="ClaimTypes.NameIdentifier"/> claim.</exception>
        /// <exception cref="FormatException">Thrown when the <see cref="ClaimTypes.NameIdentifier"/> is not a valid Guid.</exception>
        public static Guid GetId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (claim == default(Claim))
            {
                throw new ArgumentNullException("The provided ClaimsPrincipal does not contain a NameIdentifier claim.");
            }

            if (Guid.TryParse(claim.Value, out var id))
            {
                return id;
            }

            throw new FormatException("The value of the NameIdentifier claim is not a valid Guid.");
        }
    }
}
