using iDocsTestProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace iDocsTestProject.Services.Identity
{
    public interface IIdentityService
    {
        public string GenerateJwtToken (IdentityUser user, string secret);
        
    }
}
