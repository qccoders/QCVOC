// <copyright file="AccountsController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Accounts.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Domain.Accounts.Model;
    using QCVOC.Api.Security;

    [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountsController : Controller
    {
        public AccountsController(IRepository<Account> accountRepository)
        {
            AccountRepository = accountRepository;
        }

        private IRepository<Account> AccountRepository { get; set; }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete(Guid id)
        {
            var account = AccountRepository.Get(id);

            if (account == default(Account))
            {
                return NotFound();
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update(Guid id, [FromBody]AccountUpdateRequest account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAccounts = AccountRepository.GetAll();
            var existingAccountRecord = existingAccounts.Where(a => a.Id == id).FirstOrDefault();

            if (existingAccountRecord == default(Account))
            {
                return NotFound();
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

        [HttpPost("")]
        [ProducesResponseType(typeof(AccountResponse), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Create([FromBody]AccountCreateRequest account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                var err = new ModelStateDictionary();
                err.AddModelError("id", "The requested Id must be a valid Guid.");

                return BadRequest(err);
            }

            Account account = default(Account);

            try
            {
                account = AccountRepository.Get(guid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception("Error retrieving the specified Account. See inner exception for details.", ex));
            }

            if (account == default(Account))
            {
                return NotFound();
            }

            return Ok(MapAccountResponseFrom(account));
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<AccountResponse>), 200)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]QueryParameters queryParams)
        {
            return Ok(AccountRepository.GetAll(queryParams).Select(a => MapAccountResponseFrom(a)));
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
    }
}