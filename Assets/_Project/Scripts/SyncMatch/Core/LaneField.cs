using System.Collections.Generic;

namespace Heartline.SyncMatch
{
    public sealed class LaneField
    {
        public const int MaxPerLane = 2;
        readonly Dictionary<Lane, List<string>> _occ = new()
        {
            [Lane.Front] = new(),
            [Lane.Mid] = new(),
            [Lane.Back] = new()
        };
        readonly Dictionary<string, Lane> _agent = new();

        public bool Assign(string agentId, Lane lane)
        {
            if (string.IsNullOrEmpty(agentId)) return false;
            if (_occ[lane].Count >= MaxPerLane) return false;
            if (_agent.TryGetValue(agentId, out var old))
                _occ[old].Remove(agentId);
            _occ[lane].Add(agentId);
            _agent[agentId] = lane;
            return true;
        }

        public bool TryGet(string agentId, out Lane lane) => _agent.TryGetValue(agentId, out lane);

        public List<string> Occupants(Lane lane) => _occ[lane];

        public Dictionary<string, Lane>.KeyCollection AllAgents() => _agent.Keys;

        public static float DamageDealtMod(Lane lane) => lane switch
        {
            Lane.Front => 1.15f,
            Lane.Back => 0.90f,
            _ => 1.0f
        };

        public static float DamageTakenMod(Lane lane) => lane switch
        {
            Lane.Front => 1.25f,
            Lane.Back => 0.75f,
            _ => 1.0f
        };
    }
}
