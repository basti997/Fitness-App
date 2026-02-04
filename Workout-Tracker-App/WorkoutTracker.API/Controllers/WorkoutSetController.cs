using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;

namespace WorkoutTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutSetController : ControllerBase
    {
        private readonly WorkoutSetRepository _repo;

        public WorkoutSetController(IConfiguration configuration)
        {
            _repo = new WorkoutSetRepository(configuration);
        }

        // POST api/workoutset
        [HttpPost]
        public ActionResult Post([FromBody] WorkoutSet workoutSet)
        {
            if (workoutSet == null || workoutSet.WorkoutId <= 0 || workoutSet.ExerciseId <= 0)
            {
                return BadRequest("WorkoutSet info not correct");
            }

            bool status = _repo.InsertWorkoutSet(workoutSet);
            if (status)
            {
                return Ok();
            }

            return BadRequest();
        }

        // GET api/workoutset/byWorkout/{workoutId}
        [HttpGet("byWorkout/{workoutId}")]
        public ActionResult<IEnumerable<WorkoutSet>> GetSetsByWorkout([FromRoute] int workoutId)
        {
            var items = _repo.GetSetsByWorkout(workoutId);
            // return empty array when none found (frontend expects array)
            if (items == null || !items.Any()) return Ok(new List<WorkoutSet>());
            return Ok(items);
        }

        // GET api/workoutset/byExerciseAndUser/{userId}/{exerciseId}
        [HttpGet("byExerciseAndUser/{userId}/{exerciseId}")]
        public ActionResult<IEnumerable<WorkoutSet>> GetSetsByExerciseAndUser([FromRoute] int userId, [FromRoute] int exerciseId)
        {
            var items = _repo.GetSetsByExerciseAndUser(userId, exerciseId);
            if (items == null || !items.Any()) return Ok(new List<WorkoutSet>());
            return Ok(items);
        }

        // GET api/workoutset/{id}
        [HttpGet("{id}")]
        public ActionResult<WorkoutSet> GetWorkoutSet([FromRoute] int id)
        {
            WorkoutSet workoutSet = _repo.GetWorkoutSetById(id);
            if (workoutSet == null)
            {
                return NotFound();
            }
            return Ok(workoutSet);
        }

        // GET api/workoutset
        [HttpGet]
        public ActionResult<IEnumerable<WorkoutSet>> GetWorkoutSets()
        {
            return Ok(_repo.GetWorkoutSets());
        }

        // PUT api/workoutset
        [HttpPut]
        public ActionResult UpdateWorkoutSet([FromBody] WorkoutSet workoutSet)
        {
            if (workoutSet == null)
            {
                return BadRequest("WorkoutSet info not correct");
            }

            WorkoutSet existingWorkoutSet = _repo.GetWorkoutSetById(workoutSet.Id);
            if (existingWorkoutSet == null)
            {
                return NotFound($"WorkoutSet with id {workoutSet.Id} not found");
            }

            bool status = _repo.UpdateWorkoutSet(workoutSet);
            if (status)
            {
                return Ok();
            }

            return BadRequest("Something went wrong");
        }

        // DELETE api/workoutset/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteWorkoutSet([FromRoute] int id)
        {
            WorkoutSet existingWorkoutSet = _repo.GetWorkoutSetById(id);
            if (existingWorkoutSet == null)
            {
                return NotFound($"WorkoutSet with id {id} not found");
            }

            bool status = _repo.DeleteWorkoutSet(id);
            if (status)
            {
                return NoContent();
            }

            return BadRequest($"Unable to delete WorkoutSet with id {id}");
        }
    }
}