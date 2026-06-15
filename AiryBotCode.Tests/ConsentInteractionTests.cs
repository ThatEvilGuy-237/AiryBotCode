using AiryBotCode.Application.Consent;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ConsentInteractionTests
    {
        [Fact]
        public void Accept_id_round_trips()
        {
            var id = ConsentInteraction.BuildAcceptId(1318870826862379018, 405431299323461634);
            var parsed = ConsentInteraction.TryParseAcceptId(id);
            Assert.NotNull(parsed);
            Assert.Equal(1318870826862379018UL, parsed!.Value.BotId);
            Assert.Equal(405431299323461634UL, parsed.Value.UserId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("button:other:1:2")]
        [InlineData("consent:accept:1")]            // too few parts
        [InlineData("consent:accept:notanid:2")]    // non-numeric
        public void Foreign_or_malformed_ids_are_rejected(string? customId)
        {
            Assert.Null(ConsentInteraction.TryParseAcceptId(customId));
        }

        [Fact]
        public void Prompt_states_the_required_notice_and_pings_owner()
        {
            var text = ConsentInteraction.PromptText(405431299323461634);
            Assert.Contains("only to improve your experience with Airy", text);
            Assert.Contains("never used for any", text);   // no AI training
            Assert.Contains("<@405431299323461634>", text);
            Assert.Contains("Accept", text);
        }

        [Fact]
        public void Prompt_omits_ping_when_owner_unset()
        {
            var text = ConsentInteraction.PromptText(0);
            Assert.DoesNotContain("<@", text);
        }
    }
}
