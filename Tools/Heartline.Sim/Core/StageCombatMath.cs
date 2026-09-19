using System;
using System.Collections.Generic;

namespace Heartline.Sim
{
    public sealed class StageDefinition
    {
        public string Id;
        public string Type;
    }

    /// <summary>Mirrors Unity StageCombatMath for CI harness.</summary>
    public static class StageCombatMath
    {
        public static int EnemyHp(StageDefinition stage)
        {
            if (stage == null) return 900;
            int indexBonus = StageOrdinal(stage.Id) * 80;
            return stage.Type switch
            {
                "Boss" => 3500,
                "EliteLite" => 1800,
                "Grind" => 1000,
                _ => 900 + indexBonus
            };
        }

        public static int EnemyAtk(StageDefinition stage)
        {
            if (stage == null) return 60;
            return stage.Type switch
            {
                "Boss" => 110,
                "EliteLite" => 85,
                _ => 55 + StageOrdinal(stage.Id) * 4
            };
        }

        public static int StageOrdinal(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            if (id == "1-B" || id == "1-b") return 10;
            var dash = id.LastIndexOf('-');
            if (dash < 0) return 0;
            return int.TryParse(id.Substring(dash + 1), out var n) ? Math.Max(0, n - 1) : 0;
        }

        public static string PreviousStageId(string id)
        {
            if (string.IsNullOrEmpty(id) || id == "1-1") return null;
            if (id == "1-B" || id == "1-b") return "1-10";
            var ord = StageOrdinal(id);
            return ord <= 0 ? null : $"1-{ord}";
        }

        public static bool IsUnlocked(string stageId, ICollection<string> cleared, bool ch1Complete)
        {
            if (stageId == "1-1") return true;
            if (stageId == "1-B" || stageId == "1-b")
                return ch1Complete || (cleared != null && cleared.Contains("1-10"));
            var prev = PreviousStageId(stageId);
            if (prev == null) return true;
            return cleared != null && cleared.Contains(prev);
        }

        public static string UnlockReason(string stageId, ICollection<string> cleared, bool ch1Complete)
        {
            if (IsUnlocked(stageId, cleared, ch1Complete)) return null;
            if (stageId == "1-B" || stageId == "1-b")
                return "Clear boss 1-10 to unlock grind node 1-B.";
            var prev = PreviousStageId(stageId);
            if (prev != null)
                return $"Clear {prev} first.";
            return "Locked.";
        }
    }
}
