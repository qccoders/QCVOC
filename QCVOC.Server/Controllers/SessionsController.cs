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
    public class SessionsController : Controller
    {
        #region Public Constructors

        public SessionsController(
            IRepository<Account> accountRepository, 
            IJwtFactory jwtFactory, 
            IRepository<RefreshToken> refreshTokenRepository)
        {
            AccountRepository = accountRepository;
            JwtFactory = jwtFactory;
            RefreshTokenRepository = refreshTokenRepository;
        }

        #endregion Public Constructors

        #region Private Properties

        private IRepository<Account> AccountRepository { get; set; }
        private IJwtFactory JwtFactory { get; set; }
        private IRepository<RefreshToken> RefreshTokenRepository { get; set; }

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
        [ProducesResponseType(typeof(JwtSecurityToken), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult CreateSession([FromBody]SessionInfo sessionInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Account account = AccountRepository
            .GetAll()
            .Where(a => a.Name == sessionInfo.Name)
            .FirstOrDefault();

            if (account == default(Account))
            {
                return Unauthorized();
            }

            string passwordHash = Utility.ComputeSHA512Hash(sessionInfo.Password);

            if (passwordHash != account.PasswordHash)
            {
                return Unauthorized();
            }

            var accessToken = JwtFactory.GetAccessToken(account);

            var token = RefreshTokenRepository.Get(account.Id);
            Guid refreshTokenId;

            if (token == default(RefreshToken))
            {
                refreshTokenId = Guid.NewGuid();

                var refreshToken = new RefreshToken()
                {
                    TokenId = Guid.NewGuid(),
                    AccountId = account.Id,
                    Issued = DateTime.UtcNow,
                    Expires = DateTime.UtcNow, // todo: figure this out
                };

                // todo: error handling? returns 500 regardless, can improve messaging
                RefreshTokenRepository.Create(refreshToken);
            }

            var refreshJwt = JwtFactory.GetRefreshToken(refreshTokenId);
            return Ok(new JwtResponse(accessToken, refreshJwt));
        }

        [HttpPut]
        [ProducesResponseType(typeof(JwtSecurityToken), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult RefreshSession([FromBody]string refreshToken)
        {
            JwtSecurityToken refreshJwt;
            var badRequest = BadRequest("The provided refresh token is invalid.");

            if (!JwtFactory.TryParseJwtSecurityToken(refreshToken, out refreshJwt))
            {
                return badRequest;
            }

            var claim = refreshJwt.Claims.Where(c => c.Type == ClaimTypes.Hash).FirstOrDefault();

            if (claim == default(Claim))
            {
                return badRequest;
            }

            var id = claim.Value;
            Guid guid;

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out guid))
            {
                return badRequest;
            }

            var token = RefreshTokenRepository.Get(guid);

            if (token == default(RefreshToken))
            {
                return Unauthorized();
            }

            var account = AccountRepository.Get(token.AccountId);

            if (account == default(Account))
            {
                return Unauthorized();
            }

            var accessToken = JwtFactory.GetAccessToken(account);

            return Ok(new JwtResponse(accessToken, refreshJwt));
        }

        #endregion Public Methods
    }
}