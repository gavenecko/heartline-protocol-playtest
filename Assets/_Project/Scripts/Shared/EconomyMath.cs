using System;

namespace Heartline.Shared
{
    /// <summary>Stamina / Heart Energy regen from UTC timestamps (economy notes + economy_constants.json).</summary>
    public static class EconomyRegen
    {
        public static int StaminaCap => GameConstants.StaminaCap;
        public static int HeartEnergyCap => GameConstants.HeartEnergyCap;
        public static int StaminaSecondsPerPoint => GameConstants.StaminaRegenMinutes * 60;
        public static int HeartSecondsPerPoint => GameConstants.HeartRegenMinutes * 60;

        /// <summary>
        /// Apply discrete regen. When at cap, lastUtc snaps to now (no backlog).
        /// Returns points gained.
        /// </summary>
        public static int Apply(ref int value, ref long lastUtc, long nowUtc, int cap, int secondsPerPoint)
        {
            if (secondsPerPoint <= 0) return 0;
            if (value >= cap)
            {
                lastUtc = nowUtc;
                return 0;
            }
            var elapsed = nowUtc - lastUtc;
            if (elapsed < secondsPerPoint) return 0;
            var points = (int)(elapsed / secondsPerPoint);
            var before = value;
            value = Math.Min(cap, value + points);
            lastUtc += points * (long)secondsPerPoint;
            if (value >= cap) lastUtc = nowUtc;
            return value - before;
        }

        public static int ApplyStamina(ref int stamina, ref long lastUtc, long nowUtc) =>
            Apply(ref stamina, ref lastUtc, nowUtc, StaminaCap, StaminaSecondsPerPoint);

        public static int ApplyHeart(ref int heart, ref long lastUtc, long nowUtc) =>
            Apply(ref heart, ref lastUtc, nowUtc, HeartEnergyCap, HeartSecondsPerPoint);

        /// <summary>Seconds until next +1, or 0 if at cap / ready.</summary>
        public static long SecondsUntilNext(int value, long lastUtc, long nowUtc, int cap, int secondsPerPoint)
        {
            if (value >= cap || secondsPerPoint <= 0) return 0;
            var elapsed = nowUtc - lastUtc;
            if (elapsed >= secondsPerPoint) return 0;
            return secondsPerPoint - elapsed;
        }
    }

    /// <summary>Luma coin sinks for T1 gifts (shop stub).</summary>
    public static class GiftShop
    {
        public const int SunwellTeaPriceLc = 500;
        public const int ProteinBarPriceLc = 400;
        public const int InstantCocoaPriceLc = 400;
        public const int HerbalSachetPriceLc = 400;

        public static int PriceFor(string giftId) => giftId switch
        {
            "sunwell_tea_t1" => SunwellTeaPriceLc,
            "protein_bar_t1" => ProteinBarPriceLc,
            "instant_cocoa_t1" => InstantCocoaPriceLc,
            "herbal_sachet_t1" => HerbalSachetPriceLc,
            _ => 500
        };

        public static bool TryBuy(string giftId, ref int lumaCoins, ref int stockCount, out string error)
        {
            error = null;
            int price = PriceFor(giftId);
            if (lumaCoins < price)
            {
                error = $"Need {price} LC (have {lumaCoins}).";
                return false;
            }
            lumaCoins -= price;
            stockCount++;
            return true;
        }
    }
}
