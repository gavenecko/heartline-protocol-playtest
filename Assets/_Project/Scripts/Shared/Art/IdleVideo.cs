using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Heartline.Shared
{
    /// <summary>
    /// Resolve + play Imagine idle loop MP4s (Velvet / Rook) on a RawImage via VideoPlayer.
    /// Source: StreamingAssets (APK-safe URL on Android) → [Editor only] Assets/_Project/Art.
    /// </summary>
    public static class IdleVideo
    {
        static readonly HashSet<string> IdleAgentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "velvet", "velvet_noir", "rook", "rook_hale"
        };

        public static bool SupportsIdle(string agentId)
        {
            if (string.IsNullOrEmpty(agentId)) return false;
            return IdleAgentIds.Contains(agentId.Trim());
        }

        /// <summary>Relative path under StreamingAssets / _Project, e.g. Art/Characters/velvet_noir/Idle/velvet_idle_loop.mp4</summary>
        public static string IdleLoopRelativePath(string agentId)
        {
            return ArtCatalog.IdleLoopRelativePath(agentId);
        }

        /// <summary>
        /// Absolute file path if the loop exists on disk; otherwise null.
        /// Only meaningful where StreamingAssets is a real folder (Editor / desktop).
        /// </summary>
        public static string ResolveIdleLoopFilePath(string agentId)
        {
            var relative = IdleLoopRelativePath(agentId);
            if (string.IsNullOrEmpty(relative)) return null;

            var streaming = Path.Combine(Application.streamingAssetsPath, relative);
            if (File.Exists(streaming)) return streaming;

#if UNITY_EDITOR
            var projectArt = Path.Combine(Application.dataPath, "_Project", relative);
            if (File.Exists(projectArt)) return projectArt;
#endif
            return null;
        }

        /// <summary>
        /// URL for VideoPlayer.url, or null if missing.
        /// Android: StreamingAssets is inside the APK (jar:file://…!/assets/…) — File.Exists can't
        /// see it, but VideoPlayer can play that URL directly, so we hand it over unchecked and
        /// fall back to the bust/poster via <see cref="VideoPlayer.errorReceived"/> if it fails.
        /// </summary>
        public static string ResolveIdleLoopUrl(string agentId)
        {
            var relative = IdleLoopRelativePath(agentId);
            if (string.IsNullOrEmpty(relative)) return null;

            if (StreamingFiles.RequiresWebRequest)
                return StreamingFiles.PathOf(relative);

            var path = ResolveIdleLoopFilePath(agentId);
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                return new Uri(path).AbsoluteUri;
            }
            catch
            {
                return "file://" + path.Replace('\\', '/');
            }
        }

        /// <summary>
        /// Play looping idle on <paramref name="target"/> if a loop file exists.
        /// Returns false → caller should fall back to bust/poster.
        /// </summary>
        public static bool TryPlay(RawImage target, string agentId, ref VideoPlayer player, ref RenderTexture rt)
        {
            if (target == null || !SupportsIdle(agentId)) return false;
            var url = ResolveIdleLoopUrl(agentId);
            if (string.IsNullOrEmpty(url)) return false;

            EnsurePlayer(target, ref player, ref rt);
            if (player.url != url || !player.isPlaying)
            {
                player.Stop();
                player.source = VideoSource.Url;
                player.url = url;
                player.isLooping = true;
                player.playOnAwake = false;
                player.skipOnDrop = true;
                player.audioOutputMode = VideoAudioOutputMode.None;
                player.prepareCompleted -= OnPrepared;
                player.prepareCompleted += OnPrepared;
                player.errorReceived -= OnError;
                player.errorReceived += OnError;
                FallbackAgent[player] = agentId;
                player.Prepare();
                if (player.isPrepared)
                    player.Play();
            }

            target.texture = rt;
            target.color = Color.white;
            target.enabled = true;
            return true;
        }

        static void OnPrepared(VideoPlayer vp)
        {
            if (vp != null && vp.isPrepared)
                vp.Play();
        }

        /// <summary>VideoPlayer → agent id, so a failed loop can fall back to a still.</summary>
        static readonly Dictionary<VideoPlayer, string> FallbackAgent = new Dictionary<VideoPlayer, string>();

        static void OnError(VideoPlayer vp, string message)
        {
            if (vp == null) return;
            Debug.LogWarning($"[HP] Idle video failed ({vp.url}): {message} — falling back to still.");
            vp.errorReceived -= OnError;
            vp.prepareCompleted -= OnPrepared;
            FallbackAgent.TryGetValue(vp, out var agentId);
            FallbackAgent.Remove(vp);
            var raw = vp.GetComponent<RawImage>();
            if (raw == null) return;
            if (!ArtLoader.ApplyBust(raw, agentId))
                ArtLoader.ApplyTexture(raw, ArtLoader.LoadIdlePoster(agentId));
        }

        public static void Stop(ref VideoPlayer player, ref RenderTexture rt)
        {
            if (player != null)
            {
                player.prepareCompleted -= OnPrepared;
                player.errorReceived -= OnError;
                FallbackAgent.Remove(player);
                player.Stop();
                player.url = string.Empty;
            }
            // Keep components/RT for reuse; caller may destroy on disable.
        }

        public static void Dispose(ref VideoPlayer player, ref RenderTexture rt)
        {
            if (player != null)
            {
                player.prepareCompleted -= OnPrepared;
                player.errorReceived -= OnError;
                FallbackAgent.Remove(player);
                player.Stop();
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(player);
                else
                    UnityEngine.Object.DestroyImmediate(player);
                player = null;
            }
            if (rt != null)
            {
                rt.Release();
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(rt);
                else
                    UnityEngine.Object.DestroyImmediate(rt);
                rt = null;
            }
        }

        static void EnsurePlayer(RawImage target, ref VideoPlayer player, ref RenderTexture rt)
        {
            if (rt == null)
            {
                rt = new RenderTexture(ArtCatalog.IdleLoopWidth, ArtCatalog.IdleLoopHeight, 0, RenderTextureFormat.ARGB32);
                rt.name = "IdleLoopRT";
                rt.Create();
            }

            if (player == null)
            {
                player = target.GetComponent<VideoPlayer>();
                if (player == null)
                    player = target.gameObject.AddComponent<VideoPlayer>();
            }

            player.renderMode = VideoRenderMode.RenderTexture;
            player.targetTexture = rt;
            player.enabled = true;
        }
    }
}
