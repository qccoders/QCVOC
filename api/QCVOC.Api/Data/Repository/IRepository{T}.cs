// <copyright file="IRepository{T}.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using static QCVOC.Api.Controllers.AccountsController;

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
        ///     Deletes the resource matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the resource to delete.</param>
        void Delete(Guid id);

        /// <summary>
        ///     Deletes the specified <paramref name="resource"/>.
        /// </summary>
        /// <param name="resource">The resource to delete.</param>
        void Delete(T resource);

        /// <summary>
        ///     Retrieves the resource matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the resource to retrieve.</param>
        /// <returns>The resource matching the specified id.</returns>
        T Get(Guid id);

        /// <summary>
        ///     Retrieves a list of all resources in the collection.
        /// </summary>
        /// <returns>A list of all resources in the collection.</returns>
        IEnumerable<T> GetAll(QueryParameters queryParameters = null);

        /// <summary>
        ///     Updates the specified <paramref name="resource"/>.
        /// </summary>
        /// <param name="resource">The resource to update.</param>
        /// <returns>The updated resource.</returns>
        T Update(T resource);
    }
}