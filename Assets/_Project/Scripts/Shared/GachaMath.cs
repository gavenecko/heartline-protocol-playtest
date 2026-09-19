using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    public enum GachaRarity { N, R, SR, SSR }

    /// <summary>Beginner banner soft/hard pity + rates (GDD / economy notes).</summary>
    public static class GachaMath
    {
        public const double RateSsr = 0.01;
        public const double RateSr = 0.10;
        public const double RateR = 0.40;
        // N = 0.49 remainder

        public const int SoftPitySr = 10;
        public const int SoftPitySsr = 30;
        public const int HardPitySsr = 40;
        public const int CrystalCostSingle = 160;
        public const int CrystalCostTen = 1600;
        public const int TicketCostTen = 10;

        public const int FragSsr = 50;
        public const int FragSr = 20;
        public const int FragR = 5;
        public const int FragN = 1;

        public static string RatesUiText() =>
            "Beginner Banner (local)\n" +
            "SSR 1% · SR 10% · R 40% · N 49%\n" +
            $"Soft pity: SR@{SoftPitySr} · SSR@{SoftPitySsr} · hard SSR@{HardPitySsr}\n" +
            $"Cost: {TicketCostTen} Echo Tickets OR {CrystalCostTen} Pulse Crystals (10-pull)\n" +
            "Free sample 10-pull once. Duplicate → Echo Fragments. No IAP.";

        /// <summary>
        /// Resolve one pull. pitySrCounts / pitySsrCounts = pulls since last SR/SSR (incremented each pull).
        /// Soft: at SoftPitySr force ≥SR if no SR/SSR yet in counter; at SoftPitySsr force ≥SSR;
        /// Hard: at HardPitySsr force SSR.
        /// </summary>
        public static GachaRarity Roll(Random rng, ref int pitySr, ref int pitySsr)
        {
            pitySr++;
            pitySsr++;

            GachaRarity rarity;
            if (pitySsr >= HardPitySsr)
                rarity = GachaRarity.SSR;
            else if (pitySsr >= SoftPitySsr)
                rarity = GachaRarity.SSR; // soft guarantee path
            else if (pitySr >= SoftPitySr)
            {
                // Force at least SR (50/50 SR vs SSR among forced)
                rarity = rng.NextDouble() < 0.1 ? GachaRarity.SSR : GachaRarity.SR;
            }
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
            {
                pitySr = 0;
            }
            return rarity;
        }

        public static int FragmentsFor(GachaRarity rarity) => rarity switch
        {
            GachaRarity.SSR => FragSsr,
            GachaRarity.SR => FragSr,
            GachaRarity.R => FragR,
            _ => FragN
        };

        public static string PickAgent(GachaRarity rarity, Random rng)
        {
            return rarity switch
            {
                GachaRarity.SSR => "mira",
                GachaRarity.SR => rng.Next(2) == 0 ? "wren" : "wren",
                GachaRarity.R => rng.Next(2) == 0 ? "ivy" : "juniper",
                _ => "n_filler_" + (char)('a' + rng.Next(4))
            };
        }

        /// <summary>Apply unlock or fragment conversion. Returns fragment delta (0 if new unlock).</summary>
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
                    error = $"Need {TicketCostTen} Echo Tickets (have {beginnerTickets}).";
                    return false;
                }
                beginnerTickets -= TicketCostTen;
                return true;
            }
            if (pulseCrystals < CrystalCostTen)
            {
                error = $"Need {CrystalCostTen} Pulse Crystals (have {pulseCrystals}).";
                return false;
            }
            pulseCrystals -= CrystalCostTen;
            return true;
        }
    }
}
