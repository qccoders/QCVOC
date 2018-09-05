// <copyright file="Service.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Service.Data.Model
{
    using System;

    /// <summary>
    ///     A Service.
    /// </summary>
    public class Service : IEquatable<Service>
    {
        /// <summary>
        ///     Gets or sets the id of the service.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of the service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the id of the user who created the service.
        /// </summary>
        public Guid CreationById { get; set; }

        /// <summary>
        ///     Gets or sets date on which the Service was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///    Compares two Service instances.
        /// </summary>
        /// <param name="service">The service to which to compare.</param>
        /// <returns>A value indicating whether the compared instances are equal.</returns>
        public bool Equals(Service service)
        {
            if (service == null)
            {
                return this == null;
            }

            return this.Id == service.Id
            && this.Name == service.Name
            && this.Description == service.Description
            && this.CreationById == service.CreationById
            && this.CreationDate - service.CreationDate <= TimeSpan.FromSeconds(1);
        }
    }
}