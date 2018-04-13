using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QCVOC.DTO;

namespace QCVOC.Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("test");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]SessionInfo sessionInfo)
        {
            return Ok(sessionInfo);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
