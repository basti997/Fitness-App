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
            cmd.CommandText = "select * from WorkoutSets where set_id = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

            var data = GetData(dbConn, cmd);

            if (data != null && data.Read())
            {
                return new WorkoutSet(Convert.ToInt32(data["set_id"]))
                {
                    WorkoutId = Convert.ToInt32(data["workout_id"]),
                    ExerciseId = Convert.ToInt32(data["exercise_id"]),
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
                cmd.CommandText = "select * from WorkoutSets";

                var data = GetData(dbConn, cmd);

                if (data != null)
                {
                    while (data.Read())
                    {
                        WorkoutSet ws = new WorkoutSet(Convert.ToInt32(data["set_id"]))
                        {
                            WorkoutId = Convert.ToInt32(data["workout_id"]),
                            ExerciseId = Convert.ToInt32(data["exercise_id"]),
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
                dbConn.Open();

                var cmd = dbConn.CreateCommand();
                cmd.CommandText =
                    @"insert into WorkoutSets
                      (workout_id, exercise_id, set_number, weight, reps)
                      values
                      (@workout_id, @exercise_id, @set_number, @weight, @reps)";

                cmd.Parameters.AddWithValue("@workout_id", NpgsqlDbType.Integer, ws.WorkoutId);
                cmd.Parameters.AddWithValue("@exercise_id", NpgsqlDbType.Integer, ws.ExerciseId);
                cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
                cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Numeric, ws.Weight);
                cmd.Parameters.AddWithValue("@reps", NpgsqlDbType.Integer, ws.Reps);

                return cmd.ExecuteNonQuery() == 1;
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
                @"update WorkoutSets
                  set workout_id=@workout_id,
                      exercise_id=@exercise_id,
                      set_number=@set_number,
                      weight=@weight,
                      reps=@reps
                  where set_id = @id";

            cmd.Parameters.AddWithValue("@workout_id", NpgsqlDbType.Integer, ws.WorkoutId);
            cmd.Parameters.AddWithValue("@exercise_id", NpgsqlDbType.Integer, ws.ExerciseId);
            cmd.Parameters.AddWithValue("@set_number", NpgsqlDbType.Integer, ws.SetNumber);
            cmd.Parameters.AddWithValue("@weight", NpgsqlDbType.Double, ws.Weight);
            cmd.Parameters.AddWithValue("@reps", NpgsqlDbType.Integer, ws.Reps);
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
            cmd.CommandText = @"delete from WorkoutSets where set_id = @id";
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

            bool result = DeleteData(dbConn, cmd);
            return result;
        }
    }
