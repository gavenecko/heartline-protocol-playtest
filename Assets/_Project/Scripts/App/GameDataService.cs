using System.IO;
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
            var path = Path.Combine(Application.streamingAssetsPath, "Tables", file);
            if (!File.Exists(path))
            {
                Debug.LogError("Missing " + path);
                return default;
            }
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }
    }
}
