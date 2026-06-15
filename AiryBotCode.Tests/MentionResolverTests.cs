using AiryBotCode.Infrastructure.DiscordEvents;
using Xunit;

namespace AiryBotCode.Tests
{
    public class MentionResolverTests
    {
        // A fake directory: a couple of known users, everyone else unknown.
        private static string? Names(ulong id) => id switch
        {
            124654654000000000 => "Evil",
            222222222222222222 => "Kit",
            _ => null,
        };

        [Fact]
        public void Resolves_tagged_mention_to_name_and_id()
        {
            var outp = MentionResolver.Rewrite("hey <@124654654000000000> look", Names);
            Assert.Equal("hey Evil (124654654000000000) look", outp);
        }

        [Fact]
        public void Resolves_nickname_form_mention()
        {
            var outp = MentionResolver.Rewrite("<@!222222222222222222> ping", Names);
            Assert.Equal("Kit (222222222222222222) ping", outp);
        }

        [Fact]
        public void Unknown_id_falls_back_to_user_id_but_keeps_the_id()
        {
            var outp = MentionResolver.Rewrite("yo <@999999999999999999>", Names);
            Assert.Equal("yo user 999999999999999999", outp);
        }

        [Fact]
        public void Resolves_bare_snowflake_mention()
        {
            var outp = MentionResolver.Rewrite("tell @124654654000000000 hi", Names);
            Assert.Equal("tell Evil (124654654000000000) hi", outp);
        }

        [Fact]
        public void Does_not_touch_role_mentions()
        {
            var outp = MentionResolver.Rewrite("role <@&333333333333333333> stays", Names);
            Assert.Equal("role <@&333333333333333333> stays", outp);
        }

        [Fact]
        public void Does_not_touch_everyone_here_or_short_at_tokens()
        {
            Assert.Equal("@everyone @here costs @5 today",
                MentionResolver.Rewrite("@everyone @here costs @5 today", Names));
        }

        [Fact]
        public void Resolves_multiple_mentions_in_one_message()
        {
            var outp = MentionResolver.Rewrite("<@124654654000000000> and <@222222222222222222>", Names);
            Assert.Equal("Evil (124654654000000000) and Kit (222222222222222222)", outp);
        }

        [Fact]
        public void Empty_or_null_passes_through()
        {
            Assert.Equal("", MentionResolver.Rewrite("", Names));
            Assert.Equal("no mentions here", MentionResolver.Rewrite("no mentions here", Names));
        }
    }
}
