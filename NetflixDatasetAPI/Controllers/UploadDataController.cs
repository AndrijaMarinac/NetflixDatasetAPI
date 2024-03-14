using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Dal;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using System;
using System.IO;

namespace NetflixDatasetAPI.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class UploadDataController : Controller
    {
        private readonly IConfiguration _configuration;

        public UploadDataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost(Name = "UploadNetflixUserFromCSV")]
        public async Task<IActionResult> UploadNetflixUsersToDatabaseFromCSV(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return BadRequest("File does not exist");
            }

            try
            {
                bool result = await UploadDataService.ParseAndUploadCSVAsync(path, _configuration);
                if (!result)
                {
                    throw new Exception("Unable to upload CSV file to database");
                }
                return Ok("CSV file successfully uploaded");
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }

        }
    }
}
