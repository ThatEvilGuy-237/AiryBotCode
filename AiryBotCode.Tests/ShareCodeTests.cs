using AiryBotCode.Application.Services;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ShareCodeTests
    {
        [Fact]
        public void Generate_is_long_url_safe_and_unique()
        {
            var a = ShareCode.Generate();
            var b = ShareCode.Generate();

            // 32 bytes → ~43 url-safe base64 chars. High entropy, not enumerable.
            Assert.True(a.Length >= 40, $"expected >=40 chars, got {a.Length}");
            Assert.DoesNotContain('+', a);
            Assert.DoesNotContain('/', a);
            Assert.DoesNotContain('=', a);
            Assert.NotEqual(a, b);
        }

        [Fact]
        public void Matches_only_the_exact_code()
        {
            var code = ShareCode.Generate();

            Assert.True(ShareCode.Matches(code, code));
            Assert.False(ShareCode.Matches(code, code + "x"));   // longer
            Assert.False(ShareCode.Matches(code, code[..^1]));   // shorter
            Assert.False(ShareCode.Matches(code, "totally-wrong"));
        }

        [Fact]
        public void Matches_rejects_null_or_empty()
        {
            var code = ShareCode.Generate();

            Assert.False(ShareCode.Matches(code, null));
            Assert.False(ShareCode.Matches(code, ""));
            Assert.False(ShareCode.Matches(null, code));
            Assert.False(ShareCode.Matches("", code));
            Assert.False(ShareCode.Matches(null, null));
        }
    }
}
