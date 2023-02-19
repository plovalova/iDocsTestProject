using iDocsTestProject.Models;
using System.Security.Claims;

namespace iDocsTestProject.Helpers
{
    public static class UserHelper
    {
        public static UserModel GetCurrentUser(this ClaimsPrincipal user)
        {
            var currentUser = new UserModel
            {
                Id = user.Claims.FirstOrDefault(c => c.Type == "userId")?.Value,
                UserName = user.Claims.FirstOrDefault(c => c.Type == "userName")?.Value
            };

            return currentUser;
        }
    }
}
