using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    public sealed class DailyMissionDef
    {
        public string Id;
        public string Title;
        public int Target;
        public int RewardLuma;
        public int RewardCrystals;
        public int RewardTickets;
    }

    /// <summary>Three simple daily missions — retention loop for the slice.</summary>
    public static class DailyMissionMath
    {
        public static readonly DailyMissionDef[] Catalog =
        {
            new DailyMissionDef
            {
                Id = "clear_stage",
                Title = "Clear any stage",
                Target = 1,
                RewardLuma = 500,
                RewardCrystals = 0,
                RewardTickets = 0
            },
            new DailyMissionDef
            {
                Id = "room_talk",
                Title = "Talk in Mira's Room",
                Target = 1,
                RewardLuma = 400,
                RewardCrystals = 0,
                RewardTickets = 0
            },
            new DailyMissionDef
            {
                Id = "claim_job",
                Title = "Claim a Job payout",
                Target = 1,
                RewardLuma = 300,
                RewardCrystals = 50,
                RewardTickets = 1
            }
        };

        public static string TodayUtc(DateTime utcNow) => utcNow.ToString("yyyy-MM-dd");

        public static DailyMissionDef Get(string id)
        {
            foreach (var m in Catalog)
                if (m.Id == id) return m;
            return null;
        }

        /// <summary>Reset progress/claims when the UTC day rolls.</summary>
        public static bool EnsureDay(
            ref string dailyUtcDate,
            IDictionary<string, int> progress,
            ICollection<string> claimed,
            DateTime utcNow)
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

        public static bool IsComplete(IDictionary<string, int> progress, DailyMissionDef def)
        {
            if (def == null) return false;
            return GetProgress(progress, def.Id) >= def.Target;
        }

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
            if (def == null)
            {
                error = "Unknown mission.";
                return false;
            }
            if (IsClaimed(claimed, id))
            {
                error = "Already claimed.";
                return false;
            }
            if (!IsComplete(progress, def))
            {
                error = "Mission not complete.";
                return false;
            }
            lumaCoins += def.RewardLuma;
            pulseCrystals += def.RewardCrystals;
            beginnerTickets += def.RewardTickets;
            claimed.Add(id);
            return true;
        }

        /// <summary>Total daily payout if all three claimed (economy notes ~1200 LC + 50 PC + 1 tix).</summary>
        public static void Totals(out int luma, out int crystals, out int tickets)
        {
            luma = 0;
            crystals = 0;
            tickets = 0;
            foreach (var m in Catalog)
            {
                luma += m.RewardLuma;
                crystals += m.RewardCrystals;
                tickets += m.RewardTickets;
            }
        }
    }
}
