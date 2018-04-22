using QCVOC.Server.Data.Model;
using System;

namespace QCVOC.Server.Data.DTO
{
    public class Jwt
    {
        #region Public Properties

        public DateTime Expires { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }

        #endregion Public Properties
    }
}