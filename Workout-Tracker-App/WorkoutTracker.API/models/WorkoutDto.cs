using System;

namespace WorkoutTracker.API.Models
{
    public class CreateWorkoutDto
    {
        public int UserId { get; set; }
        public DateTime? WorkoutDate { get; set; }
        public string? Notes { get; set; }

        // Make external id optional for clients; server will generate when not provided
        public string? ExternalId { get; set; }
    }
}