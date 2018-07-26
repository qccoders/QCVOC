using QCVOC.Server.Data.Model.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QCVOC.Server.Data.Model
{
    public class Event
    {
        #region Public Properties

        public DateTime EndTime { get; set; }

        public IList<Account> Hosts { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}