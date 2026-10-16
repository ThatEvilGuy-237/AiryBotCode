using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ChatPicturesTests
    {
        [Fact]
        public void A_custom_emoji_reads_as_its_name_and_comes_as_a_picture()
        {
            var raw = "I am <:evil_merc:1292212189163163719>";
            Assert.Equal("I am :evil_merc:", ChatPictures.Readable(raw, []));
            var picked = Assert.Single(ChatPictures.Pick(raw, []));
            Assert.Equal(":evil_merc:", picked.Label);
            Assert.Equal("https://cdn.discordapp.com/emojis/1292212189163163719.png?size=96", picked.Image.Url);
            Assert.Equal("image/png", picked.Image.Mime);
        }

        [Fact]
        public void Animated_and_repeated_emoji_come_once_and_at_most_four()
        {
            var raw = "<a:spin:1111111111111111111> <a:spin:1111111111111111111> <:a:2222222222222222222> <:bb:3333333333333333333> <:cc:4444444444444444444> <:dd:5555555555555555555>";
            Assert.StartsWith(":spin: :spin: <:a:", ChatPictures.Readable(raw, []));
            var picked = ChatPictures.Pick(raw, []);
            Assert.Equal([":spin:", ":bb:", ":cc:", ":dd:"], picked.Select(p => p.Label));
        }

        [Fact]
        public void Stickers_are_named_in_the_text_and_lottie_ones_get_no_picture()
        {
            StickerSeen[] stickers = [new(9000000000000000001, "wave", 1), new(9000000000000000002, "dance", 4), new(9000000000000000003, "vector", 3)];
            Assert.Equal("hi [sticker wave] [sticker dance] [sticker vector]", ChatPictures.Readable("hi", stickers));
            Assert.Equal("[sticker wave]", ChatPictures.Readable("", [stickers[0]]));
            var picked = ChatPictures.Pick("", stickers);
            Assert.Equal(["[sticker wave]", "[sticker dance]"], picked.Select(p => p.Label));
            Assert.Equal("https://media.discordapp.net/stickers/9000000000000000002.gif?size=160", picked[1].Image.Url);
            Assert.Equal("image/gif", picked[1].Image.Mime);
        }

        [Fact]
        public void The_message_says_which_pictures_are_the_emoji()
        {
            var said = ChannelTalk.Message("Airy", "events", "Evil (42)", "I am :evil_merc:", [], [":evil_merc:"]);
            Assert.StartsWith("Evil (42) writes in #events:\nI am :evil_merc:", said);
            Assert.EndsWith("in this order: :evil_merc:", said);
            Assert.DoesNotContain("{{", said);
            Assert.DoesNotContain("pictures", ChannelTalk.Message("Airy", "events", "Evil (42)", "hi", []));
        }
    }
}
