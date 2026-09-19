using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;
using Heartline.ContentFlags;

namespace Heartline.App
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] Text flagsTextLegacy;

        public void BindLegacy(Text flags) => flagsTextLegacy = flags;

        void OnEnable() => RefreshFlags();

        public void RefreshFlags()
        {
            if (ContentFlagsService.Instance == null) return;
            ContentFlagsService.Instance.Load();
            if (SaveService.Instance?.Data != null)
            {
                var d = SaveService.Instance.Data;
                ContentFlagsService.Instance.SyncFromSave(d.ageGatePassed, d.sfwMode);
                d.sfwMode = true;
            }
            if (flagsTextLegacy != null)
                flagsTextLegacy.text = ContentFlagsService.Instance.TitleFlagsSummary()
                    + $"\nSlice {GameConstants.SliceVersion} · save v{GameConstants.SaveSchemaVersion}";
        }

        public void NewGame()
        {
            if (ContentFlagsService.Instance != null && !ContentFlagsService.Instance.AgeGatePassed)
            {
                if (flagsTextLegacy != null)
                    flagsTextLegacy.text = "Confirm age gate (18+) on Boot before New Game.";
                return;
            }
            SaveService.Instance.ResetAll();
            SaveService.Instance.Data.ageGatePassed = true;
            SaveService.Instance.Data.sfwMode = true;
            SaveService.Instance.Data.unlockedAgents = new System.Collections.Generic.List<string> { "mira" };
            SaveService.Instance.Save();
            ContentFlagsService.Instance?.ConfirmAgeGate();
            ContentFlagsService.Instance?.TrySetSfwMode(true, out _);
            AnalyticsSink.Emit("ftue_step", "step_id=title_new");
            PlayerPrefs.SetString("hp_pending_dialogue", "title_new");
            SceneManager.LoadScene("02_Hub");
        }

        public void Continue()
        {
            if (ContentFlagsService.Instance != null && SaveService.Instance?.Data != null)
            {
                var d = SaveService.Instance.Data;
                ContentFlagsService.Instance.SyncFromSave(d.ageGatePassed, d.sfwMode);
                d.sfwMode = true;
                d.ageGatePassed = ContentFlagsService.Instance.AgeGatePassed;
                SaveService.Instance.Save();
            }
            SceneManager.LoadScene("02_Hub");
        }

        public void OnToggleSfw()
        {
            ContentFlagsService.Instance.TrySetSfwMode(true, out var msg);
            if (SaveService.Instance?.Data != null)
            {
                SaveService.Instance.Data.sfwMode = true;
                SaveService.Instance.Save();
            }
            if (flagsTextLegacy != null)
                flagsTextLegacy.text = ContentFlagsService.Instance.TitleFlagsSummary() + "\n\n" + msg;
        }

        public void OnRequestAdult()
        {
            ContentFlagsService.Instance.TryUnlockAdult(out var msg);
            AnalyticsSink.Emit("adult_path_blocked", "slice=true");
            if (SaveService.Instance?.Data != null)
            {
                SaveService.Instance.Data.sfwMode = true;
                SaveService.Instance.Save();
            }
            if (flagsTextLegacy != null)
                flagsTextLegacy.text = ContentFlagsService.Instance.TitleFlagsSummary() + "\n\n" + msg;
        }

        public void DebugResetSave()
        {
            SaveService.Instance.ResetAll();
            PlayerPrefs.DeleteAll();
            AnalyticsSink.Emit("debug_reset", $"schema={GameConstants.SaveSchemaVersion}");
            SceneManager.LoadScene("01_Title");
        }
    }
}
