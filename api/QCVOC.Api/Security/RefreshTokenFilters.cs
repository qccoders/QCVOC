﻿// <copyright file="RefreshTokenFilters.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Refresh token pagination and filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class RefreshTokenFilters : Filters
    {
        /// <summary>
        ///     The id of the Refresh Token.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The Account id by which to filter results.
        /// </summary>
        public Guid? AccountId { get; set; }
    }
}
