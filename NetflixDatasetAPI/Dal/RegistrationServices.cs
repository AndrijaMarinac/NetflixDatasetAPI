using Microsoft.Extensions.Configuration;
using NetflixUserbaseDatasetAPI.Models;
using Npgsql;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;

namespace NetflixDatasetAPI.Dal
{
    public class RegistrationServices
    {
        public static async Task<bool> RegisterNewUserAsync(string username,string password, string email, IConfiguration configuration)
        {
            using var connection = new NpgsqlConnection(ConnectionUtils.GetConnectionString(configuration));
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT fn_insertuser(@input_username,@input_password,@input_email)", connection);

            command.Parameters.AddWithValue("input_username", NpgsqlDbType.Text, username);
            command.Parameters.AddWithValue("input_password", NpgsqlDbType.Text, password);
            command.Parameters.AddWithValue("input_email", NpgsqlDbType.Text, email);

            await using var reader = await command.ExecuteReaderAsync();

            await reader.ReadAsync();
            bool result = (bool)reader["fn_insertuser"];
            return result;
        }
    }
}
