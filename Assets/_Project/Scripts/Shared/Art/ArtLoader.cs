using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Heartline.Shared
{
    /// <summary>
    /// Offline art loader — no Addressables.
    /// Order: Resources.Load → StreamingAssets/Art LoadImage (APK-safe) → [Editor only] Assets/_Project/Art.
    /// Everything the runtime UI needs is mirrored under Assets/Resources/Art for player builds.
    /// </summary>
    public static class ArtLoader
    {
        static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();
        static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
        // Negative cache: avoids re-probing disk / APK every frame for art that doesn't exist.
        static readonly HashSet<string> Missing = new HashSet<string>();

        public static Texture2D LoadBust(string agentId) =>
            LoadTexture(ArtCatalog.BustResourcePath(agentId));

        public static Texture2D LoadSd(string agentId)
        {
            var t = LoadTexture(ArtCatalog.SdResourcePath(agentId));
            return t != null ? t : LoadBust(agentId);
        }

        public static Texture2D LoadFullbody(string agentId) =>
            LoadTexture(ArtCatalog.FullbodyResourcePath(agentId));

        public static Texture2D LoadRoom(string roomIdWithoutExt) =>
            LoadTexture(ArtCatalog.RoomResourcePath(roomIdWithoutExt));

        public static Texture2D LoadKeyVisual() =>
            LoadTexture(ArtCatalog.MarketingResourcePath(ArtCatalog.KeyVisual));

        public static Texture2D LoadNavBottom() =>
            LoadTexture(ArtCatalog.ChromeResourcePath(ArtCatalog.NavBottom));

        public static Texture2D LoadChrome(string fileWithoutExt) =>
            LoadTexture(ArtCatalog.ChromeResourcePath(fileWithoutExt));

        public static Texture2D LoadCurrency(string currencyFile) =>
            LoadChrome(currencyFile);

        public static Texture2D LoadGem(GemColor color)
        {
            var file = ArtCatalog.GemFileForColor(color);
            return string.IsNullOrEmpty(file) ? null : LoadChrome(file);
        }

        public static Texture2D LoadFrame(string rarity) =>
            LoadTexture(ArtCatalog.FrameResourcePath(rarity));

        public static Sprite BustSprite(string agentId) =>
            ToSprite(ArtCatalog.BustResourcePath(agentId), LoadBust(agentId));

        public static Sprite SdSprite(string agentId) =>
            ToSprite(ArtCatalog.SdResourcePath(agentId) ?? ArtCatalog.BustResourcePath(agentId), LoadSd(agentId));

        public static Sprite ChromeSprite(string fileWithoutExt) =>
            ToSprite(ArtCatalog.ChromeResourcePath(fileWithoutExt), LoadChrome(fileWithoutExt));

        public static Sprite GemSprite(GemColor color)
        {
            var file = ArtCatalog.GemFileForColor(color);
            return string.IsNullOrEmpty(file) ? null : ChromeSprite(file);
        }

        public static Sprite FrameSprite(string rarity) =>
            ToSprite(ArtCatalog.FrameResourcePath(rarity), LoadFrame(rarity));

        public static bool ApplyBust(RawImage target, string agentId)
        {
            if (target == null) return false;
            var tex = LoadBust(agentId);
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static bool ApplyBust(Image target, string agentId)
        {
            if (target == null) return false;
            var sp = BustSprite(agentId);
            if (sp == null)
            {
                target.enabled = false;
                return false;
            }
            target.sprite = sp;
            target.preserveAspect = true;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static bool ApplySd(RawImage target, string agentId)
        {
            if (target == null) return false;
            var tex = LoadSd(agentId);
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static bool ApplyFullbody(RawImage target, string agentId)
        {
            if (target == null) return false;
            var tex = LoadFullbody(agentId);
            if (tex == null)
            {
                // Fall back to bust so callers still get a portrait.
                return ApplyBust(target, agentId);
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static bool ApplyRoomBg(RawImage target, string roomIdWithoutExt)
        {
            if (target == null) return false;
            var tex = LoadRoom(roomIdWithoutExt);
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        /// <summary>Apply action-mapped Room BG with catalog fallback (Talk/Gift/Date/Sleepover).</summary>
        public static bool ApplyRoomBgForAction(RawImage target, string action)
        {
            if (target == null) return false;
            var primary = ArtCatalog.RoomForAction(action);
            if (ApplyRoomBg(target, primary)) return true;
            return ApplyRoomBg(target, ArtCatalog.RoomFallbackForAction(action));
        }

        public static bool ApplyChrome(RawImage target, string fileWithoutExt)
        {
            if (target == null) return false;
            var tex = LoadChrome(fileWithoutExt);
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static bool ApplyChrome(Image target, string fileWithoutExt)
        {
            if (target == null) return false;
            var sp = ChromeSprite(fileWithoutExt);
            if (sp == null) return false;
            target.sprite = sp;
            target.preserveAspect = true;
            target.color = Color.white;
            target.type = Image.Type.Simple;
            target.enabled = true;
            return true;
        }

        public static bool ApplyCurrencyIcon(RawImage target, string currencyFile) =>
            ApplyChrome(target, currencyFile);

        public static bool ApplyRarityFrame(RawImage target, string rarity) =>
            ApplyChrome(target, ArtCatalog.FrameFileForRarity(rarity));

        public static bool ApplyRarityFrame(Image target, string rarity) =>
            ApplyChrome(target, ArtCatalog.FrameFileForRarity(rarity));

        public static bool ApplyGem(Image target, GemColor color)
        {
            if (target == null) return false;
            var sp = GemSprite(color);
            if (sp == null) return false;
            target.sprite = sp;
            target.preserveAspect = true;
            target.color = Color.white;
            target.type = Image.Type.Simple;
            target.enabled = true;
            return true;
        }

        public static bool ApplyGem(RawImage target, GemColor color)
        {
            if (target == null) return false;
            var tex = LoadGem(color);
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        /// <summary>
        /// Soft-apply buttons/panels sheet to a Button Image. Keeps layout; on miss leaves existing color.
        /// </summary>
        public static bool SoftApplyButtonPanel(Image target)
        {
            if (target == null) return false;
            var sp = ChromeSprite(ArtCatalog.ButtonsPanelsSheet);
            if (sp == null) return false;
            target.sprite = sp;
            target.type = Image.Type.Simple;
            // Keep a readable tint so labels stay legible over the sheet.
            var c = target.color;
            if (c.a < 0.5f) c.a = 0.95f;
            target.color = new Color(
                Mathf.Clamp01(c.r * 0.85f + 0.15f),
                Mathf.Clamp01(c.g * 0.85f + 0.15f),
                Mathf.Clamp01(c.b * 0.85f + 0.15f),
                c.a);
            target.enabled = true;
            return true;
        }

        public static bool SoftApplyButtonPanel(Button button) =>
            button != null && SoftApplyButtonPanel(button.GetComponent<Image>());

        public static bool ApplyTexture(RawImage target, Texture2D tex)
        {
            if (target == null) return false;
            if (tex == null)
            {
                target.enabled = false;
                return false;
            }
            target.texture = tex;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        public static Texture2D LoadTexture(string resourcesPathNoExt)
        {
            if (string.IsNullOrEmpty(resourcesPathNoExt)) return null;
            if (Cache.TryGetValue(resourcesPathNoExt, out var cached) && cached != null)
                return cached;
            if (Missing.Contains(resourcesPathNoExt))
                return null;

            Texture2D tex = null;

            // 1) Resources (mirrored under Assets/Resources/Art/…)
            tex = Resources.Load<Texture2D>(resourcesPathNoExt);
            if (tex == null)
            {
                // Some Unity imports keep Texture type; try as Object then cast fails — LoadImage instead.
                var bytes = TryReadImageBytes(resourcesPathNoExt);
                if (bytes != null && bytes.Length > 0)
                {
                    tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!tex.LoadImage(bytes, true))
                    {
                        Object.Destroy(tex);
                        tex = null;
                    }
                    else
                        tex.name = resourcesPathNoExt;
                }
            }

            if (tex != null)
                Cache[resourcesPathNoExt] = tex;
            else
            {
                Missing.Add(resourcesPathNoExt);
                Debug.LogWarning("[HP] Art missing (Resources + StreamingAssets): " + resourcesPathNoExt);
            }
            return tex;
        }

        static byte[] TryReadImageBytes(string resourcesPathNoExt)
        {
            var relative = resourcesPathNoExt.Replace('\\', '/');
            string[] exts = { ".png", ".jpg", ".jpeg" };

            foreach (var ext in exts)
            {
                var relativeFile = relative + ext;

                // StreamingAssets/Art/... (works inside the Android APK via UnityWebRequest).
                var streaming = StreamingFiles.TryReadBytes(relativeFile);
                if (streaming != null && streaming.Length > 0)
                    return streaming;

#if UNITY_EDITOR
                // Editor only: Assets/ does not exist in player builds. Anything a build needs
                // must be mirrored under Assets/Resources/Art (see Docs/UI_ART_WIRING.md).
                var projectArt = Path.Combine(Application.dataPath, "_Project", relativeFile);
                if (File.Exists(projectArt))
                {
                    Debug.LogWarning("[HP] Art only found under Assets/_Project (will be MISSING in builds): " + relativeFile);
                    return File.ReadAllBytes(projectArt);
                }

                var resourcesDisk = Path.Combine(Application.dataPath, "Resources", relativeFile);
                if (File.Exists(resourcesDisk))
                    return File.ReadAllBytes(resourcesDisk);
#endif
            }

            return null;
        }


        public static Texture2D LoadIdlePoster(string agentId) =>
            LoadTexture(ArtCatalog.IdlePosterResourcePath(agentId));

        /// <summary>
        /// Prefer looping idle video for Velvet/Rook; else bust. Returns true if any portrait shown.
        /// </summary>
        public static bool ApplyPortraitOrIdle(UnityEngine.UI.RawImage target, string agentId,
            ref UnityEngine.Video.VideoPlayer player, ref RenderTexture rt, bool preferIdle)
        {
            if (target == null) return false;
            if (preferIdle && IdleVideo.SupportsIdle(agentId) && IdleVideo.TryPlay(target, agentId, ref player, ref rt))
                return true;
            IdleVideo.Stop(ref player, ref rt);
            if (ApplyBust(target, agentId)) return true;
            var poster = LoadIdlePoster(agentId);
            return ApplyTexture(target, poster);
        }

        static Sprite ToSprite(string key, Texture2D tex)
        {
            if (tex == null || string.IsNullOrEmpty(key)) return null;
            if (SpriteCache.TryGetValue(key, out var sp) && sp != null) return sp;
            sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            sp.name = key;
            SpriteCache[key] = sp;
            return sp;
        }
    }
}
