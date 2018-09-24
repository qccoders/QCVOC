// <copyright file="IRepository{T}.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common.Data.Repository
{
    using System.Collections.Generic;

    /// <summary>
    ///     A generic repository for application resource collections.
    /// </summary>
    /// <typeparam name="T">The Type of resource collection.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        ///     Creates a new resource from the specified resource.
        /// </summary>
        /// <param name="resource">The resource to create.</param>
        /// <returns>The created resource.</returns>
        T Create(T resource);

        /// <summary>
        ///     Deletes the specified <paramref name="resource"/>.
        /// </summary>
        /// <param name="resource">The resource to delete.</param>
        void Delete(T resource);

        /// <summary>
        ///     Retrieves a list of all resources in the collection.
        /// </summary>
        /// <param name="filters">Optional query filters.</param>
        /// <returns>A list of all resources in the collection.</returns>
        IEnumerable<T> GetAll(Filters filters = null);

        /// <summary>
        ///     Updates the specified <paramref name="resource"/>.
        /// </summary>
        /// <param name="resource">The resource to update.</param>
        /// <returns>The updated resource.</returns>
        T Update(T resource);
    }
}