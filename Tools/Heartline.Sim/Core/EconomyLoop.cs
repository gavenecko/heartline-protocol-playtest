using System;
using System.Collections.Generic;

namespace Heartline.Sim
{
    /// <summary>Mirrors Heartline.Shared EconomyRegen / GiftShop / GachaMath / JobMath for CI.</summary>
    public static class EconomyRegen
    {
        public const int StaminaCap = 120;
        public const int HeartEnergyCap = 60;
        public const int StaminaRegenMinutes = 6;
        public const int HeartRegenMinutes = 8;
        public static int StaminaSecondsPerPoint => StaminaRegenMinutes * 60;
        public static int HeartSecondsPerPoint => HeartRegenMinutes * 60;

        public static int Apply(ref int value, ref long lastUtc, long nowUtc, int cap, int secondsPerPoint)
        {
            if (secondsPerPoint <= 0) return 0;
            if (value >= cap)
            {
                lastUtc = nowUtc;
                return 0;
            }
            var elapsed = nowUtc - lastUtc;
            if (elapsed < secondsPerPoint) return 0;
            var points = (int)(elapsed / secondsPerPoint);
            var before = value;
            value = Math.Min(cap, value + points);
            lastUtc += points * (long)secondsPerPoint;
            if (value >= cap) lastUtc = nowUtc;
            return value - before;
        }

        public static int ApplyStamina(ref int stamina, ref long lastUtc, long nowUtc) =>
            Apply(ref stamina, ref lastUtc, nowUtc, StaminaCap, StaminaSecondsPerPoint);

        public static int ApplyHeart(ref int heart, ref long lastUtc, long nowUtc) =>
            Apply(ref heart, ref lastUtc, nowUtc, HeartEnergyCap, HeartSecondsPerPoint);
    }

    public static class GiftShop
    {
        public const int SunwellTeaPriceLc = 500;

        public static bool TryBuy(string giftId, ref int lumaCoins, ref int stockCount, out string error)
        {
            error = null;
            int price = giftId == "sunwell_tea_t1" ? SunwellTeaPriceLc : 500;
            if (lumaCoins < price)
            {
                error = $"Need {price} LC";
                return false;
            }
            lumaCoins -= price;
            stockCount++;
            return true;
        }
    }

    public enum GachaRarity { N, R, SR, SSR }

    public static class GachaMath
    {
        public const double RateSsr = 0.01;
        public const double RateSr = 0.10;
        public const double RateR = 0.40;
        public const int SoftPitySr = 10;
        public const int SoftPitySsr = 30;
        public const int HardPitySsr = 40;
        public const int CrystalCostTen = 1600;
        public const int TicketCostTen = 10;
        public const int FragSsr = 50;
        public const int FragSr = 20;
        public const int FragR = 5;
        public const int FragN = 1;

        public static GachaRarity Roll(Random rng, ref int pitySr, ref int pitySsr)
        {
            pitySr++;
            pitySsr++;

            GachaRarity rarity;
            if (pitySsr >= HardPitySsr || pitySsr >= SoftPitySsr)
                rarity = GachaRarity.SSR;
            else if (pitySr >= SoftPitySr)
                rarity = rng.NextDouble() < 0.1 ? GachaRarity.SSR : GachaRarity.SR;
            else
            {
                double roll = rng.NextDouble();
                if (roll < RateSsr) rarity = GachaRarity.SSR;
                else if (roll < RateSsr + RateSr) rarity = GachaRarity.SR;
                else if (roll < RateSsr + RateSr + RateR) rarity = GachaRarity.R;
                else rarity = GachaRarity.N;
            }

            if (rarity == GachaRarity.SSR)
            {
                pitySsr = 0;
                pitySr = 0;
            }
            else if (rarity == GachaRarity.SR)
                pitySr = 0;
            return rarity;
        }

        public static int FragmentsFor(GachaRarity rarity) => rarity switch
        {
            GachaRarity.SSR => FragSsr,
            GachaRarity.SR => FragSr,
            GachaRarity.R => FragR,
            _ => FragN
        };

