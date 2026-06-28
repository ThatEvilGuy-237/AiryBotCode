using System;
using System.Linq;
using System.Threading.Tasks;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Domain.database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiryBotCode.Api.Controllers
{
    // Team feature-suggestions board. Anyone logged into the panel can submit an
    // idea; the maintainer later adds a review (why it's good + a time estimate)
    // and the team can mark it approved. All endpoints require a JWT.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionRepository _repository;
        private readonly IShareCodeService _shareCode;

        public SuggestionsController(ISuggestionRepository repository, IShareCodeService shareCode)
        {
            _repository = repository;
            _shareCode = shareCode;
        }

        public class CreateSuggestionRequest
        {
            public string? Title { get; set; }
            public string? Body { get; set; }
            public string? SubmittedBy { get; set; }
        }

        public class RespondRequest
        {
            public string? Why { get; set; }
            public string? Estimate { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repository.GetAllAsync();
            return Ok(all.Select(ToDto));
        }

        // Submit a new idea. SubmittedBy defaults to the logged-in user's name.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSuggestionRequest body)
        {
            var title = body?.Title?.Trim();
            var text = body?.Body?.Trim();
            if (string.IsNullOrWhiteSpace(title)) return BadRequest("A title is required.");
            if (string.IsNullOrWhiteSpace(text)) return BadRequest("A description is required.");

            var who = !string.IsNullOrWhiteSpace(body?.SubmittedBy)
                ? body!.SubmittedBy!.Trim()
                : (User?.Identity?.Name ?? "anonymous");

            var entity = new Suggestion
            {
                Title = title.Length > 200 ? title[..200] : title,
                Body = text,
                SubmittedBy = who,
                CreatedAt = DateTime.UtcNow,
                Status = "new",
            };
            var saved = await _repository.AddAsync(entity);
            return StatusCode(201, ToDto(saved));
        }

        // ── Share link (capability code) management ─────────────────────────────
        // The public board lives at /suggest/{code}; the SPA builds the absolute URL
        // from its own origin, so we hand back the code + the relative path.
        [HttpGet("sharelink")]
        public async Task<IActionResult> GetShareLink()
        {
            var code = await _shareCode.GetOrCreateAsync();
            return Ok(new { code, sharePath = $"/suggest/{code}" });
        }

        // Mint a fresh code — instantly invalidates the previous share link.
        [HttpPost("sharelink/regenerate")]
        public async Task<IActionResult> RegenerateShareLink()
        {
            var code = await _shareCode.RegenerateAsync();
            return Ok(new { code, sharePath = $"/suggest/{code}" });
        }

        // Maintainer review: why it's a good idea + a rough time estimate.
        [HttpPut("{id:int}/respond")]
        public async Task<IActionResult> Respond(int id, [FromBody] RespondRequest body)
        {
            var entity = await _repository.GetAsync(id);
            if (entity == null) return NotFound();

            entity.ResponseWhy = body?.Why?.Trim();
            entity.ResponseEstimate = body?.Estimate?.Trim();
            entity.RespondedAt = DateTime.UtcNow;
            // Reviewing doesn't auto-approve; keep an approved row approved.
            if (!entity.Approved && entity.Status != "rejected") entity.Status = "reviewed";

            await _repository.UpdateAsync(entity);
            return Ok(ToDto(entity));
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var entity = await _repository.GetAsync(id);
            if (entity == null) return NotFound();
            entity.Approved = true;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.Status = "approved";
            await _repository.UpdateAsync(entity);
            return Ok(ToDto(entity));
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var entity = await _repository.GetAsync(id);
            if (entity == null) return NotFound();
            entity.Approved = false;
            entity.ApprovedAt = null;
            entity.Status = "rejected";
            await _repository.UpdateAsync(entity);
            return Ok(ToDto(entity));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repository.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        private static object ToDto(Suggestion s) => new
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
