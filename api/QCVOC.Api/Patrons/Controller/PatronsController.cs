// <copyright file="PatronsController.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Patrons.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Patrons.Data.DTO;
    using QCVOC.Api.Patrons.Data.Model;
    using QCVOC.Api.Security;

    /// <summary>
    ///     Provides endpoints for manipulation of Patron records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PatronsController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PatronsController"/> class.
        /// </summary>
        /// <param name="patronRepository">The repository used for Patron data access.</param>
        public PatronsController(IRepository<Patron> patronRepository)
        {
            PatronRepository = patronRepository;
        }

        private IRepository<Patron> PatronRepository { get; set; }

        /// <summary>
        ///     Returns a list of Patrons.
        /// </summary>
        /// <param name="filters">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Patron>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]PatronFilters filters)
        {
            return Ok(PatronRepository.GetAll(filters));
        }

        /// <summary>
        ///     Returns the Patron matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Patron to retrieve.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Patron was retrieved successfully.</response>
        /// <response code="400">The specified id was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Patron matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Patron>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get([FromRoute]Guid id)
        {
            var patron = PatronRepository.Get(id);

            if (patron == default(Patron))
            {
                return NotFound();
            }

            return Ok(patron);
        }

        /// <summary>
        ///     Enrolls a new Patron.
        /// </summary>
        /// <param name="patron">The Patron to enroll.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Patron was enrolled successfully.</response>
        /// <response code="400">The specified Patron was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="409">A Patron with the same member id or first and last names and address already exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("")]
        [Authorize]
        [ProducesResponseType(typeof(Patron), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Enroll([FromBody]PatronEnrollRequest patron)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPatron = Enumerable.Empty<Patron>();
            if (patron.MemberId != null)
            {
                existingPatron = PatronRepository.GetAll(new PatronFilters()
                {
                    MemberId = patron.MemberId,
                });
            }

            if (existingPatron.Any())
            {
                return Conflict($"A Patron with member id '{patron.MemberId}' already exists.");
            }

            existingPatron = PatronRepository.GetAll(new PatronFilters()
            {
                FirstName = patron.FirstName,
                LastName = patron.LastName,
                Address = patron.Address,
            });

            if (existingPatron.Any())
            {
                return Conflict($"A Patron with a matching first name, last name and address already exists.");
            }

            var patronRecord = new Patron()
            {
                Address = patron.Address,
                Email = patron.Email,
                EnrollmentDate = DateTime.UtcNow,
                FirstName = patron.FirstName,
                Id = Guid.NewGuid(),
                LastName = patron.LastName,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
                MemberId = patron.MemberId,
                PrimaryPhone = patron.PrimaryPhone,
                SecondaryPhone = patron.SecondaryPhone,
            };

            try
            {
                var createdPatron = PatronRepository.Create(patronRecord);
                return StatusCode(201, createdPatron);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Patron: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Updates the Patron matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Patron to update.</param>
        /// <param name="patron">The updated Patron.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Patron was updated successfully.</response>
        /// <response code="400">The specified Patron was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Patron matching the specified id could not be found.</response>
        /// <response code="409">A Patron with the same member id already exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Patron), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update([FromRoute]Guid id, [FromBody]PatronUpdateRequest patron)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patronToUpdate = PatronRepository.Get(id);

            if (patronToUpdate == default(Patron))
            {
                return NotFound();
            }

            var conflictingPatrons = Enumerable.Empty<Patron>();
            if (patron.MemberId != null)
            {
                conflictingPatrons = PatronRepository.GetAll(new PatronFilters() { MemberId = patron.MemberId });
            }

            if (conflictingPatrons.Any(p => p.Id != id))
            {
                return Conflict($"A Patron with member id '{patron.MemberId}' already exists.");
            }

            var patronRecord = new Patron()
            {
                Address = patron.Address ?? patronToUpdate.Address,
                Email = patron.Email ?? patronToUpdate.Email,
                EnrollmentDate = patronToUpdate.EnrollmentDate,
                FirstName = patron.FirstName ?? patronToUpdate.FirstName,
                Id = patronToUpdate.Id,
                LastName = patron.LastName ?? patronToUpdate.LastName,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
                MemberId = patron.MemberId ?? patronToUpdate.MemberId,
                PrimaryPhone = patron.PrimaryPhone ?? patronToUpdate.PrimaryPhone,
                SecondaryPhone = patron.SecondaryPhone ?? patronToUpdate.SecondaryPhone,
            };

            try
            {
                var updatedPatron = PatronRepository.Update(patronRecord);
                return Ok(updatedPatron);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Patron: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Deletes the Patron matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Patron to delete.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Patron was deleted successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">A Patron matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete([FromRoute]Guid id)
        {
            var patron = PatronRepository.Get(id);

            if (patron == default(Patron))
            {
                return NotFound();
            }

            try
            {
                PatronRepository.Delete(patron);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the specified Patron: {ex.Message}. See inner exception for details.", ex);
            }
        }
    }
}