        public static int ApplyResult(string agentId, GachaRarity rarity, ICollection<string> unlocked, IDictionary<string, int> fragments)
        {
            if (agentId.StartsWith("n_", StringComparison.Ordinal))
                return FragmentsFor(GachaRarity.N);
            if (unlocked == null) return 0;
            if (!unlocked.Contains(agentId))
            {
                unlocked.Add(agentId);
                return 0;
            }
            int frags = FragmentsFor(rarity);
            if (fragments != null)
            {
                fragments.TryGetValue(agentId, out var cur);
                fragments[agentId] = cur + frags;
            }
            return frags;
        }

        public static bool TrySpendTenPull(ref int beginnerTickets, ref int pulseCrystals, bool useTickets, out string error)
        {
            error = null;
            if (useTickets)
            {
                if (beginnerTickets < TicketCostTen)
                {
                    error = "tickets";
                    return false;
                }
                beginnerTickets -= TicketCostTen;
                return true;
            }
            if (pulseCrystals < CrystalCostTen)
            {
                error = "crystals";
                return false;
            }
            pulseCrystals -= CrystalCostTen;
            return true;
        }
    }

    public sealed class JobDefinition
    {
        public string Id;
        public string DisplayName;
        public int DurationMinutes;
        public int YieldLumaCoins;
        public int DebugAcceleratedSeconds;
        public string PreferredAgentId;
        public float PreferredBonusMul = 1.2f;
    }

    public static class JobMath
    {
        public const float DefaultPreferredBonus = 1.2f;

        public static readonly JobDefinition[] Catalog =
        {
            new JobDefinition { Id = "cafe_shift", DisplayName = "Café Shift", DurationMinutes = 120, YieldLumaCoins = 1200, DebugAcceleratedSeconds = 10, PreferredAgentId = "wren", PreferredBonusMul = DefaultPreferredBonus },
            new JobDefinition { Id = "pier_patrol", DisplayName = "Pier Patrol", DurationMinutes = 60, YieldLumaCoins = 600, DebugAcceleratedSeconds = 8, PreferredAgentId = "ivy", PreferredBonusMul = DefaultPreferredBonus },
            new JobDefinition { Id = "clinic_assist", DisplayName = "Clinic Assist", DurationMinutes = 90, YieldLumaCoins = 900, DebugAcceleratedSeconds = 9, PreferredAgentId = "juniper", PreferredBonusMul = DefaultPreferredBonus }
        };

        public static JobDefinition Get(string jobId)
        {
            foreach (var j in Catalog)
                if (j.Id == jobId) return j;
            return null;
        }

        public static int ComputePayout(JobDefinition job, string assignedAgentId)
        {
            if (job == null) return 0;
            if (!string.IsNullOrEmpty(job.PreferredAgentId) &&
                string.Equals(assignedAgentId, job.PreferredAgentId, StringComparison.OrdinalIgnoreCase))
                return (int)Math.Round(job.YieldLumaCoins * job.PreferredBonusMul);
            return job.YieldLumaCoins;
        }

        public static bool IsReady(string completeAtIso, DateTime utcNow)
        {
            if (string.IsNullOrEmpty(completeAtIso)) return false;
            if (!DateTime.TryParse(completeAtIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var done))
                return false;
            return utcNow >= done;
        }
    }

    /// <summary>Mirrors Heartline.Shared.GrowthMath.</summary>
    public static class GrowthMath
    {
        public const int MaxStars = 5;

        public static int XpToNextLevel(int level)
        {
            if (level < 1) level = 1;
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
            return rarity.Trim().ToUpperInvariant() switch
            {
                "N" => 1,
                "R" => 2,
                "SR" => 3,
                "SSR" => 4,
                "UR" => 5,
                _ => 1
            };
        }

