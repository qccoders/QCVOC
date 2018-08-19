using QCVOC.Api.Common;

namespace QCVOC.Api.Security
{
    public class AccountQueryParameters : QueryParameters
    {
        public Role? Role { get; set; }
    }
}
