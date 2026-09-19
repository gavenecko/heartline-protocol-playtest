using System;
using System.IO;
using UnityEngine;

namespace Heartline.Shared
{
    public static class JsonLoader
    {
        public static string StreamingPath(string relativeUnderTables)
        {
            return Path.Combine(Application.streamingAssetsPath, "Tables", relativeUnderTables);
        }

        public static string ReadStreamingText(string relativeUnderTables)
        {
            var path = StreamingPath(relativeUnderTables);
#if UNITY_ANDROID && !UNITY_EDITOR
            // Android StreamingAssets are inside jar; for slice Editor/desktop primary.
            var www = UnityEngine.Networking.UnityWebRequest.Get(path);
            www.SendWebRequest();
            while (!www.isDone) { }
            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                throw new FileNotFoundException(path + " :: " + www.error);
            return www.downloadHandler.text;
#else
            if (!File.Exists(path))
                throw new FileNotFoundException("Missing StreamingAssets table: " + path);
            return File.ReadAllText(path);
#endif
        }

        public static T LoadFromStreaming<T>(string relativeUnderTables)
        {
            var json = ReadStreamingText(relativeUnderTables);
            return JsonUtility.FromJson<T>(json);
        }
    }
}
