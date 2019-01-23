// <copyright file="ScansController.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Events.Data.Model;
    using QCVOC.Api.Scans.Data.DTO;
    using QCVOC.Api.Scans.Data.Model;
    using QCVOC.Api.Services.Data.Model;
    using QCVOC.Api.Veterans;
    using QCVOC.Api.Veterans.Data.Model;

    /// <summary>
    ///     Provides endpoints for manipulation of Scan records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ScansController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScansController"/> class.
        /// </summary>
        /// <param name="scanRepository">The repository used for Scan data access.</param>
        /// <param name="eventRepository">The repository used for Event data access.</param>
        /// <param name="veteranRepository">The repository used for Veteran data access.</param>
        /// <param name="serviceRepository">The repository used for Service data access.</param>
        public ScansController(ITripleKeyRepository<Scan> scanRepository, ISingleKeyRepository<Event> eventRepository, ISingleKeyRepository<Veteran> veteranRepository, ISingleKeyRepository<Service> serviceRepository)
        {
            ScanRepository = scanRepository;
            EventRepository = eventRepository;
            VeteranRepository = veteranRepository;
            ServiceRepository = serviceRepository;
        }

        private ITripleKeyRepository<Scan> ScanRepository { get; set; }
        private ISingleKeyRepository<Event> EventRepository { get; }
        private ISingleKeyRepository<Veteran> VeteranRepository { get; set; }
        private ISingleKeyRepository<Service> ServiceRepository { get; set; }

        /// <summary>
        ///     Returns a check in scan for the specified Veteran.
        /// </summary>
        /// <param name="eventId">The Id of the Event.</param>
        /// <param name="id">Either the Veteran Id or Card Number of the Veteran.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Scan was retrieved successfully.</response>
        /// <response code="400">The Veteran Id or Card Number is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">The specified Card Number or Veteran Id doesn't match an enrolled Veteran, or the Veteran has not checked in to the specified event.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("{eventId}/{id}/checkin")]
        [Authorize]
        [ProducesResponseType(typeof(Scan), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetCheckIn(Guid eventId, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest($"The card or veteran ID is null or empty.");
            }

            var veteran = default(Veteran);

            if (int.TryParse(id, out var cardNumber))
            {
                veteran = VeteranRepository
                    .GetAll(new VeteranFilters() { CardNumber = cardNumber })
                    .SingleOrDefault();
            }
            else if (Guid.TryParse(id, out var veteranId))
            {
                veteran = VeteranRepository.Get(veteranId);
            }
            else
            {
                return BadRequest($"The provided ID is neither a Card Number nor Veteran ID.");
            }

            if (veteran == default(Veteran))
            {
                return StatusCode(404, $"The specified Card Number or Veteran Id doesn't match an enrolled Veteran.");
            }

            var scan = ScanRepository.Get(eventId, veteran.Id, Guid.Empty);

            if (scan == default(Scan))
            {
                return StatusCode(404, $"The Veteran has not checked in for this Event.");
            }

            return Ok(scan);
        }

        /// <summary>
        ///     Returns a list of Scans.
        /// </summary>
        /// <param name="filters">Optional filtering and pagination options.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The list was retrieved successfully.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Scan>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetAll([FromQuery]ScanFilters filters)
        {
            return Ok(ScanRepository.GetAll(filters));
        }

        /// <summary>
        ///     Deletes a Check-In Scan.
        /// </summary>
        /// <param name="eventId">The Id of the Event.</param>
        /// <param name="id">Either the Veteran Id or Card Number of the Veteran.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Scan was deleted successfully.</response>
        /// <response code="400">The Veteran Id or Card Number is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">The specified Card Number or Veteran Id doesn't match an enrolled Veteran.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpDelete("{eventId}/{id}")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete(Guid eventId, string id)
        {
            return Delete(eventId, Guid.Empty, id);
        }

        /// <summary>
        ///     Deletes a Service Scan.
        /// </summary>
        /// <param name="eventId">The Id of the Event.</param>
        /// <param name="serviceId">The Id of the Service.</param>
        /// <param name="id">Either the Veteran Id or Card Number of the Veteran.</param>
        /// <returns>See attributes.</returns>
        /// <response code="204">The Scan was deleted successfully.</response>
        /// <response code="400">The Veteran Id or Card Number is invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">The specified Card Number or Veteran Id doesn't match an enrolled Veteran.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpDelete("{eventId}/{serviceId}/{id}")]
        [Authorize]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Delete(Guid eventId, Guid serviceId, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest($"The card or veteran ID is null or empty.");
            }

            var veteran = default(Veteran);

            if (int.TryParse(id, out var cardNumber))
            {
                veteran = VeteranRepository
                    .GetAll(new VeteranFilters() { CardNumber = cardNumber })
                    .SingleOrDefault();
            }
            else if (Guid.TryParse(id, out var veteranId))
            {
                veteran = VeteranRepository.Get(veteranId);
            }
            else
            {
                return BadRequest($"The provided ID is neither a Card Number nor Veteran ID.");
            }

            if (veteran == default(Veteran))
            {
                return StatusCode(404, $"The specified Card Number or Veteran Id doesn't match an enrolled Veteran.");
            }

            var scan = ScanRepository.Get(eventId, veteran.Id, serviceId);

            if (scan == default(Scan))
            {
                return StatusCode(404, $"A Scan matching the specified information could not be found.");
            }

            try
            {
                ScanRepository.Delete(eventId, veteran.Id, serviceId);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting the Scan matching the specified information: {ex.Message}.  See inner Exception for details.", ex);
            }
        }

        /// <summary>
        ///     Performs an Event Scan.
        /// </summary>
        /// <param name="scan">The scan context.</param>
        /// <returns>See attributes.</returns>
        /// <response code="201">The Scan was recorded or updated.</response>
        /// <response code="400">The specified Scan was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The Veteran has not checked in for the Event, or did not check in with a guest and is attempting to use a service with a guest.</response>
        /// <response code="404">The specified Veteran, Event or Service was invalid.</response>
        /// <response code="409">The Scan has already been recorded.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("")]
        [Authorize]
        [ProducesResponseType(typeof(Scan), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ScanError), 409)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Scan([FromBody]ScanRequest scan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetReadableString());
            }

            if (!int.TryParse(scan.CardNumber, out var cardNumber))
            {
                return BadRequest($"Card Number {scan.CardNumber} is not a valid integer.");
            }

            var @event = EventRepository.Get((Guid)scan.EventId);

            if (@event == default(Event))
            {
                StatusCode(404, "The specified Event could not be found.");
            }

            var service = ServiceRepository.Get(scan.ServiceId);

            if (service == default(Service))
            {
                return StatusCode(404, "The specified Service could not be found.");
            }

            var veteran = VeteranRepository
                .GetAll(new VeteranFilters() { CardNumber = cardNumber, IncludePhotoBase64 = true })
                .SingleOrDefault();

            if (veteran == default(Veteran))
            {
                return StatusCode(404, $"Card Number {scan.CardNumber} doesn't match an enrolled Veteran.");
            }

            var previousScans = ScanRepository.GetAll(new ScanFilters() { EventId = scan.EventId, VeteranId = veteran.Id });
            var existingCheckIn = previousScans.Where(s => s.ServiceId == Guid.Empty).SingleOrDefault();

            var scanRecord = new Scan()
            {
                EventId = (Guid)scan.EventId,
                VeteranId = veteran.Id,
                ServiceId = scan.ServiceId,
                ScanById = User.GetId(),
                ScanDate = DateTime.UtcNow,
                PlusOne = scan.PlusOne,
            };

            // check in scan
            if (scan.ServiceId == Guid.Empty)
            {
                if (existingCheckIn == default(Scan))
                {
                    return CreateScan(scanRecord, veteran);
                }
                else if (existingCheckIn.PlusOne != scanRecord.PlusOne)
                {
                    return UpdateScan(scanRecord, veteran);
                }
                else
                {
                    return Conflict(new ScanError(existingCheckIn, veteran, "Duplicate Scan"));
                }
            }

            // service scan
            if (existingCheckIn == default(Scan))
            {
                return StatusCode(403, new ScanError(scanRecord, veteran, "The Veteran has not checked in for this Event."));
            }

            if (scanRecord.PlusOne && !existingCheckIn.PlusOne)
            {
                return StatusCode(403, new ScanError(scanRecord, veteran, "The Veteran did not check in with a +1."));
            }

            var previousServiceScan = previousScans.Where(s => s.ServiceId == scan.ServiceId).SingleOrDefault();

            if (previousServiceScan != default(Scan))
            {
                return Conflict(new ScanError(previousServiceScan, veteran, "Duplicate Scan"));
            }

            return CreateScan(scanRecord, veteran);
        }

        private IActionResult CreateScan(Scan scan, Veteran veteran)
        {
            try
            {
                var createdScan = ScanRepository.Create(scan);
                return StatusCode(201, new ScanResponse(createdScan, veteran));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing Scan: {ex.Message}.  See inner Exception for details.", ex);
            }
        }

        private IActionResult UpdateScan(Scan scan, Veteran veteran)
        {
            try
            {
                var updatedScan = ScanRepository.Update(scan);
                return StatusCode(201, new ScanResponse(updatedScan, veteran));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing Scan: {ex.Message}.  See inner Exception for details.", ex);
            }
        }
    }
}