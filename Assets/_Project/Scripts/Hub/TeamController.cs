using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;

namespace Heartline.Hub
{
    public class TeamController : MonoBehaviour
    {
        [SerializeField] TMP_Text squadText;
        [SerializeField] Text squadTextLegacy;

        public void BindLegacy(Text squad) => squadTextLegacy = squad;

        void OnEnable() => Refresh();

        void Refresh()
        {
            var s = SaveService.Instance.Data.squad;
            var unlocked = string.Join(", ", SaveService.Instance.Data.unlockedAgents);
            string msg =
                $"Front: {s.front0}, {s.front1}\nMid: {s.mid0}, {s.mid1}\nBack: {s.back0}, {s.back1}\n" +
                $"Unlocked: {unlocked}\nSupport slot: LOCKED";
            HudText.Set(squadText, squadTextLegacy, msg);
        }

        public void ApplyDefaultLaneAdvice()
        {
            var s = SaveService.Instance.Data.squad;
            s.front0 = "juniper";
            s.front1 = "ivy";
            s.mid0 = "mira";
            s.mid1 = "";
            s.back0 = SaveService.Instance.Data.unlockedAgents.Contains("wren") ? "wren" : "";
            s.back1 = "";
            SaveService.Instance.Save();
            Refresh();
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
