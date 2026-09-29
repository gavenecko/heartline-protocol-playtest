using UnityEngine;

namespace Heartline.App.RuntimeUi
{
    /// <summary>
    /// Shrinks a full-stretch RectTransform to <see cref="Screen.safeArea"/> so buttons/labels
    /// never sit under a notch, punch-hole camera or gesture bar. Re-applies on rotation/resize.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        RectTransform _rt;
        Rect _lastSafe;
        Vector2Int _lastScreen;

        void Awake()
        {
            _rt = GetComponent<RectTransform>();
            Apply();
        }

        void Update()
        {
            if (_lastSafe != Screen.safeArea || _lastScreen.x != Screen.width || _lastScreen.y != Screen.height)
                Apply();
        }

        void Apply()
        {
            if (_rt == null) return;
            var safe = Screen.safeArea;
            _lastSafe = safe;
            _lastScreen = new Vector2Int(Screen.width, Screen.height);
            if (Screen.width <= 0 || Screen.height <= 0 || safe.width <= 0 || safe.height <= 0) return;
            var min = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            var max = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            _rt.anchorMin = min;
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Moves every child of <paramref name="canvas"/> into a SafeArea panel, except full-screen
        /// backgrounds (names ending in "Bg"), which should still bleed to the screen edges.
        /// Anchors are relative, so layouts keep their proportions inside the safe rect.
        /// </summary>
        public static void WrapCanvas(Canvas canvas)
        {
            if (canvas == null || canvas.transform.Find("SafeArea") != null) return;
            var go = new GameObject("SafeArea", typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(canvas.transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var move = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in canvas.transform)
                if (child != rt && !child.name.EndsWith("Bg"))
                    move.Add(child);
            foreach (var child in move)
                child.SetParent(rt, false);
            // Keep backgrounds behind everything.
            rt.SetAsLastSibling();
            go.AddComponent<SafeAreaFitter>();
        }
    }
}
