using Microsoft.Data.SqlClient;
using User.grpc.Models;
using User.grpc.Services.Interfaces;

namespace User.grpc.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;

        public UserService(IConfiguration configuration, ILogger<UserService> logger)
        {

            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public UserModel? GetUserModelByEmailAsync(string email)
        {
            UserModel user = new UserModel();
            try
            {
                SqlConnection connection = new SqlConnection(_config.GetConnectionString("AuthConnectionString"));

                _logger.LogInformation("\nQuery data example:");
                _logger.LogInformation("=========================================\n");

                connection.Open();

                String sql = $"SELECT * FROM Users WHERE Email = {email} LIMIT 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.ID = reader.GetString(0);
                            user.Provider_id = reader.GetString(3) ?? "";
                            user.Provider = reader.GetString(4) ?? "";
                            user.UserName = reader.GetString(5);
                            user.Email = reader.GetString(7);
                        }
                    }
                }
                return user;
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
                return user;
            }
        }

        public UserModel? GetUserModelByIdAsync(string userId)
        {
            //var response = await _client.GetAsync(endPointUrl + "/" + userId);
            UserModel user = new UserModel();
            try
            {
                SqlConnection connection = new SqlConnection(_config.GetConnectionString("AuthConnectionString"));
                
                    _logger.LogInformation("\nQuery data example:");
                    _logger.LogInformation("=========================================\n");

                    connection.Open();

                    String sql = "SELECT * FROM Users";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetString(0);
                                user.Provider_id = reader.GetString(3) ?? "";
                                user.Provider = reader.GetString(4) ?? "";
                                user.UserName = reader.GetString(5);
                                user.Email = reader.GetString(7);
                            }
                        }
                    }
                return user;
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
                return user;
            }
        }
    }
}
