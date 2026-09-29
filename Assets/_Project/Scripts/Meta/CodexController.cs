using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.SyncMatch;
using AgentDefinition = Heartline.SyncMatch.AgentDefinition;
using Heartline.Analytics;

namespace Heartline.Meta
{
    public class CodexController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;
        [SerializeField] RawImage portraitImage;
        [SerializeField] RawImage frameImage;
        [SerializeField] RawImage fullbodyImage;
        int _index;
        VideoPlayer _idlePlayer;
        RenderTexture _idleRt;
        string _portraitAgentId;
        bool _showFullbody;

        public void BindLegacy(Text status, RawImage portrait = null, RawImage frame = null, RawImage fullbody = null)
        {
            statusTextLegacy = status;
            if (portrait != null) portraitImage = portrait;
            if (frame != null) frameImage = frame;
            if (fullbody != null) fullbodyImage = fullbody;
            Refresh();
        }

        void OnEnable()
        {
            _index = 0;
            _showFullbody = false;
            Refresh();
            AnalyticsSink.Emit("codex_open");
        }

        void OnDisable()
        {
            IdleVideo.Dispose(ref _idlePlayer, ref _idleRt);
            _portraitAgentId = null;
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
            sb.AppendLine(_showFullbody
                ? "View: FULLBODY (Toggle Portrait for bust / idle)"
                : "View: PORTRAIT (Toggle Fullbody when art exists)");
            sb.AppendLine();
            sb.AppendLine("Roster:");
            foreach (var x in agents)
            {
                bool u = unlocked.Contains(x.id);
                sb.AppendLine($"  {(u ? "★" : "·")} {x.displayName} ({x.rarity}/{x.role})");
            }
            HudText.Set(statusText, statusTextLegacy, sb.ToString());
            ApplyPortrait(a, own);
        }

        void ApplyPortrait(AgentDefinition a, bool unlocked)
        {
            if (a == null) return;
            string agentId = a.id;
            string rarity = a.rarity ?? "N";

            if (frameImage != null)
                ArtLoader.ApplyRarityFrame(frameImage, rarity);

            if (_showFullbody)
            {
                IdleVideo.Stop(ref _idlePlayer, ref _idleRt);
                _portraitAgentId = agentId + "#full";
                if (fullbodyImage != null)
                {
                    if (portraitImage != null) portraitImage.enabled = false;
                    fullbodyImage.enabled = true;
                    ArtLoader.ApplyFullbody(fullbodyImage, agentId);
                }
                else if (portraitImage != null)
                {
                    ArtLoader.ApplyFullbody(portraitImage, agentId);
                }
                return;
            }

            if (fullbodyImage != null)
                fullbodyImage.enabled = false;

            if (portraitImage == null) return;
            portraitImage.enabled = true;
            // Idle loops only when unlocked Velvet/Rook; others (and locked) stay still bust.
            bool preferIdle = unlocked && IdleVideo.SupportsIdle(agentId);
            if (_portraitAgentId == agentId && preferIdle && _idlePlayer != null && _idlePlayer.isPlaying)
                return;
            _portraitAgentId = agentId;
            ArtLoader.ApplyPortraitOrIdle(portraitImage, agentId, ref _idlePlayer, ref _idleRt, preferIdle);
        }

        public void ToggleFullbody()
        {
            _showFullbody = !_showFullbody;
            Refresh();
        }

        public void Next()
        {
            var n = VisibleAgents().Count;
            if (n == 0) return;
            _index = (_index + 1) % n;
            _showFullbody = false;
            Refresh();
        }

        public void Prev()
        {
            var n = VisibleAgents().Count;
            if (n == 0) return;
            _index = (_index - 1 + n) % n;
            _showFullbody = false;
            Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
