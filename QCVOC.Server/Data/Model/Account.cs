using System;

namespace QCVOC.Server.Data.Model
{
    public class Account
    {
        #region Public Properties

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

        #endregion Public Properties
    }
}