        public static string FormatStars(int stars)
        {
            if (stars < 1) stars = 1;
            if (stars > 5) stars = 5;
            return new string('★', stars);
        }

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
            switch (fromStars)
            {
                case 1: fragments = 20; lumaCoins = 5_000; minAffRank = 0; return true;
                case 2: fragments = 50; lumaCoins = 20_000; minAffRank = 0; return true;
                case 3: fragments = 100; lumaCoins = 80_000; minAffRank = 2; return true;
                case 4: fragments = 200; lumaCoins = 250_000; minAffRank = 4; return true;
                default: return false;
            }
        }

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
                error = "max";
                return false;
            }
            if (!TryGetPromoteCost(stars, out int needFrag, out int needLuma, out int minAff))
            {
                error = "cost";
                return false;
            }
            if (affRank < minAff)
            {
                error = "aff";
                return false;
            }
            if (fragments < needFrag)
            {
                error = "frags";
                return false;
            }
            if (lumaCoins < needLuma)
            {
                error = "luma";
                return false;
            }
            fragments -= needFrag;
            lumaCoins -= needLuma;
            stars++;
            newLevelCap = LevelCapForStars(stars);
            return true;
        }
    }

    public sealed class DailyMissionDef
    {
        public string Id;
        public string Title;
        public int Target;
        public int RewardLuma;
        public int RewardCrystals;
        public int RewardTickets;
    }

    public static class DailyMissionMath
    {
        public static readonly DailyMissionDef[] Catalog =
        {
            new DailyMissionDef { Id = "clear_stage", Title = "Clear any stage", Target = 1, RewardLuma = 500 },
            new DailyMissionDef { Id = "room_talk", Title = "Talk in Mira's Room", Target = 1, RewardLuma = 400 },
            new DailyMissionDef { Id = "claim_job", Title = "Claim a Job payout", Target = 1, RewardLuma = 300, RewardCrystals = 50, RewardTickets = 1 }
        };

        public static string TodayUtc(DateTime utcNow) => utcNow.ToString("yyyy-MM-dd");

        public static DailyMissionDef Get(string id)
        {
            foreach (var m in Catalog)
                if (m.Id == id) return m;
            return null;
        }

        public static bool EnsureDay(ref string dailyUtcDate, IDictionary<string, int> progress, ICollection<string> claimed, DateTime utcNow)
        {
            string today = TodayUtc(utcNow);
            if (dailyUtcDate == today) return false;
            dailyUtcDate = today;
            progress?.Clear();
            claimed?.Clear();
            return true;
        }

        public static int GetProgress(IDictionary<string, int> progress, string id)
        {
            if (progress == null || string.IsNullOrEmpty(id)) return 0;
            return progress.TryGetValue(id, out var n) ? n : 0;
        }

        public static void AddProgress(IDictionary<string, int> progress, string id, int amount, int target)
        {
            if (progress == null || string.IsNullOrEmpty(id) || amount <= 0) return;
            progress.TryGetValue(id, out var cur);
            int next = cur + amount;
            if (target > 0 && next > target) next = target;
            progress[id] = next;
        }

        public static bool IsComplete(IDictionary<string, int> progress, DailyMissionDef def) =>
            def != null && GetProgress(progress, def.Id) >= def.Target;

        public static bool IsClaimed(ICollection<string> claimed, string id) =>
            claimed != null && !string.IsNullOrEmpty(id) && claimed.Contains(id);

        public static bool TryClaim(
            string id,
            IDictionary<string, int> progress,
            ICollection<string> claimed,
            ref int lumaCoins,
            ref int pulseCrystals,
            ref int beginnerTickets,
            out string error)
        {
            error = null;
            var def = Get(id);
            if (def == null) { error = "unknown"; return false; }
            if (IsClaimed(claimed, id)) { error = "claimed"; return false; }
            if (!IsComplete(progress, def)) { error = "incomplete"; return false; }
            lumaCoins += def.RewardLuma;
            pulseCrystals += def.RewardCrystals;
            beginnerTickets += def.RewardTickets;
            claimed.Add(id);
            return true;
        }

        public static void Totals(out int luma, out int crystals, out int tickets)
        {
            luma = crystals = tickets = 0;
            foreach (var m in Catalog)
            {
                luma += m.RewardLuma;
                crystals += m.RewardCrystals;
                tickets += m.RewardTickets;
            }
        }
    }
}
