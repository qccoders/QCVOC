// <copyright file="ReportsController.cs" company="QC Coders">
//     Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Reports.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QCVOC.Api.Reports.Data;

    /// <summary>
    ///     Provides endpoints for manipulation of Scan records.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json", "text/plain", "application/octet")]
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

        /// <summary>
        ///     Gets the event master report dataset.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>See attributes.</returns>
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

        /// <summary>
        ///     Gets the veteran report dataset.
        /// </summary>
        /// <param name="format">The output format ('csv' or 'json' (default))</param>
        /// <returns>See attributes.</returns>
        [HttpGet("veteran")]
        [Authorize]
        public IActionResult GetVeterans([FromQuery]string format = "csv")
        {
            var vets = ReportRepository.GetVeterans();

            if (format == "csv")
            {

                var csv = new StringBuilder();

                csv.AppendLine("Id,CardNumber,FirstName,LastName,Address,PrimaryPhone,Email,VerificationMethod,EnrollmentDate,EnrollmentBy,LastUpdateDate,LastUpdateBy");

                foreach (var vet in vets)
                {
                    csv.AppendJoin(
                        ',',
                        vet.Id,
                        vet.CardNumber,
                        Quote(vet.FirstName),
                        Quote(vet.LastName),
                        Quote(vet.Address),
                        vet.PrimaryPhone,
                        Quote(vet.Email),
                        Quote(vet.VerificationMethod),
                        vet.EnrollmentDate,
                        Quote(vet.EnrollmentBy),
                        vet.LastUpdateDate,
                        Quote(vet.LastUpdateBy));

                    csv.AppendLine();
                }

                return File(Encoding.UTF8.GetBytes(csv.ToString()), "application/octet-stream", $"veterans-{DateTime.Now.ToString("MM-dd-yyyy")}.csv");
            }

            return Ok(vets);
        }

        private string Quote(object val)
        {
            return $"\"{val}\"";
        }
    }
}