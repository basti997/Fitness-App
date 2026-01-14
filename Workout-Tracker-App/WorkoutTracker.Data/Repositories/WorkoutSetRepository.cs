namespace WorkoutTracker.Data.Repositories;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using WorkoutTracker.Data.Entities;

public class WorkoutSetRepository : BaseRepository
{
    public WorkoutSetRepository(IConfiguration configuration) : base(configuration)
    {
    }

    //-----------------------
    //Better customer journey
    //--------------------
    public List<WorkoutSet> GetSetsByWorkout(int workoutId)
    {
        var sets = new List<WorkoutSet>();
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT * FROM WorkoutSets WHERE workout_id = @workout_id ORDER BY set_number";
            cmd.Parameters.Add("@workout_id", NpgsqlDbType.Integer).Value = workoutId;

            var data = GetData(dbConn, cmd);
            while (data != null && data.Read())
            {
                sets.Add(new WorkoutSet(Convert.ToInt32(data["set_id"]))
                {
                WorkoutId = Convert.ToInt32(data["workout_id"]),
                ExerciseId = Convert.ToInt32(data["exercise_id"]),
                SetNumber = Convert.ToInt32(data["set_number"]),
                Weight = Convert.ToDouble(data["weight"]),
                Reps = Convert.ToInt32(data["reps"])
            });
        }
        return sets;
    }
    finally
    {
        dbConn?.Close();
    }
}

    public List<WorkoutSet> GetSetsByExerciseAndUser(int userId, int exerciseId)
    {
        var sets = new List<WorkoutSet>();
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"
                SELECT ws.set_id, ws.workout_id, ws.exercise_id, ws.set_number, ws.weight, ws.reps, w.workout_date
                FROM WorkoutSets ws
                INNER JOIN Workouts w ON ws.workout_id = w.workout_id
                WHERE w.user_id = @user_id AND ws.exercise_id = @exercise_id
                ORDER BY w.workout_date, ws.set_number";
            cmd.Parameters.Add("@user_id", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@exercise_id", NpgsqlDbType.Integer).Value = exerciseId;

            var data = GetData(dbConn, cmd);
            while (data != null && data.Read())
            {
                var ws = new WorkoutSet(Convert.ToInt32(data["set_id"]))
                {
                    WorkoutId = Convert.ToInt32(data["workout_id"]),
                    ExerciseId = Convert.ToInt32(data["exercise_id"]),
                    SetNumber = Convert.ToInt32(data["set_number"]),
                    Weight = Convert.ToDouble(data["weight"]),
                    Reps = Convert.ToInt32(data["reps"])
                };
            // Optional: if your entity has WorkoutDate, set it from data["workout_date"].
            sets.Add(ws);
            }
            return sets;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    public List<WorkoutSet> GetProgressForExercise(int userId, int exerciseId)
    {
        // Same as GetSetsByExerciseAndUser; keep it if you want a dedicated name for graphing
        return GetSetsByExerciseAndUser(userId, exerciseId);
    }

    //-----------------------
    //regular repository methods
    //--------------------

    //--------------------------------------------------------------------------
    // Insert new set
    //     public bool InsertWorkoutSet(WorkoutSet ws)
    //     {
    //         using var dbConn = new NpgsqlConnection(ConnectionString);
    //         dbConn.Open();

    //         using var cmd = dbConn.CreateCommand();
    //         cmd.CommandText = @"
    //         INSERT INTO WorkoutSets (workout_id, exercise_id, set_number, weight, reps)
    //         VALUES (@workout_id, @exercise_id, @set_number, @weight, @reps)";
    //         cmd.Parameters.AddWithValue("@workout_id", NpgsqlDbType.Integer, ws.WorkoutId);
    //         cmd.Parameters.AddWithValue("@exercise_id", NpgsqlDbType.Integer, ws.ExerciseId);
    //         cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
    //         cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Numeric, ws.Weight);
    //         cmd.Parameters.AddWithValue("@reps", NpgsqlDbType.Integer, ws.Reps);

    //         return cmd.ExecuteNonQuery() == 1;
    //     }

    //     // Get all sets for a workout
    //     public List<WorkoutSet> GetSetsForWorkout(int workoutId)
    //     {
    //         var sets = new List<WorkoutSet>();
    //         using var dbConn = new NpgsqlConnection(ConnectionString);
    //         dbConn.Open();

    //         using var cmd = dbConn.CreateCommand();
    //         cmd.CommandText = @"
    //         SELECT ws.*, w.workout_date FROM WorkoutSets ws
    //         JOIN Workouts w ON ws.workout_id = w.workout_id
    //         WHERE ws.workout_id = @workout_id ORDER BY ws.set_number";
    //         cmd.Parameters.AddWithValue("@workout_id", NpgsqlDbType.Integer, workoutId);

    //         using var reader = cmd.ExecuteReader();
    //         while (reader.Read())
    //         {
    //             sets.Add(new WorkoutSet((int)reader["id"])
    //             {
    //                 WorkoutId = (int)reader["workout_id"],
    //                 ExerciseId = (int)reader["exercise_id"],
    //                 SetNumber = (int)reader["set_number"],
    //                 Weight = Convert.ToDouble(reader["weight"]),
    //                 Reps = (int)reader["reps"],
    //                 WorkoutDate = (DateTime)reader["workout_date"]
    //             });
    //         }
    //         return sets;
    //     }

    //     // Get progress for exercise per user (all sets ordered)
    //     public List<WorkoutSet> GetExerciseProgress(int userId, int exerciseId)
    //     {
    //         var progress = new List<WorkoutSet>();
    //         using var dbConn = new NpgsqlConnection(ConnectionString);
    //         dbConn.Open();

    //         using var cmd = dbConn.CreateCommand();
    //         cmd.CommandText = @"
    //         SELECT ws.*, w.workout_date FROM WorkoutSets ws
    //         JOIN Workouts w ON ws.workout_id = w.workout_id
    //         WHERE w.user_id = @user_id AND ws.exercise_id = @exercise_id
    //         ORDER BY w.workout_date, ws.set_number";
    //         cmd.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, userId);
    //         cmd.Parameters.AddWithValue("@exercise_id", NpgsqlDbType.Integer, exerciseId);

    //         using var reader = cmd.ExecuteReader();
    //         while (reader.Read())
    //         {
    //             progress.Add(new WorkoutSet((int)reader["id"])
    //             {
    //                 WorkoutId = (int)reader["workout_id"],
    //                 ExerciseId = (int)reader["exercise_id"],
    //                 SetNumber = (int)reader["set_number"],
    //                 Weight = Convert.ToDouble(reader["weight"]),
    //                 Reps = (int)reader["reps"],
    //                 WorkoutDate = (DateTime)reader["workout_date"]
    //             });
    //         }
    //         return progress;
    //     }

    // }

    //--------------------------------------------------------------------------------
    //  // ---------------------------------------------------------
    // // ADD A WORKOUT SET (alias to InsertWorkoutSet)
    // // ---------------------------------------------------------
    // public bool InsertWorkoutSet(WorkoutSet ws)
    // {
    //     NpgsqlConnection dbConn = null;
    //     try
    //     {
    //         dbConn = new NpgsqlConnection(ConnectionString);
    //         var cmd = dbConn.CreateCommand();
    //         cmd.CommandText =
    //             @"INSERT INTO WorkoutSets (workout_id, exercise_id, set_number, weight, reps)
    //               VALUES (@workout_id, @exercise_id, @set_number, @weight, @reps)";

    //         cmd.Parameters.AddWithValue("@workout_id", NpgsqlDbType.Integer, ws.WorkoutId);
    //         cmd.Parameters.AddWithValue("@exercise_id", NpgsqlDbType.Integer, ws.ExerciseId);
    //         cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
    //         cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Numeric, ws.Weight);
    //         cmd.Parameters.AddWithValue("@reps", NpgsqlDbType.Integer, ws.Reps);

    //         return InsertData(dbConn, cmd); // Ensure InsertData is defined and accessible
    //     }
    //     finally
    //     {
    //         dbConn?.Close();
    //     }
    // }


    // // ---------------------------------------------------------
    // // GET ALL WORKOUT SETS FOR A GIVEN WORKOUT
    // // ---------------------------------------------------------
    // public List<WorkoutSet> GetSetsForWorkout(int workoutId)
    // {
    //     var sets = new List<WorkoutSet>();
    //     NpgsqlConnection dbConn = null;

    //     try
    //     {
    //         dbConn = new NpgsqlConnection(ConnectionString);
    //         var cmd = dbConn.CreateCommand();
    //         cmd.CommandText = "SELECT * FROM workoutset WHERE workoutid = @workoutid ORDER BY id";
    //         cmd.Parameters.Add("@workoutid", NpgsqlDbType.Integer).Value = workoutId;

    //         var data = GetData(dbConn, cmd);

    //         while (data != null && data.Read())
    //         {
    //             sets.Add(new WorkoutSet(Convert.ToInt32(data["id"]))
    //             {
    //                 WorkoutId = Convert.ToInt32(data["workoutid"]),
    //                 ExerciseId = Convert.ToInt32(data["exerciseid"]),
    //                 SetNumber = Convert.ToInt32(data["set_number"]),
    //                 Weight = Convert.ToDouble(data["weight"]),
    //                 Reps = Convert.ToInt32(data["repetitions"])
    //             });
    //         }

    //         return sets;
    //     }
    //     finally
    //     {
    //         dbConn?.Close();
    //     }
    // }

    // // ---------------------------------------------------------
    // // GET EXERCISE PROGRESS FOR USER AND EXERCISE
    // // Retrieves all sets for the exercise for the user, ordered by workout date for progress tracking
    // // ---------------------------------------------------------
    // public List<WorkoutSet> GetExerciseProgress(int userId, int exerciseId)
    // {
    //     var progressSets = new List<WorkoutSet>();
    //     NpgsqlConnection dbConn = null;

    //     try
    //     {
    //         dbConn = new NpgsqlConnection(ConnectionString);
    //         var cmd = dbConn.CreateCommand();
    //         cmd.CommandText = @"
    //             SELECT ws.id, ws.workoutid, ws.exerciseid, ws.set_number, ws.weight, ws.repetitions, w.workout_date
    //             FROM workoutset ws
    //             INNER JOIN workouts w ON ws.workoutid = w.workout_id
    //             WHERE w.user_id = @userid AND ws.exerciseid = @exerciseid
    //             ORDER BY w.workout_date, ws.id
    //         ";
    //         cmd.Parameters.Add("@userid", NpgsqlDbType.Integer).Value = userId;
    //         cmd.Parameters.Add("@exerciseid", NpgsqlDbType.Integer).Value = exerciseId;

    //         var data = GetData(dbConn, cmd);

    //         while (data != null && data.Read())
    //         {
    //             var set = new WorkoutSet(Convert.ToInt32(data["id"]))
    //             {
    //                 WorkoutId = Convert.ToInt32(data["workoutid"]),
    //                 ExerciseId = Convert.ToInt32(data["exerciseid"]),
    //                 SetNumber = Convert.ToInt32(data["set_number"]),
    //                 Weight = Convert.ToDouble(data["weight"]),
    //                 Reps = Convert.ToInt32(data["repetitions"]),
    //                 // Assuming this WorkoutSet entity includes this property for progress tracking
    //                 //WorkoutDate = Convert.ToDateTime(data["workout_date"])
    //             };
    //             progressSets.Add(set);
    //         }
    //         return progressSets;
    //     }
    //     finally
    //     {
    //         dbConn?.Close();
    //     }
    // }

    // }

    //---------------------------------------------------------------------------------------------------------

    // ---------------------------------------------------------
    // Get WorkoutSet BY ID
    // ---------------------------------------------------------
    public WorkoutSet GetWorkoutSetById(int id)
    {
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);

            var cmd = dbConn.CreateCommand();
            cmd.CommandText = "select * from workoutset where id = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

            var data = GetData(dbConn, cmd);

            if (data != null && data.Read())
            {
                return new WorkoutSet(Convert.ToInt32(data["id"]))
                {
                    WorkoutId = Convert.ToInt32(data["workoutid"]),
                    ExerciseId = Convert.ToInt32(data["exerciseid"]),
                    SetNumber = Convert.ToInt32(data["set_number"]),
                    Weight = Convert.ToDouble(data["weight"]),
                    Reps = Convert.ToInt32(data["reps"])
                };
            }

            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }

        // ---------------------------------------------------------
        // Get ALL WorkoutSets
        // ---------------------------------------------------------
        public List<WorkoutSet> GetWorkoutSets()
        {
            NpgsqlConnection dbConn = null;
            var sets = new List<WorkoutSet>();

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);

                var cmd = dbConn.CreateCommand();
                cmd.CommandText = "select * from workoutset";

                var data = GetData(dbConn, cmd);

                if (data != null)
                {
                    while (data.Read())
                    {
                        WorkoutSet ws = new WorkoutSet(Convert.ToInt32(data["id"]))
                        {
                            WorkoutId = Convert.ToInt32(data["workoutid"]),
                            ExerciseId = Convert.ToInt32(data["exerciseid"]),
                            SetNumber = Convert.ToInt32(data["set_number"]),
                            Weight = Convert.ToDouble(data["weight"]),
                            Reps = Convert.ToInt32(data["reps"])
                        };

                        sets.Add(ws);
                    }
                }

                return sets;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // ---------------------------------------------------------
        // INSERT WorkoutSet
        // ---------------------------------------------------------
        public bool InsertWorkoutSet(WorkoutSet ws)
        {
            NpgsqlConnection dbConn = null;

            try
            {
                dbConn = new NpgsqlConnection(ConnectionString);

                var cmd = dbConn.CreateCommand();
                cmd.CommandText =
                    @"insert into workoutset
                      (workoutid, exerciseid, set_number, weight, repetitions)
                      values
                      (@workoutid, @exerciseid, @set_number, @weight, @repetitions)";

                cmd.Parameters.AddWithValue("@workoutid", NpgsqlDbType.Integer, ws.WorkoutId);
                cmd.Parameters.AddWithValue("@exerciseid", NpgsqlDbType.Integer, ws.ExerciseId);
                cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
                cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Double, ws.Weight);
                cmd.Parameters.AddWithValue("@repetitions", NpgsqlDbType.Integer, ws.Reps);

                bool result = InsertData(dbConn, cmd);
                return result;
            }
            finally
            {
                dbConn?.Close();
            }
        }

        // ---------------------------------------------------------
        // UPDATE WorkoutSet
        // ---------------------------------------------------------
        public bool UpdateWorkoutSet(WorkoutSet ws)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);

            var cmd = dbConn.CreateCommand();
            cmd.CommandText =
                @"update workoutset
                  set workoutid=@workoutid,
                      exerciseid=@exerciseid,
                      set_number=@set_number,
                      weight=@weight,
                      repetitions=@repetitions
                  where id = @id";

            cmd.Parameters.AddWithValue("@workoutid", NpgsqlDbType.Integer, ws.WorkoutId);
            cmd.Parameters.AddWithValue("@exerciseid", NpgsqlDbType.Integer, ws.ExerciseId);
            cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
            cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Double, ws.Weight);
            cmd.Parameters.AddWithValue("@repetitions", NpgsqlDbType.Integer, ws.Reps);
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, ws.Id);

            bool result = UpdateData(dbConn, cmd);
            return result;
        }

        // ---------------------------------------------------------
        // DELETE WorkoutSet
        // ---------------------------------------------------------
        public bool DeleteWorkoutSet(int id)
        {
            var dbConn = new NpgsqlConnection(ConnectionString);

            var cmd = dbConn.CreateCommand();
            cmd.CommandText = @"delete from workoutset where id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            bool result = DeleteData(dbConn, cmd);
            return result;
        }
    }
