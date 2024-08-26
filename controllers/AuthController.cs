using Microsoft.AspNetCore.Mvc;
using TechnicalTestApi.Models;
using TechnicalTestApi.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TechnicalTestApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        // Constructor to inject the necessary services
        public AuthController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // POST: api/auth/register
        // Registers a new user
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User newUser)
        {
            // Check if the user already exists
            var existingUser = await _userService.GetAsyncEmail(newUser.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "User with this email already exists" });
            }

            // Hash the user's password
            var passwordHasher = new PasswordHasher<User>();
            newUser.Password = passwordHasher.HashPassword(newUser, newUser.Password);

            // Create the new user in the database
            await _userService.CreateAsync(newUser);

            // Generate a JWT token for the user
            var token = GenerateJwtToken(newUser);

            // Configure the options for the cookie that will store the JWT token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            // Store the JWT token in a cookie
            Response.Cookies.Append("jwt", token, cookieOptions);

            // Return the created user with the generated token
            return CreatedAtAction(nameof(Register), new { id = newUser.Id }, new { token });
        }

        // POST: api/auth/login
        // Authenticates a user and issues a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequests loginRequest)
        {
            var user = await _userService.GetAsyncEmail(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Verify the user's password
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Generate a new JWT token
            var token = GenerateJwtToken(user);

            // Configure the options for the cookie that will store the JWT token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            // Store the JWT token in a cookie
            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { token, message = "Logged in successfully" });
        }

        // Private method to generate a JWT token for a user
        private string GenerateJwtToken(User user)
        {
            // Define the claims that will be included in the JWT token
            var claims = new[]
            {
                new Claim("userId", user.Id),
                new Claim("email", user.Email)
            };

            // Create the key and credentials used to sign the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/auth/tokeninfo
        // Returns information about the current JWT token
        [HttpGet("tokeninfo")]
        public IActionResult GetTokenInfo()
        {
            // Get the JWT token from cookies
            var jwt = Request.Cookies["jwt"];
            if (jwt == null)
            {
                return Unauthorized(new { message = "No token found" });
            }

            // Decode the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt);

            // Extract the userId and email from the token's claims
            var userId = token.Claims.First(claim => claim.Type == "userId").Value;
            var email = token.Claims.First(claim => claim.Type == "email").Value;

            // Return the decoded information
            return Ok(new { userId, email });
        }
    }
}
