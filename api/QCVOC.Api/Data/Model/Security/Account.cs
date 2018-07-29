﻿// <copyright file="Account.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Model.Security
{
    using System;
    using Dapper.Contrib.Extensions;

    public class Account : IEquatable<Account>
    {
        #region Public Properties

        [ExplicitKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

        public bool Equals(Account other)
        {
            if (other == null)
                return (this == null);
            
            return this.Id == other.Id 
            && this.Name == other.Name
            && this.PasswordHash == other.PasswordHash
            && this.Role == other.Role;
        }

        #endregion Public Properties
    }
}