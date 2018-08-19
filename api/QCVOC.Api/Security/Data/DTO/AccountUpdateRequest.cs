namespace QCVOC.Api.Security.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Security;

    public class AccountUpdateRequest
    {
        [StringLength(maximumLength: 256, MinimumLength = 2)]
        public string Name { get; set; }

        public Role? Role { get; set; }

        [StringLength(maximumLength: 256, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
