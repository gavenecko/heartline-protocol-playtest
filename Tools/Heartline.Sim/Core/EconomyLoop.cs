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
}
