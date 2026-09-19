using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;

namespace Heartline.Gacha
{
    public class GachaController : MonoBehaviour
    {
        [SerializeField] TMP_Text ratesText;
        [SerializeField] TMP_Text resultText;

        static readonly string[] Pool = { "mira", "ivy", "juniper", "wren", "n_filler_a", "n_filler_b", "n_filler_c", "n_filler_d" };

        void OnEnable()
        {
            if (ratesText)
                ratesText.text = "Beginner Banner (local stub)\nSSR 1% · SR 10% · R 40% · N 49%\nSoft pity display: SR@10 · SSR@30 · max 40\nNo IAP. Not Now has no penalty.";
            AnalyticsSink.Emit("gacha_open", "banner_id=beginner");
        }

        public void FreeTenPull()
        {
            var d = SaveService.Instance.Data;
            if (d.beginnerFreePullUsed)
            {
                if (resultText) resultText.text = "Free sample already used.";
                return;
            }
            d.beginnerFreePullUsed = true;
            var rng = new System.Random();
            var results = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                // Guarantee SR-ish by pull 10: force wren or ivy on last if all N
                results.Add(Pool[rng.Next(Pool.Length)]);
            }
            if (results.TrueForAll(r => r.StartsWith("n_")))
                results[9] = "wren";
            foreach (var id in results)
            {
                if (id.StartsWith("n_")) continue;
                if (!d.unlockedAgents.Contains(id)) d.unlockedAgents.Add(id);
            }
            SaveService.Instance.Save();
            AnalyticsSink.Emit("gacha_pull", $"banner_id=beginner;count=10;results={string.Join(',', results)}");
            if (resultText) resultText.text = "Results: " + string.Join(", ", results);
        }

        public void NotNow()
        {
            AnalyticsSink.Emit("gacha_open", "banner_id=beginner;action=dismiss");
            SceneManager.LoadScene("02_Hub");
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
