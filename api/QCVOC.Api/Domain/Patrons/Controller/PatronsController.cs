// <copyright file="AccountsController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Patrons.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Domain.Patrons.Data.DTO;
    using QCVOC.Api.Domain.Patrons.Data.Model;
    using QCVOC.Api.Security;

    [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PatronsController : Controller
    {
        public PatronsController(IRepository<Patron> patronRepository)
        {
            PatronRepository = patronRepository;
        }

        private IRepository<Patron> PatronRepository { get; set; }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(OkResult), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                var err = new ModelStateDictionary();
                err.AddModelError("id", "The requested Id must be a valid Guid.");

                return BadRequest(err);
            }

            try
            {
                PatronRepository.Delete(guid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception("Error retrieving the specified Patron. See inner exception for details.", ex));
            }

            return Ok();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatronResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                var err = new ModelStateDictionary();
                err.AddModelError("id", "The requested Id must be a valid Guid.");

                return BadRequest(err);
            }

            Patron patron = default(Patron);

            try
            {
                patron = PatronRepository.Get(guid);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception("Error retrieving the specified Patron. See inner exception for details.", ex));
            }

            if (patron == default(Patron))
            {
                return NotFound();
            }

            return Ok(MapPatronResponseFrom(patron));
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<PatronResponse>), 200)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll()
        {
            return Ok(PatronRepository.GetAll().Select(a => MapPatronResponseFrom(a)));
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(PatronResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Post(Patron patron)
        {
            if (patron == null)
            {
                return BadRequest("Patron cannot be null.");
            }

            ModelStateDictionary err = ValidatePatron(patron);

            if (err.Keys.Any())
            {
                return BadRequest(err);
            }

            Patron patronResponse = default(Patron);

            try
            {
                patronResponse = PatronRepository.Update(patron);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception("Error createing the specified Patron. See inner exception for details.", ex));
            }

            return Ok(MapPatronResponseFrom(patronResponse));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PatronResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Put(Patron patron)
        {
            if (patron == null)
            {
                return BadRequest("Patron cannot be null.");
            }

            ModelStateDictionary err = ValidatePatron(patron);

            if (err.Keys.Any())
            {
                return BadRequest(err);
            }

            Patron patronResponse = default(Patron);

            try
            {
                patronResponse = PatronRepository.Create(patron);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Exception("Error createing the specified Patron. See inner exception for details.", ex));
            }

            return Ok(MapPatronResponseFrom(patronResponse));
        }

        public ModelStateDictionary ValidatePatron(Patron patron)
        {
            var err = new ModelStateDictionary();

            if (patron.MemberId <= 0)
            {
                err.AddModelError("memberId", "The patron's memberId must be a positive number.");
            }

            if (string.IsNullOrWhiteSpace(patron.FirstName))
            {
                err.AddModelError("firstName", "The patron's first name must be alphanumeric.");
            }

            if (string.IsNullOrWhiteSpace(patron.LastName))
            {
                err.AddModelError("lastName", "The patron's last name must be alphanumeric.");
            }

            if (string.IsNullOrWhiteSpace(patron.Address))
            {
                err.AddModelError("address", "The patron's address must be alphanumeric.");
            }

            // TODO: Better phone number validation.
            if (string.IsNullOrWhiteSpace(patron.PrimaryPhone))
            {
                err.AddModelError("primaryPhone", "The patron's primary phone number must be a valid phone number.");
            }

            // TODO: Better phone number validation.
            if (patron.SecondaryPhone != null && patron.SecondaryPhone.All(p => char.IsWhiteSpace(p)))
            {
                err.AddModelError("secondayPhone", "The patron's secondary phone number must be a valid phone number.");
            }

            if (string.IsNullOrWhiteSpace(patron.Email))
            {
                err.AddModelError("email", "The patron's email must be alphanumeric.");
            }

            if (patron.EnrollmentDate == null)
            {
                err.AddModelError("enrollmentDate", "The patron's enrollment date must be a valid date.");
            }

            return err;
        }

        private PatronResponse MapPatronResponseFrom(Patron patron)
        {
            return new PatronResponse(patron.Id, patron.MemberId, patron.FirstName,
            patron.LastName, patron.Address, patron.PrimaryPhone, patron.SecondaryPhone,
            patron.Email, patron.EnrollmentDate);
        }
    }
}