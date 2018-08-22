// <copyright file="SecurityController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Security.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Security;
    using QCVOC.Api.Security.Data.DTO;
    using QCVOC.Api.Security.Data.Model;

    /// <summary>
    ///     Provides endpoints for manipulation of API authentication Access and Refresh Tokens.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class SecurityController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SecurityController"/> class.
        /// </summary>
        /// <param name="accountRepository">The repository used for retrieval of user accounts.</param>
        /// <param name="tokenFactory">The factory used to create new tokens.</param>
        /// <param name="tokenValidator">The validator used to validate tokens.</param>
        /// <param name="refreshTokenRepository">The repository used for refresh token persistence.</param>
        public SecurityController(IRepository<Account> accountRepository, ITokenFactory tokenFactory, ITokenValidator tokenValidator, IRepository<RefreshToken> refreshTokenRepository)
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
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Login([FromBody]TokenRequest credentials)
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

            PurgeExpiredRefreshTokensFor(accountRecord.Id);

            var refreshTokenRecord = GetCurrentRefreshTokenRecordFor(accountRecord.Id);

            JwtSecurityToken refreshJwt;

            if (refreshTokenRecord == default(RefreshToken))
            {
                refreshJwt = TokenFactory.GetRefreshToken();

                refreshTokenRecord = new RefreshToken()
                {
                    Id = Guid.Parse(refreshJwt.Claims.Where(c => c.Type == "jti").FirstOrDefault().Value),
                    AccountId = accountRecord.Id,
                    Issued = refreshJwt.ValidFrom,
                    Expires = refreshJwt.ValidTo,
                };

                RefreshTokenRepository.Create(refreshTokenRecord);
            }
            else
            {
                refreshJwt = TokenFactory.GetRefreshToken(refreshTokenRecord.Id, refreshTokenRecord.Expires, refreshTokenRecord.Issued);
            }

            var accessJwt = TokenFactory.GetAccessToken(accountRecord, refreshTokenRecord.Id);
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
        [HttpPost("refresh")]
        [AllowAnonymous]
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

            var accessJwt = TokenFactory.GetAccessToken(account, refreshTokenRecord.Id);
            var refreshJwt = TokenFactory.GetRefreshToken(refreshTokenId, refreshTokenRecord.Expires, refreshTokenRecord.Issued);
            var response = new TokenResponse(accessJwt, refreshJwt);

            return Ok(response);
        }

        /// <summary>
        ///     Deletes and invalidates the Refresh Token for the current session.
        /// </summary>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Refresh Token was deleted and invalidated successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Logout()
        {
            var refreshTokenIdString = User?.Claims?.Where(c => c.Type == "jti")?.SingleOrDefault()?.Value;

            if (Guid.TryParse(refreshTokenIdString, out var refreshTokenId))
            {
                RefreshTokenRepository.Delete(refreshTokenId);
                return NoContent();
            }

            return StatusCode(500, new Exception($"The 'jti' claim is missing or contains an invalid id ({refreshTokenIdString})."));
        }

        /// <summary>
        ///     Returns a list of Accounts.
        /// </summary>
        /// <param name="queryParams">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("accounts")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(IEnumerable<AccountResponse>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]AccountQueryParameters queryParams)
        {
            return Ok(AccountRepository.GetAll(queryParams).Select(a => MapAccountResponseFrom(a)));
        }

        /// <summary>
        ///     Returns the Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Account to retrieve.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Account was retrieved successfully.</response>
        /// <response code="400">The specified id was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">An Account matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("accounts/{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get(Guid id)
        {
            var account = AccountRepository.Get(id);

            if (account == default(Account))
            {
                return NotFound();
            }

            return Ok(MapAccountResponseFrom(account));
        }

        /// <summary>
        ///     Creates a new Account.
        /// </summary>
        /// <param name="account">The Account to create.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Account was created successfully.</response>
        /// <response code="400">The specified Account was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="409">An Account with the specified name aleady exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("accounts")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(AccountResponse), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Create([FromBody]AccountCreateRequest account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (account.Role == Role.Administrator && !User.IsInRole(nameof(Role.Administrator)))
            {
                return StatusCode(403, "Administrative accounts may not be created by non-Administrative users.");
            }

            var existingAccounts = AccountRepository.GetAll();

            if (existingAccounts.Any(a => a.Name == account.Name))
            {
                return Conflict($"A user named '{account.Name}' already exists.");
            }

            var accountRecord = new Account()
            {
                Id = Guid.NewGuid(),
                Name = account.Name,
                Role = account.Role,
                PasswordHash = Utility.ComputeSHA512Hash(account.Password),
            };

            try
            {
                var createdAccount = AccountRepository.Create(accountRecord);
                return Ok(MapAccountResponseFrom(createdAccount));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating account for {account.Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Updates the Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Account to update.</param>
        /// <param name="account">The updated Account.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Account was updated successfully.</response>
        /// <response code="400">The specified Account was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">An Account matching the specified id could not be found.</response>
        /// <response code="409">An Account with the specified name aleady exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("accounts/{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update(Guid id, [FromBody]AccountUpdateRequest account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (account.Role == Role.Administrator && !User.IsInRole(nameof(Role.Administrator)))
            {
                return StatusCode(403, "Accounts may not be promited to Administrative by non-Administrative users.");
            }

            var existingAccounts = AccountRepository.GetAll();
            var existingAccountRecord = existingAccounts.Where(a => a.Id == id).FirstOrDefault();

            if (existingAccountRecord == default(Account))
            {
                return NotFound();
            }

            if (existingAccountRecord.Role == Role.Administrator && account.Role != Role.Administrator && !User.IsInRole(nameof(Role.Administrator)))
            {
                return StatusCode(403, "Administrators may not be demoted by non-Administrative users.");
            }

            if (existingAccounts.Any(a => a.Id != id && a.Name == account.Name))
            {
                return Conflict($"A user named '{account.Name}' already exists.");
            }

            var accountRecord = new Account()
            {
                Id = existingAccountRecord.Id,
                Name = account.Name ?? existingAccountRecord.Name,
                Role = account.Role ?? existingAccountRecord.Role,
                PasswordHash = account.Password == null ? existingAccountRecord.PasswordHash :
                    Utility.ComputeSHA512Hash(account.Password),
            };

            try
            {
                var updatedAccount = AccountRepository.Update(accountRecord);
                return Ok(MapAccountResponseFrom(updatedAccount));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating account for '{account.Name ?? id.ToString()}': {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Deletes the Account matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Account to dete.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Account was deleted successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">An Account matching the specified id could not be found.</response>
        /// <response code="409">The request would result in a configuration conflict.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpDelete("accounts/{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete(Guid id)
        {
            var account = AccountRepository.Get(id);

            if (account == default(Account))
            {
                return NotFound();
            }

            if (account.Role == Role.Administrator && !User.IsInRole(nameof(Role.Administrator)))
            {
                return StatusCode(403, "Administrative accounts may not be deleted by non-Administrative users.");
            }

            var accounts = AccountRepository.GetAll(new AccountQueryParameters() { Role = Role.Administrator });

            if (!accounts.Where(a => a.Id != id).Any())
            {
                return Conflict($"At least one administrative account must remain.  Create a new administrative account, then repeat this request to complete deletion.");
            }

            try
            {
                AccountRepository.Delete(account);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting account for '{account.Name ?? id.ToString()}': {ex.Message}", ex);
            }
        }

        private AccountResponse MapAccountResponseFrom(Account account)
        {
            return new AccountResponse()
            {
                Id = account.Id,
                Name = account.Name,
                Role = account.Role,
            };
        }

        private RefreshToken GetCurrentRefreshTokenRecordFor(Guid accountId)
        {
            return RefreshTokenRepository.GetAll(new RefreshTokenQueryParameters { AccountId = accountId })
                .Where(r => r.Expires >= DateTime.UtcNow)
                .FirstOrDefault();
        }

        private void PurgeExpiredRefreshTokensFor(Guid accountId)
        {
            var expiredRecords = RefreshTokenRepository.GetAll(new RefreshTokenQueryParameters { AccountId = accountId })
                .Where(r => r.Expires < DateTime.UtcNow);

            foreach (var record in expiredRecords)
            {
                RefreshTokenRepository.Delete(record.Id);
            }
        }
    }
}