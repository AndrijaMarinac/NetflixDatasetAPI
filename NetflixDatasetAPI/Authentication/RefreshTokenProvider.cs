using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Dal;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;

namespace NetflixDatasetAPI.Authentication
{
    public static class RefreshTokenProvider
    {
        public async static Task<RefreshToken> GenerateRefreshTokenAsync(User user, IConfiguration configuration)
        {
            return await LoginService.GenerateRefreshTokenAsync(int.Parse(configuration["RefreshTokenSettings:LifetimeInHours"]!),user, configuration);
        }
    }
}
