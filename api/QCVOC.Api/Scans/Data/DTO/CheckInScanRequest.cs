// <copyright file="CheckInScanRequest.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
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
    public class CheckInScanRequest
    {
        /// <summary>
        ///     Gets or sets the id of the Event.
        /// </summary>
        [Required]
        public Guid? EventId { get; set; }

        /// <summary>
        ///     Gets or sets the Veteran's card number.
        /// </summary>
        [Required]
        public int CardNumber { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Veteran brought a guest.
        /// </summary>
        [Required]
        public bool PlusOne { get; set; }
    }
}
