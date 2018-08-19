namespace QCVOC.Api.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using QCVOC.Api.Data.Model.Security;

    public class AccountCreateRequest
    {
        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public Role Role { get; set; }

        [Required]
        [StringLength(maximumLength: 256, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
