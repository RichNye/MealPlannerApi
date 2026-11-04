using MealPlannerApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipePlannerApi.Controllers;
using System.Security.Claims;

namespace MealPlannerApi.Controllers
{
    [Route("/me")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IdentityController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IdentityController(ApplicationDbContext context, ILogger<IdentityController> logger, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            _logger.LogInformation(
                "Caller IP: {Ip}",
                HttpContext.Connection.RemoteIpAddress
            );

            return Ok(new
            {
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                name = User.Identity?.Name
            });
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation(User.Identity?.Name + " logged out.");
                return Ok();
            }
            catch
            {
                return BadRequest();
            }         
        }
    }
}
