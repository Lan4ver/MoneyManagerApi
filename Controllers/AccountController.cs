using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MoneyManagerApi.Models.API;
using MoneyManagerApi.Models.API.Auth;
using MoneyManagerApi.Models.API.Error;
using MoneyManagerApi.Models.Db;
using MoneyManagerApi.Services.Auth;
using MoneyManagerApi.Services.Db;

namespace MoneyManagerApi.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private IAuthService _authService;
        private PostgreSqlDbContext _dbContext;

        public AccountController(IAuthService authService,
                                 PostgreSqlDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthRequest authRequest)
        {
            try
            {
                if (_dbContext.Users.Any(user => user.Name == authRequest.Name))
                {
                    return Conflict(new MessageError("Error", "User with this name is already exists"));
                }

                var user = _dbContext.Add(new User() { Name = authRequest.Name, Password = authRequest.Password });
                _dbContext.SaveChanges();

                return Ok(new AuthResponse() { Token = _authService.GetToken(user.Entity.Id) });
            }
            catch
            {                
                return BadRequest(new MessageError("Error", "No valid data"));
            }
        }

        [AllowAnonymous]
        [HttpPost("authorize")]
        public IActionResult Authorize([FromBody] AuthRequest authRequest)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(user => user.Name == authRequest.Name && user.Password == authRequest.Password);
                if (user != null)
                {
                    return Ok(new AuthResponse() { Token = _authService.GetToken(user.Id) });
                }
                return Conflict(new MessageError("Error", "No user with that name and/or password"));
            }
            catch
            {
                return BadRequest(new MessageError("Error", "No valid data"));
            }
        }

        [Authorize]
        [HttpGet("validate_token")]
        public IActionResult ValidateToken()
        {
            return Ok(true);
        }
    }
}
