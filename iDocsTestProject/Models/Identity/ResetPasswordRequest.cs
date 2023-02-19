using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iDocsTestProject.Models.Identity
{
    public class ResetPasswordRequest
    {
        [Required]
        [Description("IIN")]
        [MinLength(12)]
        [MaxLength(12)]
        [RegularExpression("^[0-9]*$")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
