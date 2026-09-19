using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    /// <summary>Star rating on stage clear (GDD / FTUE §3).</summary>
    public static class StageStarRating
    {
        /// <summary>1★ clear · 2★ ≥2 agents alive · 3★ ≥2 alive and turnsTaken ≤ threshold.</summary>
        public static int Rate(int agentsAlive, int turnsTaken, int turnThreshold = 8)
        {
            int stars = 1;
            if (agentsAlive >= 2) stars = 2;
            if (agentsAlive >= 2 && turnsTaken <= turnThreshold) stars = 3;
            if (stars < 1) stars = 1;
            if (stars > 3) stars = 3;
            return stars;
        }

        public static int ThresholdForStageType(string type) => type switch
        {
            "Boss" => 12,
            "EliteLite" => 10,
            _ => 8
        };

        public static string FormatStars(int stars)
        {
            if (stars < 1) stars = 1;
            if (stars > 3) stars = 3;
            return new string('★', stars) + new string('☆', 3 - stars);
        }
    }

    /// <summary>Bond Matrix stub — Mira↔Ivy Shared Lease (once/battle ATK buff).</summary>
    public static class BondMatrix
    {
        public const string MiraIvyBondId = "mira_shared_lease_ivy";
        public const float MiraIvyAtkBuff = 0.15f;

        public static bool IsMiraIvyBondActive(IEnumerable<string> squadAgentIds)
        {
            if (squadAgentIds == null) return false;
            bool mira = false, ivy = false;
            foreach (var id in squadAgentIds)
            {
                if (id == "mira") mira = true;
                else if (id == "ivy") ivy = true;
            }
            return mira && ivy;
        }

        public static float AtkMultiplier(bool bondActive, bool buffConsumed) =>
            bondActive && !buffConsumed ? 1f + MiraIvyAtkBuff : 1f;
    }

    /// <summary>4-slot squad helpers (6 physical slots, fill cap 4, max 2/lane).</summary>
    public static class SquadLogic
    {
        public const int MaxPerLane = 2;
        public const int MaxSquadSize = 4;

        public static IEnumerable<string> EnumerateSlots(string front0, string front1, string mid0, string mid1, string back0, string back1)
        {
            if (!string.IsNullOrEmpty(front0)) yield return front0;
            if (!string.IsNullOrEmpty(front1)) yield return front1;
            if (!string.IsNullOrEmpty(mid0)) yield return mid0;
            if (!string.IsNullOrEmpty(mid1)) yield return mid1;
            if (!string.IsNullOrEmpty(back0)) yield return back0;
            if (!string.IsNullOrEmpty(back1)) yield return back1;
        }

        public static int CountFilled(params string[] slots)
        {
            int n = 0;
            if (slots == null) return 0;
            foreach (var s in slots)
                if (!string.IsNullOrEmpty(s)) n++;
            return n;
        }

        public static void ClearId(string agentId,
            ref string front0, ref string front1,
            ref string mid0, ref string mid1,
            ref string back0, ref string back1)
        {
            if (front0 == agentId) front0 = "";
            if (front1 == agentId) front1 = "";
            if (mid0 == agentId) mid0 = "";
            if (mid1 == agentId) mid1 = "";
            if (back0 == agentId) back0 = "";
            if (back1 == agentId) back1 = "";
        }

        public static bool TryAssign(
            string agentId, string lane, int slotIndex,
            ICollection<string> unlocked,
            ref string front0, ref string front1,
            ref string mid0, ref string mid1,
            ref string back0, ref string back1)
        {
            if (string.IsNullOrEmpty(agentId) || unlocked == null || !unlocked.Contains(agentId))
                return false;
            if (slotIndex < 0 || slotIndex > 1) return false;
            if (lane != "Front" && lane != "Mid" && lane != "Back") return false;

            ClearId(agentId, ref front0, ref front1, ref mid0, ref mid1, ref back0, ref back1);

            switch (lane)
            {
                case "Front":
                    if (slotIndex == 0) front0 = agentId; else front1 = agentId;
                    break;
                case "Mid":
                    if (slotIndex == 0) mid0 = agentId; else mid1 = agentId;
                    break;
                case "Back":
                    if (slotIndex == 0) back0 = agentId; else back1 = agentId;
                    break;
            }

            var filled = new HashSet<string>();
            foreach (var id in EnumerateSlots(front0, front1, mid0, mid1, back0, back1))
                filled.Add(id);
            if (filled.Count > MaxSquadSize)
            {
                ClearId(agentId, ref front0, ref front1, ref mid0, ref mid1, ref back0, ref back1);
                return false;
            }
            return true;
        }

        public static string NextOption(string current, IList<string> unlocked, HashSet<string> occupiedElsewhere)
        {
            var options = new List<string> { "" };
            if (unlocked != null)
            {
                foreach (var id in unlocked)
                {
                    if (string.IsNullOrEmpty(id)) continue;
                    if (occupiedElsewhere != null && occupiedElsewhere.Contains(id) && id != current)
                        continue;
                    options.Add(id);
                }
            }
            int idx = 0;
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i] == (current ?? "")) { idx = i; break; }
            }
            return options[(idx + 1) % options.Count];
        }

        public static HashSet<string> OccupiedExcept(string exceptSlotValue,
            string front0, string front1, string mid0, string mid1, string back0, string back1)
        {
            var set = new HashSet<string>();
            foreach (var id in EnumerateSlots(front0, front1, mid0, mid1, back0, back1))
            {
                if (id != exceptSlotValue)
                    set.Add(id);
            }
            return set;
        }
    }
}
