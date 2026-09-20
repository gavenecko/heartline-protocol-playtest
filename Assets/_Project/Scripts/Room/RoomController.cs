using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
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
        [SerializeField] RawImage portraitImage;
        [SerializeField] RawImage backgroundImage;

        /// <summary>Portrait agent: mira by default; can cycle to unlocked velvet/rook for idle preview.</summary>
        string _roomAgentId = "mira";
        VideoPlayer _idlePlayer;
        RenderTexture _idleRt;

        public void BindLegacy(Text dialogue, Text aff, Text energy, RawImage portrait = null, RawImage background = null)
        {
            dialogueTextLegacy = dialogue;
            affTextLegacy = aff;
            energyTextLegacy = energy;
            if (portrait != null) portraitImage = portrait;
            if (background != null) backgroundImage = background;
            ApplyArt();
        }

        void OnEnable()
        {
            _roomAgentId = "mira";
            AnalyticsSink.Emit("room_enter", $"agent_id={_roomAgentId}");
            ApplyArt();
            Refresh();
            SetDialogue("Mira: Door's unlocked. That's rare. Come in. (SFW Aff 1–3 · adults 18+)");
            ClaimMorningIfAny();
        }

        void OnDisable()
        {
            IdleVideo.Dispose(ref _idlePlayer, ref _idleRt);
        }

        List<string> RoomPortraitAgents()
        {
            var list = new List<string> { "mira" };
            var unlocked = SaveService.Instance.Data.unlockedAgents;
            if (unlocked == null) return list;
            if (unlocked.Contains("velvet")) list.Add("velvet");
            if (unlocked.Contains("rook")) list.Add("rook");
            return list;
        }

        public void CycleRoomAgent()
        {
            var agents = RoomPortraitAgents();
            int idx = agents.IndexOf(_roomAgentId);
            if (idx < 0) idx = 0;
            idx = (idx + 1) % agents.Count;
            _roomAgentId = agents[idx];
            ApplyArt();
            Refresh();
            if (_roomAgentId == "mira")
                SetDialogue("Mira: Back to my room. Talk / Gift / Date still run on my Aff track.");
            else if (_roomAgentId == "velvet")
                SetDialogue("Velvet: Soft lighting. Slice room actions stay on Mira — this is idle preview.");
            else if (_roomAgentId == "rook")
                SetDialogue("Rook: …Don't stare. Idle loop only — Aff still Mira's.");
            else
                SetDialogue($"Room portrait: {_roomAgentId}");
            AnalyticsSink.Emit("room_agent_cycle", $"agent_id={_roomAgentId}");
        }

        void ApplyArt()
        {
            if (backgroundImage != null)
            {
                if (!ArtLoader.ApplyRoomBg(backgroundImage, ArtCatalog.RoomSuiteEvening))
                    ArtLoader.ApplyRoomBg(backgroundImage, ArtCatalog.RoomAgentLoft);
            }
            if (portraitImage == null) return;
            bool preferIdle = IdleVideo.SupportsIdle(_roomAgentId);
            ArtLoader.ApplyPortraitOrIdle(portraitImage, _roomAgentId, ref _idlePlayer, ref _idleRt, preferIdle);
        }

        void Refresh()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            d.miraAffRank = Rank(d.miraAffXp);
            string next = d.miraAffRank >= 3 ? "cap" : (d.miraAffRank == 1 ? $"{GameConstants.AffRank2Xp}" : $"{GameConstants.AffRank3Xp}");
            string guest = _roomAgentId == "mira" ? "" : $" · viewing {_roomAgentId}";
            HudText.Set(affText, affTextLegacy, $"Mira Aff {d.miraAffRank}/{GameConstants.MaxAffRankSlice}  XP {d.miraAffXp} (→{next}){guest}");
            HudText.Set(energyText, energyTextLegacy, $"HE {d.heartEnergy}/{GameConstants.HeartEnergyCap} · LC {d.lumaCoins} · Tea×{d.sunwellTeaCount}");
        }

        public void Talk()
        {
            if (_roomAgentId != "mira")
            {
                SetDialogue("Talk / Gift / Date are Mira-only in the slice. Cycle back to Mira.");
                return;
            }
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
            d.AddDailyProgress("room_talk");
            if (after > before) OnRankUp(after);
            SaveService.Instance.Save();
            Refresh();
        }

        public void Gift()
        {
            if (_roomAgentId != "mira")
            {
                SetDialogue("Talk / Gift / Date are Mira-only in the slice. Cycle back to Mira.");
                return;
            }
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
            if (_roomAgentId != "mira")
            {
                SetDialogue("Talk / Gift / Date are Mira-only in the slice. Cycle back to Mira.");
                return;
            }
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
            if (_roomAgentId != "mira")
            {
                SetDialogue("Sleepover is Mira-only in the slice. Cycle back to Mira.");
                return;
            }
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
