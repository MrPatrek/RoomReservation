using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoomReservationServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repository;
        private readonly IConfiguration _configuration;

        public UserController(ILoggerManager logger, IRepositoryWrapper repository, IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginInput)
        {
            if (loginInput is null)
            {
                return BadRequest("Invalid client request");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid login object sent from client.");
                return BadRequest("Invalid login object");
            }

            var userDb = await _repository.User.GetUserByUsernameAsync(loginInput.Username);
            if (userDb is null)
            {
                return Unauthorized("Wrong username");
            }

            string loginPasswordHash = CalculateHash(loginInput.Password, Convert.FromBase64String(userDb.PasswordSalt));

            if (loginPasswordHash.Equals(userDb.PasswordHash))
            {
                // when all checks are passed:

                string tokenKey = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Development")
                    ? _configuration.GetValue<string>("TokenKey") : Environment.GetEnvironmentVariable("TOKEN_KEY");

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new { Token = tokenString });
            }

            return Unauthorized("Wrong password");
        }

        private string CalculateHash(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
