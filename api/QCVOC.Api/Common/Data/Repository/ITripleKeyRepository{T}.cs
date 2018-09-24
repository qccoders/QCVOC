// <copyright file="ITripleKeyRepository{T}.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.Repository
{
    using System;

    /// <summary>
    ///     A generic repository for application resource collections.
    /// </summary>
    /// <typeparam name="T">The Type of resource collection.</typeparam>
    public interface ITripleKeyRepository<T> : IRepository<T>
    {
        /// <summary>
        ///     Deletes the resource matching the specified composite key.
        /// </summary>
        /// <param name="id1">The first tuple of the id to delete.</param>
        /// <param name="id2">The second tuple of the id to delete.</param>
        /// <param name="id3">The optional third tuple of the id to delete.</param>
        void Delete(Guid id1, Guid id2, Guid? id3);

        /// <summary>
        ///     Retrieves the resource matching the specified composite key.
        /// </summary>
        /// <param name="id1">The first tuple of the id to delete.</param>
        /// <param name="id2">The second tuple of the id to delete.</param>
        /// <param name="id3">The optional third tuple of the id to delete.</param>
        /// <returns>The resource matching the specified id.</returns>
        T Get(Guid id1, Guid id2, Guid? id3);
    }
}