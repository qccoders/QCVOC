// <copyright file="VeteransController.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Veterans.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QCVOC.Api.Common;
    using QCVOC.Api.Security;
    using QCVOC.Api.Veterans.Data.DTO;
    using QCVOC.Api.Veterans.Data.Model;
    using QCVOC.Api.Veterans.Data.Repository;

    /// <summary>
    ///     Provides endpoints for manipulation of Veteran records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class VeteransController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VeteransController"/> class.
        /// </summary>
        /// <param name="veteranRepository">The repository used for Veteran data access.</param>
        public VeteransController(IVeteranRepository veteranRepository)
        {
            VeteranRepository = veteranRepository;
        }

        private IVeteranRepository VeteranRepository { get; set; }

        /// <summary>
        ///     Returns a list of Veterans.
        /// </summary>
        /// <param name="filters">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Veteran>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]VeteranFilters filters)
        {
            return Ok(VeteranRepository.GetAll(filters));
        }

        /// <summary>
        ///     Returns the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran to retrieve.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Veteran was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Veteran matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Veteran>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get([FromRoute]Guid id)
        {
            var veteran = VeteranRepository.Get(id);

            if (veteran == default(Veteran))
            {
                return NotFound();
            }

            return Ok(veteran);
        }

        /// <summary>
        ///     Returns the base 64 encoded photo for the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran for which the photo is to be retrieved.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The base 64 encoded photo was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Veteran matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{id}/photo")]
        [Authorize]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetPhotoBase64([FromRoute]Guid id)
        {
            var photoBase64 = VeteranRepository.GetPhotoBase64(id);

            if (string.IsNullOrEmpty(photoBase64))
            {
                return NotFound();
            }

            return Ok(photoBase64);
        }

        /// <summary>
        ///     Enrolls a new Veteran.
        /// </summary>
        /// <param name="veteran">The Veteran to enroll.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Veteran was enrolled successfully.</response>
        /// <response code="400">The specified Veteran was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="409">The card number is already assigned to another Veteran, or a Veteran with the same first and last names and address already exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("")]
        [Authorize]
        [ProducesResponseType(typeof(Veteran), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Enroll([FromBody]VeteranEnrollRequest veteran)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetReadableString());
            }

            var existingVeteran = Enumerable.Empty<Veteran>();
            if (veteran.CardNumber != null)
            {
                existingVeteran = VeteranRepository.GetAll(new VeteranFilters()
                {
                    CardNumber = veteran.CardNumber,
                });
            }

            if (existingVeteran.Any())
            {
                return Conflict($"Card number {veteran.CardNumber} is already assigned to another veteran.");
            }

            existingVeteran = VeteranRepository.GetAll(new VeteranFilters()
            {
                FirstName = veteran.FirstName,
                LastName = veteran.LastName,
                Address = veteran.Address,
            });

            if (existingVeteran.Any())
            {
                return Conflict($"A Veteran with a matching first name, last name and address already exists.");
            }

            var veteranRecord = new Veteran()
            {
                Address = veteran.Address,
                Email = veteran.Email,
                EnrollmentDate = DateTime.UtcNow,
                EnrollmentById = User.GetId(),
                FirstName = veteran.FirstName,
                Id = Guid.NewGuid(),
                LastName = veteran.LastName,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
                CardNumber = veteran.CardNumber,
                PhotoBase64 = veteran.PhotoBase64,
                PrimaryPhone = veteran.PrimaryPhone,
                VerificationMethod = veteran.VerificationMethod,
            };

            try
            {
                var createdVeteran = VeteranRepository.Create(veteranRecord);
                return StatusCode(201, createdVeteran);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Veteran: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Updates the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran to update.</param>
        /// <param name="veteran">The updated Veteran.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Veteran was updated successfully.</response>
        /// <response code="400">The specified Veteran was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Veteran matching the specified id could not be found.</response>
        /// <response code="409">The card number is already assigned to another Veteran.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Veteran), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update([FromRoute]Guid id, [FromBody]VeteranUpdateRequest veteran)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetReadableString());
            }

            var veteranToUpdate = VeteranRepository.Get(id);

            if (veteranToUpdate == default(Veteran))
            {
                return NotFound();
            }

            var conflictingVeterans = Enumerable.Empty<Veteran>();

            if (veteran.CardNumber != null)
            {
                var duplicateId = VeteranRepository
                    .GetAll(new VeteranFilters() { CardNumber = veteran.CardNumber })
                    .Any(p => p.Id != id);

                if (duplicateId)
                {
                    return Conflict($"Card number {veteran.CardNumber} is already assigned to another veteran.");
                }
            }

            var duplicateVeteran = VeteranRepository.GetAll(new VeteranFilters()
            {
                FirstName = veteran.FirstName,
                LastName = veteran.LastName,
                Address = veteran.Address,
            }).Any(p => p.Id != id);

            if (duplicateVeteran)
            {
                return Conflict($"A Veteran with a matching first name, last name and address already exists.");
            }

            var veteranRecord = new Veteran()
            {
                Address = veteran.Address,
                Email = veteran.Email,
                FirstName = veteran.FirstName,
                Id = veteranToUpdate.Id,
                LastName = veteran.LastName,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
                CardNumber = veteran.CardNumber,
                PhotoBase64 = veteran.PhotoBase64,
                PrimaryPhone = veteran.PrimaryPhone,
                VerificationMethod = veteran.VerificationMethod,
            };

            try
            {
                var updatedVeteran = VeteranRepository.Update(veteranRecord);
                return Ok(updatedVeteran);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Veteran: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Updates base 64 encoded photo for the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran for which the base 64 encoded photo is to be updated.</param>
        /// <param name="photoBase64">The updated base 64 encoded photo.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The base 64 encoded photo for the Veteran was updated successfully.</response>
        /// <response code="400">The specified base 64 encoded photo was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Veteran matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("{id}/photo")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult UpdatePhotoBase64([FromRoute]Guid id, [FromBody]string photoBase64)
        {
            if (string.IsNullOrWhiteSpace(photoBase64))
            {
                return BadRequest("The photo data is empty.");
            }

            var veteranToUpdate = VeteranRepository.Get(id);

            if (veteranToUpdate == default(Veteran))
            {
                return NotFound();
            }

            try
            {
                var updatedPhotoBase64 = VeteranRepository.UpdatePhotoBase64(veteranToUpdate.Id, photoBase64);
                return Ok(updatedPhotoBase64);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating the photo for Veteran: {ex.Message}.  See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Deletes the Veteran matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Veteran to delete.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Veteran was deleted successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">A Veteran matching the specified id could not be found.</response>
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
            var veteran = VeteranRepository.Get(id);

            if (veteran == default(Veteran))
            {
                return NotFound();
            }

            try
            {
                VeteranRepository.Delete(veteran);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the specified Veteran: {ex.Message}. See inner exception for details.", ex);
            }
        }
    }
}