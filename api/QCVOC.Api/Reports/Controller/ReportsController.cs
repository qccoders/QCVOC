// <copyright file="ReportsController.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Controller
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QCVOC.Api.Reports.Data;

    /// <summary>
    ///     Provides endpoints for manipulation of Scan records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ReportsController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportsController"/> class.
        /// </summary>
        /// <param name="reportRepository">The repository used for Report data access.</param>
        public ReportsController(IReportRepository reportRepository)
        {
            ReportRepository = reportRepository;
        }

        private IReportRepository ReportRepository { get; set; }

        [HttpGet("event/master")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<EventMaster>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult GetEventMaster([FromQuery]DateTime startTime, [FromQuery]DateTime endTime)
        {
            if (startTime >= endTime)
            {
                return BadRequest($"Start time must be before end time.");
            }

            return Ok(ReportRepository.GetEventMaster(startTime, endTime));
        }
    }
}