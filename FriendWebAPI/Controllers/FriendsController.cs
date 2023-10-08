using Microsoft.AspNetCore.Mvc;
using NLog; // Import NLog namespace
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;

namespace FriendWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsRepository _friendsRepository;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger(); // Initialize the NLog logger

        public FriendsController(IFriendsRepository friendsRepository)
        {
            _friendsRepository = friendsRepository;
        }

        [HttpGet("json")]
        public IActionResult GetJsonFileContent()
        {
            try
            {
                // Get the path to the JSON file
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "friends.json");

                // Read the content from the file (if it exists)
                if (System.IO.File.Exists(filePath))
                {
                    string fileContent = System.IO.File.ReadAllText(filePath);
                    _logger.Info("GET JSON file content - Status Code: 200 OK");
                    return Content(fileContent, "application/json");
                }

                _logger.Warn("GET JSON file content - Status Code: 404 Not Found");
                return NotFound(); // File not found
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while retrieving JSON content.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public IActionResult CreateFriends([FromBody] CreateFriendRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.Warn("POST CreateFriends - Status Code: 400 Bad Request - Invalid request body.");
                    return BadRequest("Invalid request body.");
                }

                // Validate the incoming request for unexpected properties
                var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        _logger.Warn($"POST CreateFriends - Status Code: 400 Bad Request - {validationResult.ErrorMessage}");
                    }

                    return BadRequest(validationResults.Select(vr => vr.ErrorMessage));
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

                _logger.Info("POST CreateFriends - Status Code: 200 OK - Combined friends appended to the JSON file.");
                return Ok("Combined friends appended to the JSON file.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating friends.");
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
