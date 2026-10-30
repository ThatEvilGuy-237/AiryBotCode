using Evil.Sdk;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    /// <summary>
    /// Control-plane actions for the bot. The API and bot are separate processes
    /// sharing the database, so a reload is requested by bumping a control row;
    /// the bot polls it and restarts itself (it runs under Docker `restart: always`).
    /// </summary>
    [EvilAuthorize]
    [ApiController]
    [Route("[controller]")]
    public class BotController : ControllerBase
    {
        private readonly ICommandSettingsRepository _repository;

        public BotController(ICommandSettingsRepository repository)
        {
            _repository = repository;
        }

        // POST /api/bot/reload -> signal the bot to do a full restart.
        [HttpPost("reload")]
        public async Task<IActionResult> Reload()
        {
            // The value is a fresh timestamp; the bot restarts when it sees a value
            // newer than the one it started with.
            await _repository.SetControlValueAsync("restart", DateTime.UtcNow.ToString("O"));
            return Accepted(new { requested = true });
        }
    }
}
