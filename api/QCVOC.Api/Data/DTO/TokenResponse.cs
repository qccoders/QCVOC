﻿// <copyright file="TokenResponse.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Data.DTO
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Newtonsoft.Json;

    /// <summary>
    ///     DTO containing Access and Refresh tokens for a Token request response.
    /// </summary>
    public class TokenResponse
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        /// <param name="accessJwtSecurityToken">The Access token.</param>
        /// <param name="refreshJwtSecurityToken">The Response token.</param>
        public TokenResponse(JwtSecurityToken accessJwtSecurityToken, JwtSecurityToken refreshJwtSecurityToken)
        {
            AccessJwtSecurityToken = accessJwtSecurityToken;
            RefreshJwtSecurityToken = refreshJwtSecurityToken;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Access Token.
        /// </summary>
        [JsonIgnore]
        public JwtSecurityToken AccessJwtSecurityToken { get; set; }

        /// <summary>
        ///     Gets the Access Token string.
        /// </summary>
        public string AccessToken => new JwtSecurityTokenHandler().WriteToken(AccessJwtSecurityToken);

        /// <summary>
        ///     Gets the time at which the Access Token expires.
        /// </summary>
        public DateTime Expires => AccessJwtSecurityToken.ValidTo;

        /// <summary>
        ///     Gets the time at which the Access Token was issued.
        /// </summary>
        public DateTime Issued => AccessJwtSecurityToken.ValidFrom;

        /// <summary>
        ///     Gets or sets the Refresh Token.
        /// </summary>
        [JsonIgnore]
        public JwtSecurityToken RefreshJwtSecurityToken { get; set; }

        /// <summary>
        ///     Gets the Refresh Token string.
        /// </summary>
        public string RefreshToken => new JwtSecurityTokenHandler().WriteToken(RefreshJwtSecurityToken);

        /// <summary>
        ///     Gets the Refresh Token Guid.
        /// </summary>
        [JsonIgnore]
        public Guid RefreshTokenId => Guid.Parse(RefreshJwtSecurityToken.Id);

        /// <summary>
        ///     Gets the Token type.
        /// </summary>
        public string TokenType => JwtBearerDefaults.AuthenticationScheme;

        #endregion Public Properties
    }
}