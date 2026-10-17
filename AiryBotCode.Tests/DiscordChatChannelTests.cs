using AiryBotCode.Infrastructure.DiscordEvents;
using Xunit;

namespace AiryBotCode.Tests
{
    public class DiscordChatChannelTests
    {
        [Fact]
        public void A_short_reply_goes_out_as_one_message() =>
            Assert.Equal(["hello"], DiscordChatChannel.Split("hello"));

        [Fact]
        public void A_long_reply_breaks_on_lines_first_and_every_part_fits()
        {
            var text = string.Join("\n", Enumerable.Range(1, 60).Select(i => $"line {i} " + new string('x', 60)));
            var parts = DiscordChatChannel.Split(text).ToList();
            Assert.True(parts.Count > 1);
            Assert.All(parts, p => Assert.True(p.Length <= 2000));
            Assert.Equal(text, string.Join("\n", parts));
        }

        [Fact]
        public void One_endless_line_breaks_on_spaces_and_a_word_without_spaces_is_cut()
        {
            var words = string.Join(" ", Enumerable.Repeat("word", 1000));
            Assert.All(DiscordChatChannel.Split(words), p => Assert.True(p.Length <= 2000));
            Assert.Equal(words.Replace(" ", ""), string.Concat(DiscordChatChannel.Split(words)).Replace(" ", ""));
            var blob = new string('z', 4500);
            Assert.Equal([2000, 2000, 500], DiscordChatChannel.Split(blob).Select(p => p.Length));
        }
    }
}
