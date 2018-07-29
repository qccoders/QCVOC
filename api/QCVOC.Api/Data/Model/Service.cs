using System;

namespace QCVOC.Api.Data.Model
{
    public class Service
    {
        #region Public Properties

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Limit { get; set; }

        #endregion Public Properties
    }
}