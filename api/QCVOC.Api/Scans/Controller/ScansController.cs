// <copyright file="ScansController.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        ///     Performs an Event Scan.
        /// </summary>
        /// <param name="scan">The scan context.</param>
        /// <returns>See attributes.</returns>
        /// <response code="200">The Scan has already been recorded.</response>
        /// <response code="201">The Scan was recorded or updated.</response>
        /// <response code="400">The specified Scan was invalid.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">The Veteran has not checked in for the Event.</response>
        /// <response code="404">The specified Veteran, Event or Service was invalid.</response>
        /// <response code="500">The server encountered an error while processing the request.</response>
        [HttpPut("")]
        [Authorize]
        [ProducesResponseType(typeof(Scan), 200)]
        [ProducesResponseType(typeof(Scan), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Scan([FromBody]ScanRequest scan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @event = EventRepository.Get((Guid)scan.EventId);

            if (@event == default(Event))
            {
                StatusCode(404, "The specified Event could not be found.");
            }

            var veteran = VeteranRepository
                .GetAll(new VeteranFilters() { CardNumber = scan.CardNumber })
                .SingleOrDefault();

            if (veteran == default(Veteran))
            {
                return StatusCode(404, "The specified Card Id doesn't match an enrolled Veteran.");
            }

            var scanRecord = new Scan()
            {
                EventId = (Guid)scan.EventId,
                VeteranId = veteran.Id,
                ServiceId = scan.ServiceId,
                PlusOne = scan.PlusOne,
                ScanById = User.GetId(),
                ScanDate = DateTime.UtcNow,
            };

            var previousScans = ScanRepository.GetAll(new ScanFilters() { EventId = scan.EventId, VeteranId = veteran.Id });

            if (scan.ServiceId == Guid.Empty)
            {
                var existingCheckIn = previousScans.Where(s => s.ServiceId == Guid.Empty).SingleOrDefault();

                if (existingCheckIn == default(Scan))
                {
                    return CreateScan(scanRecord);
                }
                else if (existingCheckIn.PlusOne != scan.PlusOne)
                {
                    return UpdateScan(scanRecord);
                }
                else
                {
                    return StatusCode(200, existingCheckIn);
                }
            }

            var service = ServiceRepository.Get(scan.ServiceId);

            if (service == default(Service))
            {
                return StatusCode(404, "The specified Service could not be found.");
            }

            if (!previousScans.Where(s => s.ServiceId == Guid.Empty).Any())
            {
                return StatusCode(403, "The Veteran has not checked in for this Event.");
            }

            if (previousScans.Where(s => s.ServiceId == scan.ServiceId).Any())
            {
                return StatusCode(200, previousScans.Where(s => s.ServiceId == scan.ServiceId).SingleOrDefault());
            }

            return CreateScan(scanRecord);
        }

        private IActionResult CreateScan(Scan scan)
        {
            try
            {
                var createdScan = ScanRepository.Create(scan);
                return StatusCode(201, createdScan);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing Scan: {ex.Message}.  See inner Exception for details.", ex);
            }
        }

        private IActionResult UpdateScan(Scan scan)
        {
            try
            {
                var updatedScan = ScanRepository.Update(scan);
                return StatusCode(201, updatedScan);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error processing Scan: {ex.Message}.  See inner Exception for details.", ex);
            }
        }
    }
}