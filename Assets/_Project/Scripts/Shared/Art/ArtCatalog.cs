using System.Collections.Generic;

namespace Heartline.Shared
{
    /// <summary>
    /// Maps runtime agent ids (agents.json) → art folder names under Art/Characters/.
    /// </summary>
    public static class ArtCatalog
    {
        public const string CharactersRoot = "Art/Characters";
        public const string RoomsRoot = "Art/Environment/Rooms";
        public const string MarketingRoot = "Art/Marketing";
        public const string ChromeRoot = "Art/UI/Chrome";

        public const string BustFile = "t0_bust";
        public const string SdFile = "t0_sd";
        public const string FullbodyFile = "t0_fullbody";

        public const string RoomSuiteEvening = "room_player_suite_evening";
        public const string RoomAgentLoft = "room_agent_loft_default";
        public const string KeyVisual = "kv_heartline_protocol";
        public const string NavBottom = "ui_nav_bottom";

        static readonly Dictionary<string, string> AgentFolders = new Dictionary<string, string>
        {
            { "mira", "mira_solenne" },
            { "ivy", "ivy_cross" },
            { "juniper", "juniper_tate" },
            { "wren", "wren_halcyon" },
            { "kade", "kade_virell" },
            { "nyra", "nyra_quell" },
            { "sable", "sable_rin" },
            { "ora", "ora_finch" },
            { "luce", "luce_amaranth" },
            { "helene", "helene_voss" },
            { "hex", "zara_hex_montal" },
            { "aveline", "aveline_roux" },
            // folder-name aliases
            { "mira_solenne", "mira_solenne" },
            { "ivy_cross", "ivy_cross" },
            { "juniper_tate", "juniper_tate" },
            { "wren_halcyon", "wren_halcyon" },
            { "kade_virell", "kade_virell" },
            { "nyra_quell", "nyra_quell" },
            { "sable_rin", "sable_rin" },
            { "ora_finch", "ora_finch" },
            { "luce_amaranth", "luce_amaranth" },
            { "helene_voss", "helene_voss" },
            { "zara_hex_montal", "zara_hex_montal" },
            { "aveline_roux", "aveline_roux" },
        };

        public static bool TryGetFolder(string agentId, out string folder)
        {
            folder = null;
            if (string.IsNullOrEmpty(agentId)) return false;
            return AgentFolders.TryGetValue(agentId.Trim().ToLowerInvariant(), out folder);
        }

        public static string BustResourcePath(string agentId)
        {
            if (!TryGetFolder(agentId, out var folder)) return null;
            return $"{CharactersRoot}/{folder}/{BustFile}";
        }

        public static string SdResourcePath(string agentId)
        {
            if (!TryGetFolder(agentId, out var folder)) return null;
            return $"{CharactersRoot}/{folder}/{SdFile}";
        }

        public static string FullbodyResourcePath(string agentId)
        {
            if (!TryGetFolder(agentId, out var folder)) return null;
            return $"{CharactersRoot}/{folder}/{FullbodyFile}";
        }

        public static string RoomResourcePath(string roomIdWithoutExt) =>
            $"{RoomsRoot}/{roomIdWithoutExt}";

        public static string MarketingResourcePath(string fileWithoutExt) =>
            $"{MarketingRoot}/{fileWithoutExt}";

        public static string ChromeResourcePath(string fileWithoutExt) =>
            $"{ChromeRoot}/{fileWithoutExt}";
    }
}
