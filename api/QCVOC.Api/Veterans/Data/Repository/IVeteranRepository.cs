// <copyright file="IVeteranRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Data.Repository
{
    using System;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Veteran"/>.
    /// </summary>
    public interface IVeteranRepository : ISingleKeyRepository<Veteran>
    {
        /// <summary>
        ///     Retrieves the base 64 encoded photo for the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Veteran"/> to retrieve.</param>
        /// <returns>The base 64 encoded photo for the Veteran matching the specified id.</returns>
        string GetPhotoBase64(Guid id);
    }
}
