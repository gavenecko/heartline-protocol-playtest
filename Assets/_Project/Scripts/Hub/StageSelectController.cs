using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.SyncMatch;
using Heartline.Shared;

namespace Heartline.Hub
{
    public class StageSelectController : MonoBehaviour
    {
        [SerializeField] Transform listRoot;
        [SerializeField] Button stageButtonPrefab;
        [SerializeField] TMP_Text infoText;
        [SerializeField] Text infoTextLegacy;

        readonly List<Button> _runtimeButtons = new List<Button>();

        public void BindLegacy(Text info, Transform root = null)
        {
            infoTextLegacy = info;
            if (root != null) listRoot = root;
        }

        void Start() => BuildList();

        public static string FormatStageLabel(StageDefinition s, HeartlineSave save)
        {
            bool unlocked = StageCombatMath.IsUnlocked(s.id, save.clearedStages, save.ch1Complete);
            int stars = save.GetStageStars(s.id);
            string starTxt = stars > 0 ? StageStarRating.FormatStars(stars) : "☆☆☆";
            string lockTxt = "";
            if (!unlocked)
            {
                var reason = StageCombatMath.UnlockReason(s.id, save.clearedStages, save.ch1Complete);
                lockTxt = "  [LOCKED: " + (reason ?? "Locked") + "]";
            }
            return $"{s.id}  {s.name}  · {s.staminaCost} STA  · {starTxt}" + lockTxt;
        }

        void BuildList()
        {
            var stages = ContentRegistry.Stages?.stages;
            if (stages == null)
            {
                SetInfo("Stage table missing — open Boot first so GameDataService loads tables.");
                return;
            }

            if (listRoot != null)
            {
                foreach (Transform c in listRoot) Destroy(c.gameObject);
                _runtimeButtons.Clear();
            }

            var save = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            foreach (var s in stages)
            {
                bool unlocked = StageCombatMath.IsUnlocked(s.id, save.clearedStages, save.ch1Complete);
                string label = FormatStageLabel(s, save);

                if (stageButtonPrefab != null && listRoot != null)
                {
                    var btn = Instantiate(stageButtonPrefab, listRoot);
                    var tmp = btn.GetComponentInChildren<TMP_Text>();
                    if (tmp) tmp.text = label;
                    var legacy = btn.GetComponentInChildren<Text>();
                    if (legacy) legacy.text = label;
                    btn.interactable = unlocked;
                    var captured = s.id;
                    btn.onClick.AddListener(() => SelectStage(captured));
                }
            }

            SetInfo($"Select a stage. STA {save.stamina}/{GameConstants.StaminaCap}. ★ = best clear. Locked rows show unlock reason.");
        }

        public void RegisterRuntimeStageButton(Button btn, string stageId, string label, bool unlocked)
        {
            if (btn == null) return;
            var legacy = btn.GetComponentInChildren<Text>();
            if (legacy) legacy.text = label;
            btn.interactable = unlocked;
            var captured = stageId;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectStage(captured));
            _runtimeButtons.Add(btn);
        }

        public void SelectStage(string stageId)
        {
            var stage = ContentRegistry.GetStage(stageId);
            if (stage == null) return;
            var save = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();

            if (!StageCombatMath.IsUnlocked(stageId, save.clearedStages, save.ch1Complete))
            {
                SetInfo(StageCombatMath.UnlockReason(stageId, save.clearedStages, save.ch1Complete)
                        ?? $"Locked — clear {StageCombatMath.PreviousStageId(stageId) ?? "prior"} first.");
                return;
            }
            if (save.stamina < stage.staminaCost)
            {
                SetInfo($"Not enough Stamina ({save.stamina}/{stage.staminaCost}). Regen 1/{GameConstants.StaminaRegenMinutes} min.");
                return;
            }
            int stars = save.GetStageStars(stageId);
            string best = stars > 0 ? StageStarRating.FormatStars(stars) : "none yet";
            PlayerPrefs.SetString("hp_pending_stage", stageId);
            AnalyticsSink.Emit("stage_start", $"stage_id={stageId}");
            SetInfo($"Entering {stageId} · cost {stage.staminaCost} STA · best {best}");
            SceneManager.LoadScene("08_Battle");
        }

        void SetInfo(string msg) => HudText.Set(infoText, infoTextLegacy, msg);

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
