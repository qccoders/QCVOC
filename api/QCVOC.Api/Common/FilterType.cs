// <copyright file="FilterType.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    /// <summary>
    ///     Determines the type of filter to be applied.
    /// </summary>
    public class FilterType
    {
        private FilterType(string value)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets a basic equality filter.
        /// </summary>
        public static new FilterType Equals => new FilterType("=");

        /// <summary>
        ///     Gets an IN filter.
        /// </summary>
        public static FilterType In => new FilterType("IN");

        /// <summary>
        ///     Gets a LIKE filter.
        /// </summary>
        public static FilterType Like => new FilterType("LIKE");

        /// <summary>
        ///     Gets a BETWEEN filter.
        /// </summary>
        public static FilterType Between => new FilterType("BETWEEN");

        /// <summary>
        ///     Gets the value of the filter.
        /// </summary>
        public string Value { get; }
    }
}
