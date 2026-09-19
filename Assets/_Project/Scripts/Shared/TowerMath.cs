using System;

namespace Heartline.Shared
{
    /// <summary>
    /// Tower / Abyss stub (GDD §5): 50 floors live; slice exposes 10.
    /// Checkpoint every 5; no continue on fail — snap back to last checkpoint.
    /// Stamina costs from GameConstants / economy_constants.
    /// </summary>
    public static class TowerMath
    {
        public const int FullFloorCount = 50;
        public const int SliceFloorCount = 10;
        public const int CheckpointEvery = 5;

        /// <summary>Default STA per floor band when constants table unavailable.</summary>
        public static int DefaultStaminaCost(int floor)
        {
            if (floor < 1) floor = 1;
            if (floor <= 5) return 6;
            if (floor <= 10) return 8;
            if (floor <= 20) return 10;
            if (floor <= 35) return 12;
            return 15;
        }

        public static int StaminaCost(int floor, Func<int, int> costLookup = null)
        {
            if (costLookup != null)
            {
                int c = costLookup(floor);
                if (c > 0) return c;
            }
            return DefaultStaminaCost(floor);
        }

        public static bool IsCheckpoint(int floor) =>
            floor > 0 && floor % CheckpointEvery == 0;

        public static int CheckpointFloorFor(int highestCleared)
        {
            if (highestCleared < CheckpointEvery) return 0;
            return (highestCleared / CheckpointEvery) * CheckpointEvery;
        }

        /// <summary>Next floor the player may attempt (1-based).</summary>
        public static int NextFloor(int highestCleared) => Math.Max(0, highestCleared) + 1;

        public static bool IsFloorUnlocked(int floor, int highestCleared, int sliceCap = SliceFloorCount)
        {
            if (floor < 1 || floor > sliceCap) return false;
            return floor <= highestCleared + 1;
        }

        public static bool CanAttempt(int floor, int highestCleared, int sliceCap = SliceFloorCount)
        {
            if (floor < 1 || floor > sliceCap) return false;
            if (highestCleared >= sliceCap) return false; // slice complete
            return floor == highestCleared + 1;
        }

        public static void ApplyClear(int floor, ref int highestCleared, ref int lastCheckpoint)
        {
            if (floor > highestCleared) highestCleared = floor;
            if (IsCheckpoint(floor) && floor > lastCheckpoint)
                lastCheckpoint = floor;
            int implied = CheckpointFloorFor(highestCleared);
            if (implied > lastCheckpoint) lastCheckpoint = implied;
        }

        /// <summary>
        /// On fail: no continue — snap highestCleared to lastCheckpoint
        /// so the next attempt is lastCheckpoint+1.
        /// </summary>
        public static void ApplyFail(ref int highestCleared, int lastCheckpoint)
        {
            if (lastCheckpoint < 0) lastCheckpoint = 0;
            if (highestCleared > lastCheckpoint)
                highestCleared = lastCheckpoint;
        }

        public static int NextAttemptAfterFail(int lastCheckpoint) =>
            Math.Max(0, lastCheckpoint) + 1;

        public static bool TryClear(
            int floor,
            ref int stamina,
            ref int highestCleared,
            ref int lastCheckpoint,
            int sliceCap,
            Func<int, int> costLookup,
            out string error)
        {
            error = null;
            if (!CanAttempt(floor, highestCleared, sliceCap))
            {
                if (highestCleared >= sliceCap)
                    error = $"Slice Tower complete (floors 1–{sliceCap} of {FullFloorCount}).";
                else
                    error = $"Must attempt floor {highestCleared + 1}.";
                return false;
            }
            int cost = StaminaCost(floor, costLookup);
            if (stamina < cost)
            {
                error = $"Need {cost} STA (have {stamina}).";
                return false;
            }
            stamina -= cost;
            ApplyClear(floor, ref highestCleared, ref lastCheckpoint);
            return true;
        }

        public static bool TryFail(
            int floor,
            ref int stamina,
            ref int highestCleared,
            int lastCheckpoint,
            int sliceCap,
            Func<int, int> costLookup,
            out string error)
        {
            error = null;
            if (!CanAttempt(floor, highestCleared, sliceCap))
            {
                error = $"Must attempt floor {highestCleared + 1}.";
                return false;
            }
            int cost = StaminaCost(floor, costLookup);
            if (stamina < cost)
            {
                error = $"Need {cost} STA (have {stamina}).";
                return false;
            }
            stamina -= cost;
            ApplyFail(ref highestCleared, lastCheckpoint);
            return true;
        }
    }
}
