// <copyright file="TokensController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Controller
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common.Data;
    using QCVOC.Api.Domain.Accounts.Model;
    using QCVOC.Api.Security;
    using QCVOC.Api.Security.Data.DTO;
    using QCVOC.Api.Security.Data.Model;

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

        private IRepository<Account> AccountRepository { get; set; }
        private IRepository<RefreshToken> RefreshTokenRepository { get; set; }
        private ITokenFactory TokenFactory { get; set; }
        private ITokenValidator TokenValidator { get; set; }

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
        [Route("")]
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

            JwtSecurityToken refreshJwt;

            if (refreshTokenRecord == default(RefreshToken))
            {
                refreshJwt = TokenFactory.GetRefreshToken();

                refreshTokenRecord = new RefreshToken()
                {
                    TokenId = Guid.Parse(refreshJwt.Claims.Where(c => c.Type == "jti").FirstOrDefault().Value),
                    AccountId = accountRecord.Id,
                    Issued = refreshJwt.ValidFrom,
                    Expires = refreshJwt.ValidTo,
                };

                RefreshTokenRepository.Create(refreshTokenRecord);
            }
            else
            {
                refreshJwt = TokenFactory.GetRefreshToken(refreshTokenRecord.TokenId, refreshTokenRecord.Expires, refreshTokenRecord.Issued);
            }

            var accessJwt = TokenFactory.GetAccessToken(accountRecord, refreshTokenRecord.TokenId);
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
        [HttpPost]
        [Route("refresh")]
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

            if (!TokenValidator.TryParseAndValidateToken(refreshToken, out JwtSecurityToken parsedRefreshJwt))
            {
                return Unauthorized();
            }

            var refreshTokenIdString = parsedRefreshJwt.Claims.Where(c => c.Type == "jti").FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(refreshTokenIdString))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(refreshTokenIdString, out Guid refreshTokenId))
            {
                return Unauthorized();
            }

            var refreshTokenRecord = RefreshTokenRepository.Get(refreshTokenId);

            if (refreshTokenRecord == default(RefreshToken) || refreshTokenRecord.Expires <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var account = AccountRepository.Get(refreshTokenRecord.AccountId);

            if (account == default(Account))
            {
                return Unauthorized();
            }

            var accessJwt = TokenFactory.GetAccessToken(account, refreshTokenRecord.TokenId);
            var refreshJwt = TokenFactory.GetRefreshToken(refreshTokenId, refreshTokenRecord.Expires, refreshTokenRecord.Issued);
            var response = new TokenResponse(accessJwt, refreshJwt);

            return Ok(response);
        }
    }
}