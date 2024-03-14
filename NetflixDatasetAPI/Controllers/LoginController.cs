using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using NetflixDatasetAPI.Authentication;
using NetflixDatasetAPI.Dal;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace NetflixDatasetAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [Authorize]
        [HttpGet("JwtTest")]
        public async Task<IActionResult> JwtTest()
        {
            return Ok("Authorizaed");
        }




        [HttpGet(Name = "LoginUser")]
        public async Task<IActionResult> LoginUserAsync(string username, string password)
        {
            try
            {
                User user = await LoginService.GetUserAsync(username, password, _configuration);
                if (user.Username != username)
                {
                    return Unauthorized("Login Failed: username or password is wrong");
                }
                var jwtToken = await JwtProvider.GenerateJwtAsync(user, _configuration);

                RefreshToken refreshToken = await RefreshTokenProvider.GenerateRefreshTokenAsync(user, _configuration);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = refreshToken.ExpirationDate,
                };
                Response.Cookies.Append("RefreshToken",refreshToken.Token, cookieOptions);

                return Ok(jwtToken);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,exception.Message);
            }
        }




        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshJwtTokenAsync()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (refreshToken == null)
            {
                return Unauthorized("No refresh token");
            }
            var user = await LoginService.GetUserByRefreshToken(refreshToken, _configuration);
            if (user == null) 
            {
                return Unauthorized("Invalid refresh token");
            }
            if (user.RefreshToken.ExpirationDate < DateTime.Now)
            {
                return Unauthorized("Refresh token expired");
            }
            
            var newJwtToken = await JwtProvider.GenerateJwtAsync(user,_configuration);
            return Ok(newJwtToken);
        }


    }
}
