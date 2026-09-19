using System;
using UnityEngine;

namespace Heartline.Analytics
{
    public static class AnalyticsSink
    {
        public static void Emit(string eventName, string props = null)
        {
            var line = $"[HP Analytics] {DateTime.UtcNow:o} {eventName} {props}";
            Debug.Log(line);
        }
    }
}
