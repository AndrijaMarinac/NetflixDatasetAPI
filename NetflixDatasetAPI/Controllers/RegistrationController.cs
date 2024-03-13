using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NetflixDatasetAPI.Dal;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.Results;


namespace NetflixDatasetAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost(Name = "RegisterUser")]
        public async Task<IActionResult> RegisterUserAsync(string username, string password, string email)
        {
            User user = new User
            {
                Username = username,
                Password = password,
                Email = email,
            };

            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, context, results, true))
            {
                string messages = "";
                results.ForEach(message => messages += message + "\n");
                return BadRequest(messages);
            }

            try
            {
                bool result = await RegistrationServices.RegisterNewUserAsync(user.Username,user.Password,user.Email,_configuration);
                if (!result)
                {
                    throw new Exception("Registration failed");
                }
                return Ok($"New user successfully created");
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
