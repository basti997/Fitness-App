namespace WorkoutTracker.Data.Entities;

public class Workout
{
    public Workout(int id)
    {
        Id = id;
    }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string? Notes { get; set; }
}
