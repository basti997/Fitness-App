using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WorkoutTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        protected WorkoutRepository Repository { get; }

        public WorkoutController(WorkoutRepository repository)
        {
            Repository = repository;
        }

        // GET api/workout/{id}
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

        // GET api/workout
        [HttpGet]
        public ActionResult<IEnumerable<Workout>> GetWorkouts()
        {
            return Ok(Repository.GetWorkouts());
        }

        // POST api/workout
        [HttpPost]
        public ActionResult Post([FromBody] Workout workout)
        {
            if (workout == null)
            {
                return BadRequest("Workout info not correct");
            }

            int newId = Repository.CreateWorkout(workout);

            if (newId > 0)
            {
                return Ok(new { id = newId });
            }

            return BadRequest();
        }

        // PUT api/workout
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

        // DELETE api/workout/{id}
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

        // GET api/workout/byUser/{userId}
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

        // POST api/workout/start
        [HttpPost("start")]
        public ActionResult StartWorkout([FromBody] Workout workout)
        {
            if (workout == null || workout.UserId <= 0)
            {
                return BadRequest("Workout info not correct");
            }

            if (workout.WorkoutDate == default)
            {
                workout.WorkoutDate = DateTime.UtcNow;
            }

            int newId = Repository.CreateWorkout(workout);
            if (newId > 0)
            {
                return Ok(new { id = newId });
            }

            return BadRequest();
        }

        // POST api/workout/finish/{id}
        [HttpPost("finish/{id}")]
        public ActionResult FinishWorkout([FromRoute] int id)
        {
            var existing = Repository.GetWorkoutById(id);
            if (existing == null) return NotFound();

            return Ok();
        }
    }
}