using System;
using System.Text;
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
        bool _debugAccel = true; // slice playtest default

        public void BindLegacy(Text status) => statusTextLegacy = status;

        void OnEnable()
        {
            SaveService.Instance.Data.EnsureJobSlots();
            Refresh();
        }

        public void AssignCafe() => Assign("cafe_shift");
        public void AssignPier() => Assign("pier_patrol");
        public void AssignClinic() => Assign("clinic_assist");

        public void ClaimCafe() => ClaimOne("cafe_shift");

        void Assign(string jobId)
        {
            var def = JobMath.Get(jobId);
            if (def == null) return;
            var d = SaveService.Instance.Data;
            d.EnsureJobSlots();
            var slot = d.GetOrCreateJobSlot(jobId);
            if (!string.IsNullOrEmpty(slot.completeAtIso))
            {
                SetStatus($"{def.DisplayName} already running.");
                return;
            }
            string agent = PreferUnlocked(def.PreferredAgentId, d);
            slot.assignedAgentId = agent;
            slot.completeAtIso = JobMath.CompleteAtUtc(def, DateTime.UtcNow, _debugAccel).ToString("o");
            SaveService.Instance.Save();
            AnalyticsSink.Emit("job_start", $"job_id={jobId};agent={agent}");
            Refresh();
        }

        static string PreferUnlocked(string preferred, HeartlineSave d)
        {
            if (!string.IsNullOrEmpty(preferred) && d.unlockedAgents != null && d.unlockedAgents.Contains(preferred))
                return preferred;
            if (d.unlockedAgents != null && d.unlockedAgents.Count > 0)
                return d.unlockedAgents[0];
            return preferred ?? "mira";
        }

        public void ClaimOne(string jobId)
        {
            var def = JobMath.Get(jobId);
            var d = SaveService.Instance.Data;
            d.EnsureJobSlots();
            var slot = d.GetOrCreateJobSlot(jobId);
            if (string.IsNullOrEmpty(slot.completeAtIso))
            {
                SetStatus($"No active {def?.DisplayName ?? jobId}.");
                return;
            }
            if (!JobMath.IsReady(slot.completeAtIso, DateTime.UtcNow))
            {
                SetStatus($"{def.DisplayName} still working… (debug timer ~{def.DebugAcceleratedSeconds}s)");
                return;
            }
            int yield = JobMath.ComputePayout(def, slot.assignedAgentId);
            bool bonus = !string.IsNullOrEmpty(def.PreferredAgentId) &&
                         string.Equals(slot.assignedAgentId, def.PreferredAgentId, StringComparison.OrdinalIgnoreCase);
            d.lumaCoins += yield;
            slot.completeAtIso = null;
            string agent = slot.assignedAgentId;
            slot.assignedAgentId = null;
            SaveService.Instance.Save();
            AnalyticsSink.Emit("job_claim", $"job_id={jobId};yield={yield};bonus={bonus}");
            SetStatus($"Claimed {def.DisplayName}: +{yield} LC" + (bonus ? $" (preferred {agent} ×{def.PreferredBonusMul:0.#})" : "") + ".");
            Refresh();
        }

        public void ClaimAll()
        {
            var d = SaveService.Instance.Data;
            d.EnsureJobSlots();
            int total = 0;
            int claimed = 0;
            var sb = new StringBuilder();
            foreach (var def in JobMath.Catalog)
            {
                var slot = d.GetOrCreateJobSlot(def.Id);
                if (string.IsNullOrEmpty(slot.completeAtIso)) continue;
                if (!JobMath.IsReady(slot.completeAtIso, DateTime.UtcNow)) continue;
                int yield = JobMath.ComputePayout(def, slot.assignedAgentId);
                d.lumaCoins += yield;
                total += yield;
                claimed++;
                sb.Append($"{def.DisplayName}+{yield} ");
                AnalyticsSink.Emit("job_claim", $"job_id={def.Id};yield={yield};batch=1");
                slot.completeAtIso = null;
                slot.assignedAgentId = null;
            }
            SaveService.Instance.Save();
            if (claimed == 0)
                SetStatus("Nothing ready to claim.");
            else
                SetStatus($"Claim-all: {claimed} jobs → +{total} LC. {sb}");
            Refresh();
        }

        public void DebugCompleteNow()
        {
            var d = SaveService.Instance.Data;
            d.EnsureJobSlots();
            foreach (var slot in d.jobSlots)
            {
                if (slot == null || string.IsNullOrEmpty(slot.completeAtIso)) continue;
                slot.completeAtIso = DateTime.UtcNow.AddSeconds(-1).ToString("o");
            }
            SaveService.Instance.Save();
            Refresh();
        }

        void Refresh()
        {
            var d = SaveService.Instance.Data;
            d.EnsureJobSlots();
            var sb = new StringBuilder();
            sb.AppendLine("Jobs (timed idle · preferred agent +20% LC)");
            foreach (var def in JobMath.Catalog)
            {
                var slot = d.GetOrCreateJobSlot(def.Id);
                string state;
                if (string.IsNullOrEmpty(slot.completeAtIso))
                    state = "idle — tap Assign";
                else if (JobMath.IsReady(slot.completeAtIso, DateTime.UtcNow))
                    state = $"READY ({slot.assignedAgentId}) claim +{JobMath.ComputePayout(def, slot.assignedAgentId)}";
                else
                    state = $"running ({slot.assignedAgentId}) until {slot.completeAtIso}";
                sb.AppendLine($"· {def.DisplayName} [{def.DurationMinutes}m / {def.YieldLumaCoins}LC pref={def.PreferredAgentId}]: {state}");
            }
            SetStatus(sb.ToString());
        }

        void SetStatus(string msg) => HudText.Set(statusText, statusTextLegacy, msg);

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
