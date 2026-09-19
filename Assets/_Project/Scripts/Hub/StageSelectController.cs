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
            foreach (var s in stages)
            {
                bool unlocked = StageCombatMath.IsUnlocked(s.id, save.clearedStages, save.ch1Complete);
                bool cleared = save.clearedStages.Contains(s.id);
                string label = $"{s.id}  {s.name}  ({s.staminaCost} STA)" +
                               (cleared ? " ✓" : "") +
                               (!unlocked ? "  [LOCKED]" : "");

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

            SetInfo("Select a stage. Clear in order 1-1 → 1-10. Optional 1-B after boss.");
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
                SetInfo($"Locked — clear {StageCombatMath.PreviousStageId(stageId) ?? "prior"} first.");
                return;
            }
            if (save.stamina < stage.staminaCost)
            {
                SetInfo($"Not enough Stamina ({save.stamina}/{stage.staminaCost}).");
                return;
            }
            PlayerPrefs.SetString("hp_pending_stage", stageId);
            AnalyticsSink.Emit("stage_start", $"stage_id={stageId}");
            SceneManager.LoadScene("08_Battle");
        }

        void SetInfo(string msg) => HudText.Set(infoText, infoTextLegacy, msg);

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
