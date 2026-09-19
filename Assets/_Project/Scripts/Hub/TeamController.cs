using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.App;

namespace Heartline.Hub
{
    public class TeamController : MonoBehaviour
    {
        [SerializeField] TMP_Text squadText;
        [SerializeField] Text squadTextLegacy;

        public void BindLegacy(Text squad) => squadTextLegacy = squad;

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
            var unlocked = string.Join(", ", d.unlockedAgents);
            bool bond = BondMatrix.IsMiraIvyBondActive(SquadLogic.EnumerateSlots(
                s.front0, s.front1, s.mid0, s.mid1, s.back0, s.back1));
            int filled = SquadLogic.CountFilled(s.front0, s.front1, s.mid0, s.mid1, s.back0, s.back1);
            string msg =
                $"Owned: {unlocked}\n" +
                $"Squad {filled}/{SquadLogic.MaxSquadSize} (max 2/lane)\n" +
                $"Front: [{Slot(s.front0)}] [{Slot(s.front1)}]\n" +
                $"Mid:   [{Slot(s.mid0)}] [{Slot(s.mid1)}]\n" +
                $"Back:  [{Slot(s.back0)}] [{Slot(s.back1)}]\n" +
                $"Support slot: LOCKED\n" +
                $"Bond Matrix: {(bond ? "Mira↔Ivy ACTIVE (+15% ATK once/battle)" : "Mira↔Ivy inactive (need both)")}";
            if (DialogueService.Instance != null && DialogueService.Instance.IsPlaying)
                msg += "\n\n" + DialogueService.Instance.CurrentDisplay() + "\n[Advance Dialogue]";
            HudText.Set(squadText, squadTextLegacy, msg);
        }

        static string Slot(string id) => string.IsNullOrEmpty(id) ? "—" : id;

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
