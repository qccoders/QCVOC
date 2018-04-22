using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QCVOC.Server.Data.Model
{
    public class Event
    {
        #region Public Properties

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public IList<User> Hosts { get; set; }

        #endregion Public Properties
    }
}