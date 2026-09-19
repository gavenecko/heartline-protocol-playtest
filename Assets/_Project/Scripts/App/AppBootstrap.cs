using UnityEngine;
using UnityEngine.SceneManagement;
using Heartline.ContentFlags;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;

namespace Heartline.App
{
    public class AppBootstrap : MonoBehaviour
    {
        [SerializeField] string titleScene = "01_Title";
        [SerializeField] string hubScene = "02_Hub";

        void Start()
        {
            EnsureServices();
            ContentFlagsService.Instance.Load();
            AnalyticsSink.Emit("app_boot", $"ver={GameConstants.SliceVersion}");
            if (!ContentFlagsService.Instance.AgeGatePassed)
                return;
            var save = SaveService.Instance.Data;
            var target = (save.ch1Complete || save.clearedStages.Count > 0) ? hubScene : titleScene;
            SceneManager.LoadScene(target);
        }

        public void OnAgeConfirm()
        {
            ContentFlagsService.Instance.ConfirmAgeGate();
            AnalyticsSink.Emit("age_gate_result", "passed=true");
            SaveService.Instance.Data.ageGatePassed = true;
            SaveService.Instance.Save();
            SceneManager.LoadScene(titleScene);
        }

        public void OnAgeExit()
        {
            AnalyticsSink.Emit("age_gate_result", "passed=false");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        static void EnsureServices()
        {
            if (ContentFlagsService.Instance == null)
                new GameObject("ContentFlags").AddComponent<ContentFlagsService>();
            if (SaveService.Instance == null)
                new GameObject("Save").AddComponent<SaveService>();
            if (GameDataService.Instance == null)
                new GameObject("GameData").AddComponent<GameDataService>();
        }
    }
}
