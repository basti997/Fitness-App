using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using WorkoutTracker.Data.Entities;

namespace WorkoutTracker.Data.Repositories
{
    public class WorkoutRepository : BaseRepository
    {
        public WorkoutRepository(IConfiguration configuration) : base(configuration)
        { }

        // --------------------------------------------------------------------
        // GET WORKOUT BY ID
        // --------------------------------------------------------------------
        public Workout GetWorkoutById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Workouts WHERE workout_id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);

                if (data != null && data.Read())
                {
                    return new Workout(Convert.ToInt32(data["workout_id"]))
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        WorkoutDate = Convert.ToDateTime(data["workout_date"]),
                        Notes = data["notes"]?.ToString()
                    };
                }

                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // --------------------------------------------------------------------
        // GET ALL WORKOUTS
        // --------------------------------------------------------------------
        public List<Workout> GetWorkouts()
        {
            var workouts = new List<Workout>();
            NpgsqlConnection dbConn = null;

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();

                cmd.CommandText = "SELECT * FROM Workouts";

                var data = GetData(dbConn, cmd);

                while (data != null && data.Read())
                {
                    workouts.Add(new Workout(Convert.ToInt32(data["workout_id"]))
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        WorkoutDate = Convert.ToDateTime(data["workout_date"]),
                        Notes = data["notes"]?.ToString()
                    });
                }

                return workouts;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // --------------------------------------------------------------------
        // UPDATE WORKOUT
        // --------------------------------------------------------------------
        public bool UpdateWorkout(Workout w)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = @"
                UPDATE Workouts SET
                    user_id = @user_id,
                    workout_date = @workout_date,
                    notes = @notes
                WHERE workout_id = @id
            ";

            cmd.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, w.UserId);
            cmd.Parameters.AddWithValue("@workout_date", NpgsqlDbType.TimestampTz, w.WorkoutDate);
            cmd.Parameters.AddWithValue("@notes", NpgsqlDbType.Text, (object?)w.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, w.Id);

            return UpdateData(dbConn, cmd);
        }

        // --------------------------------------------------------------------
        // DELETE WORKOUT
        // --------------------------------------------------------------------
        public bool DeleteWorkout(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "DELETE FROM Workouts WHERE workout_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }
        
        // --------------------------------------------------------------------
        // CREATE WORKOUT — returns the new workout_id (or 0 on failure)
        // --------------------------------------------------------------------
        public int CreateWorkout(Workout workout)
        {
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
                Console.WriteLine($"CreateWorkout failed: {ex.Message}");
                return 0;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // --------------------------------------------------------------------
        // GET WORKOUTS BY USER
        // --------------------------------------------------------------------
        public List<Workout> GetWorkoutsByUser(int userId)
        {
            var workouts = new List<Workout>();
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Workouts WHERE user_id = @user_id";
                cmd.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = userId;

                var data = GetData(dbConn, cmd);
                while (data != null && data.Read())
                {
                    workouts.Add(new Workout(Convert.ToInt32(data["workout_id"]))
                    {
                        UserId = Convert.ToInt32(data["user_id"]),
                        WorkoutDate = Convert.ToDateTime(data["workout_date"]),
                        Notes = data["notes"]?.ToString()
                    });
                }
                return workouts;
            }
            finally
            {
                dbConn?.Close();
            }
        }
    }
}