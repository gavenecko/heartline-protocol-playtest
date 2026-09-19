using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Heartline.SyncMatch;

namespace Heartline.App
{
    public class GameDataService : MonoBehaviour
    {
        public static GameDataService Instance { get; private set; }

        public AgentCatalog Agents { get; private set; }
        public StageCatalog Stages { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Reload();
        }

        public void Reload()
        {
            Agents = LoadJson<AgentCatalog>("agents.json");
            Stages = LoadJson<StageCatalog>("stages_ch1.json");
            ValidateAges();
        }

        public AgentDefinition GetAgent(string id)
        {
            if (Agents?.agents == null) return null;
            return Agents.agents.Find(a => a.id == id);
        }

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

    [Serializable]
    public class StageCatalog
    {
        public int chapter = 1;
        public List<StageDefinition> stages = new List<StageDefinition>();
    }

    [Serializable]
    public class StageDefinition
    {
        public string id;
        public string name;
        public string type;
        public int staminaCost;
        public List<string> enemyTags = new List<string>();
        public string teachingGoal;
        public bool forceMiraBurst;
        public string unlockAgentOnClear;
        public bool setsCh1Complete;
        public StageRewards rewards;
    }

    [Serializable]
    public class StageRewards
    {
        public int lumaCoins;
        public int xp;
        public int crystals;
    }
}
