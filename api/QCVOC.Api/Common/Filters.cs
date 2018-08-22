// <copyright file="Filters.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file 
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
        ///     Initializes a new instance of the <see cref="Filters"/> class with the optionally specified <paramref name="filters"/>.
        /// </summary>
        /// <param name="filters">The optional filters with which to initialize the new instance.</param>
        public Filters(Filters filters = null)
        {
            Offset = filters?.Offset ?? 0;
            Limit = filters?.Limit ?? 100;
            OrderBy = filters?.OrderBy ?? SortOrder.ASC;
        }

        /// <summary>
        ///     The number of items in the page.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        ///     The starting offset for the page.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        ///     The sort order of the items.
        /// </summary>
        public SortOrder OrderBy { get; set; }
    }
}