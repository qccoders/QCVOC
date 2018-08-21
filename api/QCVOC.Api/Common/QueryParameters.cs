// <copyright file="QueryParameters.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Request pagination options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class QueryParameters
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryParameters"/> class.
        /// </summary>
        public QueryParameters()
        {
            Offset = 0;
            Limit = 100;
            OrderBy = SortOrder.ASC;
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