using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Heartline.Shared
{
    /// <summary>
    /// Reads files shipped in StreamingAssets on every platform.
    /// Desktop / Editor: StreamingAssets is a plain folder → System.IO.
    /// Android (and WebGL): StreamingAssets lives inside the APK (jar:file://…!/assets),
    /// System.IO cannot see it → UnityWebRequest (blocking; local-file requests finish fast).
    /// </summary>
    public static class StreamingFiles
    {
        /// <summary>True when StreamingAssets must be read through UnityWebRequest (inside the APK).</summary>
        public static bool RequiresWebRequest =>
            Application.streamingAssetsPath.Contains("://") || Application.platform == RuntimePlatform.Android;

        /// <summary>Absolute path / URL of a file under StreamingAssets (forward slashes).</summary>
        public static string PathOf(string relative)
        {
            relative = (relative ?? "").Replace('\\', '/').TrimStart('/');
            return Application.streamingAssetsPath.TrimEnd('/') + "/" + relative;
        }

        /// <summary>Returns bytes, or null when the file is missing / unreadable. Never throws.</summary>
        public static byte[] TryReadBytes(string relative)
        {
            var path = PathOf(relative);
            try
            {
                if (!RequiresWebRequest)
                    return File.Exists(path) ? File.ReadAllBytes(path) : null;

                using (var req = UnityWebRequest.Get(path))
                {
                    var op = req.SendWebRequest();
                    var started = DateTime.UtcNow;
                    while (!op.isDone)
                    {
                        if ((DateTime.UtcNow - started).TotalSeconds > 10)
                        {
                            req.Abort();
                            Debug.LogWarning("[HP] Timed out reading StreamingAssets: " + path);
                            return null;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                    if (req.result != UnityWebRequest.Result.Success)
                        return null;
                    return req.downloadHandler.data;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HP] Failed reading StreamingAssets " + path + " :: " + e.Message);
                return null;
            }
        }

        public static string TryReadText(string relative)
        {
            var bytes = TryReadBytes(relative);
            if (bytes == null) return null;
            // Strip UTF-8 BOM if present.
            int offset = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
            return System.Text.Encoding.UTF8.GetString(bytes, offset, bytes.Length - offset);
        }
    }

    public static class JsonLoader
    {
        public static string StreamingPath(string relativeUnderTables) =>
            StreamingFiles.PathOf("Tables/" + relativeUnderTables);

        /// <summary>Reads StreamingAssets/Tables/&lt;file&gt;; works inside an Android APK.</summary>
        public static string ReadStreamingText(string relativeUnderTables)
        {
            var text = StreamingFiles.TryReadText("Tables/" + relativeUnderTables);
            if (text == null)
                throw new FileNotFoundException("Missing StreamingAssets table: " + StreamingPath(relativeUnderTables));
            return text;
        }

        /// <summary>Like <see cref="ReadStreamingText"/> but returns null (and logs) instead of throwing.</summary>
        public static string TryReadStreamingText(string relativeUnderTables)
        {
            var text = StreamingFiles.TryReadText("Tables/" + relativeUnderTables);
            if (text == null)
                Debug.LogError("[HP] Missing StreamingAssets table: " + StreamingPath(relativeUnderTables));
            return text;
        }

        public static T LoadFromStreaming<T>(string relativeUnderTables)
        {
            var json = ReadStreamingText(relativeUnderTables);
            return JsonUtility.FromJson<T>(json);
        }
    }
}
