using System;
using System.Collections.Generic;
using WorkoutTracker.Data.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace WorkoutTracker.Data.Repositories
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        { }

        /// <summary>
        /// Get a single user by ID
        /// </summary>
        public User GetUserById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users WHERE user_id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new User((int)data["user_id"])
                    {
                        Username = data["username"].ToString(),
                        Email = data["email"].ToString(),
                        PasswordHash = data["password_hash"].ToString(),
                        CreatedAt = Convert.ToDateTime(data["created_at"])
                    };
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public List<User> GetUsers()
        {
            NpgsqlConnection dbConn = null;
            var users = new List<User>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users";

                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        var user = new User((int)data["user_id"])
                        {
                            Username = data["username"].ToString(),
                            Email = data["email"].ToString(),
                            PasswordHash = data["password_hash"].ToString(),
                            CreatedAt = Convert.ToDateTime(data["created_at"])
                        };
                        users.Add(user);
                    }
                }
                return users;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        /// <summary>
        /// Insert a new user and return created user_id (or 0 on failure)
        /// </summary>
        public int InsertUser(User user)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Users (username, email, password_hash)
                    VALUES (@username, @email, @password_hash)
                    RETURNING user_id;
                ";
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, user.Username);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, user.Email);
                cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, user.PasswordHash);

                dbConn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    return newId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InsertUser failed: {ex.Message}");
                return 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        public bool UpdateUser(User user)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users SET
                    username = @username,
                    email = @email,
                    password_hash = @password_hash
                WHERE user_id = @id
            ";
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, user.Username);
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, user.Email);
            cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, user.PasswordHash);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, user.Id);

            return UpdateData(dbConn, cmd);
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        public bool DeleteUser(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE user_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }
    }
}