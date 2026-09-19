using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Shared;
using Heartline.Analytics;

namespace Heartline.Meta
{
    public class TowerController : MonoBehaviour
    {
        [SerializeField] TMP_Text statusText;
        [SerializeField] Text statusTextLegacy;

        public void BindLegacy(Text status) => statusTextLegacy = status;

        void OnEnable() => Refresh();

        static int Cost(int floor) =>
            floor <= 5 ? GameConstants.TowerStaFloor1to5 : GameConstants.TowerStaFloor6to10;

        public void Refresh()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            int next = TowerMath.NextFloor(d.towerHighestCleared);
            var sb = new StringBuilder();
            sb.AppendLine($"Tower / Abyss · slice floors 1–{TowerMath.SliceFloorCount} of {TowerMath.FullFloorCount}");
            sb.AppendLine($"Checkpoint every {TowerMath.CheckpointEvery} · no continue on fail");
            sb.AppendLine();
            sb.AppendLine($"Highest cleared: {d.towerHighestCleared}");
            sb.AppendLine($"Last checkpoint: {(d.towerLastCheckpoint == 0 ? "none" : d.towerLastCheckpoint.ToString())}");
            sb.AppendLine($"Next attempt: floor {next}" + (next > TowerMath.SliceFloorCount ? " (slice complete)" : $" (−{TowerMath.StaminaCost(next, Cost)} STA)"));
            sb.AppendLine($"STA {d.stamina}/{GameConstants.StaminaCap}");
            sb.AppendLine();
            for (int f = 1; f <= TowerMath.SliceFloorCount; f++)
            {
                bool cleared = f <= d.towerHighestCleared;
                bool unlocked = TowerMath.IsFloorUnlocked(f, d.towerHighestCleared);
                string mark = cleared ? "✓" : (unlocked ? "►" : "·");
                string cp = TowerMath.IsCheckpoint(f) ? " [CP]" : "";
                sb.AppendLine($"  {mark} F{f}  STA {TowerMath.StaminaCost(f, Cost)}{cp}");
            }
            HudText.Set(statusText, statusTextLegacy, sb.ToString());
        }

        public void ClearNext()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            int floor = TowerMath.NextFloor(d.towerHighestCleared);
            if (!TowerMath.TryClear(floor, ref d.stamina, ref d.towerHighestCleared, ref d.towerLastCheckpoint,
                    TowerMath.SliceFloorCount, Cost, out var err))
            {
                Refresh();
                if (statusTextLegacy != null) statusTextLegacy.text += "\n\n" + err;
                return;
            }
            if (floor >= 10)
                SaveService.Instance.UnlockAgent("velvet");
            SaveService.Instance.Save();
            AnalyticsSink.Emit("tower_clear", $"floor={floor};cp={d.towerLastCheckpoint}");
            Refresh();
            if (statusTextLegacy != null)
                var bonus = (floor >= 10 && SaveService.Instance.Data.unlockedAgents != null && SaveService.Instance.Data.unlockedAgents.Contains("velvet"))
                    ? " Secret agent Velvet Noir unlocked!" : "";
                statusTextLegacy.text += $"\n\nCleared floor {floor}." + (TowerMath.IsCheckpoint(floor) ? " Checkpoint saved." : "") + bonus;
        }

        public void FailNext()
        {
            var d = SaveService.Instance.Data;
            SaveService.Instance.ApplyRegen();
            int floor = TowerMath.NextFloor(d.towerHighestCleared);
            int cpBefore = d.towerLastCheckpoint;
            if (!TowerMath.TryFail(floor, ref d.stamina, ref d.towerHighestCleared, d.towerLastCheckpoint,
                    TowerMath.SliceFloorCount, Cost, out var err))
            {
                Refresh();
                if (statusTextLegacy != null) statusTextLegacy.text += "\n\n" + err;
                return;
            }
            SaveService.Instance.Save();
            AnalyticsSink.Emit("tower_fail", $"floor={floor};snap={d.towerHighestCleared}");
            Refresh();
            if (statusTextLegacy != null)
                statusTextLegacy.text += $"\n\nFailed floor {floor}. No continue — back to checkpoint {cpBefore} (next = {TowerMath.NextAttemptAfterFail(cpBefore)}).";
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
