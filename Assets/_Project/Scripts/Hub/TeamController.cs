using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.App;
using Heartline.Analytics;
using Heartline.SyncMatch;

namespace Heartline.Hub
{
    public class TeamController : MonoBehaviour
    {
        [SerializeField] TMP_Text squadText;
        [SerializeField] Text squadTextLegacy;
        [SerializeField] RawImage portraitImage;
        [SerializeField] RawImage frameImage;
        string _selectedId = "mira";

        public void BindLegacy(Text squad, RawImage portrait = null, RawImage frame = null)
        {
            squadTextLegacy = squad;
            if (portrait != null) portraitImage = portrait;
            if (frame != null) frameImage = frame;
            Refresh();
        }

        void OnEnable()
        {
            if (DialogueService.Instance != null)
                DialogueService.Instance.TryStartTrigger("team_enter");
            Refresh();
        }

        public void Refresh()
        {
            var d = SaveService.Instance.Data;
            var s = d.squad;
            bool bond = BondMatrix.IsMiraIvyBondActive(SquadLogic.EnumerateSlots(
                s.front0, s.front1, s.mid0, s.mid1, s.back0, s.back1));
            int filled = SquadLogic.CountFilled(s.front0, s.front1, s.mid0, s.mid1, s.back0, s.back1);

            var sb = new StringBuilder();
            sb.AppendLine($"Squad {filled}/{SquadLogic.MaxSquadSize} (max 2/lane)");
            sb.AppendLine($"Front: [{Slot(s.front0)}] [{Slot(s.front1)}]");
            sb.AppendLine($"Mid:   [{Slot(s.mid0)}] [{Slot(s.mid1)}]");
            sb.AppendLine($"Back:  [{Slot(s.back0)}] [{Slot(s.back1)}]");
            sb.AppendLine($"Bond: {(bond ? "Mira↔Ivy ACTIVE" : "Mira↔Ivy inactive")}");
            sb.AppendLine();
            sb.AppendLine("— Roster (Lv / ★ / XP) —");
            if (d.unlockedAgents != null)
            {
                foreach (var id in d.unlockedAgents)
                {
                    var a = Ensure(d, id);
                    int cap = GrowthMath.LevelCapForStars(a.stars);
                    int need = a.level >= cap ? 1 : GrowthMath.XpToNextLevel(a.level);
                    string bar = a.level >= cap
                        ? "[██████████] MAX"
                        : $"{GrowthMath.XpBar(a.xp, need, 8)} {a.xp}/{need}";
                    string sel = id == _selectedId ? "►" : " ";
                    int frags = d.GetFragments(id);
                    sb.AppendLine($"{sel}{id}  Lv{a.level}/{cap}  {GrowthMath.FormatStars(a.stars)}  {bar}  frags {frags}");
                }
            }
            sb.AppendLine();
            sb.AppendLine($"Selected: {_selectedId} · Promote spends Echo Fragments + LC");
            if (!string.IsNullOrEmpty(_statusNote))
                sb.AppendLine(_statusNote);
            if (DialogueService.Instance != null && DialogueService.Instance.IsPlaying)
                sb.AppendLine().AppendLine(DialogueService.Instance.CurrentDisplay()).AppendLine("[Advance Dialogue]");
            HudText.Set(squadText, squadTextLegacy, sb.ToString());
            if (portraitImage != null)
                ArtLoader.ApplyBust(portraitImage, _selectedId);
            if (frameImage != null)
                ArtLoader.ApplyRarityFrame(frameImage, RarityFor(_selectedId));
        }

        static string RarityFor(string agentId)
        {
            var a = ContentRegistry.GetAgent(agentId);
            return a != null && !string.IsNullOrEmpty(a.rarity) ? a.rarity : "N";
        }

        static AgentSave Ensure(HeartlineSave d, string id)
        {
            int stars = id switch
            {
                "mira" => 4,
                "wren" => 3,
                "ivy" => 2,
                "juniper" => 2,
                _ => 1
            };
            int level = id == "mira" ? 5 : 3;
            var a = d.GetOrCreateAgent(id, level, stars);
            if (a.stars < 1) a.stars = stars;
            return a;
        }

        static string Slot(string id) => string.IsNullOrEmpty(id) ? "—" : id;

        public void CycleSelected()
        {
            var d = SaveService.Instance.Data;
            if (d.unlockedAgents == null || d.unlockedAgents.Count == 0) return;
            int idx = d.unlockedAgents.IndexOf(_selectedId);
            idx = (idx + 1) % d.unlockedAgents.Count;
            _selectedId = d.unlockedAgents[idx];
            Refresh();
        }

        string _statusNote;

