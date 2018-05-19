using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QCVOC.Server.Data.Model.Security;

namespace QCVOC.Server.Controllers
{
    [Authorize(Roles = nameof(Role.Administrator))]
    [ApiVersion("2")]
    [Route("api/v2/[controller]")]
    [Produces("application/json")]
    public class ValuesController : Controller
    {
        #region Public Methods

        // GET: api/Values
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new string[] { "value1", "value2" });
        }

        #endregion Public Methods
    }
}