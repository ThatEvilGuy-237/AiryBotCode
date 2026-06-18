using AiryBotCode.Application.Features.Leveling;
using Xunit;

namespace AiryBotCode.Tests
{
    public class XpCooldownTests
    {
        private static readonly DateTimeOffset T0 = new(2026, 6, 16, 8, 0, 0, TimeSpan.Zero);

        [Fact]
        public void First_grant_allowed_second_within_window_blocked()
        {
            var c = new XpCooldown();
            Assert.True(c.TryConsume(1, 60, T0));
            Assert.False(c.TryConsume(1, 60, T0.AddSeconds(30)));
            Assert.False(c.TryConsume(1, 60, T0.AddSeconds(59)));
        }

        [Fact]
        public void Grant_allowed_again_after_the_window()
        {
            var c = new XpCooldown();
            Assert.True(c.TryConsume(1, 60, T0));
            Assert.True(c.TryConsume(1, 60, T0.AddSeconds(60)));
        }

        [Fact]
        public void Users_cool_down_independently()
        {
            var c = new XpCooldown();
            Assert.True(c.TryConsume(1, 60, T0));
            Assert.True(c.TryConsume(2, 60, T0.AddSeconds(1)));   // different user → allowed
            Assert.False(c.TryConsume(1, 60, T0.AddSeconds(2)));  // user 1 still cooling
        }
    }
}
