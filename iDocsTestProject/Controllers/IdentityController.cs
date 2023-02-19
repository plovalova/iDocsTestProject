using iDocsTestProject.Configurations;
using iDocsTestProject.Models;
using iDocsTestProject.Models.Identity;
using iDocsTestProject.Models.Requests.Identity;
using iDocsTestProject.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace iDocsTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IIdentityService identityService;
        private readonly JwtConfig jwtConfig;

        public IdentityController(UserManager<IdentityUser> userManager,
                                  IIdentityService identityService,
                                  IOptions<JwtConfig> jwtConfig)
        {
            this.userManager = userManager;
            this.identityService = identityService;
            this.jwtConfig = jwtConfig.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Registration))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> Registration([FromBody] RegistrationRequest request)
        {
            var userExist = await userManager.FindByNameAsync(request.Username);
            if (userExist is not null)
                return BadRequest(new GenericResponse()
                {
                    Result = false,
                    ErrorMessage = "User already exist!"
                });
            var newUser = new IdentityUser()
            {
                UserName = request.Username
            };

            var isSucceeded = await userManager.CreateAsync(newUser, request.Password);

            if (isSucceeded.Succeeded)
            {
                var token = identityService.GenerateJwtToken(newUser, jwtConfig.Secret);

                return Ok(new GenericResponse()
                {
                    Data = new 
                    {   
                        UserId = newUser.Id,
                        UserName = request.Username,
                        Token = token
                    },
                    Result = true
                    
                });
            }

            return BadRequest(new GenericResponse()
            {
                Result = false,
                Errors = isSucceeded.Errors.Select(e => e.Description).ToList()
            });

        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> Login([FromBody] LoginRequest request)
        {
            var existingUser = await userManager.FindByNameAsync(request.Username);

            if (existingUser is null)
                return NotFound(new GenericResponse()
                {
                    Result = false,
                    ErrorMessage = "User not found!"
                });

            var checkPassword = await userManager.CheckPasswordAsync(existingUser, request.Password);

            if (!checkPassword)
                return Unauthorized(new GenericResponse()
                {
                    Result = false,
                    ErrorMessage = "Invalid credentials!"
                });


            var token = identityService.GenerateJwtToken(existingUser, jwtConfig.Secret);

            return Ok(new GenericResponse()
            {
                Data = new
                {
                    UserId = existingUser.Id,
                    UserName = existingUser.UserName,
                    Token = token
                },
                Result = true

            });

        }

        [HttpPost]
        [Route(nameof(ForgotPassword))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<GenericResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var existingUser = await userManager.FindByNameAsync(request.Username);

            if (existingUser is null)
            {
                return NotFound(new GenericResponse()
                {
                    Result = false,
                    ErrorMessage = "User not found!"
                });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(existingUser);

            return Ok(new GenericResponse()
            {
                Data = new 
                {
                    UserName = existingUser.UserName,
                    Token = token
                },
                Result = true, 
            });

        }

        [HttpPost]
        [Route(nameof(ResetPassword))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<GenericResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var existingUser = await userManager.FindByNameAsync(request.UserName);

            if (existingUser is null)
            {
                return NotFound(new GenericResponse()
                {
                    Result = false,
                    ErrorMessage = "User not found!"
                });
            }

            var resetResult = await userManager.ResetPasswordAsync(existingUser, request.Token, request.Password);

            if (!resetResult.Succeeded)
            {
                return BadRequest(new GenericResponse()
                {
                    Result = false,
                    Errors = resetResult.Errors.Select(e => e.Description).ToList()
                });
            }


            return Ok(new GenericResponse()
            {
                Data = new 
                {
                    UserName = existingUser.UserName
                },
                Result = true,
            });

        }

    }
}