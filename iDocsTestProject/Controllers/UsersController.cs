using iDocsTestProject.Helpers;
using iDocsTestProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace iDocsTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public UsersController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [HttpGet]
        [Route(nameof(GetRecievers))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<UserModel>>> GetRecievers()
        {
            var currentUser = User.GetCurrentUser();

            var recievers = new List<UserModel>();

            var users = await appDbContext.Users.Where(u=>u.Id != currentUser.Id).AsNoTracking().ToListAsync();

            users.ForEach(u =>
            {
                recievers.Add(new UserModel
                {
                    Id = u.Id,
                    UserName = u.UserName
                });
            });

            return Ok(recievers);
        }

    }
}
