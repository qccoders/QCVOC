// <copyright file="ScanRepository.cs" company="QC Coders (JP Dillingham, Nick Acosta, et. al.)">
//     Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
//     in the project root for full license information.
// </copyright>

namespace QCVOC.Api.Scans.Data.Repository
{
    using System;
    using System.Collections.Generic;
    using QCVOC.Api.Common;
    using QCVOC.Api.Common.Data.Repository;
    using QCVOC.Api.Scans.Data.Model;

    /// <summary>
    ///     Provides data access for <see cref="Scan"/>.
    /// </summary>
    public class ScanRepository : IRepository<Scan>
    {
        public Scan Create(Scan resource)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Scan resource)
        {
            throw new NotImplementedException();
        }

        public Scan Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scan> GetAll(Filters filters = null)
        {
            throw new NotImplementedException();
        }

        public Scan Update(Scan resource)
        {
            throw new NotImplementedException();
        }
    }
}
