using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heartline.Sim
{
    public sealed class AgentCatalog
    {
        [JsonPropertyName("agents")]
        public List<AgentDefinition> Agents { get; set; } = new();
    }

    public sealed class AgentDefinition
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("displayName")] public string DisplayName { get; set; }
        [JsonPropertyName("codename")] public string Codename { get; set; }
        [JsonPropertyName("age")] public int Age { get; set; }
        [JsonPropertyName("rarity")] public string Rarity { get; set; }
        [JsonPropertyName("role")] public string Role { get; set; }
        [JsonPropertyName("faction")] public string Faction { get; set; }
        [JsonPropertyName("preferredLanes")] public List<string> PreferredLanes { get; set; } = new();
        [JsonPropertyName("gemAttunements")] public List<string> GemAttunements { get; set; } = new();
        [JsonPropertyName("unlock")] public string Unlock { get; set; }
        [JsonPropertyName("gaugeSharePower")] public float GaugeSharePower { get; set; }
        [JsonPropertyName("statsLv1")] public StatBlock StatsLv1 { get; set; }
        [JsonPropertyName("statsLv60")] public StatBlock StatsLv60 { get; set; }
        [JsonPropertyName("maxLevelSoft")] public int MaxLevelSoft { get; set; } = 60;
        [JsonPropertyName("preferredGifts")] public List<string> PreferredGifts { get; set; } = new();
        [JsonPropertyName("secret")] public bool Secret { get; set; }

        public bool IsAdult => Age >= 18;

        public List<GemColor> AttunementColors()
        {
            var list = new List<GemColor>();
            if (GemAttunements == null) return list;
            foreach (var s in GemAttunements)
            {
                var g = ParseGem(s);
                if (g != GemColor.Empty) list.Add(g);
            }
            return list;
        }

        public static GemColor ParseGem(string s)
        {
            if (string.IsNullOrEmpty(s)) return GemColor.Empty;
            return s.Trim().ToLowerInvariant() switch
            {
                "crimson" => GemColor.Crimson,
                "azure" => GemColor.Azure,
                "verdant" => GemColor.Verdant,
                "amber" => GemColor.Amber,
                "violet" => GemColor.Violet,
                "prism" => GemColor.Prism,
                _ => GemColor.Empty
            };
        }

        public Lane PreferredLane()
        {
            if (PreferredLanes == null || PreferredLanes.Count == 0) return Lane.Mid;
            return PreferredLanes[0].ToLowerInvariant() switch
            {
                "front" => Lane.Front,
                "back" => Lane.Back,
                _ => Lane.Mid
            };
        }

        public StatBlock StatsAtLevel(int level)
        {
            int cap = Math.Max(MaxLevelSoft, 60);
            level = Math.Clamp(level, 1, cap);
            float t = (level - 1) / 59f;
            return new StatBlock
            {
                Hp = Lerp(StatsLv1.Hp, StatsLv60.Hp, t),
                Atk = Lerp(StatsLv1.Atk, StatsLv60.Atk, t),
                Def = Lerp(StatsLv1.Def, StatsLv60.Def, t),
                SyncRate = Lerp(StatsLv1.SyncRate, StatsLv60.SyncRate, t),
                CritRate = Lerp(StatsLv1.CritRate, StatsLv60.CritRate, t)
            };
        }

        static int Lerp(int a, int b, float t) => a + (int)((b - a) * t);
        static float Lerp(float a, float b, float t) => a + (b - a) * t;

        public static AgentCatalog LoadFromJson(string json)
            => JsonSerializer.Deserialize<AgentCatalog>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public sealed class StatBlock
    {
        [JsonPropertyName("hp")] public int Hp { get; set; }
        [JsonPropertyName("atk")] public int Atk { get; set; }
        [JsonPropertyName("def")] public int Def { get; set; }
        [JsonPropertyName("syncRate")] public float SyncRate { get; set; }
        [JsonPropertyName("critRate")] public float CritRate { get; set; }
    }
}
