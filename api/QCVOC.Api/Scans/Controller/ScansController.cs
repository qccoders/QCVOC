// <copyright file="ScansController.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Controller
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Scans.Data.DTO;
    using QCVOC.Api.Scans.Data.Model;

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
        public ScansController(ITripleKeyRepository<Scan> scanRepository)
        {
            ScanRepository = scanRepository;
        }

        private ITripleKeyRepository<Scan> ScanRepository { get; set; }

        public IActionResult GetAll([FromQuery]ScanFilters filters)
        {
            return Ok(ScanRepository.GetAll(filters));
        }

        [HttpPost("checkin")]
        [Authorize]
        [ProducesResponseType(typeof(Scan), 200)]
        [ProducesResponseType(typeof(Scan), 201)]
        [ProducesResponseType(typeof(ModelStateDictionary), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult CheckIn([FromBody]CheckInScanRequest scan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var checkInScan = ScanRepository.GetAll(new ScanFilters()
            {
                EventId = scan.EventId,
                VeteranId = scan.VeteranId,
                ServiceId = null,
            }).SingleOrDefault();

            if (checkInScan == default(Scan))
            {
                var scanRecord = new Scan()
                {
                    EventId = (Guid)scan.EventId,
                    VeteranId = (Guid)scan.VeteranId,
                    ServiceId = null,
                    PlusOne = scan.PlusOne,
                    ScanById = User.GetId(),
                    ScanDate = DateTime.UtcNow,
                };

                var createdScan = ScanRepository.Create(scanRecord);
                return StatusCode(201, createdScan);
            }

            return Ok(checkInScan);
        }

        public IActionResult Create([FromBody]ServiceScanRequest scan)
        {


            var previousScan = ScanRepository.GetAll(new ScanFilters()
            {
                EventId = scan.EventId,
                VeteranId = scan.VeteranId,
                ServiceId = scan.ServiceId,
            });

            var scanRecord = new Scan()
            {
                EventId = (Guid)scan.EventId,
                VeteranId = (Guid)scan.VeteranId,
                ServiceId = (Guid)scan.ServiceId,
                PlusOne = (bool)scan.PlusOne,
                ScanById = User.GetId(),
                ScanDate = DateTime.UtcNow,
            };
        }
    }
}