using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetflixDatasetAPI.Dal;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;

namespace NetflixDatasetAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NetflixUsersController : Controller
    {
        private readonly IConfiguration _configuration;

        public NetflixUsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(Name = "GetNetflixUsers")]
        public async Task<IActionResult> GetNetflixUsersAsync([FromQuery]NetflixUserData netflixUserDataFilter)
        {
            try
            {
                List<NetflixUserData> users = await NetflixUsersService.GetAllNetflixUserDataAsync(_configuration);

                var filteredList = users.Where(user =>
                    (netflixUserDataFilter.Country == null || user.Country == netflixUserDataFilter.Country) &&
                    (netflixUserDataFilter.Age == null || user.Age == netflixUserDataFilter.Age) &&
                    (netflixUserDataFilter.Gender == null || user.Gender == netflixUserDataFilter.Gender) &&
                    (netflixUserDataFilter.Device == null || user.Device == netflixUserDataFilter.Device) &&
                    (netflixUserDataFilter.SubscriptionType == null || user.SubscriptionType == netflixUserDataFilter.SubscriptionType) &&
                    (netflixUserDataFilter.MontlyRevenue == null || user.MontlyRevenue == netflixUserDataFilter.MontlyRevenue) &&
                    (netflixUserDataFilter.JoinDate == null || user.JoinDate == netflixUserDataFilter.JoinDate) &&
                    (netflixUserDataFilter.LastPayment == null || user.LastPayment == netflixUserDataFilter.LastPayment) &&
                    (netflixUserDataFilter.PlanDurationInDays == null || user.PlanDurationInDays == netflixUserDataFilter.PlanDurationInDays)
                    ).ToList();

                return Ok(filteredList);
            }
            catch (Exception exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
