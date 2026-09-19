using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Heartline.App;
using Heartline.Save;
using Heartline.Analytics;

namespace Heartline.SyncMatch
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] TMP_Text boardText;
        [SerializeField] TMP_Text logText;
        [SerializeField] TMP_Text enemyHpText;
        [SerializeField] TMP_Text gaugeText;

        BoardSimulator _board;
        readonly Dictionary<string, SyncGauge> _gauges = new Dictionary<string, SyncGauge>();
        readonly Dictionary<string, AgentDefinition> _agents = new Dictionary<string, AgentDefinition>();
        readonly LaneField _field = new LaneField();
        readonly List<string> _lines = new List<string>();
        int _enemyHp = 1200;
        int _enemyAtk = 70;
        int _swapsLeft = 5;
        int? _sx, _sy;
        string _stageId;
        StageDefinition _stage;

        void Start()
        {
            _stageId = PlayerPrefs.GetString("hp_pending_stage", "1-1");
            _stage = GameDataService.Instance?.Stages?.stages?.Find(s => s.id == _stageId);
            if (_stage != null)
            {
                _enemyHp = _stage.type == "Boss" ? 3500 : (_stage.type == "EliteLite" ? 1800 : 900);
                _enemyAtk = _stage.type == "Boss" ? 110 : 60;
            }

            _board = new BoardSimulator(_stageId.GetHashCode());
            SetupSquad();
            if (_stage != null && _stage.forceMiraBurst && _gauges.ContainsKey("mira"))
            {
                _gauges["mira"].Set(1f);
                Note("Tutorial: Mira Burst ready — fire it!");
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
                var def = GameDataService.Instance.GetAgent(id);
                if (def == null) continue;
                _agents[id] = def;
                _gauges[id] = new SyncGauge();
                _field.Assign(id, def.PreferredLane());
            }
            if (_agents.Count == 0)
            {
                var mira = GameDataService.Instance.GetAgent("mira");
                if (mira != null)
                {
                    _agents["mira"] = mira;
                    _gauges["mira"] = new SyncGauge();
                    _field.Assign("mira", Lane.Mid);
                }
            }
        }

        public void OnCellClicked(int x, int y)
        {
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

            int chip = (int)(result.GemsCleared * 15f * ComboMath.Multiplier(result.Combo));
            _enemyHp = Mathf.Max(0, _enemyHp - chip);
            Note($"Combo×{result.Combo} cleared {result.GemsCleared} pulse {chip}");
        }

        public void FireBurst(string agentId)
        {
            if (!_gauges.TryGetValue(agentId, out var g) || !_agents.TryGetValue(agentId, out var def)) return;
            if (!g.TryFireBurst(out _))
            {
                Note($"{agentId} Burst not ready");
                return;
            }
            var stats = def.StatsAtLevel(LevelOf(agentId));
            _field.TryGet(agentId, out var lane);
            int dmg = (int)(stats.atk * BurstMul(def.role) * LaneField.DamageDealtMod(lane));
            _enemyHp = Mathf.Max(0, _enemyHp - dmg);
            AnalyticsSink.Emit("burst_used", $"agent_id={agentId};stage_id={_stageId}");
            Note($"{def.displayName} BURST → {dmg}");
            Redraw();
            if (_enemyHp <= 0) Win();
        }

        public void FireMiraBurst() => FireBurst("mira");
        public void FireIvyBurst() => FireBurst("ivy");
        public void FireJuniperBurst() => FireBurst("juniper");
        public void FireWrenBurst() => FireBurst("wren");

        public void EndTurn()
        {
            if (_enemyHp > 0 && _agents.Count > 0)
            {
                var tank = _agents.Keys.FirstOrDefault(id => _field.TryGet(id, out var l) && l == Lane.Front)
                           ?? _agents.Keys.First();
                _field.TryGet(tank, out var lane);
                int dmg = (int)(_enemyAtk * LaneField.DamageTakenMod(lane));
                Note($"Enemy hits {tank} for {dmg} (stub)");
            }
            foreach (var g in _gauges.Values) g.TickTurnEnd();
            _swapsLeft = 5;
            Note("— Turn end —");
            Redraw();
        }

        void Win()
        {
            Note("VICTORY");
            var save = SaveService.Instance.Data;
            if (_stage != null)
            {
                save.stamina = Mathf.Max(0, save.stamina - _stage.staminaCost);
                if (_stage.rewards != null)
                {
                    save.lumaCoins += _stage.rewards.lumaCoins;
                    save.pulseCrystals += _stage.rewards.crystals;
                }
                if (!save.clearedStages.Contains(_stageId)) save.clearedStages.Add(_stageId);
                save.ch1Stage = _stageId;
                if (!string.IsNullOrEmpty(_stage.unlockAgentOnClear) && !save.unlockedAgents.Contains(_stage.unlockAgentOnClear))
                    save.unlockedAgents.Add(_stage.unlockAgentOnClear);
                if (_stage.setsCh1Complete)
                {
                    save.ch1Complete = true;
                    AnalyticsSink.Emit("ch1_complete");
                }
                if (_stageId == "1-1")
                {
                    if (!save.unlockedAgents.Contains("ivy")) save.unlockedAgents.Add("ivy");
                    if (!save.unlockedAgents.Contains("juniper")) save.unlockedAgents.Add("juniper");
                }
            }
            SaveService.Instance.Save();
            AnalyticsSink.Emit("stage_clear", $"stage_id={_stageId}");
            Invoke(nameof(BackToHub), 1.0f);
        }

        public void Retreat()
        {
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
            if (_lines.Count > 12) _lines.RemoveAt(0);
        }

        void Redraw()
        {
            if (boardText) boardText.text = _board.Dump() + $"\nSwaps {_swapsLeft}/5";
            if (enemyHpText) enemyHpText.text = $"Enemy HP {_enemyHp}";
            if (gaugeText)
            {
                var parts = new List<string>();
                foreach (var kv in _gauges)
                    parts.Add($"{kv.Key}:{(int)(kv.Value.Value * 100)}%{(kv.Value.CanBurst ? " [BURST]" : "")}");
                gaugeText.text = string.Join("  ", parts);
            }
            if (logText) logText.text = string.Join("\n", _lines);
        }
    }
}
