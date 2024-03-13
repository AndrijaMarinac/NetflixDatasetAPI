using Microsoft.Extensions.Configuration;

namespace NetflixDatasetAPI.Dal
{
    public class ConnectionUtils
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            if (connectionString == null)
                throw new Exception("No valid Connection String");

            return connectionString;
        }
    }
}
