// <copyright file="TokensController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Server.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Data.DTO;
    using QCVOC.Server.Data.DTO;
    using QCVOC.Server.Data.Model.Security;
    using QCVOC.Server.Data.Repository;
    using QCVOC.Server.Security;

    /// <summary>
    ///     Provides endpoints for manipulation of API authentication Access and Refresh Tokens.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TokensController : Controller
    {
        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokensController"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository used for retrieval of user accounts.</param>
        /// <param name="tokenFactory">The factory used to create new tokens.</param>
        /// <param name="tokenValidator">The validator used to validate tokens.</param>
        /// <param name="refreshTokenRepository">The repository used for refresh token persistence.</param>
        public TokensController(IRepository<Account> accountRepository, ITokenFactory tokenFactory, ITokenValidator tokenValidator, IRepository<RefreshToken> refreshTokenRepository)
        {
            AccountRepository = accountRepository;
            TokenFactory = tokenFactory;
            RefreshTokenRepository = refreshTokenRepository;
            TokenValidator = tokenValidator;
        }

        #endregion Public Constructors

        #region Private Properties

        private IRepository<Account> AccountRepository { get; set; }
        private IRepository<RefreshToken> RefreshTokenRepository { get; set; }
        private ITokenFactory TokenFactory { get; set; }
        private ITokenValidator TokenValidator { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        ///     Issues a new Access and new or existing Refresh Token given login credentials.
        /// </summary>
        /// <param name="credentials">The login credentials.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Tokens were issued successfully.</response>
        /// <response code="400">The specified credentials were invalid.</response>
        /// <response code="401">Authentication failed.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Create([FromBody]TokenRequest credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var accountRecord = AccountRepository.GetAll()
                .Where(a => a.Name == credentials.Name)
                .FirstOrDefault();

            if (accountRecord == default(Account))
            {
                return Unauthorized();
            }

            if (Utility.ComputeSHA512Hash(credentials.Password) != accountRecord.PasswordHash)
            {
                return Unauthorized();
            }

            var refreshTokenRecord = RefreshTokenRepository.GetAll()
                .Where(r => r.AccountId == accountRecord.Id)
                .Where(r => r.Expires >= DateTime.UtcNow)
                .FirstOrDefault();

            if (refreshTokenRecord == default(RefreshToken))
            {
                refreshTokenRecord = new RefreshToken()
                {
                    TokenId = Guid.NewGuid(),
                    AccountId = accountRecord.Id,
                    Issued = DateTime.UtcNow,
                    Expires = DateTime.UtcNow,
                };

                RefreshTokenRepository.Create(refreshTokenRecord);
            }

            var accessJwt = TokenFactory.GetAccessToken(accountRecord, refreshTokenRecord.TokenId);
            var refreshJwt = TokenFactory.GetRefreshToken(refreshTokenRecord.TokenId);

            var response = new TokenResponse(accessJwt, refreshJwt);
            return Ok(response);
        }

        /// <summary>
        ///     Issues a new Access Token given a Refresh Token.
        /// </summary>
        /// <param name="refreshToken">The existing Refresh Token.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Token was issued successfully.</response>
        /// <response code="400">The specified Refresh Token was blank.</response>
        /// <response code="401">The specified Refresh Token was invalid.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Refresh([FromBody]string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("The specified token is blank.");
            }

            if (!TokenValidator.TryParseAndValidateToken(refreshToken, out JwtSecurityToken jwtSecurityToken))
            {
                return Unauthorized();
            }

            var refreshTokenIdString = jwtSecurityToken.Claims.Where(c => c.Type == "jti").FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(refreshTokenIdString))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(refreshTokenIdString, out Guid refreshTokenId))
            {
                return Unauthorized();
            }

            var token = RefreshTokenRepository.Get(refreshTokenId);

            if (token == default(RefreshToken) || token.Expires <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var account = AccountRepository.Get(token.AccountId);

            if (account == default(Account))
            {
                return Unauthorized();
            }

            var response = TokenFactory.GetAccessToken(account, token.TokenId);
            return Ok(response);
        }

        #endregion Public Methods
    }
}