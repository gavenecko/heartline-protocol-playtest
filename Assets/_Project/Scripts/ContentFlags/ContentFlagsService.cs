using UnityEngine;

namespace Heartline.ContentFlags
{
    /// <summary>SFW-only slice flags. Adult packs compiled out.</summary>
    public class ContentFlagsService : MonoBehaviour
    {
        public static ContentFlagsService Instance { get; private set; }

        public bool AgeGatePassed { get; private set; }
        public bool SfwMode => true;
        public bool AdultUnlocked => false;
        public bool BuildIncludesAdultGroup => false;
        public bool CanShowNsfw => false;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ConfirmAgeGate()
        {
            AgeGatePassed = true;
            PlayerPrefs.SetInt("hp_age_gate", 1);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            AgeGatePassed = PlayerPrefs.GetInt("hp_age_gate", 0) == 1;
        }
    }
}
