using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        [SerializeField] Text dialogueTextLegacy;
        [SerializeField] Text affTextLegacy;
        [SerializeField] Text energyTextLegacy;

        public void BindLegacy(Text dialogue, Text aff, Text energy)
        {
            dialogueTextLegacy = dialogue;
            affTextLegacy = aff;
            energyTextLegacy = energy;
        }

        void OnEnable()
        {
            AnalyticsSink.Emit("room_enter", "agent_id=mira");
            Refresh();
            SetDialogue("Mira: Door's unlocked. That's rare. Come in. (SFW Aff 1–3 · adults 18+)");
            ClaimMorningIfAny();
        }

        void Refresh()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            d.miraAffRank = Rank(d.miraAffXp);
            string next = d.miraAffRank >= 3 ? "cap" : (d.miraAffRank == 1 ? $"{GameConstants.AffRank2Xp}" : $"{GameConstants.AffRank3Xp}");
            HudText.Set(affText, affTextLegacy, $"Mira Aff {d.miraAffRank}/{GameConstants.MaxAffRankSlice}  XP {d.miraAffXp} (→{next})");
            HudText.Set(energyText, energyTextLegacy, $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap} · LC {d.lumaCoins} · Tea×{d.sunwellTeaCount}");
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
                    SetDialogue("Not enough Heart Energy.");
                    Refresh();
                    return;
                }
            }
            d.heartEnergy -= GameConstants.TalkCostHe;
            int before = Rank(d.miraAffXp);
            d.miraAffXp += GameConstants.TalkAffXp;
            CapAffXp(d);
            int after = Rank(d.miraAffXp);
            d.miraAffRank = after;
            SetDialogue(after == 1
                ? "Mira: Sync rates spike when you listen. Annoying scientific fact."
                : after == 2
                    ? "Mira: Friendly is a soft word for covering your shift without billing you."
                    : "Mira: Close. Don't make it weird — just stay.");
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
                SetDialogue("Gifts unlock at Aff Rank 2 (Friendly). Talk more first.");
                return;
            }
            if (d.heartEnergy < GameConstants.GiftCostHe)
            {
                SetDialogue("Not enough Heart Energy.");
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
            CapAffXp(d);
            int after = Rank(d.miraAffXp);
            d.miraAffRank = after;
            SetDialogue(preferred
                ? "Mira: Sunwell Tea. You listened. Dangerous habit."
                : "Mira: Thanks. Not Corp pens — good.");
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
                SetDialogue("Dates unlock at Aff Rank 3 (Close).");
                return;
            }
            if (d.datedMiraToday)
            {
                SetDialogue("Already dated today.");
                return;
            }
            if (d.heartEnergy < GameConstants.DateCostHe)
            {
                SetDialogue("Not enough Heart Energy.");
                return;
            }
            d.heartEnergy -= GameConstants.DateCostHe;
            d.miraAffXp += GameConstants.DateAffXp;
            CapAffXp(d);
            d.miraAffRank = Rank(d.miraAffXp);
            d.datedMiraToday = true;
            SetDialogue("Mira: Pier lights. No badges. Rank Three — Close. (SFW outing)");
            AnalyticsSink.Emit("room_action", $"action=date;xp={GameConstants.DateAffXp};rank={d.miraAffRank}");
            SaveService.Instance.Save();
            Refresh();
        }

        public void Sleepover()
        {
            PlayerPrefs.SetInt("hp_sleepover_mira", 1);
            SetDialogue("Sleepover set. Next Room visit: morning +30 Aff XP. (SFW)");
            SaveService.Instance.Save();
        }

        void ClaimMorningIfAny()
        {
            if (PlayerPrefs.GetInt("hp_sleepover_mira", 0) != 1) return;
            PlayerPrefs.SetInt("hp_sleepover_mira", 0);
            var d = SaveService.Instance.Data;
            d.miraAffXp += GameConstants.SleepoverMorningXp;
            CapAffXp(d);
            d.miraAffRank = Rank(d.miraAffXp);
            SaveService.Instance.Save();
            SetDialogue("Morning light. Mira left tea. +30 Aff XP.");
            Refresh();
        }

        void OnRankUp(int rank)
        {
            AnalyticsSink.Emit("aff_rank_up", $"agent_id=mira;new_rank={rank}");
            if (rank == 2)
            {
                SaveService.Instance.Data.sunwellTeaCount += 2;
                SetDialogue("Mira: Aff Rank Up — Friendly. Gifts unlocked. (+2 Sunwell Tea)");
            }
            else if (rank == 3)
                SetDialogue("Mira: Aff Rank Up — Close. Dates unlocked. Slice Aff cap reached.");
        }

        static void CapAffXp(HeartlineSave d)
        {
            if (d.miraAffXp > GameConstants.AffRank3Xp + 200)
                d.miraAffXp = GameConstants.AffRank3Xp + 200;
        }

        static int Rank(int xp)
        {
            if (xp >= GameConstants.AffRank3Xp) return 3;
            if (xp >= GameConstants.AffRank2Xp) return 2;
            return 1;
        }

        void SetDialogue(string msg) => HudText.Set(dialogueText, dialogueTextLegacy, msg);


        public void BuySunwellTea()
        {
            var d = SaveService.Instance.Data;
            if (!GiftShop.TryBuy("sunwell_tea_t1", ref d.lumaCoins, ref d.sunwellTeaCount, out var err))
            {
                SetDialogue(err);
                Refresh();
                return;
            }
            SaveService.Instance.Save();
            AnalyticsSink.Emit("shop_buy", $"item=sunwell_tea_t1;price={GiftShop.SunwellTeaPriceLc}");
            SetDialogue($"Bought Sunwell Tea (−{GiftShop.SunwellTeaPriceLc} LC). Stock ×{d.sunwellTeaCount}.");
            Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
