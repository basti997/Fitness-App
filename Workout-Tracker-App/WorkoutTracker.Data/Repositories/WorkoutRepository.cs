using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Data.Repositories
{
    public class WorkoutRepository : BaseRepository
    {
        public WorkoutRepository(IConfiguration configuration) : base(configuration) { }

        // Get all workouts for a user (latest first)
        public List<Workout> GetWorkoutsByUserId(int userId)
        {
            NpgsqlConnection dbConn = null;
            var list = new List<Workout>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT workout_id, user_id, workout_date, notes FROM Workouts WHERE user_id = @userId ORDER BY workout_date DESC";
                cmd.Parameters.Add("@userId", NpgsqlDbType.Integer).Value = userId;

                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        var w = new Workout(Convert.ToInt32(data["workout_id"]))
                        {
                            UserId = Convert.ToInt32(data["user_id"]),
                            WorkoutDate = Convert.ToDateTime(data["workout_date"]),
                            Notes = data["notes"]?.ToString() ?? string.Empty
                        };
                        list.Add(w);
                    }
                }
                return list;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // Get single workout by id
        public Workout GetWorkoutById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT workout_id, user_id, workout_date, notes FROM Workouts WHERE workout_id = @id LIMIT 1";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new Workout(Convert.ToInt32(data["workout_id"]))
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        WorkoutDate = Convert.ToDateTime(data["workout_date"]),
                        Notes = data["notes"]?.ToString() ?? string.Empty
                    };
                }
                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // Insert new workout, returns new id or 0
        public int InsertWorkout(Workout workout)
        {
            if (workout == null) return 0;

            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Workouts (user_id, workout_date, notes)
                    VALUES (@user_id, @workout_date, @notes)
                    RETURNING workout_id;
                ";
                cmd.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, workout.UserId);
                cmd.Parameters.AddWithValue("@workout_date", NpgsqlDbType.TimestampTz, workout.WorkoutDate);
                cmd.Parameters.AddWithValue("@notes", NpgsqlDbType.Text, (object?)workout.Notes ?? DBNull.Value);

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
                Console.WriteLine($"InsertWorkout failed: {ex.Message}");
                return 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }
        public bool UpdateWorkout(Workout workout)
{
    if (workout == null || workout.Id <= 0) return false;

    NpgsqlConnection dbConn = null;
    try
    {
        dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Workouts
            SET workout_date = @workout_date,
                notes = @notes
            WHERE workout_id = @id;
        ";
        cmd.Parameters.AddWithValue("@workout_date", NpgsqlDbType.TimestampTz, workout.WorkoutDate);
        cmd.Parameters.AddWithValue("@notes", NpgsqlDbType.Text, (object?)workout.Notes ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, workout.Id);

        dbConn.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"UpdateWorkout failed: {ex.Message}");
        return false;
    }
    finally
    {
        dbConn?.Close();
    }
}

public bool DeleteWorkout(int id)
{
    if (id <= 0) return false;

    NpgsqlConnection dbConn = null;
    try
    {
        dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();
        cmd.CommandText = @"
            DELETE FROM Workouts
            WHERE workout_id = @id;
        ";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        dbConn.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DeleteWorkout failed: {ex.Message}");
        return false;
    }
    finally
    {
        dbConn?.Close();
    }
}
    }
}