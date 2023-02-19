using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iDocsTestProject.Models.Requests.Identity
{
    public class RegistrationRequest
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
