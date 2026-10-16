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
        public void A_tenor_gif_reads_as_a_gif_with_its_words_and_comes_as_its_still_frame()
        {
            var raw = "when the build fails https://tenor.com/view/facepalm-face-gif-12345678 <:evil_merc:1292212189163163719>";
            EmbedSeen[] embeds = [new("Gifv", "https://tenor.com/view/facepalm-face-gif-12345678", "https://media.tenor.com/0SyZFIR7vv8AAAAe/facepalm-face.png")];
            Assert.Equal("when the build fails [gif: facepalm face] :evil_merc:", ChatPictures.Readable(raw, [], embeds));
            var picked = ChatPictures.Pick(raw, [], embeds);
            Assert.Equal(["[gif: facepalm face]", ":evil_merc:"], picked.Select(p => p.Label));
            Assert.Equal("https://media.tenor.com/0SyZFIR7vv8AAAAe/facepalm-face.png", picked[0].Image.Url);
            Assert.Equal("image/png", picked[0].Image.Mime);
        }

        [Fact]
        public void A_gif_link_without_a_preview_still_reads_as_a_gif()
        {
            Assert.Equal("[gif: cat dance]", ChatPictures.Readable("https://tenor.com/nl/view/cat-dance-gif-998877", []));
            Assert.Equal("[gif: happy dog]", ChatPictures.Readable("https://giphy.com/gifs/happy-dog-xT9IgG50Fb7Mi0prBC", []));
            Assert.Equal("[gif]", ChatPictures.Readable("https://media.example.org/x/funny.gif", []));
            Assert.Empty(ChatPictures.Pick("https://tenor.com/view/cat-dance-gif-998877", [], []));
            Assert.True(ChatPictures.HasLink("look https://tenor.com/view/cat-dance-gif-998877"));
            Assert.False(ChatPictures.HasLink("no link here"));
        }

        [Fact]
        public void An_image_link_comes_as_a_picture_and_other_links_stay_as_they_are()
        {
            var raw = "see https://i.example.org/pic.jpg and https://example.org/article";
            EmbedSeen[] embeds = [new("Image", "https://i.example.org/pic.jpg", "https://i.example.org/pic.jpg"), new("Article", "https://example.org/article", "https://example.org/og.png")];
            Assert.Equal("see [image] and https://example.org/article", ChatPictures.Readable(raw, [], embeds));
            var picked = Assert.Single(ChatPictures.Pick(raw, [], embeds));
            Assert.Equal("[image]", picked.Label);
            Assert.Equal("image/jpeg", picked.Image.Mime);
        }

        [Fact]
        public void Joining_in_shows_the_stretch_and_says_nobody_pinged()
        {
            HeardLine[] stretch = [new(1, "kirra", "anyone watched the new audio?", false), new(2, "tomas", "not yet https://tenor.com/view/cat-dance-gif-1", false)];
            var said = ChannelTalk.JoinIn("Airy", "events", "tomas (7)", stretch);
            Assert.StartsWith("Said in #events since Airy last spoke, oldest first:\nkirra: anyone watched the new audio?\ntomas: not yet", said);
            Assert.Contains("Nobody pinged you; you chose to join in.", said);
            Assert.EndsWith("Last to write was tomas (7).", said);
            Assert.DoesNotContain("{{", said);
        }

        [Fact]
        public void A_message_that_names_her_reminds_her_it_is_about_her()
        {
            var said = ChannelTalk.Message("Airy", "events", "Evil (42)", "she is the mark 3 of airy development", [], null, aboutYou: true);
            Assert.Contains("she is the mark 3 of airy development\n\nHere Airy, the fox and the bot mean you", said);
            Assert.DoesNotContain("Here Airy", ChannelTalk.Message("Airy", "events", "Evil (42)", "hi", []));
        }

        [Fact]
        public void The_message_says_which_pictures_are_the_emoji()
        {
            var said = ChannelTalk.Message("Airy", "events", "Evil (42)", "I am :evil_merc:", [], [":evil_merc:"]);
            Assert.StartsWith("Evil (42) writes in #events:\nI am :evil_merc:", said);
            Assert.Contains("in this order: :evil_merc:. They are reactions, not shared photos", said);
            Assert.DoesNotContain("{{", said);
            Assert.DoesNotContain("pictures", ChannelTalk.Message("Airy", "events", "Evil (42)", "hi", []));
        }
    }
}
