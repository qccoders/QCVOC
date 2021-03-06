﻿// <copyright file="ScanRequest.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     DTO containing Scan context.
    /// </summary>
    public class ScanRequest
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
        public string CardNumber { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Service.
        /// </summary>
        /// <remarks>
        ///     If null, represents a check-in Scan.
        /// </remarks>
        public Guid ServiceId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Veteran brought a guest.
        /// </summary>
        public bool PlusOne { get; set; }
    }
}
