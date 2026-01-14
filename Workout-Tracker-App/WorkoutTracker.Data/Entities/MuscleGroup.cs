namespace WorkoutTracker.Data.Entities
{
    public class MuscleGroup
    {
        // These property names MUST match the C# code in your Repository.
        // We will match them to the SQL columns in the Repository file.
    public MuscleGroup(int id)
    {
        MuscleGroupId = id;
    }
    public int MuscleGroupId { get; set; }
    public string Name { get; set; }
    }
}
