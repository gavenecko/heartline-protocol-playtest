using UnityEngine;
using UnityEngine.SceneManagement;
using Heartline.Save;
using Heartline.Analytics;

namespace Heartline.App
{
    public class TitleController : MonoBehaviour
    {
        public void NewGame()
        {
            SaveService.Instance.ResetAll();
            SaveService.Instance.Data.ageGatePassed = true;
            SaveService.Instance.Data.unlockedAgents = new System.Collections.Generic.List<string> { "mira" };
            SaveService.Instance.Save();
            AnalyticsSink.Emit("ftue_step", "step_id=title_new");
            PlayerPrefs.SetString("hp_pending_dialogue", "title_new");
            SceneManager.LoadScene("02_Hub");
        }

        public void Continue()
        {
            SceneManager.LoadScene("02_Hub");
        }
    }
}
