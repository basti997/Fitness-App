using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        // GET api/user/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUser([FromRoute] int id)
        {
            User user = Repository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET api/user
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(Repository.GetUsers());
        }

        // POST api/user
        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User info not correct");
            }

            int newId = Repository.InsertUser(user);
            if (newId > 0)
            {
                return Ok(new { id = newId });
            }

            return BadRequest();
        }

        // PUT api/user
        [HttpPut]
        public ActionResult UpdateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User info not correct");
            }

            User existingUser = Repository.GetUserById(user.Id);
            if (existingUser == null)
            {
                return NotFound($"User with id {user.Id} not found");
            }

            bool status = Repository.UpdateUser(user);
            if (status)
            {
                return Ok();
            }

            return BadRequest("Something went wrong");
        }

        // DELETE api/user/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser([FromRoute] int id)
        {
            User existingUser = Repository.GetUserById(id);
            if (existingUser == null)
            {
                return NotFound($"User with id {id} not found");
            }

            bool status = Repository.DeleteUser(id);
            if (status)
            {
                return NoContent();
            }

            return BadRequest($"Unable to delete user with id {id}");
        }
    }
}