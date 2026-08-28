using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiryBotCode.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AiryBotCode.Api.Controllers
{
    public sealed record StartPairingRequest(string? DeviceName, string? Os, string? AppVersion);
    public sealed record CodeRequest(string? Code);

    /// <summary>
    /// Pairing a native app that has no credential yet. The app asks for a code,
    /// shows it along with its own machine name so the human can see which device is
    /// asking, and polls. Someone already signed in approves that code in the browser
    /// and the app's next poll returns a token.
    ///
    /// This replaces handing the token back over a custom URL scheme: browsers refuse
    /// a scheme launch that has no user gesture behind it, silently, which made desktop
    /// sign-in look like it simply did nothing.
    /// </summary>
    [ApiController]
    [Route("api/auth/device")]
    [Produces("application/json")]
    public class DeviceController : ControllerBase
    {
        private readonly DevicePairingService _pairings;
        private readonly IConfiguration _configuration;

        public DeviceController(DevicePairingService pairings, IConfiguration configuration)
        {
            _pairings = pairings;
            _configuration = configuration;
        }

        /// <summary>The app has no credential here, so this is open — all it can do is
        /// create a pending request that a signed-in human must then approve.</summary>
        [AllowAnonymous]
        [HttpPost("start")]
        public IActionResult Start([FromBody] StartPairingRequest body)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var request = _pairings.Start(body?.DeviceName ?? "", body?.Os ?? "", body?.AppVersion ?? "", ip);

            return Ok(new
            {
                code = request.Code,
                deviceId = request.DeviceId,
                deviceSecret = request.DeviceSecret,
                expiresAt = request.ExpiresAt,
                pollSeconds = 2,
                approveUrl = $"{_configuration["Discord:FrontendUri"]}/?device={request.Code}",
            });
        }

        /// <summary>Polled by the app. The device secret is required, so a code seen
        /// over someone's shoulder cannot be used to collect the token.</summary>
        [AllowAnonymous]
        [HttpGet("poll")]
        public IActionResult Poll([FromQuery] string deviceId, [FromQuery] string secret)
        {
            var result = _pairings.Poll(deviceId ?? "", secret ?? "");
            if (result is null) return Ok(new { status = "expired" });

            var (state, token) = result.Value;
            return state switch
            {
                PairingState.Approved => Ok(new { status = "approved", token }),
                PairingState.Denied => Ok(new { status = "denied" }),
                _ => Ok(new { status = "pending" }),
            };
        }

        /// <summary>What the browser shows before you approve: which machine, which
        /// build, from where, and when it asked.</summary>
        [Authorize]
        [HttpGet("{code}")]
        public IActionResult Get(string code)
        {
            var request = _pairings.Find(code);
            if (request is null) return NotFound(new { message = "That code is unknown or has expired." });

            return Ok(new
            {
                code = request.Code,
                deviceName = request.DeviceName,
                os = request.Os,
                appVersion = request.AppVersion,
                requestedFromIp = request.RequestedFromIp,
                requestedAt = request.RequestedAt,
                expiresAt = request.ExpiresAt,
                state = request.State.ToString().ToLowerInvariant(),
            });
        }

        [Authorize]
        [HttpGet("pending")]
        public IActionResult Pending() => Ok(_pairings.Pending().Select(r => new
        {
            code = r.Code,
            deviceName = r.DeviceName,
            os = r.Os,
            appVersion = r.AppVersion,
            requestedFromIp = r.RequestedFromIp,
            requestedAt = r.RequestedAt,
        }));

        [Authorize]
        [HttpPost("approve")]
        public IActionResult Approve([FromBody] CodeRequest body)
        {
            var request = _pairings.Find(body?.Code ?? "");
            if (request is null) return NotFound(new { message = "That code is unknown or has expired." });
            if (request.State != PairingState.Pending) return Conflict(new { message = "That request was already answered." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("nameid") ?? "unknown";
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("unique_name") ?? userId;

            // The device gets its own token for the approving user, not a copy of the
            // browser's — so revoking or expiring one does not silently affect the other.
            var token = GenerateJwt(userId, userName);
            if (!_pairings.Approve(request.Code, token, userName))
                return Conflict(new { message = "That request was already answered." });

            return Ok(new { approved = true, deviceName = request.DeviceName });
        }

        [Authorize]
        [HttpPost("deny")]
        public IActionResult Deny([FromBody] CodeRequest body)
        {
            if (!_pairings.Deny(body?.Code ?? ""))
                return NotFound(new { message = "That code is unknown or has expired." });
            return Ok(new { denied = true });
        }

        private string GenerateJwt(string userId, string userName)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]!);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            return handler.WriteToken(handler.CreateToken(descriptor));
        }
    }
}
