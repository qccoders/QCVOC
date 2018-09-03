// <copyright file="Extensions.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Security.Claims;
    using Dapper;

    /// <summary>
    ///     Miscellaneous extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Returns the value of the <see cref="ClaimTypes.NameIdentifier"/> claim for the specified <paramref name="user"/>
        /// </summary>
        /// <param name="user">The ClaimsPrincipal instance from which to retrieve the id.</param>
        /// <returns>The retrieved id.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the supplied <see cref="ClaimsPrincipal"/> doesn't contain a <see cref="ClaimTypes.NameIdentifier"/> claim.</exception>
        /// <exception cref="FormatException">Thrown when the <see cref="ClaimTypes.NameIdentifier"/> is not a valid Guid.</exception>
        public static Guid GetId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (claim == default(Claim))
            {
                throw new ArgumentNullException("The provided ClaimsPrincipal does not contain a NameIdentifier claim.");
            }

            if (Guid.TryParse(claim.Value, out var id))
            {
                return id;
            }

            throw new FormatException("The value of the NameIdentifier claim is not a valid Guid.");
        }

        /// <summary>
        ///     Conditionally adds the specified where clause if the specified value(s) are non-null.
        /// </summary>
        /// <param name="builder">The SqlBuilder instance to which to apply the filter.</param>
        /// <param name="filterType">The type of filter to apply.</param>
        /// <param name="field">The field to filter.</param>
        /// <param name="values">The value to filter by.</param>
        /// <returns>The specified SqlBuilder instance with the filter applied.</returns>
        public static SqlBuilder ApplyFilter(this SqlBuilder builder, FilterType filterType, string field, params object[] values)
        {
            if (values == null || !values.Any(v => v != null))
            {
                return builder;
            }

            filterType = filterType ?? FilterType.Equals;
            var valueField = field.Replace('.', '.');

            string sql = string.Empty;
            ExpandoObject parms = new ExpandoObject();

            switch (filterType.Value)
            {
                case "BETWEEN":
                    if (values.Length != 2)
                    {
                        throw new ArgumentException($"BETWEEN filters must supply exactly two values (supplied: {values.Length}).");
                    }

                    sql = $"{field} BETWEEN @{field}_start AND @{valueField}_end";

                    parms.AddProperty($"{field}_start", values[0]);
                    parms.AddProperty($"{field}_end", values[1]);

                    return builder.Where(sql, parms);
                case "IN":
                    sql = $"{field} IN ({string.Join(',', values.Select((v, i) => $"@{valueField}_{i}"))}";

                    foreach (var value in values.Select((v, i) => new { Index = i, Value = v }))
                    {
                        parms.AddProperty($"{field}_{value.Index}", value.Value);
                    }

                    return builder.Where(sql, parms);
                case "LIKE":
                    if (values.Length != 1)
                    {
                        throw new ArgumentException($"LIKE filters must supply exactly one value (supplied: {values.Length}).");
                    }

                    sql = $"{field} LIKE @{valueField}";

                    parms.AddProperty($"{field}", values[0]);

                    return builder.Where(sql, parms);
                case "=":
                    if (values.Length != 1)
                    {
                        throw new ArgumentException($"EQUALS filters must supply exactly one value (supplied: {values.Length}).");
                    }

                    sql = $"{field} = @{valueField}";

                    parms.AddProperty($"{field}", values[0]);

                    return builder.Where(sql, parms);
                default:
                    break;
            }

            return builder;
        }

        private static ExpandoObject AddProperty(this ExpandoObject obj, string name, object value)
        {
            var dict = (IDictionary<string, object>)obj;

            if (dict.ContainsKey(name))
            {
                dict[name] = value;
            }
            else
            {
                dict.Add(name, value);
            }

            return obj;
        }
    }
}
