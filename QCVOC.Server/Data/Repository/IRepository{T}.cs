// <copyright file="IRepository{T}.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server.Data.Repository
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A generic repository for application resource collections.
    /// </summary>
    /// <typeparam name="T">The Type of resource collection.</typeparam>
    public interface IRepository<T>
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new resource from the specified resource.
        /// </summary>
        /// <param name="newResource">The resource to create.</param>
        void Create(T newResource);

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
        IEnumerable<T> GetAll();

        /// <summary>
        ///     Updates the resource matching the specified <paramref name="id"/> with the specified <paramref name="updatedResource"/>.
        /// </summary>
        /// <param name="id">The id of the resource to update.</param>
        /// <param name="updatedResource">The information with which to update the resource.</param>
        void Update(Guid id, T updatedResource);

        /// <summary>
        ///     Updates the specified <paramref name="resource"/> with the specified <paramref name="updatedResource"/>.
        /// </summary>
        /// <param name="resource">The resource to update.</param>
        /// <param name="updatedResource">The information with which to update the resource.</param>
        void Update(T resource, T updatedResource);

        #endregion Public Methods
    }
}