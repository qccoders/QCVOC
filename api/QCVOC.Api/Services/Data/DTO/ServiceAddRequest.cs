// <copyright file="ServiceAddRequest.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Services.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing new Service details for a Service add request.
    /// </summary>
    public class ServiceAddRequest
    {
        /// <summary>
        ///     Gets or sets the name of the Service.
        /// </summary>
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description of the Service.
        /// </summary>
        [StringLength(maximumLength: 256, MinimumLength = 1)]
        public string Description { get; set; }
    }
}
