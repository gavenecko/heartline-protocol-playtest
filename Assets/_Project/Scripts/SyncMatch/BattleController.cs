using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Heartline.Save;
using Heartline.Analytics;
using Heartline.Shared;

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
        int _phase = 1;

        public string StageId => _stageId;
        public int EnemyHp => _enemyHp;
        public int SwapsLeft => _swapsLeft;
        public BoardSimulator Board => _board;

        public void BindLegacy(Text board, Text log, Text enemy, Text gauge, Text lane = null, Text status = null)
        {
            boardTextLegacy = board;
            logTextLegacy = log;
            enemyHpTextLegacy = enemy;
            gaugeTextLegacy = gauge;
            laneTextLegacy = lane;
            statusTextLegacy = status;
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
            if (_stage != null && _stage.forceMiraBurst && _gauges.ContainsKey("mira"))
            {
                _gauges["mira"].Set(1f);
                Note("Tutorial: Mira Burst ready — fire it!");
            }
            if (_stage != null)
                Note($"{_stage.id} {_stage.name} · {_stage.teachingGoal}");
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
            if (_ended) return;
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
            if (_ended) return;
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
            if (_ended) return;
            if (!_gauges.TryGetValue(agentId, out var g) || !_agents.TryGetValue(agentId, out var def)) return;
            if (!g.TryFireBurst(out _))
            {
                Note($"{agentId} Burst not ready");
                return;
            }
            var stats = def.StatsAtLevel(LevelOf(agentId));
            _field.TryGet(agentId, out var lane);
            int dmg = (int)(stats.atk * BurstMul(def.role) * LaneField.DamageDealtMod(lane) * EnemyTagDamageMul());
            if (def.role == "Support")
            {
                HealSquad((int)(stats.atk * 0.8f));
                Note($"{def.displayName} BURST heal");
            }
            else
            {
                _enemyHp = Mathf.Max(0, _enemyHp - dmg);
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
            if (_ended) return;
            if (_enemyHp > 0 && _agents.Count > 0)
            {
                var tank = _agents.Keys.FirstOrDefault(id => _field.TryGet(id, out var l) && l == Lane.Front)
                           ?? _agents.Keys.First();
                _field.TryGet(tank, out var lane);
                int dmg = (int)(_enemyAtk * LaneField.DamageTakenMod(lane));
                if (_agentHp.ContainsKey(tank))
                {
                    _agentHp[tank] = Mathf.Max(0, _agentHp[tank] - dmg);
                    Note($"Enemy hits {tank} for {dmg} (HP {_agentHp[tank]})");
                    if (_agentHp.Values.All(h => h <= 0))
                    {
                        Lose();
                        return;
                    }
                }
                else Note($"Enemy hits {tank} for {dmg} (stub)");
            }
            foreach (var g in _gauges.Values) g.TickTurnEnd();
            _swapsLeft = 5;
            Note("— Turn end —");
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
                    save.lumaCoins += _stage.rewards.lumaCoins;
                    save.pulseCrystals += _stage.rewards.crystals;
                    GrantSquadXp(_stage.rewards.xp);
                    Note($"+{_stage.rewards.lumaCoins} LC · +{_stage.rewards.xp} XP" +
                         (_stage.rewards.crystals > 0 ? $" · +{_stage.rewards.crystals} PC" : ""));
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
            SaveService.Instance.Save();
            AnalyticsSink.Emit("stage_clear", $"stage_id={_stageId}");
            Invoke(nameof(BackToHub), 1.2f);
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
            if (xp <= 0) return;
            var save = SaveService.Instance.Data;
            foreach (var id in _agents.Keys)
            {
                EnsureAgentSave(id);
                var a = save.agents.Find(x => x.id == id);
                if (a == null) continue;
                a.xp += xp;
                while (a.xp >= 100 && a.level < 60)
                {
                    a.xp -= 100;
                    a.level++;
                }
            }
        }

        static void EnsureAgentSave(string id)
        {
            var save = SaveService.Instance.Data;
            if (save.agents.Exists(x => x.id == id)) return;
            save.agents.Add(new AgentSave { id = id, level = id == "mira" ? 5 : 3 });
        }

        public void Retreat()
        {
            if (_ended) return;
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
                statusTextLegacy.text = $"{_stage.id} {_stage.name}";
        }
    }
}
