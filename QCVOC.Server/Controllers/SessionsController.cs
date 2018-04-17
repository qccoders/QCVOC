using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QCVOC.Data.DTO;
using QCVOC.Server.Data.Model;
using QCVOC.Server.Data.Repository;
using QCVOC.Server.Security;

namespace QCVOC.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        private IUserRepository UserRepository { get; set; }
        private IJwtFactory JwtFactory { get; set; }

        public SessionsController(IUserRepository userRepository, IJwtFactory jwtFactory)
        {
            UserRepository = userRepository;
            JwtFactory = jwtFactory;
        }

        [HttpPost]
        public IActionResult Post([FromBody]SessionInfo sessionInfo)
        {
            if (sessionInfo == default(SessionInfo) || string.IsNullOrEmpty(sessionInfo.Name) || string.IsNullOrEmpty(sessionInfo.Password))
            {
                return BadRequest("Invalid session info; please supply a Name and a Password.");
            }

            User user = UserRepository.Get(sessionInfo.Name);

            if (user == default(User))
            {
                return Unauthorized();
            }

            string passwordHash = Utility.ComputeSHA512Hash(sessionInfo.Password);

            if (passwordHash != user.PasswordHash)
            {
                return Unauthorized();
            }

            return Ok(JwtFactory.GetJwt(user));
        }
    }
}
