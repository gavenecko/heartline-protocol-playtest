using System.Collections.Generic;

namespace Heartline.SyncMatch
{
    public sealed class SyncGaugeConfig
    {
        public float BasicThreshold = 0.20f;
        public float BurstThreshold = 1.0f;
        public int BurstLockTurns = 1;
        public float AttunedFillPerGem = 0.04f;
        public float PrismFillPerGem = 0.05f;
        public float OffColorFillPerGem = 0.01f;
    }

    public sealed class SyncGauge
    {
        public float Value { get; private set; }
        public int LockTurnsRemaining { get; private set; }
        public SyncGaugeConfig Config { get; }
        public bool IsLocked => LockTurnsRemaining > 0;
        public bool CanBasic => !IsLocked && Value >= Config.BasicThreshold;
        public bool CanBurst => !IsLocked && Value >= Config.BurstThreshold;

        public SyncGauge(SyncGaugeConfig config = null)
        {
            Config = config ?? new SyncGaugeConfig();
        }

        public void Set(float v) => Value = Clamp01(v);

        public float ApplyClears(IReadOnlyList<GemColor> attunements, Dictionary<GemColor, int> cleared, float shareBonus = 0f)
        {
            if (IsLocked || cleared == null) return 0f;
            float gained = 0f;
            foreach (var kv in cleared)
            {
                int n = kv.Value;
                if (n <= 0) continue;
                float per;
                if (kv.Key == GemColor.Prism)
                    per = Config.PrismFillPerGem;
                else if (IsAttuned(attunements, kv.Key))
                    per = Config.AttunedFillPerGem;
                else
                    per = Config.OffColorFillPerGem;
                gained += per * n;
            }
            gained *= (1f + shareBonus);
            Value = Clamp01(Value + gained);
            return gained;
        }

        public bool TryFireBurst(out float consumed)
        {
            consumed = 0f;
            if (!CanBurst) return false;
            consumed = Value;
            Value = 0f;
            LockTurnsRemaining = Config.BurstLockTurns;
            return true;
        }

        public void TickTurnEnd()
        {
            if (LockTurnsRemaining > 0) LockTurnsRemaining--;
        }

        public void AddFlat(float amount)
        {
            if (IsLocked) return;
            Value = Clamp01(Value + amount);
        }

        static bool IsAttuned(IReadOnlyList<GemColor> att, GemColor c)
        {
            if (att == null) return false;
            for (int i = 0; i < att.Count; i++)
                if (att[i] == c) return true;
            return false;
        }

        static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
    }

    public static class ComboMath
    {
        public static float Multiplier(int combo)
        {
            if (combo >= 8) return 2.0f;
            if (combo >= 5) return 1.5f;
            if (combo >= 3) return 1.2f;
            return 1.0f;
        }
    }
}
