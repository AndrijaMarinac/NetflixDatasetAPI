using Microsoft.Extensions.Configuration;
using NetflixDatasetAPI.Models;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;
using System.Web.Http.Results;
using System.Web.WebPages;

namespace NetflixDatasetAPI.Dal
{
    public class LoginService
    {
        public static async Task<User> GetUserAsync(string username,string password, IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM fn_getuser(@input_username,@input_password)", connection);

            command.Parameters.AddWithValue("input_username", NpgsqlDbType.Text, username);
            command.Parameters.AddWithValue("input_password", NpgsqlDbType.Text, password);

            await using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            if (!reader.HasRows)
            {
                return new User();
            }

            return new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = "",
                Email = reader.GetString(2),
                RegistrationDate = reader.GetDateTime(3),
                LastLoginDate = reader.GetDateTime(4),
            };
        }




        public static async Task<User> GetUserByRefreshToken(string refreshToken, IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM fn_getuserbyrefreshtoken(@input_refreshtoken)", connection);

            command.Parameters.AddWithValue("input_refreshtoken", NpgsqlDbType.Text,refreshToken);

            await using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            if (!reader.HasRows)
            {
                return new User();
            }

            return new User
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = "",
                Email = reader.GetString(2),
                RegistrationDate = reader.GetDateTime(3),
                LastLoginDate = reader.GetDateTime(4),
                RefreshToken = new RefreshToken
                {
                    Token = reader.GetString(5),
                    LifetimeInHours = reader.GetInt32(6),
                    CreationDate = reader.GetDateTime(7),
                    ExpirationDate = reader.GetDateTime(7).AddHours(reader.GetInt32(6)),
                }
            };
        }





        public static async Task<RefreshToken> GenerateRefreshTokenAsync(int LifetimeInHours,User user, IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT * FROM fn_GenerateRefreshToken(@input_Lifetime_in_hours,@input_fkUserID)", connection);

            command.Parameters.AddWithValue("input_Lifetime_in_hours", NpgsqlDbType.Integer, LifetimeInHours);
            command.Parameters.AddWithValue("input_fkUserID", NpgsqlDbType.Integer, user.UserId);

            await using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();

            return new RefreshToken
            {
                Token = reader.GetString(0),
                LifetimeInHours = reader.GetInt32(1),
                CreationDate = reader.GetDateTime(2),
                ExpirationDate = reader.GetDateTime(2).AddHours(reader.GetInt32(1)),
            };
        }
    }
}
