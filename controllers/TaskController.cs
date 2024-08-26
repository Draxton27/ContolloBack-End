using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnicalTestApi.Models;
using TechnicalTestApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TechnicalTestApi.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly NoteService _noteService;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        // Constructor to inject the necessary services
        public TaskController(NoteService noteService, UserService userService, IConfiguration configuration)
        {
            _noteService = noteService;
            _userService = userService;
            _configuration = configuration;
        }

        // GET: api/tasks
        // Retrieve all notes for the authenticated user
        [HttpGet]
        public async Task<ActionResult<List<Note>>> GetNotes()
        {
            var userId = GetUserIdFromToken();  // Get user ID from JWT token
            var notes = await _noteService.GetAsync(userId);  // Fetch notes for the user
            return Ok(notes);  // Return the list of notes
        }

        // GET: api/tasks/{id}
        // Retrieve a specific note by its ID for the authenticated user
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNoteById(string id)
        {
            var userId = GetUserIdFromToken();  // Get user ID from JWT token
            var note = await _noteService.GetNoteAsync(userId, id);  // Fetch the note by ID
            return Ok(note);  // Return the note
        }

        // POST: api/tasks
        // Create a new note for the authenticated user
        [HttpPost]
        public async Task<ActionResult> CreateNote(Note newNote)
        {
            var userId = GetUserIdFromToken();  // Get user ID from JWT token

            // Create the note associated with the user
            await _noteService.CreateAsync(userId, newNote);

            // Return the response with the newly created note
            return CreatedAtAction(nameof(GetNoteById), new { id = newNote.Id }, newNote);
        }

        // PUT: api/tasks/{id}
        // Update an existing note for the authenticated user
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(string id, Note updatedNote)
        {
            var userId = GetUserIdFromToken();  // Get user ID from JWT token
            var existingNote = await _noteService.GetNoteAsync(userId, id);  // Fetch the existing note by ID
            if (existingNote == null)
            {
                return NotFound();  // Return 404 if the note doesn't exist
            }

            updatedNote.Id = id;  // Ensure the note ID is set
            await _noteService.UpdateAsync(userId, id, updatedNote);  // Update the note
            return NoContent();  // Return 204 No Content to indicate success
        }

        // DELETE: api/tasks/{id}
        // Delete an existing note by its ID for the authenticated user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(string id)
        {
            var userId = GetUserIdFromToken();  // Get user ID from JWT token
            var note = await _noteService.GetNoteAsync(userId, id);  // Fetch the note by ID
            if (note == null)
            {
                return NotFound();  // Return 404 if the note doesn't exist
            }

            await _noteService.RemoveAsync(userId, id);  // Delete the note
            return NoContent();  // Return 204 No Content to indicate success
        }

        // Private method to extract the user ID from the JWT token in cookies
        private string GetUserIdFromToken()
        {
            var jwt = Request.Cookies["jwt"];  // Retrieve JWT from cookies
            if (jwt == null)
            {
                throw new UnauthorizedAccessException("JWT token is missing from the cookies.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token;

            try
            {
                token = tokenHandler.ReadJwtToken(jwt);  // Read and parse the JWT token
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Invalid JWT token.");  // Handle invalid JWT
            }

            // Extract the user ID from the token claims
            var userId = token.Claims.First(claim => claim.Type == "userId").Value;
            return userId;  // Return the extracted user ID
        }
    }
}
