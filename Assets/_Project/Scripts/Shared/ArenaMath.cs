using System;
using System.Collections.Generic;

namespace Heartline.Shared
{
    /// <summary>
    /// Light async Arena stub (GDD §5): 3 free attacks/day, fake defense teams,
    /// Arena Marks currency, weekly ranking note. No live PvP.
    /// </summary>
    public static class ArenaMath
    {
        public const int FreeAttacksPerDay = 3;
        public const int MarksWin = 25;
        public const int MarksLoss = 5;
        public const int MarksDraw = 10;

        public sealed class DefenseTeam
        {
            public string HandlerName;
            public string[] AgentIds;
            public int PowerRating;
            public bool IsWinLikely; // stub: predetermined outcome for slice
        }

        public static readonly DefenseTeam[] FakeOpponents =
        {
            new DefenseTeam
            {
                HandlerName = "Quay Rookie",
                AgentIds = new[] { "ivy", "juniper" },
                PowerRating = 1200,
                IsWinLikely = true
            },
            new DefenseTeam
            {
                HandlerName = "Circuit Fan",
                AgentIds = new[] { "mira", "ivy", "wren" },
                PowerRating = 2400,
                IsWinLikely = true
            },
            new DefenseTeam
            {
                HandlerName = "Aurelia Scout",
                AgentIds = new[] { "mira", "juniper", "wren", "ivy" },
                PowerRating = 3600,
                IsWinLikely = false
            }
        };

        public static string TodayUtc(DateTime utcNow) => utcNow.ToString("yyyy-MM-dd");

        /// <summary>ISO week key (UTC Monday-based) for weekly ranking UI note.</summary>
        public static string WeekKeyUtc(DateTime utcNow)
        {
            // Monday-start week id: year-Www
            var d = utcNow.Date;
            int diff = ((int)d.DayOfWeek + 6) % 7; // Mon=0
            var monday = d.AddDays(-diff);
            int week = System.Globalization.ISOWeek.GetWeekOfYear(monday);
            int year = System.Globalization.ISOWeek.GetYear(monday);
            return $"{year}-W{week:D2}";
        }

        public static string WeeklyNote(string weekKey, int arenaMarks, int attacksUsedToday) =>
            $"Week {weekKey} · ranking rewards settle Mon 00:00 UTC · Marks {arenaMarks} · attacks left {Math.Max(0, FreeAttacksPerDay - attacksUsedToday)}/{FreeAttacksPerDay}";

        public static bool EnsureDay(ref string arenaUtcDate, ref int attacksUsed, DateTime utcNow)
        {
            string today = TodayUtc(utcNow);
            if (arenaUtcDate == today) return false;
            arenaUtcDate = today;
            attacksUsed = 0;
            return true;
        }

        public static int AttacksRemaining(int attacksUsed) =>
            Math.Max(0, FreeAttacksPerDay - Math.Max(0, attacksUsed));

        public static DefenseTeam PickOpponent(int attacksUsed)
        {
            if (FakeOpponents.Length == 0) return null;
            int idx = Math.Abs(attacksUsed) % FakeOpponents.Length;
            return FakeOpponents[idx];
        }

        public static bool TryAttack(
            ref int attacksUsed,
            ref int arenaMarks,
            DefenseTeam opponent,
            out bool won,
            out int marksGained,
            out string error)
        {
            won = false;
            marksGained = 0;
            error = null;
            if (AttacksRemaining(attacksUsed) <= 0)
            {
                error = "No free Arena attacks left today (3/day).";
                return false;
            }
            if (opponent == null)
            {
                error = "No defense team.";
                return false;
            }
            attacksUsed++;
            won = opponent.IsWinLikely;
            marksGained = won ? MarksWin : MarksLoss;
            arenaMarks += marksGained;
            return true;
        }

        /// <summary>Marks earned for N wins + M losses (economy / shop planning).</summary>
        public static int MarksForRecord(int wins, int losses) =>
            Math.Max(0, wins) * MarksWin + Math.Max(0, losses) * MarksLoss;
    }
}
