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
    /// Path order mirrors <see cref="ArtLoader"/>: StreamingAssets → Assets/_Project/Art.
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

        /// <summary>Absolute file path if the loop exists on disk; otherwise null.</summary>
        public static string ResolveIdleLoopFilePath(string agentId)
        {
            var relative = IdleLoopRelativePath(agentId);
            if (string.IsNullOrEmpty(relative)) return null;

            var streaming = Path.Combine(Application.streamingAssetsPath, relative);
            if (File.Exists(streaming)) return streaming;

            var projectArt = Path.Combine(Application.dataPath, "_Project", relative);
            if (File.Exists(projectArt)) return projectArt;

            return null;
        }

        /// <summary>file:// URL for VideoPlayer.url, or null if missing.</summary>
        public static string ResolveIdleLoopUrl(string agentId)
        {
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
                player.Prepare();
                player.prepareCompleted -= OnPrepared;
                player.prepareCompleted += OnPrepared;
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

        public static void Stop(ref VideoPlayer player, ref RenderTexture rt)
        {
            if (player != null)
            {
                player.prepareCompleted -= OnPrepared;
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
