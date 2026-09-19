using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heartline.SyncMatch
{
    [Serializable]
    public class AgentCatalog
    {
        public List<AgentDefinition> agents = new List<AgentDefinition>();
    }

    [Serializable]
    public class AgentDefinition
    {
        public string id;
        public string displayName;
        public string codename;
        public int age;
        public string rarity;
        public string role;
        public string faction;
        public List<string> preferredLanes = new List<string>();
        public List<string> gemAttunements = new List<string>();
        public string unlock;
        public float gaugeSharePower;
        public StatBlock statsLv1;
        public StatBlock statsLv60;
        public int baseStars = 1;
        public int maxLevelSoft = 60;
        public List<string> preferredGifts = new List<string>();
        public string roleBlurb;
        public string contentNotes;
        public bool secret;
        public bool codexHiddenUntilUnlock;

        public bool IsAdult => age >= 18;

        public List<GemColor> AttunementColors()
        {
            var list = new List<GemColor>();
            if (gemAttunements == null) return list;
            foreach (var s in gemAttunements)
            {
                var g = ParseGem(s);
                if (g != GemColor.Empty) list.Add(g);
            }
            return list;
        }

        public static GemColor ParseGem(string s)
        {
            if (string.IsNullOrEmpty(s)) return GemColor.Empty;
            switch (s.Trim().ToLowerInvariant())
            {
                case "crimson": return GemColor.Crimson;
                case "azure": return GemColor.Azure;
                case "verdant": return GemColor.Verdant;
                case "amber": return GemColor.Amber;
                case "violet": return GemColor.Violet;
                case "prism": return GemColor.Prism;
                default: return GemColor.Empty;
            }
        }

        public Lane PreferredLane()
        {
            if (preferredLanes == null || preferredLanes.Count == 0) return Lane.Mid;
            switch (preferredLanes[0].ToLowerInvariant())
            {
                case "front": return Lane.Front;
                case "back": return Lane.Back;
                default: return Lane.Mid;
            }
        }

        public StatBlock StatsAtLevel(int level)
        {
            int cap = Mathf.Max(maxLevelSoft, 60);
            level = Mathf.Clamp(level, 1, cap);
            float t = (level - 1) / 59f;
            return new StatBlock
            {
                hp = Mathf.RoundToInt(Mathf.Lerp(statsLv1.hp, statsLv60.hp, t)),
                atk = Mathf.RoundToInt(Mathf.Lerp(statsLv1.atk, statsLv60.atk, t)),
                def = Mathf.RoundToInt(Mathf.Lerp(statsLv1.def, statsLv60.def, t)),
                syncRate = Mathf.Lerp(statsLv1.syncRate, statsLv60.syncRate, t),
                critRate = Mathf.Lerp(statsLv1.critRate, statsLv60.critRate, t)
            };
        }
    }

    [Serializable]
    public class StatBlock
    {
        public int hp;
        public int atk;
        public int def;
        public float syncRate;
        public float critRate;
    }
}
