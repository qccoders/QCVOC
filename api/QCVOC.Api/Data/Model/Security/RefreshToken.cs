﻿// <copyright file="RefreshToken.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.Model.Security
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Dapper.Contrib.Extensions;

    public class RefreshToken : IEquatable<RefreshToken>
    {
        [ExplicitKey]
        public Guid AccountId { get; set; }

        [Column("expires")]
        public DateTime Expires { get; set; }

        [Column("issued")]
        public DateTime Issued { get; set; }

        [Column("tokenid")]
        public Guid TokenId { get; set; }

        public bool Equals(RefreshToken token)
        {
            if (token == null)
            {
                return this == null;
            }

            return this.TokenId == token.TokenId
            && this.AccountId == token.AccountId
            && (this.Expires - token.Expires <= TimeSpan.FromSeconds(1))
            && (this.Issued - token.Issued <= TimeSpan.FromSeconds(1));
        }
    }
}