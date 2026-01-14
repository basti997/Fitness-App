namespace WorkoutTracker.API.Controllers;

using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class ExerciseController : ControllerBase
{
    protected ExerciseRepository Repository { get; }

    public ExerciseController(ExerciseRepository repository)
    {
        Repository = repository;
    }

    // GET api/exercise/{id}
    [HttpGet("{id}")]
    public ActionResult<Exercises> GetExercise([FromRoute] int id)
    {
        Exercises exercise = Repository.GetExerciseById(id);
        if (exercise == null)
        {
            return NotFound();
        }
        return Ok(exercise);
    }

    // GET api/exercise
    [HttpGet]
    public ActionResult<IEnumerable<Exercises>> GetExercises()
    {
        return Ok(Repository.GetExercises());
    }

    // POST api/exercise
    [HttpPost]
    public ActionResult Post([FromBody] Exercises exercise)
    {
        if (exercise == null)
        {
            return BadRequest("Exercise info not correct");
        }

        bool status = Repository.InsertExercise(exercise);
        if (status)
        {
            return Ok();
        }

        return BadRequest();
    }

    // PUT api/exercise
    [HttpPut]
    public ActionResult UpdateExercise([FromBody] Exercises exercise)
    {
        if (exercise == null)
        {
            return BadRequest("Exercise info not correct");
        }

        Exercises existingExercise = Repository.GetExerciseById(exercise.ExerciseId);
        if (existingExercise == null)
        {
            return NotFound($"Exercise with id {exercise.ExerciseId} not found");
        }

        bool status = Repository.UpdateExercise(exercise);
        if (status)
        {
            return Ok();
        }

        return BadRequest("Something went wrong");
    }

    // DELETE api/exercise/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteExercise([FromRoute] int id)
    {
        Exercises existingExercise = Repository.GetExerciseById(id);
        if (existingExercise == null)
        {
            return NotFound($"Exercise with id {id} not found");
        }

        bool status = Repository.DeleteExercise(id);
        if (status)
        {
            return NoContent();
        }

        return BadRequest($"Unable to delete exercise with id {id}");
    }
    
    // GET api/exercise/byMuscleGroup/{muscleGroupId}
    [HttpGet("byMuscleGroup/{muscleGroupId}")]
    public ActionResult<IEnumerable<Exercises>> GetExercisesByMuscleGroup([FromRoute] int muscleGroupId)
    {
        var exercises = Repository.GetExercisesByMuscleGroup(muscleGroupId);
        if (exercises == null || !exercises.Any())
            {
                return NotFound();
            }
        return Ok(exercises);
    }
}
