namespace QCVOC.Server.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Data.DTO;
    using QCVOC.Server.Data.Model;
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

        public SessionsController(IRepository<Account> accountRepository, IJwtFactory jwtFactory)
        {
            AccountRepository = accountRepository;
            JwtFactory = jwtFactory;
        }

        #endregion Public Constructors

        #region Private Properties

        private IRepository<Account> AccountRepository { get; set; }
        private IJwtFactory JwtFactory { get; set; }

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
        public IActionResult Post([FromBody]SessionInfo sessionInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Account account = AccountRepository.GetAll().Where(a => a.Name == sessionInfo.Name).FirstOrDefault();

            if (account == default(Account))
            {
                return Unauthorized();
            }

            string passwordHash = Utility.ComputeSHA512Hash(sessionInfo.Password);

            if (passwordHash != account.PasswordHash)
            {
                return Unauthorized();
            }

            var jwt = JwtFactory.GetJwt(account);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(token);
        }

        #endregion Public Methods
    }
}