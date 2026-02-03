using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Data.Repositories;
using WorkoutTracker.Data.Entities;
using System;

namespace WorkoutTracker.Api.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repo;

        public UserController(IConfiguration configuration)
        {
            _repo = new UserRepository(configuration);
        }

        public class LoginDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class CreateUserDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        // POST api/user/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email)) return BadRequest();
            var user = _repo.GetUserByEmail(dto.Email);
            if (user == null) return NotFound();
            // NOTE: production must use hashed password comparison
            if (!string.Equals(user.PasswordHash, dto.Password, StringComparison.Ordinal))
                return Unauthorized();

            return Ok(new { id = user.Id, username = user.Username, email = user.Email, createdAt = user.CreatedAt });
        }

        // POST api/user
        [HttpPost]
        public IActionResult Create([FromBody] CreateUserDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest();

            var user = new User(0)
            {
                Username = string.IsNullOrWhiteSpace(dto.Username) ? dto.Email.Split('@')[0] : dto.Username,
                Email = dto.Email,
                PasswordHash = dto.Password // hash in real app
            };

            var newId = _repo.InsertUser(user);
            if (newId == 0) return StatusCode(500, "Could not create user");

            var created = _repo.GetUserById(newId);
            if (created == null)
            {
                return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId, username = user.Username, email = user.Email, createdAt = DateTime.UtcNow });
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { id = created.Id, username = created.Username, email = created.Email, createdAt = created.CreatedAt });
        }

        // GET api/user/{id}
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var user = _repo.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(new { id = user.Id, username = user.Username, email = user.Email, createdAt = user.CreatedAt });
        }
    }
}