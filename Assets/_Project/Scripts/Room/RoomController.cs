using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.Analytics;

namespace Heartline.Room
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] TMP_Text dialogueText;
        [SerializeField] TMP_Text affText;
        [SerializeField] TMP_Text energyText;

        void OnEnable()
        {
            AnalyticsSink.Emit("room_enter", "agent_id=mira");
            Refresh();
            if (dialogueText)
                dialogueText.text = "Mira: Door's unlocked. That's rare. Come in.";
            ClaimMorningIfAny();
        }

        void Refresh()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            d.miraAffRank = Rank(d.miraAffXp);
            if (affText) affText.text = $"Mira Aff {d.miraAffRank}/3  XP {d.miraAffXp}";
            if (energyText) energyText.text = $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap}";
        }

        public void Talk()
        {
            var d = SaveService.Instance.Data;
            if (d.heartEnergy < GameConstants.TalkCostHe)
            {
                if (PlayerPrefs.GetInt("hp_ftue_he_topup", 0) == 0)
                {
                    d.heartEnergy += 20;
                    PlayerPrefs.SetInt("hp_ftue_he_topup", 1);
                }
                else
                {
                    if (dialogueText) dialogueText.text = "Not enough Heart Energy.";
                    Refresh();
                    return;
                }
            }
            d.heartEnergy -= GameConstants.TalkCostHe;
            int before = Rank(d.miraAffXp);
            d.miraAffXp += GameConstants.TalkAffXp;
            int after = Rank(d.miraAffXp);
            d.miraAffRank = after;
            if (dialogueText)
                dialogueText.text = after == 1
                    ? "Mira: Sync rates spike when you listen. Annoying scientific fact."
                    : "Mira: Friendly is a soft word for covering your shift without billing you.";
            AnalyticsSink.Emit("room_action", $"action=talk;xp={GameConstants.TalkAffXp};rank={after}");
            if (after > before) OnRankUp(after);
            SaveService.Instance.Save();
            Refresh();
        }

        public void Gift()
        {
            var d = SaveService.Instance.Data;
            if (Rank(d.miraAffXp) < 2)
            {
                if (dialogueText) dialogueText.text = "Gifts unlock at Aff Rank 2.";
                return;
            }
            if (d.heartEnergy < GameConstants.GiftCostHe)
            {
                if (dialogueText) dialogueText.text = "Not enough Heart Energy.";
                return;
            }
            bool preferred = d.sunwellTeaCount > 0;
            if (preferred) d.sunwellTeaCount--;
            d.heartEnergy -= GameConstants.GiftCostHe;
            int xp = preferred
                ? Mathf.RoundToInt(GameConstants.GiftAffXp * GameConstants.GiftPreferredMul)
                : GameConstants.GiftAffXp;
            int before = Rank(d.miraAffXp);
            d.miraAffXp += xp;
            int after = Rank(d.miraAffXp);
            d.miraAffRank = after;
            if (dialogueText)
                dialogueText.text = preferred
                    ? "Mira: Sunwell Tea. You listened. Dangerous habit."
                    : "Mira: Thanks. Not Corp pens — good.";
            AnalyticsSink.Emit("room_action", $"action=gift;xp={xp};rank={after}");
            if (after > before) OnRankUp(after);
            SaveService.Instance.Save();
            Refresh();
        }

        public void Date()
        {
            var d = SaveService.Instance.Data;
            if (Rank(d.miraAffXp) < 3)
            {
                if (dialogueText) dialogueText.text = "Dates unlock at Aff Rank 3.";
                return;
            }
            if (d.datedMiraToday)
            {
                if (dialogueText) dialogueText.text = "Already dated today.";
                return;
            }
            if (d.heartEnergy < GameConstants.DateCostHe)
            {
                if (dialogueText) dialogueText.text = "Not enough Heart Energy.";
                return;
            }
            d.heartEnergy -= GameConstants.DateCostHe;
            d.miraAffXp += GameConstants.DateAffXp;
            d.miraAffRank = Rank(d.miraAffXp);
            d.datedMiraToday = true;
            if (dialogueText) dialogueText.text = "Mira: Pier lights. No badges. Rank Three — Close.";
            AnalyticsSink.Emit("room_action", $"action=date;xp={GameConstants.DateAffXp};rank={d.miraAffRank}");
            SaveService.Instance.Save();
            Refresh();
        }

        public void Sleepover()
        {
            PlayerPrefs.SetInt("hp_sleepover_mira", 1);
            if (dialogueText) dialogueText.text = "Sleepover set. Morning: +30 Aff XP.";
            SaveService.Instance.Save();
        }

        void ClaimMorningIfAny()
        {
            if (PlayerPrefs.GetInt("hp_sleepover_mira", 0) != 1) return;
            PlayerPrefs.SetInt("hp_sleepover_mira", 0);
            var d = SaveService.Instance.Data;
            d.miraAffXp += GameConstants.SleepoverMorningXp;
            d.miraAffRank = Rank(d.miraAffXp);
            SaveService.Instance.Save();
            if (dialogueText) dialogueText.text = "Morning light. Mira left tea. +30 Aff XP.";
            Refresh();
        }

        void OnRankUp(int rank)
        {
            AnalyticsSink.Emit("aff_rank_up", $"agent_id=mira;new_rank={rank}");
            if (rank == 2)
            {
                SaveService.Instance.Data.sunwellTeaCount += 2;
                if (dialogueText) dialogueText.text += "\n[Aff Rank Up — Friendly. Gifts unlocked.]";
            }
            else if (rank == 3 && dialogueText)
                dialogueText.text += "\n[Aff Rank Up — Close. Dates unlocked.]";
        }

        static int Rank(int xp)
        {
            if (xp >= GameConstants.AffRank3Xp) return 3;
            if (xp >= GameConstants.AffRank2Xp) return 2;
            return 1;
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
