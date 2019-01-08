// <copyright file="ScanError.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.DTO
{
    using System;
    using QCVOC.Api.Scans.Data.Model;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     DTO containing a Scan error.
    /// </summary>
    public class ScanError
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScanError"/> class with the specified <paramref name="scan"/>, <paramref name="veteran"/>, and <paramref name="message"/>.
        /// </summary>
        /// <param name="scan">The Scan to include in the response.</param>
        /// <param name="veteran">The Veteran associated with the scan, if applicable.</param>
        /// <param name="message">The error message associated with the scan.</param>
        public ScanError(Scan scan, Veteran veteran, string message)
        {
            EventId = scan.EventId;
            VeteranId = scan.VeteranId;
            Veteran = veteran;
            ServiceId = scan.ServiceId;
            PlusOne = scan.PlusOne;
            Message = message;
        }

        /// <summary>
        ///     Gets or sets the id of the Event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Veteran.
        /// </summary>
        public Guid VeteranId { get; set; }

        /// <summary>
        ///     Gets or sets the Veteran associated with the Scan.
        /// </summary>
        public Veteran Veteran { get; set; }

        /// <summary>
        ///     Gets or sets the id of the Service.
        /// </summary>
        public Guid? ServiceId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the Veteran brought a guest.
        /// </summary>
        public bool PlusOne { get; set; }

        /// <summary>
        ///     Gets or sets an error message.
        /// </summary>
        public string Message { get; set; }
    }
}
