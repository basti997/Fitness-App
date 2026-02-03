using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace WorkoutTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        protected UserRepository Repository { get; }

        public UserController(UserRepository repository)
        {
            Repository = repository;
        }

        // DTOs
        public class CreateUserRequest
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? PasswordHash { get; set; } // frontend sends raw password here; controller will hash
        }

        public class LoginRequest
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        // GET api/user/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            var user = Repository.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET api/user
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(Repository.GetUsers());
        }

        // POST api/user (create)
        [HttpPost]
        public ActionResult CreateUser([FromBody] CreateUserRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Username))
            {
                return BadRequest(new { error = "Username and Email are required" });
            }

            // Hash the provided password (if present)
            string passwordToStore = req.PasswordHash ?? "";
            if (!string.IsNullOrEmpty(passwordToStore))
            {
                passwordToStore = ComputeHash(passwordToStore);
            }

            var user = new User
            {
                Username = req.Username,
                Email = req.Email,
                PasswordHash = passwordToStore,
                CreatedAt = DateTime.UtcNow
            };

            int newId = Repository.InsertUser(user);
            if (newId > 0)
            {
                return Ok(new { id = newId });
            }

            return BadRequest(new { error = "Could not insert user" });
        }

        // POST api/user/login
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email))
            {
                return BadRequest(new { error = "Email and password required" });
            }

            var user = Repository.GetUserByEmail(req.Email);
            if (user == null) return NotFound(new { error = "User not found" });

            string provided = req.Password ?? "";

            bool match = false;
            try
            {
                var hashedProvided = ComputeHash(provided);
                if (!string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash == hashedProvided) match = true;
            }
            catch { }

            // fallback: allow plaintext stored password (for older records)
            if (!match && user.PasswordHash == provided) match = true;

            if (!match) return Unauthorized(new { error = "Invalid credentials" });

            return Ok(new { id = user.Id, userName = user.Username, email = user.Email, createdAt = user.CreatedAt });
        }

        // PUT api/user
        [HttpPut]
        public ActionResult UpdateUser([FromBody] User user)
        {
            if (user == null) return BadRequest(new { error = "User info not correct" });

            var existingUser = Repository.GetUserById(user.Id);
            if (existingUser == null) return NotFound(new { error = $"User {user.Id} not found" });

            var ok = Repository.UpdateUser(user);
            if (ok) return Ok();
            return BadRequest(new { error = "Update failed" });
        }

        // DELETE api/user/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser([FromRoute] int id)
        {
            var existingUser = Repository.GetUserById(id);
            if (existingUser == null) return NotFound(new { error = $"User {id} not found" });

            var ok = Repository.DeleteUser(id);
            if (ok) return NoContent();
            return BadRequest(new { error = "Delete failed" });
        }

        // Helper: SHA-256 hex string
        private static string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}