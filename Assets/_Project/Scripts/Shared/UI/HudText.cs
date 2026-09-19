using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Heartline.Shared
{
    /// <summary>Prefer TMP when wired in Editor; fall back to uGUI Text from runtime builders.</summary>
    public static class HudText
    {
        public static void Set(TMP_Text tmp, Text legacy, string value)
        {
            if (tmp != null) tmp.text = value;
            if (legacy != null) legacy.text = value;
        }
    }
}
