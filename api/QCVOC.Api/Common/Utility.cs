// <copyright file="Utility.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///     Utility methods.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        ///     Computes and returns the SHA512 hash of the specified string.
        /// </summary>
        /// <param name="content">The string for which the SHA512 hash is to be computed.</param>
        /// <returns>The SHA512 hash of the specified string.</returns>
        public static string ComputeSHA512Hash(string content)
        {
            return ComputeSHA512Hash(Encoding.ASCII.GetBytes(content));
        }

        /// <summary>
        ///     Computes and returns the SHA512 hash of the specified byte array.
        /// </summary>
        /// <param name="content">The byte array for which the SHA512 hash is to be computed.</param>
        /// <returns>The SHA512 hash of the specified byte array.</returns>
        public static string ComputeSHA512Hash(byte[] content)
        {
            byte[] hash;

            using (SHA512 sha512 = new SHA512Managed())
            {
                hash = sha512.ComputeHash(content);
            }

            StringBuilder stringBuilder = new StringBuilder(128);

            foreach (byte b in hash)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Attempts to retrieve the value of the given <paramref name="settingName"/> from
        ///     <code>
        /// appsettings.json
        ///     </code>
        ///     , returning <see cref="string.Empty"/> if the setting could not be retrieved.
        /// </summary>
        /// <param name="settingName">The name of the setting to be retrieved.</param>
        /// <returns>The value of the retrieved <paramref name="settingName"/>.</returns>
        public static string GetAppSetting(string settingName)
        {
            var retVal = string.Empty;

            try
            {
                var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json").Build();

                retVal = config[settingName];
            }
            catch
            {
            }

            return retVal;
        }

        /// <summary>
        ///     Attempts to retrieve the value of the given <paramref name="settingName"/> from environment variables, returning
        ///     <see cref="string.Empty"/> if the setting could not be retrieved.
        /// </summary>
        /// <param name="settingName">The name of the setting to be retrieved.</param>
        /// <returns>The value of the retrieved <paramref name="settingName"/>.</returns>
        public static string GetEnvironmentVariable(string settingName)
        {
            var retVal = string.Empty;

            try
            {
                retVal = Environment.GetEnvironmentVariable(settingName);
            }
            catch
            {
            }

            return retVal;
        }

        /// <summary>
        ///     Attempts to retrieve the value of the given <paramref name="settingName"/>, first from environment variables, then
        ///     appsettings.json. Throws <see cref="InvalidOperationException"/> if the setting could not be retrieved.
        /// </summary>
        /// <typeparam name="T">The expected value <see cref="Type"/>.</typeparam>
        /// <param name="settingName">The name of the setting to be retrieved.</param>
        /// <returns>The value of the retrieved <paramref name="settingName"/> converted to the desired <see cref="Type"/><typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the retrieved setting cannot be cast to the target type.</exception>
        /// <exception cref="FormatException">Thrown when the retrieved setting is in an improper format.</exception>
        /// <exception cref="OverflowException">Thrown when the retrieved setting is too large for the destination type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when a general error is encountered while retrieving the setting.
        /// </exception>
        public static T GetSetting<T>(string settingName)
        {
            var defaultValue = default(T);

            try
            {
                defaultValue = (T)typeof(Settings.Defaults).GetField(settingName.Split('_')[1]).GetValue(null);
            }
            catch (Exception)
            {
            }

            var retVal = GetSetting<T>(settingName, defaultValue);

            if (retVal == null)
            {
                throw new InvalidOperationException("The specified setting could not be found, and a default value was neither given nor could be retrieved from application defaults.  Check the configuration.");
            }

            return retVal;
        }

        /// <summary>
        ///     Attempts to retrieve the value of the given <paramref name="settingName"/>, first from environment variables, then
        ///     appsettings.json. Returns the given <paramref name="defaultValue"/> if the setting could not be retrieved.
        /// </summary>
        /// <typeparam name="T">The expected value <see cref="Type"/>.</typeparam>
        /// <param name="settingName">The name of the setting to be retrieved.</param>
        /// <param name="defaultValue">The default value to substitute if the setting could not be retrieved.</param>
        /// <returns>The value of the retrieved <paramref name="settingName"/> converted to the desired <see cref="Type"/><typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the retrieved setting cannot be cast to the target type.</exception>
        /// <exception cref="FormatException">Thrown when the retrieved setting is in an improper format.</exception>
        /// <exception cref="OverflowException">Thrown when the retrieved setting is too large for the destination type.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when a general error is encountered while retrieving the setting.
        /// </exception>
        public static T GetSetting<T>(string settingName, T defaultValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException(settingName);
            }

            var retVal = GetEnvironmentVariable(settingName);

            if (string.IsNullOrEmpty(retVal))
            {
                retVal = GetAppSetting(settingName);
            }

            if (string.IsNullOrEmpty(retVal))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(retVal, typeof(T));
        }

        /// <summary>
        ///     Converts a ModelStateDictionary into a human-readable format.
        /// </summary>
        /// <param name="dictionary">The ModelStateDictionary to format.</param>
        /// <returns>The formatted error string.</returns>
        public static string GetReadableString(this ModelStateDictionary dictionary)
        {
            if (dictionary.IsValid)
            {
                return string.Empty;
            }

            var fields = dictionary.Keys
                .Select(key => (key, dictionary.Root.GetModelStateForProperty(key)))
                .Select(field => field.Item1 + ": " +
                    field.Item2.Errors
                        .Select(error => error.ErrorMessage)
                        .Select(error => error.TrimEnd('.'))
                        .Aggregate((a, b) => a + ", " + b));

            return fields.Aggregate((a, b) => a + "; " + b);
        }
    }
}