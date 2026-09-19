using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.Analytics;

namespace Heartline.Meta
{
    public class DailyMissionsController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;

        public void BindLegacy(Text status) => statusTextLegacy = status;

        void OnEnable() => Refresh();

        public void Refresh()
        {
            var d = SaveService.Instance.Data;
            d.EnsureDailyDay(DateTime.UtcNow);
            var progress = d.DailyProgressMap();
            var sb = new StringBuilder();
            sb.AppendLine($"Daily Missions · UTC {d.dailyUtcDate}");
            sb.AppendLine("Claim all three ≈ 1200 LC + 50 PC + 1 Echo Ticket");
            sb.AppendLine();
            foreach (var m in DailyMissionMath.Catalog)
            {
                int cur = DailyMissionMath.GetProgress(progress, m.Id);
                bool done = DailyMissionMath.IsComplete(progress, m);
                bool claimed = DailyMissionMath.IsClaimed(d.dailyClaimedIds, m.Id);
                string state = claimed ? "CLAIMED" : (done ? "READY" : "…");
                string reward = $"+{m.RewardLuma} LC";
                if (m.RewardCrystals > 0) reward += $" +{m.RewardCrystals} PC";
                if (m.RewardTickets > 0) reward += $" +{m.RewardTickets} Tix";
                sb.AppendLine($"[{state}] {m.Title}  {cur}/{m.Target}  → {reward}");
            }
            sb.AppendLine();
            sb.AppendLine($"Wallet: LC {d.lumaCoins} · PC {d.pulseCrystals} · Tix {d.beginnerTickets}");
            HudText.Set(statusText, statusTextLegacy, sb.ToString());
        }

        public void ClaimClearStage() => Claim("clear_stage");
        public void ClaimRoomTalk() => Claim("room_talk");
        public void ClaimJob() => Claim("claim_job");

        public void ClaimAllReady()
        {
            Claim("clear_stage", quiet: true);
            Claim("room_talk", quiet: true);
            Claim("claim_job", quiet: true);
            Refresh();
        }

        void Claim(string id, bool quiet = false)
        {
            var d = SaveService.Instance.Data;
            d.EnsureDailyDay(DateTime.UtcNow);
            var progress = d.DailyProgressMap();
            if (d.dailyClaimedIds == null) d.dailyClaimedIds = new System.Collections.Generic.List<string>();
            if (!DailyMissionMath.TryClaim(id, progress, d.dailyClaimedIds,
                    ref d.lumaCoins, ref d.pulseCrystals, ref d.beginnerTickets, out var err))
            {
                if (!quiet)
                {
                    Refresh();
                    if (statusTextLegacy != null) statusTextLegacy.text += $"\n\n{err}";
                }
                return;
            }
            d.WriteDailyProgress(progress);
            SaveService.Instance.Save();
            AnalyticsSink.Emit("daily_claim", $"mission_id={id}");
            if (!quiet) Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
