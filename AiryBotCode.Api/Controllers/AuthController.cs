using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

namespace AiryBotCode.Api.Controllers
{
    // DTO to hold user info from Discord
    public class DiscordUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }

    // Body for the first-factor password gate.
    public class GateRequest
    {
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Gate tokens are signed with the same secret but carry a distinct
        // audience so they can NEVER be replayed as an access token against the
        // [Authorize] endpoints (which require the normal Jwt:Audience).
        private const string GateAudience = "AiryBotCode-Gate";

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // First factor: verify the shared access password. On success we hand back
        // a short-lived gate token that the login app passes to Discord as `state`
        // so the callback can prove the password step was actually completed.
        [HttpPost("gate")]
        public IActionResult Gate([FromBody] GateRequest request)
        {
            var expected = _configuration["Panel:GatePassword"];
            if (string.IsNullOrEmpty(expected))
            {
                return StatusCode(500, "Access gate is not configured.");
            }

            var provided = request?.Password ?? string.Empty;
            var providedBytes = Encoding.UTF8.GetBytes(provided);
            var expectedBytes = Encoding.UTF8.GetBytes(expected);

            // Constant-time comparison to avoid leaking the password via timing.
            if (providedBytes.Length != expectedBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
            {
                return Unauthorized();
            }

            return Ok(new { token = GenerateGateToken() });
        }

        [HttpGet("discord/redirect")]
        public async Task<IActionResult> DiscordRedirect([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Discord authorization code is missing.");
            }

            // Second-factor guard: the `state` must be a valid, unexpired gate
            // token, i.e. the password step was genuinely completed first.
            if (!ValidateGateToken(state))
            {
                return Unauthorized("The access gate was not satisfied. Start again from the login page.");
            }

            try
            {
                // 1. Exchange the code for an access token
                var tokenResponse = await ExchangeCodeForToken(code);
                if (tokenResponse == null)
                {
                    return Unauthorized("Failed to get access token from Discord.");
                }

                // 2. Use the access token to get user information
                var user = await GetDiscordUser(tokenResponse.AccessToken);
                if (user == null)
                {
                    return Unauthorized("Failed to get user info from Discord.");
                }

                // 2b. Enforce the allowlist: if Discord:AllowedUserIds is configured,
                // only those Discord user IDs may obtain a token. Empty/absent = allow any.
                var allowed = _configuration.GetSection("Discord:AllowedUserIds").Get<string[]>();
                if (allowed is { Length: > 0 } && !allowed.Contains(user.Id))
                {
                    return Unauthorized("This Discord account is not authorized to access the control panel.");
                }

                // 3. Generate a JWT for the user
                var tokenString = GenerateJwt(user);

                // 4. Redirect back to the frontend with the token
                var frontendUrl = _configuration["Discord:FrontendUri"];
                return Redirect($"{frontendUrl}#token={tokenString}");
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private async Task<DiscordTokenResponse> ExchangeCodeForToken(string code)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenUrl = "https://discord.com/api/oauth2/token";
            
            var requestBody = new Dictionary<string, string>
            {
                { "client_id", _configuration["Discord:ClientId"] },
                { "client_secret", _configuration["Discord:ClientSecret"] },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _configuration["Discord:RedirectUri"] } 
            };

            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestBody));

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DiscordTokenResponse>(responseContent);
        }

        private async Task<DiscordUser> GetDiscordUser(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            var userUrl = "https://discord.com/api/users/@me";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.GetAsync(userUrl);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DiscordUser>(responseContent);
        }
        
        private string GenerateGateToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("purpose", "gate") }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = GateAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool ValidateGateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = GateAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(30)
                }, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwt(DiscordUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class DiscordTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
