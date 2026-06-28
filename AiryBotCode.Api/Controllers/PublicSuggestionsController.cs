using System;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AiryBotCode.Api.Controllers
{
    // PUBLIC, capability-gated view of the suggestions board. The code in the URL is
    // the ONLY credential: it is validated server-side on EVERY request, and a bad or
    // missing code returns 404 (we don't even confirm the board exists, so the URL
    // can't be probed). Grants create + read ONLY — approval/respond/delete stay on the
    // JWT-gated SuggestionsController. [AllowAnonymous] because there is no auth here;
    // the code is the auth.
    [ApiController]
    [AllowAnonymous]
    [Route("api/public/suggestions")]
    public class PublicSuggestionsController : ControllerBase
    {
        // Caps on the unauthenticated create surface (reject, don't silently truncate).
        private const int MaxTitle = 200;
        private const int MaxBody = 4000;
        private const int MaxSubmittedBy = 80;

        private readonly ISuggestionRepository _repository;
        private readonly IShareCodeService _shareCode;

        public PublicSuggestionsController(ISuggestionRepository repository, IShareCodeService shareCode)
        {
            _repository = repository;
            _shareCode = shareCode;
        }

        public class CreateRequest
        {
            public string? Title { get; set; }
            public string? Body { get; set; }
            public string? SubmittedBy { get; set; }
        }

        // Read the board. Invalid code → 404 (don't confirm the board exists).
        [HttpGet("{code}")]
        public async Task<IActionResult> List(string code)
        {
            if (!await _shareCode.ValidateAsync(code)) return NotFound();

            var all = await _repository.GetAllAsync();
            return Ok(all.Select(ToPublicDto));
        }

        // Submit an idea via the share link. Invalid code → 404. Caps + trim.
        // Per-IP rate limited (5/min) to blunt spam on the unauthenticated surface.
        [HttpPost("{code}")]
        [EnableRateLimiting("public-suggest-create")]
        public async Task<IActionResult> Create(string code, [FromBody] CreateRequest body)
        {
            if (!await _shareCode.ValidateAsync(code)) return NotFound();

            var title = body?.Title?.Trim();
            var text = body?.Body?.Trim();
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("A title is required.");
            if (string.IsNullOrWhiteSpace(text)) return BadRequest("A description is required.");
            if (title.Length > MaxTitle) return BadRequest($"Title too long (max {MaxTitle}).");
            if (text.Length > MaxBody) return BadRequest($"Description too long (max {MaxBody}).");

            var who = !string.IsNullOrWhiteSpace(body?.SubmittedBy) ? body!.SubmittedBy!.Trim() : "anonymous";
            if (who.Length > MaxSubmittedBy) who = who[..MaxSubmittedBy];

            var entity = new Suggestion
            {
                Title = title,
                Body = text,
                SubmittedBy = who,
                CreatedAt = DateTime.UtcNow,
                Status = "new",
            };
            var saved = await _repository.AddAsync(entity);
            return StatusCode(201, ToPublicDto(saved));
        }

        // Public projection: the (non-sensitive) board data + review responses so a
        // submitter can see their idea's status — but no admin actions are exposed here.
        private static object ToPublicDto(Suggestion s) => new
        {
            id = s.Id,
            title = s.Title,
            body = s.Body,
            submittedBy = s.SubmittedBy,
            createdAt = s.CreatedAt,
            status = s.Status,
            responseWhy = s.ResponseWhy,
            responseEstimate = s.ResponseEstimate,
            respondedAt = s.RespondedAt,
            approved = s.Approved,
            approvedAt = s.ApprovedAt,
        };
    }
}
