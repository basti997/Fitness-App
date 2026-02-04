using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.API.Models;
using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;

namespace WorkoutTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WorkoutRepository _repo;

        public WorkoutController(IConfiguration configuration)
        {
            _configuration = configuration;
            _repo = new WorkoutRepository(configuration);
        }

        // GET api/workout
        // Added so simple List requests to /api/workout do not return 405.
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _repo.GetAllWorkouts();
            return Ok(list);
        }

        // GET api/workout/user/29
        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(int userId)
        {
            if (userId <= 0) return BadRequest("Invalid userId");
            var list = _repo.GetWorkoutsByUserId(userId);
            return Ok(list);
        }

        // GET api/workout/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid id");
            var w = _repo.GetWorkoutById(id);
            if (w == null) return NotFound();
            return Ok(w);
        }

        // POST api/workout
        [HttpPost]
        public IActionResult Create([FromBody] CreateWorkoutDto dto)
        {
            if (dto == null) return BadRequest("Missing request body");
            if (dto.UserId <= 0) return BadRequest("UserId missing or invalid");

            // Ensure an idempotency token exists even if client omitted it
            dto.ExternalId ??= Guid.NewGuid().ToString();

            var entity = new Workout
            {
                UserId = dto.UserId,
                WorkoutDate = dto.WorkoutDate ?? DateTime.UtcNow,
                Notes = dto.Notes ?? string.Empty,
                ExternalId = dto.ExternalId
            };

            var newId = _repo.InsertWorkout(entity);
            if (newId <= 0) return StatusCode(500, "Failed to create workout");

            return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
        }

        // (Optional) add Update/Delete endpoints as needed
    }
}