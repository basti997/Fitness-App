namespace WorkoutTracker.Data.Entities;

public class Exercises
{
    public Exercises(int id) 
        { 
            ExerciseId = id; 
        }

        // Primary key
        public int ExerciseId { get; set; } // maps to exercise_id

        // Name of the exercise
        public string Name { get; set; }

        // Optional description on how to perform the exercise
        public string Description { get; set; }

        // Foreign key linking to the MuscleGroups table
        public int MuscleGroupId { get; set; }
}
