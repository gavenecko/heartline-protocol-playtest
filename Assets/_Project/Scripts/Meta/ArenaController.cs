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
    public class ArenaController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;

        public void BindLegacy(Text status) => statusTextLegacy = status;

        void OnEnable() => Refresh();

        public void Refresh()
        {
            var d = SaveService.Instance.Data;
            ArenaMath.EnsureDay(ref d.arenaUtcDate, ref d.arenaAttacksUsed, DateTime.UtcNow);
            string week = ArenaMath.WeekKeyUtc(DateTime.UtcNow);
            var sb = new StringBuilder();
            sb.AppendLine("Arena · async defense teams (stub)");
            sb.AppendLine(ArenaMath.WeeklyNote(week, d.arenaMarks, d.arenaAttacksUsed));
            sb.AppendLine();
            sb.AppendLine($"Arena Marks: {d.arenaMarks}");
            sb.AppendLine($"Free attacks today: {ArenaMath.AttacksRemaining(d.arenaAttacksUsed)}/{ArenaMath.FreeAttacksPerDay}");
            sb.AppendLine();
            sb.AppendLine("Opponents (fake):");
            for (int i = 0; i < ArenaMath.FakeOpponents.Length; i++)
            {
                var o = ArenaMath.FakeOpponents[i];
                sb.AppendLine($"  · {o.HandlerName}  PR {o.PowerRating}  [{string.Join("/", o.AgentIds)}]");
            }
            sb.AppendLine();
            sb.AppendLine("Weekly ranking rewards settle Monday 00:00 UTC (note only — no shop yet).");
            HudText.Set(statusText, statusTextLegacy, sb.ToString());
        }

        public void AttackNext()
        {
            var d = SaveService.Instance.Data;
            ArenaMath.EnsureDay(ref d.arenaUtcDate, ref d.arenaAttacksUsed, DateTime.UtcNow);
            var opp = ArenaMath.PickOpponent(d.arenaAttacksUsed);
            if (!ArenaMath.TryAttack(ref d.arenaAttacksUsed, ref d.arenaMarks, opp, out bool won, out int gained, out var err))
            {
                Refresh();
                if (statusTextLegacy != null) statusTextLegacy.text += "\n\n" + err;
                return;
            }
            if (won)
            {
                d.arenaWinsLifetime++;
                if (d.arenaWinsLifetime >= 3)
                    SaveService.Instance.UnlockAgent("rook");
            }
            SaveService.Instance.Save();
            AnalyticsSink.Emit("arena_attack", $"won={won};marks={gained};opp={opp.HandlerName}");
            Refresh();
            if (statusTextLegacy != null)
            {
                var secret = (won && d.arenaWinsLifetime >= 3 && d.unlockedAgents != null && d.unlockedAgents.Contains("rook"))
                    ? " · Secret agent Rook Hale unlocked!" : "";
                statusTextLegacy.text += $"\n\nvs {opp.HandlerName}: {(won ? "WIN" : "LOSS")} · +{gained} Arena Marks (career wins {d.arenaWinsLifetime})" + secret;
            }
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
