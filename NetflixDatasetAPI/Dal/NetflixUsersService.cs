using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using NpgsqlTypes;
using System.Net.Http.Formatting;

namespace NetflixDatasetAPI.Dal
{
    public class NetflixUsersService
    {
        public static async Task<List<NetflixUserData>> GetAllNetflixUserDataAsync(IConfiguration configuration)
        {
            List<NetflixUserData> netflixUsers = new List<NetflixUserData>();

            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM fn_GetAllNetflixUses()", connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                netflixUsers.Add(new NetflixUserData()
                {
                    NetflixUserId = reader.GetInt32(0),
                    Country = reader.GetString(1),
                    Age = reader.GetInt32(2),
                    Gender = reader.GetString(3),
                    Device = reader.GetString(4),
                    SubscriptionType = reader.GetString(5),
                    MontlyRevenue = reader.GetInt32(6),
                    JoinDate = reader.GetDateTime(7),
                    LastPayment = reader.GetDateTime(8),
                    PlanDurationInDays = reader.GetInt32(9)
                });
            }

            return netflixUsers;
        }

    }
}
