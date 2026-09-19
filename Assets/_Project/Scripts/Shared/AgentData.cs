using System;
using System.Collections.Generic;

namespace Heartline.Shared
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
        public int heightCm;
        public string voiceNote;
        public int baseStars;
        public int maxLevelSoft;
        public string unlock;
        public StatBlock statsLv1;
        public StatBlock statsLv60;
        public float gaugeSharePower;
        public float healPower;
        public float shieldPower;
        public SkillRefs skills;
        public List<string> preferredGifts = new List<string>();
        public List<string> dislikedGifts = new List<string>();
        public List<string> bondPartnerIds = new List<string>();
        public PaletteHex paletteHex;
        public string roleBlurb;
        public string contentNotes;
        public bool secret;
        public bool codexHiddenUntilUnlock;

        public bool IsAdult => age >= 18;

        public GemColor PrimaryAttunement()
        {
            if (gemAttunements == null || gemAttunements.Count == 0) return GemColor.Crimson;
            return ParseGem(gemAttunements[0]);
        }

        public bool IsAttuned(GemColor color)
        {
            if (gemAttunements == null) return false;
            if (color == GemColor.Prism)
                return gemAttunements.Exists(g => ParseGem(g) == GemColor.Prism);
            foreach (var g in gemAttunements)
                if (ParseGem(g) == color) return true;
            return false;
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
            int cap = Math.Max(maxLevelSoft, 60);
            level = Math.Max(1, Math.Min(level, cap));
            float t = (level - 1) / 59f;
            return new StatBlock
            {
                hp = Lerp(statsLv1.hp, statsLv60.hp, t),
                atk = Lerp(statsLv1.atk, statsLv60.atk, t),
                def = Lerp(statsLv1.def, statsLv60.def, t),
                syncRate = Lerp(statsLv1.syncRate, statsLv60.syncRate, t),
                critRate = Lerp(statsLv1.critRate, statsLv60.critRate, t)
            };
        }

        static int Lerp(int a, int b, float t) => a + (int)((b - a) * t);
        static float Lerp(float a, float b, float t) => a + (b - a) * t;
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

    [Serializable]
    public class SkillRefs
    {
        public string passiveId;
        public string basicId;
        public string burstId;
        public string bondId;
    }

    [Serializable]
    public class PaletteHex
    {
        public string primary;
        public string accent;
    }
}
