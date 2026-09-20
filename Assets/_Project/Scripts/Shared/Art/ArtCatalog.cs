using System.Collections.Generic;

namespace Heartline.Shared
{
    /// <summary>
    /// Maps runtime agent ids (agents.json) → art folder names under Art/Characters/.
    /// Also names chrome / room / marketing paths under Art/.
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

        public const string IdleFolder = "Idle";
        public const int IdleLoopWidth = 720;
        public const int IdleLoopHeight = 1280;

        // Room BGs
        public const string RoomSuiteEvening = "room_player_suite_evening";
        public const string RoomAgentLoft = "room_agent_loft_default";
        public const string RoomCafe = "room_cafe_shift";
        public const string RoomRainLounge = "room_rain_window_lounge";
        public const string RoomNeonOverlook = "room_neon_date_overlook";
        public const string RoomRooftop = "room_rooftop_garden_night";
        public const string RoomSleepoverMorning = "room_sleepover_morning";
        public const string RoomClinic = "room_clinic_guest";

        public const string KeyVisual = "kv_heartline_protocol";
        public const string NavBottom = "ui_nav_bottom";
        public const string ButtonsPanelsSheet = "ui_buttons_panels_sheet";

        // Currency singles
        public const string CurrencySta = "ui_currency_sta";
        public const string CurrencyHe = "ui_currency_he";
        public const string CurrencyLc = "ui_currency_lc";
        public const string CurrencyPc = "ui_currency_pc";
        public const string CurrencyTicket = "ui_currency_ticket";
        public const string CurrencyMarks = "ui_currency_marks";

        // Gem chips
        public const string GemCrimson = "ui_gem_crimson";
        public const string GemAzure = "ui_gem_azure";
        public const string GemVerdant = "ui_gem_verdant";
        public const string GemAmber = "ui_gem_amber";
        public const string GemViolet = "ui_gem_violet";
        public const string GemPrism = "ui_gem_prism";

        // Rarity frames
        public const string FrameN = "ui_frame_n";
        public const string FrameR = "ui_frame_r";
        public const string FrameSr = "ui_frame_sr";
        public const string FrameSsr = "ui_frame_ssr";
        public const string FrameUr = "ui_frame_ur";

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
            { "velvet", "velvet_noir" },
            { "rook", "rook_hale" },
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
            { "velvet_noir", "velvet_noir" },
            { "rook_hale", "rook_hale" },
        };

        /// <summary>Short file stem for idle loops: velvet → velvet_idle_loop, rook → rook_idle_loop.</summary>
        static readonly Dictionary<string, string> IdleLoopStems = new Dictionary<string, string>
        {
            { "velvet", "velvet_idle_loop" },
            { "velvet_noir", "velvet_idle_loop" },
            { "rook", "rook_idle_loop" },
            { "rook_hale", "rook_idle_loop" },
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

        /// <summary>
        /// Relative path including extension for StreamingAssets / disk load,
        /// e.g. Art/Characters/velvet_noir/Idle/velvet_idle_loop.mp4
        /// </summary>
        public static string IdleLoopRelativePath(string agentId)
        {
            if (string.IsNullOrEmpty(agentId)) return null;
            var key = agentId.Trim().ToLowerInvariant();
            if (!IdleLoopStems.TryGetValue(key, out var stem)) return null;
            if (!TryGetFolder(key, out var folder)) return null;
            return $"{CharactersRoot}/{folder}/{IdleFolder}/{stem}.mp4";
        }

        public static string IdlePosterResourcePath(string agentId)
        {
            if (string.IsNullOrEmpty(agentId)) return null;
            var key = agentId.Trim().ToLowerInvariant();
            if (!IdleLoopStems.TryGetValue(key, out var stem)) return null;
            if (!TryGetFolder(key, out var folder)) return null;
            // poster file is velvet_idle_poster (stem without _loop)
            var poster = stem.Replace("_idle_loop", "_idle_poster");
            return $"{CharactersRoot}/{folder}/{IdleFolder}/{poster}";
        }

        public static string RoomResourcePath(string roomIdWithoutExt) =>
            $"{RoomsRoot}/{roomIdWithoutExt}";

        public static string MarketingResourcePath(string fileWithoutExt) =>
            $"{MarketingRoot}/{fileWithoutExt}";

        public static string ChromeResourcePath(string fileWithoutExt) =>
            $"{ChromeRoot}/{fileWithoutExt}";

        public static string CurrencyResourcePath(string currencyKey) =>
            ChromeResourcePath(currencyKey);

        public static string GemResourcePath(string gemKey) =>
            ChromeResourcePath(gemKey);

        /// <summary>Map rarity string (N/R/SR/SSR/UR) → frame chrome file stem.</summary>
        public static string FrameFileForRarity(string rarity)
        {
            if (string.IsNullOrEmpty(rarity)) return FrameN;
            switch (rarity.Trim().ToUpperInvariant())
            {
                case "UR": return FrameUr;
                case "SSR": return FrameSsr;
                case "SR": return FrameSr;
                case "R": return FrameR;
                default: return FrameN;
            }
        }

        public static string FrameResourcePath(string rarity) =>
            ChromeResourcePath(FrameFileForRarity(rarity));

        /// <summary>Map Shared.GemColor → chrome gem file stem.</summary>
        public static string GemFileForColor(GemColor color) => color switch
        {
            GemColor.Crimson => GemCrimson,
            GemColor.Azure => GemAzure,
            GemColor.Verdant => GemVerdant,
            GemColor.Amber => GemAmber,
            GemColor.Violet => GemViolet,
            GemColor.Prism => GemPrism,
            _ => null
        };

        /// <summary>
        /// Room BG for Room actions. Talk→suite (fallback loft), Gift→cafe (fallback rain),
        /// Date→neon overlook (fallback rooftop), Sleepover→morning.
        /// </summary>
        public static string RoomForAction(string action)
        {
            if (string.IsNullOrEmpty(action)) return RoomSuiteEvening;
            switch (action.Trim().ToLowerInvariant())
            {
                case "talk": return RoomSuiteEvening;
                case "gift": return RoomCafe;
                case "date": return RoomNeonOverlook;
                case "sleepover":
                case "sleep":
                case "morning": return RoomSleepoverMorning;
                case "loft": return RoomAgentLoft;
                case "rain": return RoomRainLounge;
                case "rooftop": return RoomRooftop;
                case "clinic": return RoomClinic;
                default: return RoomSuiteEvening;
            }
        }

        public static string RoomFallbackForAction(string action)
        {
            if (string.IsNullOrEmpty(action)) return RoomAgentLoft;
            switch (action.Trim().ToLowerInvariant())
            {
                case "talk": return RoomAgentLoft;
                case "gift": return RoomRainLounge;
                case "date": return RoomRooftop;
                case "sleepover":
                case "sleep":
                case "morning": return RoomSuiteEvening;
                default: return RoomAgentLoft;
            }
        }
    }
}
