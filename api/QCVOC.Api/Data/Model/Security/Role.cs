// <copyright file="Role.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Model.Security
{
    /// <summary>
    ///     Defines application User Roles.
    /// </summary>
    public enum Role
    {
        /// <summary>
        ///     The default User Role.
        /// </summary>
        User = 0,

        /// <summary>
        ///     The supervisory User Role.
        /// </summary>
        Supervisor = 1,

        /// <summary>
        ///     The administrative User Role.
        /// </summary>
        Administrator = 2,
    }
}