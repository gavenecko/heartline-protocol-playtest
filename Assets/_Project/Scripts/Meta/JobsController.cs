using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;

namespace Heartline.Meta
{
    public class JobsController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        const string CafeId = "cafe_shift";
        const int YieldCoins = 1200;
        const int DurationSecondsDebug = 10; // accelerated for playtest

        void OnEnable() => Refresh();

        public void AssignCafe()
        {
            var d = SaveService.Instance.Data;
            if (!string.IsNullOrEmpty(d.cafeJobCompleteAtIso))
            {
                if (statusText) statusText.text = "Café Shift already running.";
                return;
            }
            d.cafeAssignedAgentId = "wren";
            d.cafeJobCompleteAtIso = DateTime.UtcNow.AddSeconds(DurationSecondsDebug).ToString("o");
            SaveService.Instance.Save();
            AnalyticsSink.Emit("job_start", $"job_id={CafeId}");
            Refresh();
        }

        public void ClaimCafe()
        {
            var d = SaveService.Instance.Data;
            if (string.IsNullOrEmpty(d.cafeJobCompleteAtIso))
            {
                if (statusText) statusText.text = "No active job.";
                return;
            }
            if (!DateTime.TryParse(d.cafeJobCompleteAtIso, out var done) || DateTime.UtcNow < done)
            {
                if (statusText) statusText.text = "Still working… (debug timer ~10s)";
                return;
            }
            d.lumaCoins += YieldCoins;
            d.cafeJobCompleteAtIso = null;
            d.cafeAssignedAgentId = null;
            SaveService.Instance.Save();
            AnalyticsSink.Emit("job_claim", $"job_id={CafeId};yield={YieldCoins}");
            if (statusText) statusText.text = $"Claimed +{YieldCoins} Luma Coins.";
        }

        public void DebugCompleteNow()
        {
            var d = SaveService.Instance.Data;
            if (string.IsNullOrEmpty(d.cafeJobCompleteAtIso)) return;
            d.cafeJobCompleteAtIso = DateTime.UtcNow.AddSeconds(-1).ToString("o");
            SaveService.Instance.Save();
            Refresh();
        }

        void Refresh()
        {
            var d = SaveService.Instance.Data;
            if (statusText == null) return;
            if (string.IsNullOrEmpty(d.cafeJobCompleteAtIso))
                statusText.text = "Café Shift idle. Assign Wren (or any) for coins.";
            else
                statusText.text = $"Café running (agent={d.cafeAssignedAgentId}) until {d.cafeJobCompleteAtIso}";
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
