using Microsoft.AspNetCore.Mvc;

namespace FriendWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsRepository _friendsRepository;  
        public FriendsController(IFriendsRepository friendsRepository)
        {
            _friendsRepository = friendsRepository;
        }

        [HttpGet("json")]
        public IActionResult GetJsonFileContent()
        {
            // Get the path to the JSON file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "friends.json");

            // Read the content from the file (if it exists)
            if (System.IO.File.Exists(filePath))
            {
                string fileContent = System.IO.File.ReadAllText(filePath);
                return Content(fileContent, "application/json");
            }

            return NotFound(); // File not found
        }


        [HttpPost]
        public IActionResult CreateFriends([FromBody] CreateFriendRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request body.");
            }
            // Combine the two friends
            string combinedFriends = $"{request.friend1}, {request.friend2}";

            // Get the path to the JSON file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "friends.json");

            // Read the existing content from the file (if it exists)
            string existingContent = System.IO.File.Exists(filePath)
                ? System.IO.File.ReadAllText(filePath)
                : string.Empty;

            // Update the content with the new friend combination
            string updatedContent = $"{existingContent}{Environment.NewLine}{combinedFriends}";

            // Write the updated content back to the file
            System.IO.File.WriteAllText(filePath, updatedContent);

            // Return a success response
            return Ok("Combined friends appended to the JSON file.");
        }

    }
}