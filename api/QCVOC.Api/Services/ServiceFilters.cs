// <copyright file="ServiceFilters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Service request filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class ServiceFilters : Filters
    {
        /// <summary>
        ///     The Id of the service.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The description of the service.
        /// </summary>
        public string Description { get; set; }
    }
}
