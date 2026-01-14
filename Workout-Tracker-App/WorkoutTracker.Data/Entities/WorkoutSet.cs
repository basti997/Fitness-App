namespace WorkoutTracker.Data.Entities;

public class WorkoutSet
{
    public WorkoutSet(int id)
    {
        Id = id;
    }
    public int Id { get; set; }
    public int WorkoutId { get; set; }
    public int ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public double Weight { get; set; }
    public int Reps { get; set; }
    public DateTime? WorkoutDate { get; set; } // join info for progress time display

}