        public void PromoteSelected()
        {
            var d = SaveService.Instance.Data;
            var a = Ensure(d, _selectedId);
            int frags = d.GetFragments(_selectedId);
            int luma = d.lumaCoins;
            int stars = a.stars;
            if (!GrowthMath.TryPromote(ref stars, ref luma, ref frags, d.miraAffRank, out int newCap, out var err))
            {
                _statusNote = $"Promote failed: {err}";
                Refresh();
                return;
            }
            d.lumaCoins = luma;
            SetFragmentsAbsolute(d, _selectedId, frags);
            a.stars = stars;
            if (a.level > newCap)
            {
                a.level = newCap;
                a.xp = 0;
            }
            _statusNote = $"Promoted {_selectedId} → {GrowthMath.FormatStars(stars)} (cap Lv{newCap})";
            SaveService.Instance.Save();
            AnalyticsSink.Emit("agent_promote", $"agent_id={_selectedId};stars={stars};cap={newCap}");
            Refresh();
        }

        static void SetFragmentsAbsolute(HeartlineSave d, string agentId, int amount)
        {
            if (d.echoFragmentEntries == null) d.echoFragmentEntries = new List<string>();
            for (int i = 0; i < d.echoFragmentEntries.Count; i++)
            {
                if (d.echoFragmentEntries[i] != null && d.echoFragmentEntries[i].StartsWith(agentId + ":"))
                {
                    d.echoFragmentEntries[i] = agentId + ":" + amount;
                    return;
                }
            }
            d.echoFragmentEntries.Add(agentId + ":" + amount);
        }

        public void CycleSlot(string lane, int slotIndex)
        {
            var d = SaveService.Instance.Data;
            var s = d.squad;
            string current = GetSlot(s, lane, slotIndex);
            var occupied = SquadLogic.OccupiedExcept(current, s.front0, s.front1, s.mid0, s.mid1, s.back0, s.back1);
            string next = SquadLogic.NextOption(current, d.unlockedAgents, occupied);
            string f0 = s.front0, f1 = s.front1, m0 = s.mid0, m1 = s.mid1, b0 = s.back0, b1 = s.back1;
            if (string.IsNullOrEmpty(next))
            {
                ClearSlot(ref f0, ref f1, ref m0, ref m1, ref b0, ref b1, lane, slotIndex);
            }
            else if (!SquadLogic.TryAssign(next, lane, slotIndex, d.unlockedAgents, ref f0, ref f1, ref m0, ref m1, ref b0, ref b1))
            {
                Refresh();
                return;
            }
            s.front0 = f0; s.front1 = f1; s.mid0 = m0; s.mid1 = m1; s.back0 = b0; s.back1 = b1;
            SaveService.Instance.Save();
            Refresh();
        }

        public void CycleFront0() => CycleSlot("Front", 0);
        public void CycleFront1() => CycleSlot("Front", 1);
        public void CycleMid0() => CycleSlot("Mid", 0);
        public void CycleMid1() => CycleSlot("Mid", 1);
        public void CycleBack0() => CycleSlot("Back", 0);
        public void CycleBack1() => CycleSlot("Back", 1);

        static string GetSlot(SquadSave s, string lane, int idx) => lane switch
        {
            "Front" => idx == 0 ? s.front0 : s.front1,
            "Mid" => idx == 0 ? s.mid0 : s.mid1,
            "Back" => idx == 0 ? s.back0 : s.back1,
            _ => ""
        };

        static void ClearSlot(ref string f0, ref string f1, ref string m0, ref string m1, ref string b0, ref string b1, string lane, int idx)
        {
            switch (lane)
            {
                case "Front": if (idx == 0) f0 = ""; else f1 = ""; break;
                case "Mid": if (idx == 0) m0 = ""; else m1 = ""; break;
                case "Back": if (idx == 0) b0 = ""; else b1 = ""; break;
            }
        }

        public void ApplyDefaultLaneAdvice()
        {
            var d = SaveService.Instance.Data;
            var s = d.squad;
            s.front0 = d.unlockedAgents.Contains("juniper") ? "juniper" : "";
            s.front1 = d.unlockedAgents.Contains("ivy") ? "ivy" : "";
            s.mid0 = d.unlockedAgents.Contains("mira") ? "mira" : "";
            s.mid1 = "";
            s.back0 = d.unlockedAgents.Contains("wren") ? "wren" : "";
            s.back1 = "";
            SaveService.Instance.Save();
            Refresh();
        }

        public void AdvanceDialogue()
        {
            if (DialogueService.Instance == null || !DialogueService.Instance.IsPlaying) return;
            DialogueService.Instance.Advance();
            Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
