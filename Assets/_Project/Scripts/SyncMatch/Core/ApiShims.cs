using System.Collections.Generic;

namespace Heartline.SyncMatch
{
    public static class SyncGaugeShims
    {
        public static bool CanBurst(this SyncGauge g) => g.CanBurst;
        public static bool TryFireBurst(this SyncGauge g, out float consumed) => g.TryFireBurst(out consumed);
        public static float ApplyClears(this SyncGauge g, IReadOnlyList<GemColor> att, Dictionary<GemColor, int> cleared, float share = 0f)
            => g.ApplyClears(att, cleared, share);
        public static void TickTurnEnd(this SyncGauge g) => g.TickTurnEnd();
    }

    public static class BoardShims
    {
        public static bool InBounds(this BoardSimulator b, int x, int y) => b.InBounds(x, y);
        public static string Dump(this BoardSimulator b) => b.Dump();
    }

    public static class LaneShims
    {
        public static float DamageDealtMod(Lane lane) => LaneField.DamageDealtMod(lane);
        public static float DamageTakenMod(Lane lane) => LaneField.DamageTakenMod(lane);
    }
}
