using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    [Serializable]
    public class EconomyConstants
    {
        public int staminaCap = 120;
        public int staminaRegenMinutes = 6;
        public int heartEnergyCap = 60;
        public int heartEnergyRegenMinutes = 8;
        public WalletSeed startingWallet = new WalletSeed();
        public RoomEconomy room = new RoomEconomy();
        public Dictionary<string, int> stageCosts = new Dictionary<string, int>();
        public List<ComboTier> comboMultipliers = new List<ComboTier>();
        public SyncGaugeConfig syncGauge = new SyncGaugeConfig();
        public Dictionary<string, LaneModifier> laneModifiers = new Dictionary<string, LaneModifier>();
        public JobsConfig jobs = new JobsConfig();
        public float debugStaminaRegenMultiplier = 10f;

        public float ComboMultiplier(int combo)
        {
            float best = 1f;
            if (comboMultipliers == null) return best;
            foreach (var t in comboMultipliers)
                if (combo >= t.minCombo && t.multiplier >= best)
                    best = t.multiplier;
            return best;
        }
    }

    [Serializable]
    public class WalletSeed
    {
        public int stamina = 120;
        public int heartEnergy = 60;
        public int lumaCoins = 5000;
        public int pulseCrystals = 1600;
        public int beginnerTickets = 10;
        public int standardTickets = 0;
    }

    [Serializable]
    public class RoomEconomy
    {
        public int talkCostHe = 5;
        public int talkAffXp = 20;
        public int giftCostHe = 8;
        public int giftAffXpBase = 40;
        public float giftPreferredMultiplier = 1.5f;
        public int dateCostHe = 15;
        public int dateAffXp = 100;
        public int datePerDay = 1;
        public int sleepoverAffXpMorning = 30;
        public int maxAffRankSlice = 3;

        public int RankForXp(int cumulativeXp)
        {
            int rank = 1;
            if (cumulativeXp >= 100) rank = 2;
            if (cumulativeXp >= 300) rank = 3;
            return Math.Min(rank, maxAffRankSlice);
        }

        public int XpToNextRank(int cumulativeXp)
        {
            int rank = RankForXp(cumulativeXp);
            if (rank >= maxAffRankSlice) return 0;
            int next = rank == 1 ? 100 : 300;
            return Math.Max(0, next - cumulativeXp);
        }
    }

    [Serializable]
    public class ComboTier
    {
        public int minCombo;
        public float multiplier;
    }

    [Serializable]
    public class SyncGaugeConfig
    {
        public float basicThreshold = 0.20f;
        public float burstThreshold = 1.0f;
        public int burstLockTurns = 1;
        public float attunedFillPerGem = 0.04f;
        public float prismFillPerGem = 0.05f;
        public float offColorFillPerGem = 0.01f;
    }

    [Serializable]
    public class LaneModifier
    {
        public float damageDealt = 1f;
        public float damageTaken = 1f;
        public float healBonus = 1f;
    }

    [Serializable]
    public class JobsConfig
    {
        public JobDef cafe_shift = new JobDef();
    }

    [Serializable]
    public class JobDef
    {
        public int durationMinutes = 120;
        public int yieldLumaCoins = 1200;
        public int debugAcceleratedSeconds = 10;
    }
}
