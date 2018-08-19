namespace QCVOC.Api.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Data.Model.Security;

    public class AccountUpdateRequest
    {

        public string Name { get; set; }

        public Role? Role { get; set; }

        public string Password { get; set; }
    }
}
