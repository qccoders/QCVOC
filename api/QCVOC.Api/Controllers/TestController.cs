// <copyright file="TestController.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Controllers
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    /// <summary>
    ///     Provides a test endpoint.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TestController : Controller
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        public TestController()
        {
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(Exception), 500)]
        public IActionResult Test(string test)
        {
            return Ok(test);
        }
    }
}