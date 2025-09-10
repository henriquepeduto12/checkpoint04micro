
using Microsoft.AspNetCore.Mvc;
using UserSessionApi.Services;

namespace UserSessionApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISessionService _session;

        public AuthController(ISessionService session)
        {
            _session = session;
        }

        // Simples endpoint de "login" que retorna o perfil do usuário (mock: userId vem da requisição)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest(new { error = "userId is required" });

            var user = await _session.GetUserProfileAsync(request.UserId, ct);
            if (user is null)
                return NotFound(new { error = "user not found" });

            return Ok(user);
        }
    }

    public class LoginRequest
    {
        public string UserId { get; set; } = default!;
    }
}
