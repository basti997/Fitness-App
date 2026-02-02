namespace WorkoutTracker.API.Controllers;

using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutSetController : ControllerBase
    {
        protected WorkoutSetRepository Repository { get; }

        public WorkoutSetController(WorkoutSetRepository repository)
        {
            Repository = repository;
        }
    //-----------------------
    //Better customer journey
    //--------------------
    // POST api/workoutset
        [HttpPost]
        public ActionResult Post([FromBody] WorkoutSet workoutSet)
        {
            if (workoutSet == null || workoutSet.WorkoutId <= 0 || workoutSet.ExerciseId <= 0)
            {
                return BadRequest("WorkoutSet info not correct");
            }

            bool status = Repository.InsertWorkoutSet(workoutSet);
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
        var items = Repository.GetSetsByWorkout(workoutId);
        if (items == null || !items.Any()) return NotFound();
        return Ok(items);
    }

    // GET api/workoutset/byExerciseAndUser/{userId}/{exerciseId}
    [HttpGet("byExerciseAndUser/{userId}/{exerciseId}")]
    public ActionResult<IEnumerable<WorkoutSet>> GetSetsByExerciseAndUser([FromRoute] int userId, [FromRoute] int exerciseId)
    {
        var items = Repository.GetSetsByExerciseAndUser(userId, exerciseId);
        if (items == null || !items.Any()) return NotFound();
        return Ok(items);
    }
    
    //-----------------------
    //basic controllers
    //------------------------

    // GET api/workoutset/5
    [HttpGet("{id}")]
        public ActionResult<WorkoutSet> GetWorkoutSet([FromRoute] int id)
        {
            WorkoutSet workoutSet = Repository.GetWorkoutSetById(id);
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
            return Ok(Repository.GetWorkoutSets());
        }

        // PUT api/workoutset
        [HttpPut]
        public ActionResult UpdateWorkoutSet([FromBody] WorkoutSet workoutSet)
        {
            if (workoutSet == null)
            {
                return BadRequest("WorkoutSet info not correct");
            }

            WorkoutSet existingWorkoutSet = Repository.GetWorkoutSetById(workoutSet.Id);
            if (existingWorkoutSet == null)
            {
                return NotFound($"WorkoutSet with id {workoutSet.Id} not found");
            }

            bool status = Repository.UpdateWorkoutSet(workoutSet);
            if (status)
            {
                return Ok();
            }

            return BadRequest("Something went wrong");
        }

        // DELETE api/workoutset/5
        [HttpDelete("{id}")]
        public ActionResult DeleteWorkoutSet([FromRoute] int id)
        {
            WorkoutSet existingWorkoutSet = Repository.GetWorkoutSetById(id);
            if (existingWorkoutSet == null)
            {
                return NotFound($"WorkoutSet with id {id} not found");
            }

            bool status = Repository.DeleteWorkoutSet(id);
            if (status)
            {
                return NoContent();
            }

            return BadRequest($"Unable to delete WorkoutSet with id {id}");
        }
    }