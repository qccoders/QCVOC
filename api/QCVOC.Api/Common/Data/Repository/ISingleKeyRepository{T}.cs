// <copyright file="ISingleKeyRepository{T}.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.Repository
{
    using System;

    /// <summary>
    ///     A generic repository for application resource collections.
    /// </summary>
    /// <typeparam name="T">The Type of resource collection.</typeparam>
    public interface ISingleKeyRepository<T> : IRepository<T>
    {
        /// <summary>
        ///     Deletes the resource matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the resource to delete.</param>
        void Delete(Guid id);

        /// <summary>
        ///     Retrieves the resource matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the resource to retrieve.</param>
        /// <returns>The resource matching the specified id.</returns>
        T Get(Guid id);
    }
}