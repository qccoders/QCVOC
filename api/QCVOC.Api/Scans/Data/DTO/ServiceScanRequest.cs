// <copyright file="ServiceScanRequest.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing Scan context.
    /// </summary>
    public class ServiceScanRequest
    {
        /// <summary>
        ///     Gets or sets the id of the Event.
        /// </summary>
        [Required]
        public Guid? EventId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Veteran.
        /// </summary>
        [Required]
        public Guid? VeteranId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Service.
        /// </summary>
        /// <remarks>
        ///     If null, represents a check-in Scan.
        /// </remarks>
        [Required]
        public Guid? ServiceId { get; set; }
    }
}
