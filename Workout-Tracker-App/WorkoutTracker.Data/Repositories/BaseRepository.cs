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
        conn.Open();
        cmd.ExecuteNonQuery();
        return true;
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
