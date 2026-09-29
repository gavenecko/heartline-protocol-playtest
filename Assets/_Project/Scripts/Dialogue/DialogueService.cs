using System.Collections.Generic;
using UnityEngine;
using Heartline.Save;
using Heartline.Shared;
using Heartline.Shared.Dialogue;
using Heartline.Analytics;

namespace Heartline.App
{
    /// <summary>Loads dialogue_ch1.json and drives FTUE / chapter sequences (SFW · 18+).</summary>
    public class DialogueService : MonoBehaviour
    {
        public static DialogueService Instance { get; private set; }

        DialogueCatalog _catalog;
        DialogueRunner _runner;
        DialoguePlayback _active;

        public bool IsPlaying => _active != null && !_active.IsComplete;
        public DialoguePlayback Active => _active;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Reload();
        }

        public void Reload()
        {
            _catalog = LoadCatalog();
            var seen = SaveService.Instance != null ? SaveService.Instance.Data.seenDialogueIds : null;
            _runner = new DialogueRunner(_catalog, seen);
            if (SaveService.Instance != null)
                _runner.HandlerName = SaveService.Instance.Data.handlerName;
            _active = null;
        }

        public void SyncFromSave()
        {
            if (_runner == null) { Reload(); return; }
            if (SaveService.Instance == null) return;
            var save = SaveService.Instance.Data;
            _runner.HandlerName = save.handlerName;
            if (save.seenDialogueIds == null) return;
            foreach (var id in save.seenDialogueIds)
                _runner.MarkSeen(id);
        }

        static DialogueCatalog LoadCatalog()
        {
            // Android: StreamingAssets is inside the APK, so System.IO.File can't see it.
            var json = StreamingFiles.TryReadText("Tables/dialogue_ch1.json");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[HP] Missing dialogue_ch1.json at " + JsonLoader.StreamingPath("dialogue_ch1.json"));
                return new DialogueCatalog();
            }
            try
            {
                return JsonUtility.FromJson<DialogueCatalog>(json) ?? new DialogueCatalog();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[HP] Bad dialogue_ch1.json: " + e.Message);
                return new DialogueCatalog();
            }
        }

        public bool TryStart(string sequenceId)
        {
            SyncFromSave();
            if (!_runner.TryStart(sequenceId, out var pb)) return false;
            _active = pb;
            AnalyticsSink.Emit("ftue_step", $"step_id={sequenceId};action=start");
            return true;
        }

        public bool TryStartTrigger(string trigger, string stageId = null)
        {
            SyncFromSave();
            if (!_runner.TryStartByTrigger(trigger, stageId, out var pb)) return false;
            _active = pb;
            AnalyticsSink.Emit("ftue_step", $"step_id={pb.SequenceId};trigger={trigger};action=start");
            return true;
        }

        public string CurrentDisplay() => _active?.CurrentDisplay() ?? "";

        public bool Advance()
        {
            if (_active == null) return false;
            if (!_active.Advance())
            {
                CompleteActive();
                return false;
            }
            return true;
        }

        public void CompleteActive()
        {
            if (_active == null) return;
            var id = _active.SequenceId;
            _runner.Complete(_active);
            PersistSeen();
            AnalyticsSink.Emit("ftue_step", $"step_id={id};action=complete");
            _active = null;
        }

        public void SkipAll()
        {
            if (_active == null) return;
            CompleteActive();
        }

        void PersistSeen()
        {
            if (SaveService.Instance == null || _runner == null) return;
            var save = SaveService.Instance.Data;
            if (save.seenDialogueIds == null) save.seenDialogueIds = new List<string>();
            foreach (var id in _runner.SeenIds)
            {
                if (!save.seenDialogueIds.Contains(id))
                    save.seenDialogueIds.Add(id);
            }
            SaveService.Instance.Save();
        }
    }
}
