namespace AiryBotCode.Application.Features.Leveling
{
    /// <summary>
    /// The MEE6-style quadratic level curve, made tunable. The XP to advance FROM
    /// <paramref name="level"/> to the next is <c>quad·level² + linear·level + baseXp</c>
    /// (defaults 5/50/100). Total XP is the cumulative sum, and the level for a given
    /// total XP is found by subtracting each tier's cost in turn.
    /// </summary>
    public static class LevelMath
    {
        public static long XpForNext(int level, long baseXp, long linear, long quad)
            => quad * (long)level * level + linear * level + baseXp;

        public static long TotalXpForLevel(int level, long baseXp, long linear, long quad)
        {
            long total = 0;
            for (int n = 0; n < level; n++)
                total += XpForNext(n, baseXp, linear, quad);
            return total;
        }

        /// <summary>
        /// Resolve a cumulative XP total into the current level, how much XP is banked
        /// into that level, and how much the next level costs.
        /// </summary>
        public static (int Level, long IntoLevel, long NeededForNext) Resolve(
            long xp, long baseXp, long linear, long quad)
        {
            if (xp < 0) xp = 0;
            int level = 0;
            long remaining = xp;
            long needed = XpForNext(0, baseXp, linear, quad);
            while (needed > 0 && remaining >= needed)
            {
                remaining -= needed;
                level++;
                needed = XpForNext(level, baseXp, linear, quad);
            }
            return (level, remaining, needed);
        }
    }
}
