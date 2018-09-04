// <copyright file="TokenResponse.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Data.DTO
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Newtonsoft.Json;

    /// <summary>
    ///     DTO containing Access and Refresh tokens for a Token request response.
    /// </summary>
    public class TokenResponse
    {
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
        public long Expires => ((DateTimeOffset)AccessJwtSecurityToken.ValidTo).ToUnixTimeSeconds();

        /// <summary>
        ///     Gets the value of the NameIdentifier claim from the Access Token.
        /// </summary>
        public Guid Id => Guid.Parse(AccessJwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value);

        /// <summary>
        ///     Gets the time at which the Access Token was issued.
        /// </summary>
        public long Issued => ((DateTimeOffset)AccessJwtSecurityToken.ValidFrom).ToUnixTimeSeconds();

        /// <summary>
        ///     Gets a value indicating whether a password reset for the Account is required.
        /// </summary>
        public bool PasswordResetRequired => bool.Parse(AccessJwtSecurityToken.Claims.Where(c => c.Type == "pwd").SingleOrDefault().Value);

        /// <summary>
        ///     Gets the value of the Name claim from the Access Token.
        /// </summary>
        public string Name => AccessJwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Name).SingleOrDefault().Value;

        /// <summary>
        ///     Gets the value of the Not Before claim from the Access Token.
        /// </summary>
        public long NotBefore => long.Parse(AccessJwtSecurityToken.Claims.Where(c => c.Type == "nbf").SingleOrDefault().Value);

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
        ///     Gets the value of the Role claim from the Access Token.
        /// </summary>
        public string Role => AccessJwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Role).SingleOrDefault().Value;

        /// <summary>
        ///     Gets the Token type.
        /// </summary>
        public string TokenType => JwtBearerDefaults.AuthenticationScheme;
    }
}