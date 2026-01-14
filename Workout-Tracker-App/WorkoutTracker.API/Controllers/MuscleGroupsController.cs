namespace CourseAdminSystem.API.Controllers;

using WorkoutTracker.Data.Entities;
using WorkoutTracker.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]

    public class MuscleGroupController : ControllerBase
    {
        protected MuscleGroupRepository Repository { get; }

        public MuscleGroupController(MuscleGroupRepository repository)
        {
            Repository = repository;
        }

        // ---------------------------------------------------------------
        // GET MuscleGroup by ID
        // ---------------------------------------------------------------
        [HttpGet("{id}")]
        public ActionResult<MuscleGroup> GetMuscleGroup([FromRoute] int id)
        {
            MuscleGroup group = Repository.GetMuscleGroupById(id);

            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // ---------------------------------------------------------------
        // GET all MuscleGroups
        // ---------------------------------------------------------------
        [HttpGet]
        public ActionResult<IEnumerable<MuscleGroup>> GetMuscleGroups()
        {
            return Ok(Repository.GetMuscleGroups());
        }

        // ---------------------------------------------------------------
        // POST new MuscleGroup
        // ---------------------------------------------------------------
        [HttpPost]
        public ActionResult Post([FromBody] MuscleGroup group)
        {
            if (group == null)
            {
                return BadRequest("MuscleGroup info not correct");
            }

            bool status = Repository.InsertMuscleGroup(group);

            if (status)
            {
                return Ok();
            }

            return BadRequest();
        }

        // ---------------------------------------------------------------
        // PUT update MuscleGroup
        // ---------------------------------------------------------------
        [HttpPut]
        public ActionResult UpdateMuscleGroup([FromBody] MuscleGroup group)
        {
            if (group == null)
            {
                return BadRequest("MuscleGroup info not correct");
            }

            MuscleGroup existingGroup = Repository.GetMuscleGroupById(group.MuscleGroupId);

            if (existingGroup == null)
            {
                return NotFound($"MuscleGroup with id {group.MuscleGroupId} not found");
            }

            bool status = Repository.UpdateMuscleGroup(group);

            if (status)
            {
                return Ok();
            }

            return BadRequest("Something went wrong");
        }

        // ---------------------------------------------------------------
        // DELETE MuscleGroup by ID
        // ---------------------------------------------------------------
        [HttpDelete("{id}")]
        public ActionResult DeleteMuscleGroup([FromRoute] int id)
        {
            MuscleGroup existingGroup = Repository.GetMuscleGroupById(id);

            if (existingGroup == null)
            {
                return NotFound($"MuscleGroup with id {id} not found");
            }

            bool status = Repository.DeleteMuscleGroup(id);

            if (status)
            {
                return NoContent();
            }

            return BadRequest($"Unable to delete MuscleGroup with id {id}");
        }
    }


// using Microsoft.AspNetCore.Mvc;
// using WorkoutTracker.Data.Entities;
// using WorkoutTracker.Data.Repositories;

// // We must wrap our controller in a "namespace"
// namespace WorkoutTracker.API.Controllers
// {
//     // --- THIS IS THE FIX for your 404 ---
//     // This "label" tells .NET this class is an API Controller
//     [ApiController] 
//     // This "label" sets the URL to "api/MuscleGroups"
//     [Route("api/[controller]")] 
//     // --- END OF FIX ---

//     public class MuscleGroupsController : ControllerBase 
//     {
//         private readonly MuscleGroupRepository _muscleGroupRepo;

//         // The constructor asks for the Repository
//         public MuscleGroupsController(MuscleGroupRepository muscleGroupRepo)
//         {
//             _muscleGroupRepo = muscleGroupRepo;
//         }

//         // This function runs when you send a GET request
//         [HttpGet] 
//         public async Task<IActionResult> GetAllMuscleGroups() 
//         {
//             // It calls the repository to get the data
//             var muscleGroups = await _muscleGroupRepo.GetAllAsync();
//             // It returns the data as JSON
//             return Ok(muscleGroups); 
//         }
//     }
// }