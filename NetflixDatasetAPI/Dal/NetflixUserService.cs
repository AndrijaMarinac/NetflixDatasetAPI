using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using PostgreSQLCopyHelper;
using System.IO;

namespace NetflixDatasetAPI.Dal
{
    public class NetflixUserService
    {
        public static async Task<bool> ParseAndUploadCSVAsync(string path, IConfiguration configuration)
        {
            using FileStream sourceFileStream = System.IO.File.Open(path, FileMode.Open);
            using StreamReader reader = new StreamReader(sourceFileStream);

            ConnectionUtils utils = new ConnectionUtils();
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));

            List<NetflixUser> netflixUsers = new List<NetflixUser>();
            List<SubscriptionDetail> subscriptionDetails = new List<SubscriptionDetail>();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                subscriptionDetails.Add(new SubscriptionDetail()
                {
                    NetflixUserId = int.Parse(values[0]),
                    SubscriptionType = values[1],
                    MontlyRevenue = int.Parse(values[2]),
                    JoinDate = DateTime.Parse(values[3]),
                    LastPayment = DateTime.Parse(values[4]),
                    PlanDurationInDays = ParseCSVUtils.PlanDurationToDays(values[9]),
                });
                netflixUsers.Add(new NetflixUser()
                {
                    NetflixUserId = int.Parse(values[0]),
                    Country = values[5],
                    Age = int.Parse(values[6]),
                    Gender = values[7],
                    Device = values[8]
                });
            }

            var netflixUserCopyHelper = new PostgreSQLCopyHelper<NetflixUser>("public", "netflixusers")
                .MapInteger("netflixuserid", x => x.NetflixUserId)
                .MapText("country", x => x.Country)
                .MapInteger("age", x => x.Age)
                .MapText("gender",  x => x.Gender)
                .MapText("Device", x => x.Device);

            var rowsAffected = await netflixUserCopyHelper.SaveAllAsync(connection, netflixUsers);
            if (rowsAffected < 0)
            {
                return false;
            }

            var subscriptionDetailCopyHelper = new PostgreSQLCopyHelper<SubscriptionDetail>("public", "subscriptiondetails")
               .MapInteger("netflixuserid", x => x.NetflixUserId)
               .MapText("subscriptiontype", x => x.SubscriptionType)
               .MapInteger("monthyrevenue", x => x.MontlyRevenue)
               .MapDate("joindate", x => x.JoinDate)
               .MapDate("lastpaymentdate", x => x.LastPayment)
               .MapInteger("plandurationindays", x => x.PlanDurationInDays);

            rowsAffected = await subscriptionDetailCopyHelper.SaveAllAsync(connection, subscriptionDetails);
            if (rowsAffected < 0)
            {
                return false;
            }

            return true;
        }
    }
}
