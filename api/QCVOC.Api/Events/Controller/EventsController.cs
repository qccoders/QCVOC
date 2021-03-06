﻿// <copyright file="EventsController.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Events.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Events.Data.Model;
    using QCVOC.Api.Security;

    /// <summary>
    ///     Provides endpoints for manipulation of Event records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class EventsController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventsController"/> class.
        /// </summary>
        /// <param name="eventRepository">The repository used for Event data access.</param>
        public EventsController(ISingleKeyRepository<Event> eventRepository)
        {
            EventRepository = eventRepository;
        }

        private ISingleKeyRepository<Event> EventRepository { get; }

        /// <summary>
        ///     Returns a list of Events.
        /// </summary>
        /// <param name="filters">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Event>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]EventFilters filters)
        {
            return Ok(EventRepository.GetAll(filters));
        }

        /// <summary>
        ///     Returns the Event matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Event to retrieve.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Event was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">An Event matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Event>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Get([FromRoute]Guid id)
        {
            var veteran = EventRepository.Get(id);

            if (veteran == default(Event))
            {
                return NotFound();
            }

            return Ok(veteran);
        }

        /// <summary>
        ///     Creates a new Event.
        /// </summary>
        /// <param name="event">The Event to create.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Event was created successfully.</response>
        /// <response code="400">The specified Event was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="409">The specified Event conflicts with an existing event.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPost("")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(Event), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Create([FromBody]EventRequest @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetReadableString());
            }

            var existingEvent = EventRepository
                .GetAll(new EventFilters() { Name = @event.Name })
                .Any(e => e.StartDate == @event.StartDate && e.EndDate == e.EndDate);

            if (existingEvent)
            {
                return Conflict("The specified Event already exists.");
            }

            var eventRecord = new Event()
            {
                Id = Guid.NewGuid(),
                Name = @event.Name,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                CreationDate = DateTime.UtcNow,
                CreationById = User.GetId(),
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
            };

            try
            {
                var createdEvent = EventRepository.Create(eventRecord);
                return StatusCode(201, createdEvent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating the specified Event: {ex.Message}.  See inner Exception for details.", ex);
            }
        }

        /// <summary>
        ///     Updates the Event matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Event to update.</param>
        /// <param name="event">The updated Event.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Event was updated successfully.</response>
        /// <response code="400">The specified Event was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">An Event matching the specified id could not be found.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Supervisor))]
        [ProducesResponseType(typeof(Event), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Update([FromRoute]Guid id, [FromBody]EventRequest @event)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetReadableString());
            }

            var eventToUpdate = EventRepository.Get(id);

            if (eventToUpdate == default(Event))
            {
                return NotFound();
            }

            var eventRecord = new Event()
            {
                Id = eventToUpdate.Id,
                Name = @event.Name,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                LastUpdateDate = DateTime.UtcNow,
                LastUpdateById = User.GetId(),
            };

            try
            {
                var updatedEvent = EventRepository.Update(eventRecord);
                return Ok(updatedEvent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating the specified Event: {ex.Message}.  See inner Exception for details.", ex);
            }
        }

        /// <summary>
        ///     Deletes the Event matching the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the Event to delete.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Event was deleted successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The user has insufficient rights to perform this operation.</response>
        /// <response code="404">An Event matching the specified id could not be found.</response>
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
            var @event = EventRepository.Get(id);

            if (@event == default(Event))
            {
                return NotFound();
            }

            try
            {
                EventRepository.Delete(@event);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the specified Event: {ex.Message}.  See inner Exception for details.", ex);
            }
        }
    }
}