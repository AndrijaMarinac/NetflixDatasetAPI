using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.IO;

namespace NetflixDatasetAPI.Dal
{
    public class UploadDataService
    {
        public static async Task<bool> ParseAndUploadCSVAsync(string path, IConfiguration configuration)
        {
            using FileStream sourceFileStream = System.IO.File.Open(path, FileMode.Open);
            using StreamReader reader = new StreamReader(sourceFileStream);

            ConnectionUtils utils = new ConnectionUtils();
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            List<NetflixUser> netflixUsers = new List<NetflixUser>();
            List<SubscriptionDetail> subscriptionDetails = new List<SubscriptionDetail>();

            //Parsing header
            string line = reader.ReadLine();
            
            //Parsing values to lists
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                string[] values = line.Split(',');

                subscriptionDetails.Add(new SubscriptionDetail()
                {
                    NetflixUserId = int.Parse(values[0]),
                    SubscriptionType = values[1],
                    MontlyRevenue = int.Parse(values[2]),
                    JoinDate = DateTime.ParseExact(values[3],"dd-MM-yy",null),
                    LastPayment = DateTime.ParseExact(values[4],"dd-MM-yy", null),
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

            //Inserts each value from list to DB
            for (int i = 0; i < netflixUsers.Count; i++)
            {
                //Get Max Id in netflixusers table
                var countNetflixUsersCommnad = new NpgsqlCommand("SELECT MAX(netflixuserid) FROM netflixusers", connection);
                var NpgSQLResultReader = await countNetflixUsersCommnad.ExecuteReaderAsync();

                //Set max id++ as the new id for NetflixUser being inserted
                await NpgSQLResultReader.ReadAsync();

                int NetflixUserIdInDB = 1;
                if (!await NpgSQLResultReader.IsDBNullAsync(0))
                {
                    int MaxId = NpgSQLResultReader.GetInt32(0);
                    NetflixUserIdInDB = ++MaxId;
                }
                NpgSQLResultReader.Close();

                //Insert NetflixUser
                using var insertNetflixUserCommand = new NpgsqlCommand("SELECT fn_InsertNetflixUser(@input_netflixuserid,@input_country,@input_age,@input_gender,@input_device)", connection);
                insertNetflixUserCommand.Parameters.AddWithValue("input_netflixuserid", NpgsqlDbType.Integer, NetflixUserIdInDB);
                insertNetflixUserCommand.Parameters.AddWithValue("input_country", NpgsqlDbType.Text, netflixUsers[i].Country);
                insertNetflixUserCommand.Parameters.AddWithValue("input_age", NpgsqlDbType.Integer, netflixUsers[i].Age);
                insertNetflixUserCommand.Parameters.AddWithValue("input_gender", NpgsqlDbType.Text, netflixUsers[i].Gender);
                insertNetflixUserCommand.Parameters.AddWithValue("input_device", NpgsqlDbType.Text, netflixUsers[i].Device);
                NpgSQLResultReader = await insertNetflixUserCommand.ExecuteReaderAsync();

                await NpgSQLResultReader.ReadAsync();
                bool insertNetflixUserResult = NpgSQLResultReader.GetBoolean(0);
                if (!insertNetflixUserResult)
                {
                    throw new Exception($"CSV upload stopped: Unable to insert netflix user: {netflixUsers[i].NetflixUserId}");
                }
                NpgSQLResultReader.Close();


                //Insert Subscribtion Detail
                using var InsertSubscriptionDetailCommand = new NpgsqlCommand("SELECT fn_insertsubscrptiondetail(@input_fk_netflixuserid,@input_subscriptiontype,@input_monthlyrevenue,@input_joindate,@input_lastPaymentdate,@input_plandurationindays)", connection);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_fk_netflixuserid", NpgsqlDbType.Integer, NetflixUserIdInDB);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_subscriptiontype", NpgsqlDbType.Text, subscriptionDetails[i].SubscriptionType);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_monthlyrevenue", NpgsqlDbType.Integer, subscriptionDetails[i].MontlyRevenue);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_joindate", NpgsqlDbType.Date, subscriptionDetails[i].JoinDate);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_lastPaymentdate", NpgsqlDbType.Date, subscriptionDetails[i].LastPayment);
                InsertSubscriptionDetailCommand.Parameters.AddWithValue("input_plandurationindays", NpgsqlDbType.Integer, subscriptionDetails[i].PlanDurationInDays);
                NpgSQLResultReader = await InsertSubscriptionDetailCommand.ExecuteReaderAsync();

                await NpgSQLResultReader.ReadAsync();
                bool InsertSubscriptionDetailResult = NpgSQLResultReader.GetBoolean(0);
                if (!InsertSubscriptionDetailResult)
                {
                    throw new Exception($"CSV upload stopped: Unable to insert subscription details for: {netflixUsers[i].NetflixUserId}");
                }
                NpgSQLResultReader.Close();


                //Dispose commnads
                countNetflixUsersCommnad.Dispose();
                NpgSQLResultReader.Dispose();
            }
            return true;
        }
    }
}
