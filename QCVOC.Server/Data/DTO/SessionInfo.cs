using System.ComponentModel.DataAnnotations;

namespace QCVOC.Data.DTO
{
    public class SessionInfo
    {
        #region Public Properties

        [StringLength(60, MinimumLength = 2, ErrorMessage = "Account names must be between 2 and 60 characters in length.")]
        public string Name { get; set; }

        [StringLength(256, MinimumLength = 5, ErrorMessage = "Account passwords must be between 5 and 256 characters in length.")]
        public string Password { get; set; }

        #endregion Public Properties
    }
}