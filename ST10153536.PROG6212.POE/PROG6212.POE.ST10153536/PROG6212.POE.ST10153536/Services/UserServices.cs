using Microsoft.Extensions.Options;
using PROG6212.POE.ST10153536.Interfaces;
using PROG6212.POE.ST10153536.Models;
using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Identity;

namespace PROG6212.POE.ST10153536.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionString = connectionStrings.Value.DefaultConnection;
        }

        public RegistrationResult RegisterUser(Users user)
        {
            try
            {
                var passwordHasher = new PasswordHasher<Users>();

                user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);

                Console.WriteLine($"Attempting to register user: {user.Username}");
                Console.WriteLine($"Hashed Password: {user.PasswordHash}");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database.");

                    connection.Execute("INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)", user);
                    Console.WriteLine("User successfully inserted into the database.");
                }

                return new RegistrationResult { Success = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return new RegistrationResult { Success = false, ErrorMessage = "Registration failed." };
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var user = connection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<Users>();
                    Console.WriteLine($"Entered Password: {password}");
                    Console.WriteLine($"Hashed Password from Database: {user.PasswordHash}");

                    var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    Console.WriteLine($"Verification result: {passwordVerificationResult}");

                    return passwordVerificationResult == PasswordVerificationResult.Success;
                }

                return false;
            }
        }

        object IUserService.GetUserByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Users>("SELECT * FROM Users WHERE Username = @Username", new { Username = username });
            }
        }
    }
}
