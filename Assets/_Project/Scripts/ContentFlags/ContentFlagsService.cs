using UnityEngine;

namespace Heartline.ContentFlags
{
    /// <summary>
    /// Persistable content flags (PlayerPrefs). Slice stays SFW: Adult path blocked.
    /// Age gate + SFW Mode are shown on Title; callers mirror into HeartlineSave.
    /// </summary>
    public class ContentFlagsService : MonoBehaviour
    {
        public static ContentFlagsService Instance { get; private set; }

        public bool AgeGatePassed { get; private set; }
        public bool SfwMode { get; private set; } = true;
        public bool AdultUnlocked => false;
        public bool AdultConsent => false;
        public bool BuildIncludesAdultGroup => false;
        public bool RegionAllowsAdult => false;
        public bool CanShowNsfw => false;

        const string PrefAge = "hp_age_gate";
        const string PrefSfw = "hp_sfw_mode";
        const string PrefAdult = "hp_adult_unlocked";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Load()
        {
            AgeGatePassed = PlayerPrefs.GetInt(PrefAge, 0) == 1;
            SfwMode = true; // slice: always SFW
            PlayerPrefs.SetInt(PrefSfw, 1);
            PlayerPrefs.SetInt(PrefAdult, 0);
        }

        public void SyncFromSave(bool ageGatePassed, bool sfwMode)
        {
            if (ageGatePassed) AgeGatePassed = true;
            SfwMode = true;
            _ = sfwMode;
            WritePrefs();
        }

        public void ConfirmAgeGate()
        {
            AgeGatePassed = true;
            WritePrefs();
        }

        /// <summary>SFW Mode is locked ON for the slice. Attempts to disable are rejected.</summary>
        public bool TrySetSfwMode(bool enabled, out string message)
        {
            SfwMode = true;
            WritePrefs();
            if (!enabled)
            {
                message = "Adult path blocked in this slice. SFW Mode stays ON.";
                return false;
            }
            message = "SFW Mode ON · Adult packs unavailable.";
            return true;
        }

        /// <summary>Adult unlock always fails in slice builds.</summary>
        public bool TryUnlockAdult(out string message)
        {
            SfwMode = true;
            WritePrefs();
            message = "Adult Content unavailable — slice build excludes NSFW packs.";
            return false;
        }

        void WritePrefs()
        {
            PlayerPrefs.SetInt(PrefAge, AgeGatePassed ? 1 : 0);
            PlayerPrefs.SetInt(PrefSfw, 1);
            PlayerPrefs.SetInt(PrefAdult, 0);
            PlayerPrefs.Save();
        }

        public string TitleFlagsSummary()
        {
            string age = AgeGatePassed ? "Age gate: confirmed 18+" : "Age gate: required";
            return $"{age}\nSFW Mode: ON (locked)\nAdult Content: BLOCKED · Aff caps at 3 · Juniper is 21";
        }
    }
}
