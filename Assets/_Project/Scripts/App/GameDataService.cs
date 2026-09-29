using UnityEngine;
using Heartline.SyncMatch;

namespace Heartline.App
{
    public class GameDataService : MonoBehaviour
    {
        public static GameDataService Instance { get; private set; }

        public AgentCatalog Agents => ContentRegistry.Agents;
        public StageCatalog Stages => ContentRegistry.Stages;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Reload();
        }

        public void Reload()
        {
            var agents = LoadJson<AgentCatalog>("agents.json");
            var stages = LoadJson<StageCatalog>("stages_ch1.json");
            ContentRegistry.Set(agents, stages);
            ValidateAges();
        }

        public AgentDefinition GetAgent(string id) => ContentRegistry.GetAgent(id);

        void ValidateAges()
        {
            if (Agents?.agents == null) return;
            foreach (var a in Agents.agents)
                if (a.age < 18)
                    Debug.LogError($"[HP] Agent age < 18: {a.id}={a.age}");
        }

        static T LoadJson<T>(string file)
        {
            // Android: StreamingAssets is inside the APK, so System.IO.File can't see it.
            var json = Heartline.Shared.JsonLoader.TryReadStreamingText(file);
            if (string.IsNullOrEmpty(json)) return default;
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HP] Bad JSON in {file}: {e.Message}");
                return default;
            }
        }
    }
}
