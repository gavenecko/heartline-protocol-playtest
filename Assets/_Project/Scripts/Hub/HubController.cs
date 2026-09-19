using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;

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

        public void BindLegacy(Text sta, Text he, Text coins, Text crystals, Text status)
        {
            staminaTextLegacy = sta;
            heartTextLegacy = he;
            coinsTextLegacy = coins;
            crystalsTextLegacy = crystals;
            statusTextLegacy = status;
        }

        void OnEnable()
        {
            RefreshHud();
            AnalyticsSink.Emit("hub_enter");
        }

        public void RefreshHud()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            HudText.Set(staminaText, staminaTextLegacy, $"STA {d.stamina}/{GameConstants.StaminaCap}");
            HudText.Set(heartText, heartTextLegacy, $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap}");
            HudText.Set(coinsText, coinsTextLegacy, $"LC {d.lumaCoins}");
            HudText.Set(crystalsText, crystalsTextLegacy, $"PC {d.pulseCrystals}");
            string st = d.ch1Complete ? "Ch.1 Clear" : $"Progress {d.ch1Stage} · cleared {d.clearedStages.Count}";
            HudText.Set(statusText, statusTextLegacy, st);
        }

        public void OpenStages() => SceneManager.LoadScene("03_StageSelect");
        public void OpenRoom() => SceneManager.LoadScene("04_Room");
        public void OpenTeam() => SceneManager.LoadScene("05_Team");
        public void OpenGacha() => SceneManager.LoadScene("06_Gacha");
        public void OpenJobs() => SceneManager.LoadScene("07_Jobs");

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
                "SFW Mode ON · Adult packs unavailable · Aff caps at 3 · Ages 18+ (Juniper 21)");
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
