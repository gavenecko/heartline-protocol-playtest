using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.App;
using Heartline.Save;
using Heartline.Analytics;

namespace Heartline.Hub
{
    public class StageSelectController : MonoBehaviour
    {
        [SerializeField] Transform listRoot;
        [SerializeField] Button stageButtonPrefab;
        [SerializeField] TMP_Text infoText;

        void Start() => BuildList();

        void BuildList()
        {
            var stages = GameDataService.Instance?.Stages?.stages;
            if (stages == null || listRoot == null) return;
            foreach (Transform c in listRoot) Destroy(c.gameObject);
            foreach (var s in stages)
            {
                if (stageButtonPrefab == null) break;
                var btn = Instantiate(stageButtonPrefab, listRoot);
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label) label.text = $"{s.id}  {s.name}  ({s.staminaCost} STA)";
                var captured = s.id;
                btn.onClick.AddListener(() => SelectStage(captured));
            }
        }

        public void SelectStage(string stageId)
        {
            var stage = GameDataService.Instance.Stages.stages.Find(x => x.id == stageId);
            if (stage == null) return;
            var save = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            if (save.stamina < stage.staminaCost)
            {
                if (infoText) infoText.text = "Not enough Stamina.";
                return;
            }
            PlayerPrefs.SetString("hp_pending_stage", stageId);
            AnalyticsSink.Emit("stage_start", $"stage_id={stageId}");
            SceneManager.LoadScene("08_Battle");
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
