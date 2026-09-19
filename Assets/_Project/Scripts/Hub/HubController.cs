using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;
using Heartline.App;

namespace Heartline.Hub
{
    public class HubController : MonoBehaviour
    {
        [SerializeField] TMP_Text staminaText;
        [SerializeField] TMP_Text heartText;
        [SerializeField] TMP_Text coinsText;
        [SerializeField] TMP_Text crystalsText;
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text staminaTextLegacy;
        [SerializeField] Text heartTextLegacy;
        [SerializeField] Text coinsTextLegacy;
        [SerializeField] Text crystalsTextLegacy;
        [SerializeField] Text statusTextLegacy;
        [SerializeField] RawImage featuredPortrait;

        public void BindLegacy(Text sta, Text he, Text coins, Text crystals, Text status, RawImage featured = null)
        {
            staminaTextLegacy = sta;
            heartTextLegacy = he;
            coinsTextLegacy = coins;
            crystalsTextLegacy = crystals;
            statusTextLegacy = status;
            if (featured != null) featuredPortrait = featured;
            ApplyFeaturedPortrait();
        }

        void OnEnable()
        {
            RefreshHud();
            ApplyFeaturedPortrait();
            AnalyticsSink.Emit("hub_enter");
            TryPlayHubDialogue();
        }

        void ApplyFeaturedPortrait()
        {
            if (featuredPortrait == null) return;
            ArtLoader.ApplyBust(featuredPortrait, FeaturedAgentId());
        }

        static string FeaturedAgentId()
        {
            var d = SaveService.Instance?.Data;
            if (d?.unlockedAgents != null)
            {
                if (d.unlockedAgents.Contains("mira")) return "mira";
                if (d.unlockedAgents.Count > 0) return d.unlockedAgents[0];
            }
            return "mira";
        }

        void TryPlayHubDialogue()
        {
            if (DialogueService.Instance == null) return;
            var pending = PlayerPrefs.GetString("hp_pending_dialogue", "");
            if (!string.IsNullOrEmpty(pending))
            {
                PlayerPrefs.DeleteKey("hp_pending_dialogue");
                if (DialogueService.Instance.TryStart(pending) || DialogueService.Instance.TryStartTrigger(pending))
                {
                    ShowDialogueStatus();
                    return;
                }
            }
            if (DialogueService.Instance.TryStartTrigger("hub_enter"))
                ShowDialogueStatus();
        }

        void ShowDialogueStatus()
        {
            if (DialogueService.Instance == null || !DialogueService.Instance.IsPlaying) return;
            HudText.Set(statusText, statusTextLegacy, DialogueService.Instance.CurrentDisplay());
        }

        public void AdvanceDialogue()
        {
            if (DialogueService.Instance == null || !DialogueService.Instance.IsPlaying) return;
            if (!DialogueService.Instance.Advance())
                RefreshHud();
            else
                ShowDialogueStatus();
        }

        public void RefreshHud()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long staNext = EconomyRegen.SecondsUntilNext(d.stamina, d.lastStaminaUtc, now, GameConstants.StaminaCap, EconomyRegen.StaminaSecondsPerPoint);
            long heNext = EconomyRegen.SecondsUntilNext(d.heartEnergy, d.lastHeartUtc, now, GameConstants.HeartEnergyCap, EconomyRegen.HeartSecondsPerPoint);
            string staExtra = d.stamina >= GameConstants.StaminaCap ? " (full)" : (staNext > 0 ? $" (+1 {staNext / 60}m)" : " (+1 soon)");
            string heExtra = d.heartEnergy >= GameConstants.HeartEnergyCap ? " (full)" : (heNext > 0 ? $" (+1 {heNext / 60}m)" : " (+1 soon)");
            HudText.Set(staminaText, staminaTextLegacy, $"STA {d.stamina}/{GameConstants.StaminaCap}{staExtra}");
            HudText.Set(heartText, heartTextLegacy, $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap}{heExtra}");
            HudText.Set(coinsText, coinsTextLegacy, $"LC {d.lumaCoins}");
            HudText.Set(crystalsText, crystalsTextLegacy, $"PC {d.pulseCrystals} · Tix {d.beginnerTickets} · AM {d.arenaMarks}");
            string st = d.ch1Complete ? "Ch.1 Clear" : $"Progress {d.ch1Stage} · cleared {d.clearedStages.Count}";
            HudText.Set(statusText, statusTextLegacy, st);
        }

        public void OpenStages() => SceneManager.LoadScene("03_StageSelect");
        public void OpenRoom() => SceneManager.LoadScene("04_Room");
        public void OpenTeam() => SceneManager.LoadScene("05_Team");
        public void OpenGacha() => SceneManager.LoadScene("06_Gacha");
        public void OpenJobs() => SceneManager.LoadScene("07_Jobs");
        public void OpenMissions() => SceneManager.LoadScene("09_Missions");
        public void OpenArena() => SceneManager.LoadScene("10_Arena");
        public void OpenTower() => SceneManager.LoadScene("11_Tower");
        public void OpenCodex() => SceneManager.LoadScene("12_Codex");

        public void OpenMail()
        {
            var d = SaveService.Instance.Data;
            if (PlayerPrefs.GetInt("hp_mail_welcome", 0) == 0)
            {
                d.lumaCoins += 500;
                PlayerPrefs.SetInt("hp_mail_welcome", 1);
                HudText.Set(statusText, statusTextLegacy, "Mail: Welcome Handler — +500 LC");
            }
            else if (d.ch1Complete && PlayerPrefs.GetInt("hp_mail_ch1", 0) == 0)
            {
                d.pulseCrystals += 50;
                PlayerPrefs.SetInt("hp_mail_ch1", 1);
                HudText.Set(statusText, statusTextLegacy, "Mail: Chapter 1 Victory — +50 PC");
            }
            else
                HudText.Set(statusText, statusTextLegacy, "Mail inbox empty.");
            SaveService.Instance.Save();
            RefreshHud();
        }

        public void OpenSettings()
        {
            HudText.Set(statusText, statusTextLegacy,
                "SFW Mode ON (locked) · Adult BLOCKED · Aff≤3 · Arena/Tower/Codex stubs · Ages 18+ (Juniper 21)");
        }

        public void DebugAddStamina()
        {
            var d = SaveService.Instance.Data;
            d.stamina = GameConstants.StaminaCap;
            d.heartEnergy = GameConstants.HeartEnergyCap;
            SaveService.Instance.Save();
            RefreshHud();
        }
    }
}
