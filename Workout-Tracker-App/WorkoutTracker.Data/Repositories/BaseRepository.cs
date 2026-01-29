namespace WorkoutTracker.Data.Repositories;

using Npgsql;
using Microsoft.Extensions.Configuration;

public class BaseRepository
{
    protected string ConnectionString { get; }

    public BaseRepository(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("AppProgDb");
    }

    /// <summary>
    /// Executes a SELECT query and returns an open NpgsqlDataReader.
    /// NOTE: The caller is responsible for closing the connection/reader.
    /// </summary>
    protected NpgsqlDataReader GetData(NpgsqlConnection conn, NpgsqlCommand cmd)
    {
        conn.Open();
        return cmd.ExecuteReader();
    }

    /// <summary>
    /// Executes an INSERT command.
    /// Returns TRUE if execution succeeded.
    /// </summary>
    protected bool InsertData(NpgsqlConnection conn, NpgsqlCommand cmd)
{
    try {
        conn.Open();
        int rowsAffected = cmd.ExecuteNonQuery();  // ✅ Capture result!
        return rowsAffected > 0;                  // ✅ TRUE only if rows inserted
    }
    catch (Exception ex) {
        Console.WriteLine($"Insert failed: {ex.Message}");  // ✅ Log errors!
        return false;
    }
    finally {
        conn?.Close();
    }
}

    /// <summary>
    /// Executes an UPDATE command.
    /// Returns TRUE if execution succeeded.
    /// </summary>
    protected bool UpdateData(NpgsqlConnection conn, NpgsqlCommand cmd)
    {
        conn.Open();
        cmd.ExecuteNonQuery();
        return true;
    }

    /// <summary>
    /// Executes a DELETE command.
    /// Returns TRUE if execution succeeded.
    /// </summary>
    protected bool DeleteData(NpgsqlConnection conn, NpgsqlCommand cmd)
    {
        conn.Open();
        cmd.ExecuteNonQuery();
        return true;
    }
}
