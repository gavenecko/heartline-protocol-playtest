namespace Heartline.Shared
{
    /// <summary>Board gem colors. Prism is wild.</summary>
    public enum GemColor
    {
        Crimson = 0,
        Azure = 1,
        Verdant = 2,
        Amber = 3,
        Violet = 4,
        Prism = 5,
        Empty = 6
    }

    public enum Lane
    {
        Front = 0,
        Mid = 1,
        Back = 2
    }

    public enum AgentRole
    {
        Striker,
        Breaker,
        Guardian,
        Support,
        Controller,
        Synergist
    }

    public enum Rarity
    {
        N,
        R,
        SR,
        SSR,
        UR
    }

    public enum SkillType
    {
        Basic,
        Burst,
        Passive,
        Bond
    }

    public static class ArtPalette
    {
        public const string Crimson = "#E23B4A";
        public const string Azure = "#2F7BFF";
        public const string Verdant = "#2DBE6A";
        public const string Amber = "#F0A12A";
        public const string Violet = "#8B5CF6";
        public const string Prism = "#F5F7FF";
        public const string NeonMagenta = "#FF2D8A";
        public const string SyncCyan = "#00D4FF";
        public const string QuayIndigo = "#12182B";
        public const string AtelierGold = "#E8C87A";
        public const string Ink900 = "#0B1020";
    }
}
