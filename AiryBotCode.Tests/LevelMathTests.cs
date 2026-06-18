using AiryBotCode.Application.Features.Leveling;
using Xunit;

namespace AiryBotCode.Tests
{
    public class LevelMathTests
    {
        // MEE6 defaults: 5·n² + 50·n + 100.
        private const long Base = 100, Lin = 50, Quad = 5;

        [Theory]
        [InlineData(0, 100)]   // 0→1
        [InlineData(1, 155)]   // 1→2
        [InlineData(2, 220)]   // 2→3
        [InlineData(3, 295)]   // 3→4
        [InlineData(4, 380)]   // 4→5
        public void XpForNext_matches_the_mee6_curve(int level, long expected)
        {
            Assert.Equal(expected, LevelMath.XpForNext(level, Base, Lin, Quad));
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(2, 255)]
        [InlineData(3, 475)]
        [InlineData(5, 1150)]   // 100+155+220+295+380
        public void TotalXpForLevel_is_the_cumulative_sum(int level, long expected)
        {
            Assert.Equal(expected, LevelMath.TotalXpForLevel(level, Base, Lin, Quad));
        }

        [Fact]
        public void Resolve_zero_xp_is_level_zero_with_full_first_tier_remaining()
        {
            var (level, into, needed) = LevelMath.Resolve(0, Base, Lin, Quad);
            Assert.Equal(0, level);
            Assert.Equal(0, into);
            Assert.Equal(100, needed);
        }

        [Fact]
        public void Resolve_exact_threshold_lands_on_the_new_level()
        {
            var (level, into, needed) = LevelMath.Resolve(1150, Base, Lin, Quad);
            Assert.Equal(5, level);
            Assert.Equal(0, into);
            Assert.Equal(LevelMath.XpForNext(5, Base, Lin, Quad), needed);   // 475
        }

        [Fact]
        public void Resolve_reports_progress_inside_a_level()
        {
            // One XP short of level 5 (needs 1150): level 4, almost full bar.
            var (level, into, needed) = LevelMath.Resolve(1149, Base, Lin, Quad);
            Assert.Equal(4, level);
            Assert.Equal(379, into);     // 1149 - 770 (total for L4)
            Assert.Equal(380, needed);   // XpForNext(4)
        }
    }
}
