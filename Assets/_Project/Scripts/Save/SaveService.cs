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
        public int schema = 2;
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
        public List<string> stageStarEntries = new List<string>(); // "1-1:3"
        public List<string> seenDialogueIds = new List<string>();
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

        public int GetStageStars(string stageId)
        {
            if (stageStarEntries == null || string.IsNullOrEmpty(stageId)) return 0;
            foreach (var e in stageStarEntries)
            {
                if (string.IsNullOrEmpty(e)) continue;
                var parts = e.Split(':');
                if (parts.Length == 2 && parts[0] == stageId && int.TryParse(parts[1], out var n))
                    return n;
            }
            return 0;
        }

        public void SetStageStars(string stageId, int stars)
        {
            if (string.IsNullOrEmpty(stageId)) return;
            if (stars < 1) stars = 1;
            if (stars > 3) stars = 3;
            if (stageStarEntries == null) stageStarEntries = new List<string>();
            int best = GetStageStars(stageId);
            if (best > stars) stars = best;
            for (int i = 0; i < stageStarEntries.Count; i++)
            {
                if (stageStarEntries[i] != null && stageStarEntries[i].StartsWith(stageId + ":"))
                {
                    stageStarEntries[i] = stageId + ":" + stars;
                    return;
                }
            }
            stageStarEntries.Add(stageId + ":" + stars);
        }

        public void MarkDialogueSeen(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (seenDialogueIds == null) seenDialogueIds = new List<string>();
            if (!seenDialogueIds.Contains(id)) seenDialogueIds.Add(id);
        }
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
                    if (Data.seenDialogueIds == null) Data.seenDialogueIds = new List<string>();
                    if (Data.stageStarEntries == null) Data.stageStarEntries = new List<string>();
                    if (Data.squad == null) Data.squad = new SquadSave();
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
                },
                squad = new SquadSave
                {
                    front0 = "",
                    front1 = "",
                    mid0 = "mira",
                    mid1 = "",
                    back0 = "",
                    back1 = ""
                },
                seenDialogueIds = new List<string>(),
                stageStarEntries = new List<string>()
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
