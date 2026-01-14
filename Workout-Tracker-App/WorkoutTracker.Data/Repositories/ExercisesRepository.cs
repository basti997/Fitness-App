namespace WorkoutTracker.Data.Repositories;

using System;
using System.Collections.Generic;
using WorkoutTracker.Data.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;

    public class ExerciseRepository : BaseRepository
    {
        public ExerciseRepository(IConfiguration configuration) : base(configuration)
        {
        }

        // Get a single exercise by ID
        public Exercises GetExerciseById(int id)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Exercises WHERE exercise_id = @id";
                cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

                var data = GetData(dbConn, cmd);
                if (data != null && data.Read())
                {
                    return new Exercises(Convert.ToInt32(data["exercise_id"]))
                    {
                        Name = data["name"].ToString(),
                        Description = data["description"].ToString(),
                        MuscleGroupId = Convert.ToInt32(data["muscle_group_id"])
                    };
                }

                return null;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // Get all exercises
        public List<Exercises> GetExercises()
        {
            NpgsqlConnection dbConn = null;
            var exercises = new List<Exercises>();
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Exercises";

                var data = GetData(dbConn, cmd);
                if (data != null)
                {
                    while (data.Read())
                    {
                        var e = new Exercises(Convert.ToInt32(data["exercise_id"]))
                        {
                            Name = data["name"].ToString(),
                            Description = data["description"].ToString(),
                            MuscleGroupId = Convert.ToInt32(data["muscle_group_id"])
                        };
                        exercises.Add(e);
                    }
                }
                return exercises;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // Insert a new exercise
        public bool InsertExercise(Exercises e)
        {
            NpgsqlConnection dbConn = null;
            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);
                var cmd = dbConn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Exercises (name, description, muscle_group_id)
                    VALUES (@name, @description, @muscle_group_id)";
                
                cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, e.Name);
                cmd.Parameters.AddWithValue("@description", NpgsqlDbType.Text, e.Description ?? "");
                cmd.Parameters.AddWithValue("@muscle_group_id", NpgsqlDbType.Integer, e.MuscleGroupId);

                return InsertData(dbConn, cmd);
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // Update an existing exercise
        public bool UpdateExercise(Exercises e)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Exercises SET
                    name = @name,
                    description = @description,
                    muscle_group_id = @muscle_group_id
                WHERE exercise_id = @id";

            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, e.Name);
            cmd.Parameters.AddWithValue("@description", NpgsqlDbType.Text, e.Description ?? "");
            cmd.Parameters.AddWithValue("@muscle_group_id", NpgsqlDbType.Integer, e.MuscleGroupId);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, e.ExerciseId);

            return UpdateData(dbConn, cmd);
        }

        // Delete an exercise by ID
        public bool DeleteExercise(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "DELETE FROM Exercises WHERE exercise_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            return DeleteData(dbConn, cmd);
        }
        
        // Get Exercise by Muscle Group
    public List<Exercises> GetExercisesByMuscleGroup(int muscleGroupId)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Exercises WHERE muscle_group_id = @muscleGroupId";
            cmd.Parameters.Add("@muscleGroupId", NpgsqlDbType.Integer).Value = muscleGroupId;

            var data = GetData(dbConn, cmd);
            var result = new List<Exercises>();

            if (data != null)
            {
                while (data.Read())
                {
                    var exercise = new Exercises(Convert.ToInt32(data["exercise_id"]))
                    {
                        Name = data["name"].ToString(),
                        Description = data["description"] == DBNull.Value
                            ? null
                            : data["description"].ToString(),
                        MuscleGroupId = Convert.ToInt32(data["muscle_group_id"])
                    };

                    result.Add(exercise);
                }
            }

            return result;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    }
