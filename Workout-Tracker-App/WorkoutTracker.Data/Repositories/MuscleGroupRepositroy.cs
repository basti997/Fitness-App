namespace WorkoutTracker.Data.Repositories;

using System; // Needed for CommandType
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; // Needed to read the appsettings.json
using Npgsql; // Needed for PostgreSQL
using NpgsqlTypes;
using WorkoutTracker.Data.Entities; // Needed to use MuscleGroup

public class MuscleGroupRepository : BaseRepository
{
    public MuscleGroupRepository(IConfiguration configuration) : base(configuration)
    { }

    // ---------------------------------------------------------------
    // GET MuscleGroup by ID
    // ---------------------------------------------------------------
    public MuscleGroup GetMuscleGroupById(int id)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "SELECT * FROM MuscleGroups WHERE muscle_group_id = @id";
            cmd.Parameters.Add("@id", NpgsqlDbType.Integer).Value = id;

            var data = GetData(dbConn, cmd);

            if (data != null && data.Read())
            {
                return new MuscleGroup(Convert.ToInt32(data["muscle_group_id"]))
                {
                    Name = data["name"].ToString()
                };
            }

            return null;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    // ---------------------------------------------------------------
    // GET all MuscleGroups
    // ---------------------------------------------------------------
    public List<MuscleGroup> GetMuscleGroups()
    {
        var groups = new List<MuscleGroup>();
        NpgsqlConnection dbConn = null;

        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = "SELECT * FROM MuscleGroups";

            var data = GetData(dbConn, cmd);

            while (data != null && data.Read())
            {
                groups.Add(new MuscleGroup(Convert.ToInt32(data["muscle_group_id"]))
                {
                    Name = data["name"].ToString()
                });
            }

            return groups;
        }
        finally
        {
            dbConn?.Close();
        }
    }

    // ---------------------------------------------------------------
    // INSERT MuscleGroup
    // ---------------------------------------------------------------
    public bool InsertMuscleGroup(MuscleGroup group)
    {
        NpgsqlConnection dbConn = null;
        try
        {
            dbConn = new NpgsqlConnection(ConnectionString);
            var cmd = dbConn.CreateCommand();

            cmd.CommandText = @"
                    INSERT INTO MuscleGroups (name)
                    VALUES (@name)
                ";

            cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, group.Name);

            return InsertData(dbConn, cmd);
        }
        finally
        {
            dbConn?.Close();
        }
    }

    // ---------------------------------------------------------------
    // UPDATE MuscleGroup
    // ---------------------------------------------------------------
    public bool UpdateMuscleGroup(MuscleGroup group)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = @"
                UPDATE MuscleGroups SET
                    name = @name
                WHERE muscle_group_id = @id
            ";

        cmd.Parameters.AddWithValue("@name", NpgsqlDbType.Text, group.Name);
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, group.MuscleGroupId);

        return UpdateData(dbConn, cmd);
    }

    // ---------------------------------------------------------------
    // DELETE MuscleGroup
    // ---------------------------------------------------------------
    public bool DeleteMuscleGroup(int id)
    {
        var dbConn = new NpgsqlConnection(ConnectionString);
        var cmd = dbConn.CreateCommand();

        cmd.CommandText = "DELETE FROM MuscleGroups WHERE muscle_group_id = @id";
        cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, id);

        return DeleteData(dbConn, cmd);
    }
}






//{
    // This is our "Repository" (our "modular level")
    // Its ONLY job is to talk to the database.
//     public class MuscleGroupRepository
//     {
//         private readonly string _connectionString;

//         // 1. The constructor gets the "Configuration" (your appsettings.json)
//         public MuscleGroupRepository(IConfiguration configuration)
//         {
//             // 2. It finds your password and saves it in a private variable.
//             _connectionString = configuration.GetConnectionString("DefaultConnection");
//         }

//         // 3. This is the simple function our Controller will call.
//         public async Task<IEnumerable<MuscleGroup>> GetAllAsync()
//         {
//             var muscleGroups = new List<MuscleGroup>();

//             // 4. We use "await using" to create a connection.
//             //    It automatically opens and closes the connection for us.
//             await using (var db = new NpgsqlConnection(_connectionString))
//             {
//                 // 5. This is our SQL query. It uses the correct column names.
//                 var sql = "SELECT muscle_group_id, name FROM MuscleGroups";
                
//                 await using (var cmd = new NpgsqlCommand(sql, db))
//                 {
//                     await db.OpenAsync(); // Open the connection
//                     var reader = await cmd.ExecuteReaderAsync();

//                     // 6. Loop through every row the database gives us
//                     while (await reader.ReadAsync())
//                     {
//                         // 7. Create a new MuscleGroup object and fill it
//                         var muscleGroup = new MuscleGroup
//                         {
//                             MuscleGroupId = (int)reader["muscle_group_id"],
//                             Name = (string)reader["name"]
//                         };
//                         muscleGroups.Add(muscleGroup);
//                     }
//                 }
//             }
//             // 8. Return the final list!
//             return muscleGroups;
//         }
//     }
// }