using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Data.Repositories
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        { }

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
                        Username = data["username"]?.ToString() ?? string.Empty,
                        Email = data["email"]?.ToString() ?? string.Empty,
                        PasswordHash = data["password_hash"]?.ToString() ?? string.Empty,
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

        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users WHERE email = @email LIMIT 1";
                cmd.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = email;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new User(Convert.ToInt32(data["user_id"]))
                    {
                        Username = data["username"]?.ToString() ?? string.Empty,
                        Email = data["email"]?.ToString() ?? string.Empty,
                        PasswordHash = data["password_hash"]?.ToString() ?? string.Empty,
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
                        var user = new User(Convert.ToInt32(data["user_id"]))
                        {
                            Username = data["username"]?.ToString() ?? string.Empty,
                            Email = data["email"]?.ToString() ?? string.Empty,
                            PasswordHash = data["password_hash"]?.ToString() ?? string.Empty,
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

        public int InsertUser(User user)
        {
            if (user == null) return 0;

            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Users (username, email, password_hash, created_at)
                    VALUES (@username, @email, @password_hash, @created_at)
                    RETURNING user_id;
                ";
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, (object?)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, (object?)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, (object?)user.PasswordHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@created_at", NpgsqlDbType.TimestampTz, user.CreatedAt);

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

        public bool UpdateUser(User user)
        {
            if (user == null) return false;

            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Users SET
                    username = @username,
                    email = @email,
                    password_hash = @password_hash
                WHERE user_id = @id
            ";
            cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, (object?)user.Username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, (object?)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, (object?)user.PasswordHash ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, user.Id);

            return UpdateData(dbConn, cmd);
        }

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