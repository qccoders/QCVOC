// <copyright file="AccountQueryParameters.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using QCVOC.Api.Common;

    /// <summary>
    ///     Account request pagination and filtering options.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Omitted for Swashbuckle compatibility.")]
    public class AccountQueryParameters : QueryParameters
    {
        /// <summary>
        ///     The Role by which to filter results.
        /// </summary>
        public Role? Role { get; set; }

        /// <summary>
        ///     The id of the Account.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The name associated with the Account.
        /// </summary>
        public string Name { get; set; }
    }
}
