using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartline.DevTools
{
    /// <summary>
    /// Playtest bug-report helper (development builds + Editor only).
    /// Captures warnings / errors / exceptions and shows them in an on-screen overlay.
    ///   • Open/close: tap the small "LOG" button (top-right corner) or tap with 3 fingers.
    ///   • Copy: puts the full report (device info + recent log + stack traces) on the clipboard,
    ///     ready to paste into a chat / issue.
    /// Self-contained: IMGUI only, no scene wiring, auto-created on startup.
    /// </summary>
    public sealed class DevLogOverlay : MonoBehaviour
    {
        const int MaxEntries = 300;

        struct Entry
        {
            public DateTime time;
            public LogType type;
            public string message;
            public string stack;
            public string scene;
            public int count;
        }

        static DevLogOverlay _instance;
        static readonly object Lock = new object();
        static readonly List<Entry> Pending = new List<Entry>();

        readonly List<Entry> _entries = new List<Entry>();
        bool _open;
        bool _showInfo;
        int _errorCount;
        int _warningCount;
        int _unseenErrors;
        Vector2 _scroll;
        string _toast;
        float _toastUntil;
        GUIStyle _label, _button, _box;
        float _styleScale = -1f;
        static string _currentScene = "";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (!Debug.isDebugBuild) return; // release builds: nothing is created
            if (_instance != null) return;
            var go = new GameObject("[DevLogOverlay]");
            go.hideFlags = HideFlags.DontSave;
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<DevLogOverlay>();
        }

        void Awake()
        {
            Application.logMessageReceivedThreaded += OnLog;
            SceneManager.activeSceneChanged += OnSceneChanged;
            _currentScene = SceneManager.GetActiveScene().name;
        }

        void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= OnLog;
            SceneManager.activeSceneChanged -= OnSceneChanged;
            if (_instance == this) _instance = null;
        }

        static void OnSceneChanged(Scene from, Scene to)
        {
            _currentScene = to.name;
            Debug.Log("[HP] Scene → " + to.name);
        }

        // May be called from any thread.
        static void OnLog(string message, string stack, LogType type)
        {
            lock (Lock)
            {
                Pending.Add(new Entry
                {
                    time = DateTime.Now,
                    type = type,
                    message = message ?? "",
                    stack = stack ?? "",
                    scene = _currentScene,
                    count = 1
                });
                if (Pending.Count > MaxEntries) Pending.RemoveAt(0);
            }
        }

        static bool IsProblem(LogType t) => t != LogType.Log;
        static bool IsError(LogType t) => t == LogType.Error || t == LogType.Exception || t == LogType.Assert;

        void Update()
        {
            lock (Lock)
            {
                foreach (var e in Pending) Add(e);
                Pending.Clear();
            }

            // 3-finger tap toggles the overlay (legacy Input Manager: works with touch on Android).
            if (Input.touchCount == 3)
            {
                bool anyBegan = false;
                for (int i = 0; i < 3; i++)
                    if (Input.GetTouch(i).phase == TouchPhase.Began) anyBegan = true;
                if (anyBegan) Toggle();
            }
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.BackQuote)) Toggle();
#endif
        }

        void Add(Entry e)
        {
            // Collapse immediate repeats (e.g. an error thrown every frame).
            if (_entries.Count > 0)
            {
                var last = _entries[_entries.Count - 1];
                if (last.type == e.type && last.message == e.message)
                {
                    last.count++;
                    last.time = e.time;
                    _entries[_entries.Count - 1] = last;
                    return;
                }
            }
            _entries.Add(e);
            if (_entries.Count > MaxEntries) _entries.RemoveAt(0);
            if (IsError(e.type)) { _errorCount++; if (!_open) _unseenErrors++; }
            else if (e.type == LogType.Warning) _warningCount++;
        }

        void Toggle()
        {
            _open = !_open;
            if (_open) { _unseenErrors = 0; _scroll = new Vector2(0, float.MaxValue); }
        }

        void EnsureStyles()
        {
            // Scale with density so it's finger-sized on phones (baseline 160 dpi).
            float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;
            float scale = Mathf.Clamp(dpi / 160f, 1f, 4f);
            if (Mathf.Approximately(scale, _styleScale) && _label != null) return;
            _styleScale = scale;
            _label = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(11 * scale), wordWrap = true, richText = true };
            _button = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(12 * scale) };
            _box = new GUIStyle(GUI.skin.box);
        }

        void OnGUI()
        {
            EnsureStyles();
            GUI.depth = -1000;
            var safe = Screen.safeArea;
            // IMGUI's origin is top-left; safeArea's is bottom-left.
            float top = Screen.height - safe.yMax;
            float s = _styleScale;
            float btnW = 64 * s, btnH = 28 * s;

            // Corner toggle button with error badge.
            var badge = _unseenErrors > 0 ? $"LOG ({_unseenErrors})" : "LOG";
            var prev = GUI.color;
            GUI.color = _unseenErrors > 0 ? new Color(1f, 0.45f, 0.45f, 0.9f) : new Color(1f, 1f, 1f, 0.45f);
            if (GUI.Button(new Rect(safe.xMax - btnW - 4, top + 4, btnW, btnH), badge, _button))
                Toggle();
            GUI.color = prev;

            if (Time.unscaledTime < _toastUntil && !string.IsNullOrEmpty(_toast))
                GUI.Label(new Rect(safe.xMin + 8, top + 8 + btnH, safe.width - 16, btnH), _toast, _label);

            if (!_open) return;

            var panel = new Rect(safe.xMin + 6, top + btnH + 10, safe.width - 12, safe.height * 0.62f);
            GUI.color = new Color(0f, 0f, 0f, 0.92f);
            GUI.Box(panel, GUIContent.none, _box);
            GUI.Box(panel, GUIContent.none, _box);
            GUI.color = prev;

            GUILayout.BeginArea(new Rect(panel.x + 6, panel.y + 6, panel.width - 12, panel.height - 12));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Copy", _button, GUILayout.Height(btnH))) Copy();
            if (GUILayout.Button("Clear", _button, GUILayout.Height(btnH))) Clear();
            if (GUILayout.Button(_showInfo ? "All logs" : "Warn+Err", _button, GUILayout.Height(btnH))) _showInfo = !_showInfo;
            if (GUILayout.Button("Close", _button, GUILayout.Height(btnH))) _open = false;
            GUILayout.EndHorizontal();
            GUILayout.Label($"<b>{_errorCount}</b> errors · <b>{_warningCount}</b> warnings · scene {_currentScene} · v{Application.version}", _label);

            _scroll = GUILayout.BeginScrollView(_scroll);
            int shown = 0;
            foreach (var e in _entries)
            {
                if (!_showInfo && !IsProblem(e.type)) continue;
                shown++;
                string color = IsError(e.type) ? "#ff7070" : (e.type == LogType.Warning ? "#ffd060" : "#c0c0c0");
                string rep = e.count > 1 ? $" ×{e.count}" : "";
                GUILayout.Label($"<color={color}>[{e.time:HH:mm:ss}] {e.type}{rep} ({e.scene})</color>\n{Escape(e.message)}", _label);
                if (IsError(e.type) && !string.IsNullOrEmpty(e.stack))
                    GUILayout.Label($"<color=#909090>{Escape(Trim(e.stack, 600))}</color>", _label);
            }
            if (shown == 0)
                GUILayout.Label("No warnings or errors yet. (Toggle 'All logs' to see everything.)", _label);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        static string Escape(string s) => s.Replace("<", "‹").Replace(">", "›");
        static string Trim(string s, int max) => s.Length <= max ? s : s.Substring(0, max) + "…";

        void Clear()
        {
            _entries.Clear();
            _errorCount = _warningCount = _unseenErrors = 0;
        }

        void Copy()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Heartline Protocol playtest log ===");
            sb.AppendLine($"App {Application.productName} v{Application.version} ({Application.identifier}) · Unity {Application.unityVersion}");
            sb.AppendLine($"Device {SystemInfo.deviceModel} · {SystemInfo.operatingSystem} · {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsDeviceType}) · RAM {SystemInfo.systemMemorySize}MB");
            sb.AppendLine($"Screen {Screen.width}x{Screen.height} @ {Screen.dpi:0}dpi · safe {Screen.safeArea}");
            sb.AppendLine($"Scene {_currentScene} · captured {DateTime.Now:yyyy-MM-dd HH:mm:ss zzz}");
            sb.AppendLine($"{_errorCount} errors · {_warningCount} warnings");
            sb.AppendLine();
            foreach (var e in _entries)
            {
                if (!_showInfo && !IsProblem(e.type)) continue;
                sb.AppendLine($"[{e.time:HH:mm:ss}] {e.type}{(e.count > 1 ? " x" + e.count : "")} ({e.scene}): {e.message}");
                if (IsProblem(e.type) && !string.IsNullOrEmpty(e.stack))
                    sb.AppendLine("    " + Trim(e.stack, 1500).Replace("\n", "\n    "));
            }
            GUIUtility.systemCopyBuffer = sb.ToString();
            _toast = "Copied log to clipboard — paste it into your bug report.";
            _toastUntil = Time.unscaledTime + 3f;
        }
    }
}
