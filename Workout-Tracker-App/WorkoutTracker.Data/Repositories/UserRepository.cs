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

        /// <summary>
        /// Get a single user by ID
        /// </summary>
        public User GetUserById(int id)
        {
            if (id <= 0) return null;
            var sql = "SELECT user_id, username, email, password_hash, created_at FROM Users WHERE user_id = @id LIMIT 1";
            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User(Convert.ToInt32(reader["user_id"]))
                        {
                            Username = reader["username"]?.ToString() ?? string.Empty,
                            Email = reader["email"]?.ToString() ?? string.Empty,
                            PasswordHash = reader["password_hash"]?.ToString() ?? string.Empty,
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get a single user by email (or null if not found)
        /// </summary>
        public User GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var sql = "SELECT user_id, username, email, password_hash, created_at FROM Users WHERE email = @email LIMIT 1";
            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.Add("@email", NpgsqlDbType.Varchar).Value = email;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User(Convert.ToInt32(reader["user_id"]))
                        {
                            Username = reader["username"]?.ToString() ?? string.Empty,
                            Email = reader["email"]?.ToString() ?? string.Empty,
                            PasswordHash = reader["password_hash"]?.ToString() ?? string.Empty,
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public List<User> GetUsers()
        {
            var users = new List<User>();
            var sql = "SELECT user_id, username, email, password_hash, created_at FROM Users ORDER BY user_id";
            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User(Convert.ToInt32(reader["user_id"]))
                        {
                            Username = reader["username"]?.ToString() ?? string.Empty,
                            Email = reader["email"]?.ToString() ?? string.Empty,
                            PasswordHash = reader["password_hash"]?.ToString() ?? string.Empty,
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }
            return users;
        }

        /// <summary>
        /// Insert a new user and return created user_id (or 0 on failure)
        /// </summary>
        public int InsertUser(User user)
        {
            if (user == null) return 0;

            var sql = @"
                INSERT INTO Users (username, email, password_hash)
                VALUES (@username, @email, @password_hash)
                RETURNING user_id;
            ";

            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, (object?)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, (object?)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, (object?)user.PasswordHash ?? DBNull.Value);

                try
                {
                    conn.Open();
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
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        public bool UpdateUser(User user)
        {
            if (user == null || user.Id <= 0) return false;

            var sql = @"
                UPDATE Users SET
                    username = @username,
                    email = @email,
                    password_hash = @password_hash
                WHERE user_id = @id
            ";
            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@username", NpgsqlDbType.Varchar, (object?)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, (object?)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@password_hash", NpgsqlDbType.Varchar, (object?)user.PasswordHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, user.Id);

                conn.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }

        /// <summary>
        /// Delete a user by ID
        /// </summary>
        public bool DeleteUser(int id)
        {
            if (id <= 0) return false;

            var sql = "DELETE FROM Users WHERE user_id = @id";
            using (var conn = new NpgsqlConnection(ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

                conn.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
    }
}