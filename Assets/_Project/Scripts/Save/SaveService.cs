using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Heartline.Shared;

namespace Heartline.Save
{
    [Serializable]
    public class HeartlineSave
    {
        public int schema = 1;
        public string playerIdLocal;
        public bool ageGatePassed;
        public bool sfwMode = true;
        public string handlerName = "Handler";
        public string ch1Stage = "1-1";
        public bool ch1Complete;
        public int stamina = GameConstants.StaminaCap;
        public int heartEnergy = GameConstants.HeartEnergyCap;
        public int lumaCoins = 5000;
        public int pulseCrystals = 1600;
        public int beginnerTickets = 10;
        public long lastStaminaUtc;
        public long lastHeartUtc;
        public List<AgentSave> agents = new List<AgentSave>();
        public List<string> unlockedAgents = new List<string> { "mira" };
        public List<string> clearedStages = new List<string>();
        public SquadSave squad = new SquadSave();
        public int miraAffXp;
        public int miraAffRank = 1;
        public bool datedMiraToday;
        public int sunwellTeaCount = 2;
        public bool beginnerFreePullUsed;
        public string cafeJobCompleteAtIso;
        public string cafeAssignedAgentId;

        // Convenience aliases for UI/battle scripts
        public int Stamina { get => stamina; set => stamina = value; }
        public int HeartEnergy { get => heartEnergy; set => heartEnergy = value; }
        public int LumaCoins { get => lumaCoins; set => lumaCoins = value; }
        public int PulseCrystals { get => pulseCrystals; set => pulseCrystals = value; }
        public List<string> UnlockedAgents => unlockedAgents;
        public List<string> ClearedStages => clearedStages;
    }

    [Serializable]
    public class AgentSave
    {
        public string id;
        public int level = 1;
        public int xp;
    }

    [Serializable]
    public class SquadSave
    {
        public string front0 = "juniper";
        public string front1 = "ivy";
        public string mid0 = "mira";
        public string mid1 = "";
        public string back0 = "wren";
        public string back1 = "";
    }

    public class SaveService : MonoBehaviour
    {
        public static SaveService Instance { get; private set; }
        public HeartlineSave Data { get; private set; }

        string PathFile => System.IO.Path.Combine(Application.persistentDataPath, "heartline_save_v1.json");

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadOrCreate();
        }

        public void LoadOrCreate()
        {
            if (File.Exists(PathFile))
            {
                try
                {
                    Data = JsonUtility.FromJson<HeartlineSave>(File.ReadAllText(PathFile));
                    if (Data == null) throw new Exception("null save");
                }
                catch
                {
                    Data = NewGame();
                }
            }
            else Data = NewGame();
            ApplyRegen();
        }

        public HeartlineSave NewGame()
        {
            var s = new HeartlineSave
            {
                playerIdLocal = Guid.NewGuid().ToString("N"),
                lastStaminaUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                lastHeartUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                agents = new List<AgentSave>
                {
                    new AgentSave { id = "mira", level = 5 }
                }
            };
            return s;
        }

        public void Save()
        {
            ApplyRegen();
            File.WriteAllText(PathFile, JsonUtility.ToJson(Data, true));
        }

        public void ResetAll()
        {
            if (File.Exists(PathFile)) File.Delete(PathFile);
            Data = NewGame();
            Save();
        }

        public void ApplyRegen()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Regen(ref Data.stamina, ref Data.lastStaminaUtc, now, GameConstants.StaminaCap, GameConstants.StaminaRegenMinutes * 60);
            Regen(ref Data.heartEnergy, ref Data.lastHeartUtc, now, GameConstants.HeartEnergyCap, GameConstants.HeartRegenMinutes * 60);
        }

        static void Regen(ref int value, ref long lastUtc, long now, int cap, int secondsPerPoint)
        {
            if (value >= cap) { lastUtc = now; return; }
            var elapsed = now - lastUtc;
            if (elapsed < secondsPerPoint) return;
            var points = (int)(elapsed / secondsPerPoint);
            value = Math.Min(cap, value + points);
            lastUtc += points * (long)secondsPerPoint;
        }
    }
}
