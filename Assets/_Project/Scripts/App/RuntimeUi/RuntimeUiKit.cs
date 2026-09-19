using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Heartline.App.RuntimeUi
{
    /// <summary>Minimal uGUI builders so scenes play without Editor Canvas wiring.</summary>
    public static class RuntimeUiKit
    {
        public static Font DefaultFont()
        {
            var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null) f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return f;
        }

        public static Canvas CreateCanvas(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            return canvas;
        }

        public static Text Label(Transform parent, string name, string text, int fontSize, TextAnchor anchor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color? color = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = DefaultFont();
            t.text = text;
            t.fontSize = fontSize;
            t.alignment = anchor;
            t.color = color ?? Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = t.rectTransform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return t;
        }

        public static Button Button(Transform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax, UnityAction onClick, Color? bg = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = bg ?? new Color(0.14f, 0.18f, 0.32f, 0.95f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            if (onClick != null) btn.onClick.AddListener(onClick);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            var txt = Label(go.transform, "Label", label, 28, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            txt.raycastTarget = false;
            return btn;
        }

        public static ScrollRect ScrollColumn(Transform parent, string name, out Transform content)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            var rootRt = root.AddComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0.05f, 0.12f);
            rootRt.anchorMax = new Vector2(0.95f, 0.78f);
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;
            var scroll = root.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(root.transform, false);
            var vpRt = viewport.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = Vector2.zero;
            vpRt.offsetMax = Vector2.zero;
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.2f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(viewport.transform, false);
            var cRt = contentGo.AddComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot = new Vector2(0.5f, 1);
            cRt.sizeDelta = new Vector2(0, 0);
            var layout = contentGo.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 8;
            layout.padding = new RectOffset(8, 8, 8, 8);
            contentGo.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = vpRt;
            scroll.content = cRt;
            content = contentGo.transform;
            return scroll;
        }
    }
}
