namespace WorkoutTracker.API.Controllers;

using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorkoutController : ControllerBase
{
    protected WorkoutRepository Repository { get; }

    public WorkoutController(WorkoutRepository repository)
    {
        Repository = repository;
    }

    // ---------------------------------------------------------------
    // GET workout by ID
    // ---------------------------------------------------------------
    [HttpGet("{id}")]
    public ActionResult<Workout> GetWorkout([FromRoute] int id)
    {
        Workout workout = Repository.GetWorkoutById(id);

        if (workout == null)
        {
            return NotFound();
        }

        return Ok(workout);
    }

    // ---------------------------------------------------------------
    // GET all workouts
    // ---------------------------------------------------------------
    [HttpGet]
    public ActionResult<IEnumerable<Workout>> GetWorkouts()
    {
        return Ok(Repository.GetWorkouts());
    }

    // ---------------------------------------------------------------
    // POST new workout
    // ---------------------------------------------------------------
    [HttpPost]
    public ActionResult Post([FromBody] Workout workout)
    {
        if (workout == null)
        {
            return BadRequest("Workout info not correct");
        }

        bool status = Repository.InsertWorkout(workout);

        if (status)
        {
            return Ok();
        }

        return BadRequest();
    }

    // ---------------------------------------------------------------
    // PUT update workout
    // ---------------------------------------------------------------
    [HttpPut]
    public ActionResult UpdateWorkout([FromBody] Workout workout)
    {
        if (workout == null)
        {
            return BadRequest("Workout info not correct");
        }

        Workout existingWorkout = Repository.GetWorkoutById(workout.Id);

        if (existingWorkout == null)
        {
            return NotFound($"Workout with id {workout.Id} not found");
        }

        bool status = Repository.UpdateWorkout(workout);

        if (status)
        {
            return Ok();
        }

        return BadRequest("Something went wrong");
    }

    // ---------------------------------------------------------------
    // DELETE workout by ID
    // ---------------------------------------------------------------
    [HttpDelete("{id}")]
    public ActionResult DeleteWorkout([FromRoute] int id)
    {
        Workout existingWorkout = Repository.GetWorkoutById(id);

        if (existingWorkout == null)
        {
            return NotFound($"Workout with id {id} not found");
        }

        bool status = Repository.DeleteWorkout(id);

        if (status)
        {
            return NoContent();
        }

        return BadRequest($"Unable to delete workout with id {id}");
    }

    // ---------------------------------------------------------------
    // GET workouts by user ID
    // ---------------------------------------------------------------
    [HttpGet("byUser/{userId}")]
    public ActionResult<IEnumerable<Workout>> GetWorkoutsByUser([FromRoute] int userId)
    {
        var workouts = Repository.GetWorkoutsByUser(userId);
        if (workouts == null || !workouts.Any())
        {
            return NotFound();
        }
        return Ok(workouts);
    }
    //--------------------
    //New code for better journey
    //--------------------
    // POST api/workout/start
    [HttpPost("start")]
    public ActionResult StartWorkout([FromBody] Workout workout)
    {
        if (workout == null || workout.UserId <= 0)
        {
            return BadRequest("Workout info not correct");
        }

        // If client doesn’t set time, default will be set by DB
        if (workout.WorkoutDate == default)
        {
            workout.WorkoutDate = DateTime.UtcNow;
        }

        bool status = Repository.InsertWorkout(workout);
        if (status)
        {
            return Ok();
        }

        return BadRequest();
    }

    // POST api/workout/finish/{id}
    [HttpPost("finish/{id}")]
    public ActionResult FinishWorkout([FromRoute] int id)
    {
        // If you don’t have a status column, this can be a no-op
        // Optionally validate the workout exists:
        var existing = Repository.GetWorkoutById(id);
        if (existing == null) return NotFound();

        // No state change without a column; return 200 OK.
        return Ok();
    }

    }
