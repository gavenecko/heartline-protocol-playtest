using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;

namespace Heartline.Meta
{
    public class JobsController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;
        const string CafeId = "cafe_shift";
        const int YieldCoins = 1200;
        const int DurationSecondsDebug = 10;

        public void BindLegacy(Text status) => statusTextLegacy = status;

        void OnEnable() => Refresh();

        public void AssignCafe()
        {
            var d = SaveService.Instance.Data;
            if (!string.IsNullOrEmpty(d.cafeJobCompleteAtIso))
            {
                SetStatus("Café Shift already running.");
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
                SetStatus("No active job.");
                return;
            }
            if (!DateTime.TryParse(d.cafeJobCompleteAtIso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var done) || DateTime.UtcNow < done)
            {
                SetStatus("Still working… (debug timer ~10s)");
                return;
            }
            d.lumaCoins += YieldCoins;
            d.cafeJobCompleteAtIso = null;
            d.cafeAssignedAgentId = null;
            SaveService.Instance.Save();
            AnalyticsSink.Emit("job_claim", $"job_id={CafeId};yield={YieldCoins}");
            SetStatus($"Claimed +{YieldCoins} Luma Coins.");
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
            if (string.IsNullOrEmpty(d.cafeJobCompleteAtIso))
                SetStatus("Café Shift idle. Assign Wren for coins.");
            else
                SetStatus($"Café running (agent={d.cafeAssignedAgentId}) until {d.cafeJobCompleteAtIso}");
        }

        void SetStatus(string msg) => HudText.Set(statusText, statusTextLegacy, msg);

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
