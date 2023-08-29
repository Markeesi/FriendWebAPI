using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

            // Create a new FriendCombination instance
            var newCombination = new FriendCombination
            {
                Friend1 = request.friend1,
                Friend2 = request.friend2
            };

            // Get the path to the JSON file
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "friends.json");

            // Deserialize existing content from the file (if it exists)
            List<FriendCombination> existingCombinations = new List<FriendCombination>();
            if (System.IO.File.Exists(filePath))
            {
                string existingContent = System.IO.File.ReadAllText(filePath);
                existingCombinations = JsonSerializer.Deserialize<List<FriendCombination>>(existingContent);
            }

            // Add the new friend combination
            existingCombinations.Add(newCombination);

            // Serialize the updated content back to JSON
            string updatedContent = JsonSerializer.Serialize(existingCombinations, new JsonSerializerOptions { WriteIndented = true });

            // Write the updated content back to the file
            System.IO.File.WriteAllText(filePath, updatedContent);

            // Return a success response
            return Ok("Combined friends appended to the JSON file.");
        }


    }
}