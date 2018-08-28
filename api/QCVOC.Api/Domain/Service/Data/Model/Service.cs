// <copyright file="Service.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Domain.Service.Data.Model
{
    using System;

    public class Service
    {
        public Guid Id { get; set; }
        public int Limit { get; set; }
        public string Name { get; set; }
    }
}