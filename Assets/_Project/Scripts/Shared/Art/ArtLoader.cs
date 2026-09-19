using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Heartline.Shared
{
    /// <summary>
    /// Offline art loader — no Addressables.
    /// Order: Resources.Load → StreamingAssets/Art LoadImage → Application.dataPath/_Project/Art LoadImage.
    /// Busts are also mirrored under Assets/Resources/Art for player builds.
    /// </summary>
    public static class ArtLoader
    {
        static readonly Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();
        static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();

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

        public static Sprite BustSprite(string agentId) =>
            ToSprite(ArtCatalog.BustResourcePath(agentId), LoadBust(agentId));

        public static Sprite SdSprite(string agentId) =>
            ToSprite(ArtCatalog.SdResourcePath(agentId) ?? ArtCatalog.BustResourcePath(agentId), LoadSd(agentId));

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

            Texture2D tex = null;

            // 1) Resources (mirrored under Assets/Resources/Art/…)
            tex = Resources.Load<Texture2D>(resourcesPathNoExt);
            if (tex == null)
            {
                // Some Unity imports keep Texture type; try as Object then cast fails — LoadImage instead.
                var bytes = TryReadPngBytes(resourcesPathNoExt);
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
            return tex;
        }

        static byte[] TryReadPngBytes(string resourcesPathNoExt)
        {
            var relativePng = resourcesPathNoExt.Replace('\\', '/') + ".png";

            // StreamingAssets/Art/...
            var streaming = Path.Combine(Application.streamingAssetsPath, relativePng);
            if (File.Exists(streaming))
                return File.ReadAllBytes(streaming);

            // Editor / repo: Assets/_Project/Art/...
            var projectArt = Path.Combine(Application.dataPath, "_Project", relativePng);
            if (File.Exists(projectArt))
                return File.ReadAllBytes(projectArt);

            // Resources folder on disk (pre-import / Editor): Assets/Resources/Art/...
            var resourcesDisk = Path.Combine(Application.dataPath, "Resources", relativePng);
            if (File.Exists(resourcesDisk))
                return File.ReadAllBytes(resourcesDisk);

            return null;
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
