// <copyright file="AccountsController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Data.DTO;
    using QCVOC.Api.Data.Model.Security;
    using QCVOC.Api.Data.Repository;

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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get(string id)
        {
            Guid guid;

            if (!Guid.TryParse(id, out guid))
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
        public IActionResult GetAll()
        {
            return Ok(AccountRepository.GetAll().Select(a => MapAccountResponseFrom(a)));
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