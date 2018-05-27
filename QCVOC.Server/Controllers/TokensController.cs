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

    [AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TokensController : Controller
    {
        #region Public Constructors
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
        private ITokenFactory TokenFactory { get; set; }
        private IRepository<RefreshToken> RefreshTokenRepository { get; set; }
        private ITokenValidator TokenValidator { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        ///     Starts a new session with the provided name and password.
        /// </summary>
        /// <param name="sessionInfo">The name and password with which to create the session.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">Session started successfully.</response>
        /// <response code="400">The provided input is invalid.</response>
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

            return CreateSession(credentials);
        }

        [HttpPut]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Refresh([FromBody]string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken) || TokenValidator.TryValidateToken(refreshToken))
            {
                return BadRequest("The specified token is invalid.");
            }

            return GetAccessTokenFromRefreshToken(refreshToken);
        }

        #endregion Public Methods

        #region Private Methods

        private IActionResult CreateSession(TokenRequest credentials)
        {
            if (string.IsNullOrEmpty(credentials.Name) || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest("The specified session info is invalid; both a user name and password must be supplied.");
            }

            var account = AccountRepository.GetAll().Where(a => a.Name == credentials.Name).FirstOrDefault();

            if (account == default(Account))
            {
                return Unauthorized();
            }

            string passwordHash = Utility.ComputeSHA512Hash(credentials.Password);

            if (passwordHash != account.PasswordHash)
            {
                return Unauthorized();
            }

            RefreshToken refreshTokenRecord = null;
            var refreshTokenId = Guid.NewGuid();

            if (refreshTokenRecord == null)
            {
                var refreshToken = new RefreshToken()
                {
                    TokenId = refreshTokenId,
                    AccountId = account.Id,
                    Issued = DateTime.UtcNow,
                    Expires = DateTime.UtcNow,
                };

                //RefreshTokenRepository.Create(refreshToken);
            }
            else
            {
                refreshTokenId = refreshTokenRecord.TokenId;
            }

            var jwt = TokenFactory.GetAccessToken(account, refreshTokenId);
            return Ok(jwt);
        }

        private IActionResult GetAccessTokenFromRefreshToken(string refreshToken)
        {
            if (!TokenValidator.TryParseAndValidateToken(refreshToken, out JwtSecurityToken jwtSecurityToken))
            {
                return Unauthorized();
            }

            var refreshTokenIdString = jwtSecurityToken.Claims.Where(c => c.Type == ClaimTypes.Hash).FirstOrDefault()?.Value;

            if (string.IsNullOrEmpty(refreshTokenIdString))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(refreshTokenIdString, out Guid refreshTokenId))
            {
                return Unauthorized();
            }

            var token = RefreshTokenRepository.Get(refreshTokenId);

            if (token == default(RefreshToken) && token.Expires <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var account = AccountRepository.Get(token.AccountId);

            if (account == default(Account))
            {
                return Unauthorized();
            }

            return Ok(TokenFactory.GetAccessToken(account, token.TokenId));
        }

        #endregion Private Methods
    }
}