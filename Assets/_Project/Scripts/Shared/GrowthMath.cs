using System;

namespace Heartline.Shared
{
    /// <summary>
    /// Agent XP / level caps by stars + promote costs (GDD §9).
    /// Soft caps: ★40 · ★★50 · ★★★60 · ★★★★70 · ★★★★★80.
    /// </summary>
    public static class GrowthMath
    {
        public const int MaxStars = 5;
        public const int MinStars = 1;

        /// <summary>XP required to go from <paramref name="level"/> → level+1.</summary>
        public static int XpToNextLevel(int level)
        {
            if (level < 1) level = 1;
            // Curve tuned so Ch.1 farming lands ~Lv 28–35 by D7 (economy notes).
            return 40 + 12 * level;
        }

        public static int LevelCapForStars(int stars)
        {
            if (stars < 1) stars = 1;
            if (stars > 5) stars = 5;
            return stars switch
            {
                1 => 40,
                2 => 50,
                3 => 60,
                4 => 70,
                5 => 80,
                _ => 40
            };
        }

        public static int BaseStarsForRarity(string rarity)
        {
            if (string.IsNullOrEmpty(rarity)) return 1;
            switch (rarity.Trim().ToUpperInvariant())
            {
                case "N": return 1;
                case "R": return 2;
                case "SR": return 3;
                case "SSR": return 4;
                case "UR": return 5;
                default: return 1;
            }
        }

        public static string FormatStars(int stars)
        {
            if (stars < 1) stars = 1;
            if (stars > 5) stars = 5;
            return new string('★', stars);
        }

        /// <summary>
        /// Apply XP with soft level cap. Returns levels gained.
        /// Excess XP past cap is discarded (slice: no overflow bank).
        /// </summary>
        public static int ApplyXp(ref int level, ref int xp, int gain, int levelCap)
        {
            if (gain <= 0) return 0;
            if (levelCap < 1) levelCap = 1;
            if (level > levelCap) level = levelCap;
            if (level >= levelCap)
            {
                xp = 0;
                return 0;
            }

            xp += gain;
            int gained = 0;
            while (level < levelCap)
            {
                int need = XpToNextLevel(level);
                if (xp < need) break;
                xp -= need;
                level++;
                gained++;
            }
            if (level >= levelCap)
            {
                level = levelCap;
                xp = 0;
            }
            return gained;
        }

        public static string XpBar(int xp, int need, int width = 10)
        {
            if (need <= 0) need = 1;
            if (width < 4) width = 4;
            float t = Math.Clamp(xp / (float)need, 0f, 1f);
            int filled = (int)Math.Round(t * width);
            if (filled > width) filled = width;
            return "[" + new string('█', filled) + new string('░', width - filled) + "]";
        }

        public static bool TryGetPromoteCost(int fromStars, out int fragments, out int lumaCoins, out int minAffRank)
        {
            fragments = 0;
            lumaCoins = 0;
            minAffRank = 0;
            return fromStars switch
            {
                1 => Set(out fragments, 20, out lumaCoins, 5_000, out minAffRank, 0),
                2 => Set(out fragments, 50, out lumaCoins, 20_000, out minAffRank, 0),
                3 => Set(out fragments, 100, out lumaCoins, 80_000, out minAffRank, 2),
                4 => Set(out fragments, 200, out lumaCoins, 250_000, out minAffRank, 4),
                _ => false
            };
        }

        static bool Set(out int frags, int f, out int luma, int l, out int aff, int a)
        {
            frags = f;
            luma = l;
            aff = a;
            return true;
        }

        /// <summary>
        /// Spend Echo Fragments + Luma to raise stars by 1 when eligible.
        /// Promote mats are stubbed as free in the vertical slice.
        /// </summary>
        public static bool TryPromote(
            ref int stars,
            ref int lumaCoins,
            ref int fragments,
            int affRank,
            out int newLevelCap,
            out string error)
        {
            newLevelCap = LevelCapForStars(stars);
            error = null;
            if (stars >= MaxStars)
            {
                error = "Already max ★★★★★.";
                return false;
            }
            if (!TryGetPromoteCost(stars, out int needFrag, out int needLuma, out int minAff))
            {
                error = "Cannot promote from this star rank.";
                return false;
            }
            if (affRank < minAff)
            {
                error = $"Need Aff Rank ≥{minAff} (have {affRank}).";
                return false;
            }
            if (fragments < needFrag)
            {
                error = $"Need {needFrag} Echo Fragments (have {fragments}).";
                return false;
            }
            if (lumaCoins < needLuma)
            {
                error = $"Need {needLuma} LC (have {lumaCoins}).";
                return false;
            }
            fragments -= needFrag;
            lumaCoins -= needLuma;
            stars++;
            newLevelCap = LevelCapForStars(stars);
            return true;
        }
    }
}
