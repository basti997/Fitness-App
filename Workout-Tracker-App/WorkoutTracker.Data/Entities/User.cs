namespace WorkoutTracker.Data.Entities;

public class User
{
    public User() { }
    
    public User(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
}
