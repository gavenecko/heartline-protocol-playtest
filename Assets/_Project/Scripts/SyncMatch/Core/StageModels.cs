using System;
using System.Collections.Generic;

namespace Heartline.SyncMatch
{
    [Serializable]
    public class StageCatalog
    {
        public int chapter = 1;
        public List<StageDefinition> stages = new List<StageDefinition>();
    }

    [Serializable]
    public class StageDefinition
    {
        public string id;
        public string name;
        public string type;
        public int staminaCost;
        public List<string> enemyTags = new List<string>();
        public string teachingGoal;
        public bool forceMiraBurst;
        public string unlockAgentOnClear;
        public bool setsCh1Complete;
        public StageRewards rewards;
    }

    [Serializable]
    public class StageRewards
    {
        public int lumaCoins;
        public int xp;
        public int crystals;
    }

    /// <summary>Pure combat sizing from stage table — shared with Heartline.Sim.</summary>
    public static class StageCombatMath
    {
        public static int EnemyHp(StageDefinition stage)
        {
            if (stage == null) return 900;
            int indexBonus = StageOrdinal(stage.id) * 80;
            return stage.type switch
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
            return stage.type switch
            {
                "Boss" => 110,
                "EliteLite" => 85,
                _ => 55 + StageOrdinal(stage.id) * 4
            };
        }

        /// <summary>1-1 → 0 … 1-10 → 9; 1-B → 10.</summary>
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
    }
}
