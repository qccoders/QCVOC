// <copyright file="Filters.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Request filters.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class Filters
    {
        /// <summary>
        ///     The number of items in the page.
        /// </summary>
        public int Limit { get; set; } = 100;

        /// <summary>
        ///     The starting offset for the page.
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        ///     The sort order of the items.
        /// </summary>
        public SortOrder OrderBy { get; set; } = SortOrder.ASC;
    }
}