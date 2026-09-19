using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    public sealed class JobDefinition
    {
        public string Id;
        public string DisplayName;
        public int DurationMinutes;
        public int YieldLumaCoins;
        public int DebugAcceleratedSeconds;
        public string PreferredAgentId;
        public float PreferredBonusMul;
    }

    /// <summary>Timed idle jobs — Café + Pier Patrol + Clinic Assist.</summary>
    public static class JobMath
    {
        public const float DefaultPreferredBonus = 1.2f;

        public static readonly JobDefinition[] Catalog =
        {
            new JobDefinition
            {
                Id = "cafe_shift",
                DisplayName = "Café Shift",
                DurationMinutes = 120,
                YieldLumaCoins = 1200,
                DebugAcceleratedSeconds = 10,
                PreferredAgentId = "wren",
                PreferredBonusMul = DefaultPreferredBonus
            },
            new JobDefinition
            {
                Id = "pier_patrol",
                DisplayName = "Pier Patrol",
                DurationMinutes = 60,
                YieldLumaCoins = 600,
                DebugAcceleratedSeconds = 8,
                PreferredAgentId = "ivy",
                PreferredBonusMul = DefaultPreferredBonus
            },
            new JobDefinition
            {
                Id = "clinic_assist",
                DisplayName = "Clinic Assist",
                DurationMinutes = 90,
                YieldLumaCoins = 900,
                DebugAcceleratedSeconds = 9,
                PreferredAgentId = "juniper",
                PreferredBonusMul = DefaultPreferredBonus
            }
        };

        public static JobDefinition Get(string jobId)
        {
            if (string.IsNullOrEmpty(jobId)) return null;
            foreach (var j in Catalog)
                if (j.Id == jobId) return j;
            return null;
        }

        public static int ComputePayout(JobDefinition job, string assignedAgentId)
        {
            if (job == null) return 0;
            int baseYield = job.YieldLumaCoins;
            if (!string.IsNullOrEmpty(job.PreferredAgentId) &&
                string.Equals(assignedAgentId, job.PreferredAgentId, StringComparison.OrdinalIgnoreCase))
            {
                return (int)Math.Round(baseYield * job.PreferredBonusMul);
            }
            return baseYield;
        }

        public static DateTime CompleteAtUtc(JobDefinition job, DateTime startedUtc, bool debugAccelerated)
        {
            if (job == null) return startedUtc;
            if (debugAccelerated)
                return startedUtc.AddSeconds(Math.Max(1, job.DebugAcceleratedSeconds));
            return startedUtc.AddMinutes(job.DurationMinutes);
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
