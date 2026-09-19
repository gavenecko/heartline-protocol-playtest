using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.SyncMatch;
using Heartline.Analytics;

namespace Heartline.Meta
{
    public class CodexController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;
        [SerializeField] RawImage portraitImage;
        int _index;

        public void BindLegacy(Text status, RawImage portrait = null)
        {
            statusTextLegacy = status;
            if (portrait != null) portraitImage = portrait;
            Refresh();
        }

        void OnEnable()
        {
            _index = 0;
            Refresh();
            AnalyticsSink.Emit("codex_open");
        }

        List<AgentDefinition> Agents()
        {
            var list = new List<AgentDefinition>();
            if (ContentRegistry.Agents?.agents != null)
                list.AddRange(ContentRegistry.Agents.agents);
            return list;
        }

        List<AgentDefinition> VisibleAgents()
        {
            var unlocked = SaveService.Instance.Data.unlockedAgents ?? new List<string>();
            var list = new List<AgentDefinition>();
            foreach (var a in Agents())
            {
                if (a == null) continue;
                if (a.codexHiddenUntilUnlock && !unlocked.Contains(a.id)) continue;
                list.Add(a);
            }
            return list;
        }

        public void Refresh()
        {
            var agents = VisibleAgents();
            var unlocked = SaveService.Instance.Data.unlockedAgents
                           ?? new List<string>();
            var sb = new StringBuilder();
            sb.AppendLine($"Agent Codex · {VisibleAgents().Count} shown · secrets stay hidden until unlocked");
            sb.AppendLine("Playable slice: Mira / Ivy / Juniper / Wren · rest = locked teasers");
            sb.AppendLine();
            if (agents.Count == 0)
            {
                sb.AppendLine("No agents.json loaded.");
                HudText.Set(statusText, statusTextLegacy, sb.ToString());
                return;
            }
            if (_index < 0) _index = 0;
            if (_index >= agents.Count) _index = agents.Count - 1;
            var a = agents[_index];
            bool own = unlocked.Contains(a.id);
            string lockState = own ? "UNLOCKED" : "LOCKED";
            sb.AppendLine($"[{_index + 1}/{agents.Count}] {lockState}");
            sb.AppendLine($"{a.displayName}  ·  {a.codename}");
            sb.AppendLine($"{a.rarity} {a.role} · {a.faction} · age {a.age}");
            if (!string.IsNullOrEmpty(a.roleBlurb))
                sb.AppendLine(a.roleBlurb);
            else if (!string.IsNullOrEmpty(a.contentNotes))
                sb.AppendLine(a.contentNotes);
            sb.AppendLine();
            sb.AppendLine($"Unlock: {a.unlock ?? "—"}");
            if (!own)
                sb.AppendLine(a.secret
                    ? "(SECRET — clear Tower floor 10 to recruit)"
                    : "(Teaser — recruit via story / gacha in live build)");
            sb.AppendLine();
            sb.AppendLine("Roster:");
            foreach (var x in agents)
            {
                bool u = unlocked.Contains(x.id);
                sb.AppendLine($"  {(u ? "★" : "·")} {x.displayName} ({x.rarity}/{x.role})");
            }
            HudText.Set(statusText, statusTextLegacy, sb.ToString());
            if (portraitImage != null && agents.Count > 0)
                ArtLoader.ApplyBust(portraitImage, agents[_index].id);
        }

        public void Next()
        {
            var n = VisibleAgents().Count;
            if (n == 0) return;
            _index = (_index + 1) % n;
            Refresh();
        }

        public void Prev()
        {
            var n = VisibleAgents().Count;
            if (n == 0) return;
            _index = (_index - 1 + n) % n;
            Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
