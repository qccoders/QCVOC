// <copyright file="IVeteranRepository.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Data.Repository
{
    using System.Collections.Generic;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Veteran"/>.
    /// </summary>
    public interface IVeteranRepository : ISingleKeyRepository<Veteran>
    {
        /// <summary>
        ///     Retrieves all Veterans after applying optional <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <param name="includePhotoBase64">A value indicating whether the photobase64 column should be included in the results.</param>
        /// <returns>A list of Veterans</returns>
        IEnumerable<Veteran> GetAll(Filters filters = null, bool includePhotoBase64 = false);
    }
}