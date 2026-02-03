using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Data.Repositories;
using WorkoutTracker.Data.Entities;
using System;

namespace WorkoutTracker.Api.Controllers
{
    [ApiController]
    [Route("api/workout")]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutRepository _repo;

        public WorkoutController(IConfiguration configuration)
        {
            _repo = new WorkoutRepository(configuration);
        }

        public class CreateWorkoutDto
        {
            public int UserId { get; set; }
            public DateTime? WorkoutDate { get; set; }
            public string Notes { get; set; }
        }

        // GET api/workout/user/{userId}
        [HttpGet("user/{userId:int}")]
        public IActionResult GetForUser(int userId)
        {
            if (userId <= 0) return BadRequest();
            var list = _repo.GetWorkoutsByUserId(userId);
            return Ok(list);
        }

        // GET api/workout/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0) return BadRequest();
            var w = _repo.GetWorkoutById(id);
            if (w == null) return NotFound();
            return Ok(w);
        }

        // POST api/workout
        [HttpPost]
        public IActionResult Create([FromBody] CreateWorkoutDto dto)
        {
            if (dto == null || dto.UserId <= 0) return BadRequest();
            var workout = new Workout(0)
            {
                UserId = dto.UserId,
                WorkoutDate = dto.WorkoutDate ?? DateTime.UtcNow,
                Notes = dto.Notes ?? string.Empty
            };

            var id = _repo.InsertWorkout(workout);
            if (id == 0) return StatusCode(500, "Could not create workout");
            return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
        }
          // PUT api/workout
        [HttpPut]
        public IActionResult Update([FromBody] Workout workout)
        {
            if (workout == null || workout.Id <= 0) return BadRequest();
            var ok = _repo.UpdateWorkout(workout);
            if (!ok) return StatusCode(500, "Could not update workout");
            return NoContent(); // standard for successful PUT with no body
        }

        // DELETE api/workout/{id}
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var ok = _repo.DeleteWorkout(id);
            if (!ok) return NotFound(); // nothing deleted
            return NoContent();
        }
    }
}