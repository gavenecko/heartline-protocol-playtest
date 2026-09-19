using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;

namespace Heartline.Gacha
{
    public class GachaController : MonoBehaviour
    {
        [SerializeField] TMP_Text ratesText;
        [SerializeField] TMP_Text resultText;
        [SerializeField] Text ratesTextLegacy;
        [SerializeField] Text resultTextLegacy;

        public void BindLegacy(Text rates, Text result)
        {
            ratesTextLegacy = rates;
            resultTextLegacy = result;
        }

        void OnEnable()
        {
            RefreshRates();
            AnalyticsSink.Emit("gacha_open", "banner_id=beginner");
        }

        void RefreshRates()
        {
            var d = SaveService.Instance.Data;
            HudText.Set(ratesText, ratesTextLegacy,
                GachaMath.RatesUiText() +
                $"\nPity: SR {d.beginnerPitySr}/{GachaMath.SoftPitySr} · SSR {d.beginnerPitySsr}/{GachaMath.HardPitySsr}" +
                $"\nEcho Tickets {d.beginnerTickets} · PC {d.pulseCrystals}" +
                (d.beginnerFreePullUsed ? "\nFree sample: used" : "\nFree sample: available"));
        }

        public void FreeTenPull()
        {
            var d = SaveService.Instance.Data;
            if (d.beginnerFreePullUsed)
            {
                HudText.Set(resultText, resultTextLegacy, "Free sample already used. Use Tickets or Crystals.");
                return;
            }
            d.beginnerFreePullUsed = true;
            var summary = RunTenPull(d, new System.Random());
            SaveService.Instance.Save();
            AnalyticsSink.Emit("gacha_pull", "banner_id=beginner;count=10;paid=free;" + summary.Analytics);
            HudText.Set(resultText, resultTextLegacy, summary.Display);
            RefreshRates();
        }

        public void PullWithTickets()
        {
            var d = SaveService.Instance.Data;
            if (!GachaMath.TrySpendTenPull(ref d.beginnerTickets, ref d.pulseCrystals, useTickets: true, out var err))
            {
                HudText.Set(resultText, resultTextLegacy, err);
                return;
            }
            var summary = RunTenPull(d, new System.Random());
            SaveService.Instance.Save();
            AnalyticsSink.Emit("gacha_pull", "banner_id=beginner;count=10;paid=tickets;" + summary.Analytics);
            HudText.Set(resultText, resultTextLegacy, summary.Display);
            RefreshRates();
        }

        public void PullWithCrystals()
        {
            var d = SaveService.Instance.Data;
            if (!GachaMath.TrySpendTenPull(ref d.beginnerTickets, ref d.pulseCrystals, useTickets: false, out var err))
            {
                HudText.Set(resultText, resultTextLegacy, err);
                return;
            }
            var summary = RunTenPull(d, new System.Random());
            SaveService.Instance.Save();
            AnalyticsSink.Emit("gacha_pull", "banner_id=beginner;count=10;paid=crystals;" + summary.Analytics);
            HudText.Set(resultText, resultTextLegacy, summary.Display);
            RefreshRates();
        }

        sealed class PullSummary
        {
            public string Display;
            public string Analytics;
        }

        static PullSummary RunTenPull(HeartlineSave d, System.Random rng)
        {
            var lines = new List<string>();
            int fragTotal = 0;
            var ids = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var rarity = GachaMath.Roll(rng, ref d.beginnerPitySr, ref d.beginnerPitySsr);
                string agent = GachaMath.PickAgent(rarity, rng);
                ids.Add($"{agent}({rarity})");
                bool owned = !agent.StartsWith("n_") && d.unlockedAgents.Contains(agent);
                if (agent.StartsWith("n_"))
                {
                    fragTotal += GachaMath.FragmentsFor(GachaRarity.N);
                    lines.Add($"{agent} N → +{GachaMath.FragN} frag");
                }
                else if (!owned)
                {
                    d.unlockedAgents.Add(agent);
                    lines.Add($"NEW {agent} {rarity}");
                }
                else
                {
                    int fr = GachaMath.FragmentsFor(rarity);
                    d.AddFragments(agent, fr);
                    fragTotal += fr;
                    lines.Add($"DUP {agent} {rarity} → +{fr} frag");
                }
            }
            var sb = new StringBuilder();
            sb.AppendLine("10-pull results:");
            foreach (var l in lines) sb.AppendLine("· " + l);
            if (fragTotal > 0) sb.AppendLine($"Echo Fragments gained: {fragTotal}");
            return new PullSummary
            {
                Display = sb.ToString(),
                Analytics = $"results={string.Join(',', ids)};frags={fragTotal}"
            };
        }

        public void NotNow()
        {
            AnalyticsSink.Emit("gacha_open", "banner_id=beginner;action=dismiss");
            SceneManager.LoadScene("02_Hub");
        }

        public void Back() => SceneManager.LoadScene("02_Hub");
    }
}
