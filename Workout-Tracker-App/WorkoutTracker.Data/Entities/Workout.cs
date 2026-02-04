namespace WorkoutTracker.Data.Entities;

public class Workout
{
    // Parameterless constructor so ASP.NET model binding can create this type from JSON
    public Workout() { }

    // Existing convenience constructor
    public Workout(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime WorkoutDate { get; set; }
    public string? Notes { get; set; }

    // Client-provided idempotency token (nullable)
    public string? ExternalId { get; set; }
}