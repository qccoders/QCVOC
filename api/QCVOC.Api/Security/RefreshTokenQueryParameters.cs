namespace QCVOC.Api.Security
{
    using System;
    using QCVOC.Api.Common;

    public class RefreshTokenQueryParameters : QueryParameters
    {
        public Guid? AccountId { get; set; }
    }
}
