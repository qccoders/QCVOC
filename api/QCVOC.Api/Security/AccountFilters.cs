// <copyright file="AccountFilters.cs" company="JP Dillingham, Nick Acosta, et. al.">
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
    public class AccountFilters : Filters
    {
        /// <summary>
        ///     The id of the Account.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        ///     The name associated with the Account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Whether a password reset is required for the Account.
        /// </summary>
        public bool? PasswordResetRequired { get; set; }

        /// <summary>
        ///     The Role by which to filter results.
        /// </summary>
        public Role? Role { get; set; }

        /// <summary>
        ///     The starting time of the Account creation date range.
        /// </summary>
        public DateTime? CreationDateStart { get; set; }

        /// <summary>
        ///     The ending time of the Account creation date range.
        /// </summary>
        public DateTime? CreationDateEnd { get; set; }

        /// <summary>
        ///     The id of the user which created the Account.
        /// </summary>
        public Guid? CreationById { get; set; }

        /// <summary>
        ///     The starting time of a last updated date range.
        /// </summary>
        public DateTime? LastUpdateDateStart { get; set; }

        /// <summary>
        ///     The ending time of a last updated date range.
        /// </summary>
        public DateTime? LastUpdateDateEnd { get; set; }

        /// <summary>
        ///    The id of the user which performed the last update.
        /// </summary>
        public Guid? LastUpdateById { get; set; }
    }
}
