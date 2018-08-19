// <copyright file="Event.cs" company="JP Dillingham, Nick Acosta, et. al.">
//     Copyright (c) JP Dillingham, Nick Acosta, et. al.. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Event.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Security.Data.Model;

    public class Event
    {
        public DateTime EndTime { get; set; }

        public IList<Account> Hosts { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }
    }
}