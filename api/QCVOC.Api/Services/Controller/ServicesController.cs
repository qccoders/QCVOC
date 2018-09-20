// <copyright file = "ServicesController.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Services.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Security;
    using QCVOC.Api.Services.Data.DTO;
    using QCVOC.Api.Services.Data.Model;

    /// <summary>
    ///     Provides endpoints for manipulation of Service records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ServicesController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        /// <param name="serviceRepository">The respository used for Service data access.</param>
        public ServicesController(ISingleKeyRepository<Service> serviceRepository)
        {
            ServiceRepository = serviceRepository;
        }

        private ISingleKeyRepository<Service> ServiceRepository { get; set; }

        /// <summary>
        ///     Returns a list of Services.
        /// </summary>
        /// <param name="filters">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Service>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]ServiceFilters filters)
        {
            return Ok(ServiceRepository.GetAll(filters));
        }

        /// <summary>
        ///     Returns the Service matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Service to retrieve.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Service was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Service matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Service>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get([FromRoute]Guid id)
        {
            var service = ServiceRepository.Get(id);

            if (service == default(Service))
            {
                return NotFound();
            }

            return Ok(service);
        }

        /// <summary>
        ///     Creates a new service.
        /// </summary>
        /// <param name="service">The Service to create.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Service was created successfully.</response>
        /// <response code="400">The specified Service was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="409">A Service with the same name already exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(Service), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Create([FromBody]ServiceAddRequest service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingService = ServiceRepository.GetAll(new ServiceFilters()
            {
                Name = service.Name
            });

            if (existingService.Any())
            {
                return Conflict($"A Service with a matching name already exists.");
            }

            var serviceRecord = new Service()
            {
                Name = service.Name,
                Description = service.Description,
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                CreationById = User.GetId(),
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
            };

            try
            {
                var createdService = ServiceRepository.Create(serviceRecord);
                return StatusCode(201, createdService);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Service: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Updates the Service matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Service to update.</param>
        /// <param name="service">The updated Service.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Service was updated successfully.</response>
        /// <response code="400">The specified Service was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">A Service matching the specified id could not be found.</response>
        /// <response code="409">A Service with the same name already exists.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(Service), 200)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(string), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update([FromRoute]Guid id, [FromBody]ServiceUpdateRequest service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var serviceToUpdate = ServiceRepository.Get(id);

            if (serviceToUpdate == default(Service))
            {
                return NotFound();
            }

            var duplicateServices = ServiceRepository
                .GetAll(new ServiceFilters() { Name = service.Name })
                .Any(s => s.Id != id);

            if (duplicateServices)
            {
                return Conflict($"A Service with this name already exists.");
            }

            var serviceRecord = new Service()
            {
                Name = service.Name,
                Description = service.Description,
                Id = serviceToUpdate.Id,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
            };

            try
            {
                var updatedService = ServiceRepository.Update(serviceRecord);
                return Ok(updatedService);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Service: {ex.Message}. See inner exception for details.", ex);
            }
        }

        /// <summary>
        ///     Deletes the Service matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Service to delete.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Service was deleted successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">A Service matching the specified id could not be found.</response>
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
            var service = ServiceRepository.Get(id);

            if (service == default(Service))
            {
                return NotFound();
            }

            try
            {
                ServiceRepository.Delete(service);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the specified Service: {ex.Message}. See inner exception for details.", ex);
            }
        }
    }
}
