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
        public int schema = GameConstants.SaveSchemaVersion;
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
        public int beginnerPitySr;
        public int beginnerPitySsr;
        public List<string> echoFragmentEntries = new List<string>(); // "mira:50"
        public List<JobSlotSave> jobSlots = new List<JobSlotSave>();

        // Legacy single-slot fields (migrated into jobSlots)
        public string cafeJobCompleteAtIso;
        public string cafeAssignedAgentId;

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

        public int GetFragments(string agentId)
        {
            if (echoFragmentEntries == null || string.IsNullOrEmpty(agentId)) return 0;
            foreach (var e in echoFragmentEntries)
            {
                if (string.IsNullOrEmpty(e)) continue;
                var parts = e.Split(':');
                if (parts.Length == 2 && parts[0] == agentId && int.TryParse(parts[1], out var n))
                    return n;
            }
            return 0;
        }

        public void AddFragments(string agentId, int amount)
        {
            if (string.IsNullOrEmpty(agentId) || amount <= 0) return;
            if (echoFragmentEntries == null) echoFragmentEntries = new List<string>();
            int cur = GetFragments(agentId);
            for (int i = 0; i < echoFragmentEntries.Count; i++)
            {
                if (echoFragmentEntries[i] != null && echoFragmentEntries[i].StartsWith(agentId + ":"))
                {
                    echoFragmentEntries[i] = agentId + ":" + (cur + amount);
                    return;
                }
            }
            echoFragmentEntries.Add(agentId + ":" + (cur + amount));
        }

        public JobSlotSave GetOrCreateJobSlot(string jobId)
        {
            if (jobSlots == null) jobSlots = new List<JobSlotSave>();
            foreach (var s in jobSlots)
                if (s != null && s.jobId == jobId) return s;
            var created = new JobSlotSave { jobId = jobId };
            jobSlots.Add(created);
            return created;
        }

        public void EnsureJobSlots()
        {
            if (jobSlots == null) jobSlots = new List<JobSlotSave>();
            foreach (var def in JobMath.Catalog)
                GetOrCreateJobSlot(def.Id);
            // Migrate legacy café fields
            if (!string.IsNullOrEmpty(cafeJobCompleteAtIso))
            {
                var cafe = GetOrCreateJobSlot("cafe_shift");
                if (string.IsNullOrEmpty(cafe.completeAtIso))
                {
                    cafe.completeAtIso = cafeJobCompleteAtIso;
                    cafe.assignedAgentId = cafeAssignedAgentId;
                }
                cafeJobCompleteAtIso = null;
                cafeAssignedAgentId = null;
            }
        }
    }

    [Serializable]
    public class JobSlotSave
    {
        public string jobId;
        public string assignedAgentId;
        public string completeAtIso;
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

        string PathFile => System.IO.Path.Combine(Application.persistentDataPath, "heartline_save_v3.json");
        string LegacyPath => System.IO.Path.Combine(Application.persistentDataPath, "heartline_save_v1.json");

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadOrCreate();
        }

        public void LoadOrCreate()
        {
            string path = File.Exists(PathFile) ? PathFile : (File.Exists(LegacyPath) ? LegacyPath : null);
            if (path != null)
            {
                try
                {
                    Data = JsonUtility.FromJson<HeartlineSave>(File.ReadAllText(path));
                    if (Data == null) throw new Exception("null save");
                    Migrate(Data);
                }
                catch
                {
                    Data = NewGame();
                }
            }
            else Data = NewGame();
            ApplyRegen();
        }

        static void Migrate(HeartlineSave d)
        {
            if (d.seenDialogueIds == null) d.seenDialogueIds = new List<string>();
            if (d.stageStarEntries == null) d.stageStarEntries = new List<string>();
            if (d.echoFragmentEntries == null) d.echoFragmentEntries = new List<string>();
            if (d.squad == null) d.squad = new SquadSave();
            if (d.jobSlots == null) d.jobSlots = new List<JobSlotSave>();
            d.EnsureJobSlots();
            if (d.schema < GameConstants.SaveSchemaVersion)
                d.schema = GameConstants.SaveSchemaVersion;
        }

        public HeartlineSave NewGame()
        {
            var s = new HeartlineSave
            {
                schema = GameConstants.SaveSchemaVersion,
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
                stageStarEntries = new List<string>(),
                echoFragmentEntries = new List<string>(),
                jobSlots = new List<JobSlotSave>()
            };
            s.EnsureJobSlots();
            return s;
        }

        public void Save()
        {
            ApplyRegen();
            Data.schema = GameConstants.SaveSchemaVersion;
            File.WriteAllText(PathFile, JsonUtility.ToJson(Data, true));
        }

        public void ResetAll()
        {
            if (File.Exists(PathFile)) File.Delete(PathFile);
            if (File.Exists(LegacyPath)) File.Delete(LegacyPath);
            Data = NewGame();
            Save();
        }

        public void ApplyRegen()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            EconomyRegen.ApplyStamina(ref Data.stamina, ref Data.lastStaminaUtc, now);
            EconomyRegen.ApplyHeart(ref Data.heartEnergy, ref Data.lastHeartUtc, now);
        }
    }
}
