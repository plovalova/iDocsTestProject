using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iDocsTestProject.Models.Identity
{
    public class LoginRequest
    {
        [Required]
        [Description("IIN")]
        [MinLength(12)]
        [MaxLength(12)]
        [RegularExpression("^[0-9]*$")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
