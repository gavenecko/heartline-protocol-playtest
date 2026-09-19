using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;
using Heartline.App;

namespace Heartline.SyncMatch
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] TMP_Text boardText;
        [SerializeField] TMP_Text logText;
        [SerializeField] TMP_Text enemyHpText;
        [SerializeField] TMP_Text gaugeText;
        [SerializeField] Text boardTextLegacy;
        [SerializeField] Text logTextLegacy;
        [SerializeField] Text enemyHpTextLegacy;
        [SerializeField] Text gaugeTextLegacy;
        [SerializeField] Text laneTextLegacy;
        [SerializeField] Text statusTextLegacy;

        BoardSimulator _board;
        readonly Dictionary<string, SyncGauge> _gauges = new Dictionary<string, SyncGauge>();
        readonly Dictionary<string, AgentDefinition> _agents = new Dictionary<string, AgentDefinition>();
        readonly Dictionary<string, int> _agentHp = new Dictionary<string, int>();
        readonly LaneField _field = new LaneField();
        readonly List<string> _lines = new List<string>();
        int _enemyHp = 1200;
        int _enemyMaxHp = 1200;
        int _enemyAtk = 70;
        int _swapsLeft = 5;
        int? _sx, _sy;
        string _stageId;
        StageDefinition _stage;
        bool _ended;
        int _turnsTaken;
        bool _bondBuffConsumed;
        bool _retreatArmed;
        bool _assistTipShown;
        int _phase = 1;
        bool _showResults;
        string _resultsSummary = "";
        Text resultsTextLegacy;
        readonly System.Collections.Generic.List<(string id, int beforeLv, int afterLv, int xp, int need, int cap)> _xpLines
            = new System.Collections.Generic.List<(string, int, int, int, int, int)>();
        int _rewardLc;
        int _rewardPc;
        int _rewardXp;
        int _rewardStars;
        int _rewardFrags;
        string _fragAgentId;


        public string StageId => _stageId;
        public int EnemyHp => _enemyHp;
        public int SwapsLeft => _swapsLeft;
        public BoardSimulator Board => _board;

        public void BindLegacy(Text board, Text log, Text enemy, Text gauge, Text lane = null, Text status = null, Text results = null)
        {
            boardTextLegacy = board;
            logTextLegacy = log;
            enemyHpTextLegacy = enemy;
            gaugeTextLegacy = gauge;
            laneTextLegacy = lane;
            statusTextLegacy = status;
            resultsTextLegacy = results;
        }

        void Start()
        {
            _stageId = PlayerPrefs.GetString("hp_pending_stage", "1-1");
            _stage = ContentRegistry.GetStage(_stageId);
            _enemyHp = _enemyMaxHp = StageCombatMath.EnemyHp(_stage);
            _enemyAtk = StageCombatMath.EnemyAtk(_stage);

            float prism = (_stage?.enemyTags != null && _stage.enemyTags.Contains("prism_spawn")) ? 0.10f : 0.04f;
            _board = new BoardSimulator(_stageId.GetHashCode(), prism);
            SetupSquad();
            _turnsTaken = 0;
            _bondBuffConsumed = false;
            _retreatArmed = false;
            _assistTipShown = false;
            if (DialogueService.Instance != null)
                DialogueService.Instance.TryStartTrigger("pre_stage", _stageId);
            if (BondMatrix.IsMiraIvyBondActive(_agents.Keys))
                Note("Bond Matrix: Mira↔Ivy Shared Lease ready (+15% ATK once).");
            if (_stage != null && _stage.forceMiraBurst && _gauges.ContainsKey("mira"))
            {
                _gauges["mira"].Set(1f);
                Note("Tutorial: Mira Burst ready — fire it!");
            }
            if (_stage != null)
                Note($"{_stage.id} {_stage.name} · {_stage.teachingGoal}");
            Note("— Player Phase — match gems, fill gauges, fire Bursts.");
            if (_stageId == "1-9" || (_stage?.enemyTags != null && _stage.enemyTags.Contains("telegraph")))
            {
                Note("Tip: Assist Aim highlights a valid ≥3 swap.");
                _assistTipShown = true;
            }
            Redraw();
        }

        void SetupSquad()
        {
            var save = SaveService.Instance.Data;
            var ids = new[]
            {
                save.squad.front0, save.squad.front1,
                save.squad.mid0, save.squad.mid1,
                save.squad.back0, save.squad.back1
            };
            foreach (var id in ids.Where(i => !string.IsNullOrEmpty(i) && save.unlockedAgents.Contains(i)).Distinct())
            {
                var def = ContentRegistry.GetAgent(id);
                if (def == null) continue;
                _agents[id] = def;
                _gauges[id] = new SyncGauge();
                var lane = PreferredLaneFromSquad(id, save.squad) ?? def.PreferredLane();
                _field.Assign(id, lane);
                var stats = def.StatsAtLevel(LevelOf(id));
                _agentHp[id] = stats.hp;
            }
            if (_agents.Count == 0)
            {
                var mira = ContentRegistry.GetAgent("mira");
                if (mira != null)
                {
                    _agents["mira"] = mira;
                    _gauges["mira"] = new SyncGauge();
                    _field.Assign("mira", Lane.Mid);
                    _agentHp["mira"] = mira.StatsAtLevel(5).hp;
                }
            }
        }

        static Lane? PreferredLaneFromSquad(string id, SquadSave squad)
        {
            if (id == squad.front0 || id == squad.front1) return Lane.Front;
            if (id == squad.mid0 || id == squad.mid1) return Lane.Mid;
            if (id == squad.back0 || id == squad.back1) return Lane.Back;
            return null;
        }

        public void OnCellClicked(int x, int y)
        {
            if (_ended || _showResults) return;
            if (_swapsLeft <= 0) { Note("No swaps left — End Turn."); return; }
            if (_sx == null) { _sx = x; _sy = y; Note($"Selected ({x},{y})"); Redraw(); return; }
            int x0 = _sx.Value, y0 = _sy.Value;
            _sx = _sy = null;
            if (!_board.TrySwap(x0, y0, x, y, out var result))
            {
                Note("Invalid swap / no match.");
                Redraw();
                return;
            }
            _swapsLeft--;
            ApplyMatch(result);
            Redraw();
            if (_enemyHp <= 0) Win();
        }

        public void AssistAimSwap()
        {
            if (_ended || _showResults) return;
            if (!_assistTipShown)
            {
                Note("Assist Aim: picks a valid ≥3 swap for you.");
                _assistTipShown = true;
            }
            for (int y = 0; y < BoardSimulator.Height; y++)
            for (int x = 0; x < BoardSimulator.Width; x++)
            {
                foreach (var (dx, dy) in new[] { (1, 0), (0, 1) })
                {
                    int nx = x + dx, ny = y + dy;
                    if (!_board.InBounds(nx, ny)) continue;
                    var probe = new BoardSimulator(7);
                    var copy = new GemColor[BoardSimulator.Width, BoardSimulator.Height];
                    for (int ix = 0; ix < BoardSimulator.Width; ix++)
                    for (int iy = 0; iy < BoardSimulator.Height; iy++)
                        copy[ix, iy] = _board.Get(ix, iy);
                    probe.LoadFrom(copy);
                    if (probe.TrySwap(x, y, nx, ny, out _))
                    {
                        OnCellClicked(x, y);
                        OnCellClicked(nx, ny);
                        return;
                    }
                }
            }
            Note("Assist Aim: no move.");
        }

        void ApplyMatch(MatchResolveResult result)
        {
            float share = 0f;
            if (_agents.TryGetValue("mira", out var mira))
                share = mira.gaugeSharePower * 0.25f;

            foreach (var kv in _gauges)
            {
                if (!_agents.TryGetValue(kv.Key, out var def)) continue;
                kv.Value.ApplyClears(def.AttunementColors(), result.ClearedByColor, share);
            }

            float tagMul = EnemyTagDamageMul();
            int chip = (int)(result.GemsCleared * 15f * ComboMath.Multiplier(result.Combo) * tagMul);
            _enemyHp = Mathf.Max(0, _enemyHp - chip);
            Note($"Combo×{result.Combo} cleared {result.GemsCleared} → {chip} dmg");
            CheckBossPhase();
        }

        float EnemyTagDamageMul()
        {
            if (_stage?.enemyTags == null) return 1f;
            if (_stage.enemyTags.Contains("armor") || _stage.enemyTags.Contains("armored_light"))
                return 0.85f;
            if (_stage.enemyTags.Contains("soft")) return 1.15f;
            return 1f;
        }

        void CheckBossPhase()
        {
            if (_stage == null || _stage.type != "Boss") return;
            if (_phase == 1 && _enemyHp <= _enemyMaxHp / 2)
            {
                _phase = 2;
                _enemyAtk = (int)(_enemyAtk * 1.25f);
                Note("Phase 2 — Lease Reclaimer shifts lanes! ATK up.");
            }
        }

        public void FireBurst(string agentId)
        {
            if (_ended || _showResults) return;
            if (!_gauges.TryGetValue(agentId, out var g) || !_agents.TryGetValue(agentId, out var def)) return;
            if (!g.TryFireBurst(out _))
            {
                Note($"{agentId} Burst not ready");
                return;
            }
            var stats = def.StatsAtLevel(LevelOf(agentId));
            _field.TryGet(agentId, out var lane);
            float bondMul = BondMatrix.AtkMultiplier(BondMatrix.IsMiraIvyBondActive(_agents.Keys), _bondBuffConsumed);
            int dmg = (int)(stats.atk * bondMul * BurstMul(def.role) * LaneField.DamageDealtMod(lane) * EnemyTagDamageMul());
            if (def.role == "Support")
            {
                HealSquad((int)(stats.atk * 0.8f));
                Note($"{def.displayName} BURST heal");
            }
            else
            {
                _enemyHp = Mathf.Max(0, _enemyHp - dmg);
                if (bondMul > 1f)
                {
                    _bondBuffConsumed = true;
                    Note("Bond Matrix: Shared Lease ATK buff consumed.");
                }
                Note($"{def.displayName} BURST → {dmg}");
            }
            AnalyticsSink.Emit("burst_used", $"agent_id={agentId};stage_id={_stageId}");
            Redraw();
            if (_enemyHp <= 0) Win();
        }

        void HealSquad(int amount)
        {
            foreach (var id in _agentHp.Keys.ToList())
            {
                if (!_agents.TryGetValue(id, out var def)) continue;
                int max = def.StatsAtLevel(LevelOf(id)).hp;
                _agentHp[id] = Mathf.Min(max, _agentHp[id] + amount);
            }
        }

        public void FireMiraBurst() => FireBurst("mira");
        public void FireIvyBurst() => FireBurst("ivy");
        public void FireJuniperBurst() => FireBurst("juniper");
        public void FireWrenBurst() => FireBurst("wren");

        public void EndTurn()
        {
            if (_ended || _showResults) return;
            _retreatArmed = false;
            Note("— Enemy Phase —");
            if (_enemyHp > 0 && _agents.Count > 0)
            {
                var tank = _agents.Keys.FirstOrDefault(id => _field.TryGet(id, out var l) && l == Lane.Front)
                           ?? _agents.Keys.First();
                _field.TryGet(tank, out var lane);
                int dmg = (int)(_enemyAtk * LaneField.DamageTakenMod(lane));
                if (_agentHp.ContainsKey(tank))
                {
                    _agentHp[tank] = Mathf.Max(0, _agentHp[tank] - dmg);
                    Note($"Enemy ATK {_enemyAtk} (StageCombatMath) hits {tank} for {dmg} [{lane}] (HP {_agentHp[tank]})");
                    if (_agentHp.Values.All(h => h <= 0))
                    {
                        Lose();
                        return;
                    }
                }
                else Note($"Enemy ATK {_enemyAtk} hits {tank} for {dmg} (stub)");
            }
            foreach (var g in _gauges.Values) g.TickTurnEnd();
            _swapsLeft = 5;
            _turnsTaken++;
            Note($"— Player Phase — swaps refreshed (turn {_turnsTaken})");
            Redraw();
        }


        void Win()
        {
            if (_ended) return;
            _ended = true;
            Note("VICTORY");
            var save = SaveService.Instance.Data;
            if (_stage != null)
            {
                if (_stage.staminaCost > 0)
                    save.stamina = Mathf.Max(0, save.stamina - _stage.staminaCost);
                if (_stage.rewards != null)
                {
                    _rewardLc = _stage.rewards.lumaCoins;
                    _rewardPc = _stage.rewards.crystals;
                    _rewardXp = _stage.rewards.xp;
                    save.lumaCoins += _rewardLc;
                    save.pulseCrystals += _rewardPc;
                    GrantSquadXp(_rewardXp);
                    Note($"+{_rewardLc} LC · +{_rewardXp} XP" +
                         (_rewardPc > 0 ? $" · +{_rewardPc} PC" : ""));
                }
                if (!save.clearedStages.Contains(_stageId)) save.clearedStages.Add(_stageId);
                save.ch1Stage = _stageId;
                if (!string.IsNullOrEmpty(_stage.unlockAgentOnClear) && !save.unlockedAgents.Contains(_stage.unlockAgentOnClear))
                {
                    save.unlockedAgents.Add(_stage.unlockAgentOnClear);
                    EnsureAgentSave(_stage.unlockAgentOnClear);
                    Note($"Unlocked {_stage.unlockAgentOnClear}!");
                }
                if (_stage.setsCh1Complete)
                {
                    save.ch1Complete = true;
                    AnalyticsSink.Emit("ch1_complete");
                }
                if (_stageId == "1-1")
                {
                    if (!save.unlockedAgents.Contains("ivy")) save.unlockedAgents.Add("ivy");
                    if (!save.unlockedAgents.Contains("juniper")) save.unlockedAgents.Add("juniper");
                    EnsureAgentSave("ivy");
                    EnsureAgentSave("juniper");
                    Note("Ivy + Juniper joined the squad.");
                }
            }
            
            int alive = _agentHp.Count > 0 ? _agentHp.Values.Count(h => h > 0) : _agents.Count;
            string stype = _stage != null ? _stage.type : "Story";
            int threshold = StageStarRating.ThresholdForStageType(stype);
            int stars = StageStarRating.Rate(alive, _turnsTaken, threshold);
            save.SetStageStars(_stageId, stars);
            _rewardStars = stars;
            GrantClearFragments(stars);
            Note($"Clear {StageStarRating.FormatStars(stars)} (alive {alive}, turns {_turnsTaken}/{threshold})");
            if (DialogueService.Instance != null)
                DialogueService.Instance.TryStartTrigger("post_stage", _stageId);

            save.AddDailyProgress("clear_stage");
            SaveService.Instance.Save();
            AnalyticsSink.Emit("stage_clear", $"stage_id={_stageId}");
            ShowResultsOverlay();
        }

        void Lose()
        {
            if (_ended) return;
            _ended = true;
            Note("DEFEAT — returning to Hub (no soft-lock).");
            // Tutorial 1-1 stamina already 0; no extra drain on fail
            AnalyticsSink.Emit("stage_fail", $"stage_id={_stageId};reason=wipe");
            Invoke(nameof(BackToHub), 1.2f);
        }

        void GrantSquadXp(int xp)
        {
            _xpLines.Clear();
            if (xp <= 0) return;
            var save = SaveService.Instance.Data;
            foreach (var id in _agents.Keys)
            {
                EnsureAgentSave(id);
                var a = save.agents.Find(x => x.id == id);
                if (a == null) continue;
                int before = a.level;
                int cap = GrowthMath.LevelCapForStars(a.stars < 1 ? 1 : a.stars);
                GrowthMath.ApplyXp(ref a.level, ref a.xp, xp, cap);
                int need = a.level >= cap ? 1 : GrowthMath.XpToNextLevel(a.level);
                _xpLines.Add((id, before, a.level, a.xp, need, cap));
            }
        }

        void GrantClearFragments(int stageStars)
        {
            _rewardFrags = 0;
            _fragAgentId = null;
            if (stageStars <= 0 || _agents.Count == 0) return;
            // Slice: ★ earned → Echo Fragments for first squad member (promote fuel).
            string id = null;
            foreach (var k in _agents.Keys) { id = k; break; }
            if (string.IsNullOrEmpty(id)) return;
            EnsureAgentSave(id);
            SaveService.Instance.Data.AddFragments(id, stageStars);
            _rewardFrags = stageStars;
            _fragAgentId = id;
        }

        void ShowResultsOverlay()
        {
            _showResults = true;
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("══ RESULTS ══");
            sb.AppendLine($"{_stageId}  {StageStarRating.FormatStars(_rewardStars)}");
            sb.AppendLine($"+{_rewardLc} Luma Coins");
            if (_rewardPc > 0) sb.AppendLine($"+{_rewardPc} Pulse Crystals");
            sb.AppendLine($"+{_rewardXp} Squad XP");
            if (_rewardFrags > 0)
                sb.AppendLine($"+{_rewardFrags} Echo Fragments → {_fragAgentId}");
            sb.AppendLine();
            foreach (var row in _xpLines)
            {
                string bar = row.afterLv >= row.cap
                    ? "[██████████] MAX"
                    : $"{GrowthMath.XpBar(row.xp, row.need)} {row.xp}/{row.need}";
                string lvl = row.beforeLv == row.afterLv
                    ? $"Lv{row.afterLv}/{row.cap}"
                    : $"Lv{row.beforeLv}→{row.afterLv}/{row.cap}";
                sb.AppendLine($"{row.id}  {lvl}  {bar}");
            }
            sb.AppendLine();
            sb.AppendLine("[Continue] → Hub");
            _resultsSummary = sb.ToString();
            if (resultsTextLegacy != null)
            {
                resultsTextLegacy.gameObject.SetActive(true);
                resultsTextLegacy.text = _resultsSummary;
            }
            Note("Results ready — tap Continue.");
            Redraw();
        }

        public void DismissResults()
        {
            if (!_showResults) return;
            _showResults = false;
            SceneManager.LoadScene("02_Hub");
        }

        static void EnsureAgentSave(string id)
        {
            var save = SaveService.Instance.Data;
            int stars = id switch
            {
                "mira" => 4,
                "wren" => 3,
                "ivy" => 2,
                "juniper" => 2,
                _ => 1
            };
            int level = id == "mira" ? 5 : 3;
            var a = save.GetOrCreateAgent(id, level, stars);
            if (a.stars < 1) a.stars = stars;
        }

        public void Retreat()
        {
            if (_ended || _showResults) return;
            if (!_retreatArmed)
            {
                _retreatArmed = true;
                Note("Retreat? Tap Retreat again to confirm. (Progress not saved.)");
                Redraw();
                return;
            }
            _ended = true;
            AnalyticsSink.Emit("stage_fail", $"stage_id={_stageId};reason=retreat");
            SceneManager.LoadScene("02_Hub");
        }

        void BackToHub() => SceneManager.LoadScene("02_Hub");

        int LevelOf(string id)
        {
            var a = SaveService.Instance.Data.agents.Find(x => x.id == id);
            return a != null ? a.level : 5;
        }

        static float BurstMul(string role) => role switch
        {
            "Striker" => 2.2f,
            "Synergist" => 1.8f,
            "Guardian" => 1.0f,
            "Support" => 1.3f,
            _ => 2.0f
        };

        void Note(string msg)
        {
            _lines.Add(msg);
            if (_lines.Count > 14) _lines.RemoveAt(0);
        }

        void Redraw()
        {
            string board = _board != null
                ? _board.Dump() + $"\nSwaps {_swapsLeft}/5  sel={(_sx.HasValue ? $"({_sx},{_sy})" : "-")}"
                : "";
            HudText.Set(boardText, boardTextLegacy, board);

            string enemy = $"Enemy HP {_enemyHp}/{_enemyMaxHp}" + (_phase > 1 ? "  P2" : "");
            HudText.Set(enemyHpText, enemyHpTextLegacy, enemy);

            var parts = new List<string>();
            foreach (var kv in _gauges)
            {
                string hp = _agentHp.TryGetValue(kv.Key, out var h) ? $" HP{h}" : "";
                string lane = _field.TryGet(kv.Key, out var l) ? $" {l}" : "";
                parts.Add($"{kv.Key}:{(int)(kv.Value.Value * 100)}%{(kv.Value.CanBurst ? " [BURST]" : "")}{lane}{hp}");
            }
            HudText.Set(gaugeText, gaugeTextLegacy, string.Join("\n", parts));
            HudText.Set(logText, logTextLegacy, string.Join("\n", _lines));

            if (laneTextLegacy != null)
            {
                var lanes = new List<string>();
                foreach (var id in _agents.Keys)
                {
                    _field.TryGet(id, out var l);
                    lanes.Add($"{l}:{id}");
                }
                laneTextLegacy.text = "Lanes " + string.Join(" · ", lanes);
            }
            if (statusTextLegacy != null && _stage != null)
                statusTextLegacy.text = _showResults ? "VICTORY — Results" : $"{_stage.id} {_stage.name}";
            if (resultsTextLegacy != null && _showResults)
                resultsTextLegacy.text = _resultsSummary;
        }
    }
}
