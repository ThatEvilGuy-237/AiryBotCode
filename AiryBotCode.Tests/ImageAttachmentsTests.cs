using AiryBotCode.Application.Hive;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ImageAttachmentsTests
    {
        [Fact]
        public void Picks_images_by_content_type()
        {
            var imgs = ImageAttachments.Pick(new (string?, string?, string?)[]
            {
                ("https://cdn/x.png", "x.png", "image/png"),
                ("https://cdn/doc.pdf", "doc.pdf", "application/pdf"),
            });
            Assert.Single(imgs);
            Assert.Equal("https://cdn/x.png", imgs[0].Url);
            Assert.Equal("x.png", imgs[0].Name);
            Assert.Equal("image/png", imgs[0].Mime);
        }

        [Fact]
        public void Falls_back_to_extension_when_content_type_missing()
        {
            var imgs = ImageAttachments.Pick(new (string?, string?, string?)[]
            {
                ("https://cdn/a.jpeg", "a.jpeg", null),
                ("https://cdn/b.webp", "b.webp", ""),
                ("https://cdn/c.txt", "c.txt", null),   // not an image
            });
            Assert.Equal(2, imgs.Count);
            Assert.Equal("image/jpeg", imgs[0].Mime);
            Assert.Equal("image/webp", imgs[1].Mime);
        }

        [Fact]
        public void Skips_entries_with_no_url_and_handles_null_input()
        {
            Assert.Empty(ImageAttachments.Pick(new (string?, string?, string?)[] { ("", "x.png", "image/png") }));
            Assert.Empty(ImageAttachments.Pick(null!));
        }

        [Fact]
        public void Names_unnamed_image()
        {
            var imgs = ImageAttachments.Pick(new (string?, string?, string?)[] { ("https://cdn/x", null, "image/png") });
            Assert.Single(imgs);
            Assert.Equal("image", imgs[0].Name);
        }
    }
}
