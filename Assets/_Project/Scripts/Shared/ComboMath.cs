namespace Heartline.Shared
{
    public static class ComboMath
    {
        /// <summary>Default combo curve: 1→1.0, 3→1.2, 5→1.5, 8+→2.0</summary>
        public static float Multiplier(int combo, EconomyConstants eco = null)
        {
            if (eco != null) return eco.ComboMultiplier(combo);
            if (combo >= 8) return 2.0f;
            if (combo >= 5) return 1.5f;
            if (combo >= 3) return 1.2f;
            return 1.0f;
        }
    }
}
