using UnityEngine;
using UnityEngine.SceneManagement;
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

        void OnEnable()
        {
            RefreshHud();
            AnalyticsSink.Emit("hub_enter");
        }

        public void RefreshHud()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            if (staminaText) staminaText.text = $"STA {d.stamina}/{GameConstants.StaminaCap}";
            if (heartText) heartText.text = $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap}";
            if (coinsText) coinsText.text = $"LC {d.lumaCoins}";
            if (crystalsText) crystalsText.text = $"PC {d.pulseCrystals}";
            if (statusText) statusText.text = d.ch1Complete ? "Ch.1 Clear" : $"Stage {d.ch1Stage}";
        }

        public void OpenStages() => SceneManager.LoadScene("03_StageSelect");
        public void OpenRoom() => SceneManager.LoadScene("04_Room");
        public void OpenTeam() => SceneManager.LoadScene("05_Team");
        public void OpenGacha() => SceneManager.LoadScene("06_Gacha");
        public void OpenJobs() => SceneManager.LoadScene("07_Jobs");
        public void OpenMail()
        {
            if (statusText) statusText.text = "Mail: Welcome Handler — +500 LC";
            SaveService.Instance.Data.lumaCoins += 500;
            SaveService.Instance.Save();
            RefreshHud();
        }
        public void OpenSettings()
        {
            if (statusText) statusText.text = "SFW Mode ON · Adult packs unavailable in this slice";
        }
    }
}
