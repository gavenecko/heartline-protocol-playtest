using System;
using System.Collections.Generic;

namespace Heartline.Shared.Dialogue
{
    [Serializable]
    public class DialogueCatalog
    {
        public List<DialogueSequence> sequences = new List<DialogueSequence>();
    }

    [Serializable]
    public class DialogueSequence
    {
        public string id;
        public string trigger;
        public string stageId;
        public bool once = true;
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    [Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
    }

    /// <summary>Data-driven dialogue sequencer (SFW · adults 18+).</summary>
    public sealed class DialogueRunner
    {
        readonly DialogueCatalog _catalog;
        readonly HashSet<string> _seen;
        string _handlerName = "Handler";

        public DialogueRunner(DialogueCatalog catalog, IEnumerable<string> seenIds = null)
        {
            _catalog = catalog ?? new DialogueCatalog();
            _seen = new HashSet<string>(seenIds ?? Array.Empty<string>(), StringComparer.Ordinal);
        }

        public string HandlerName
        {
            get => _handlerName;
            set => _handlerName = string.IsNullOrWhiteSpace(value) ? "Handler" : value.Trim();
        }

        public IReadOnlyCollection<string> SeenIds => _seen;

        public bool HasSeen(string sequenceId) =>
            !string.IsNullOrEmpty(sequenceId) && _seen.Contains(sequenceId);

        public void MarkSeen(string sequenceId)
        {
            if (!string.IsNullOrEmpty(sequenceId))
                _seen.Add(sequenceId);
        }

        public string Format(string text)
        {
            if (string.IsNullOrEmpty(text)) return text ?? "";
            return text.Replace("{Handler}", _handlerName);
        }

        public bool TryGetSequence(string id, out DialogueSequence seq)
        {
            seq = null;
            if (_catalog?.sequences == null || string.IsNullOrEmpty(id)) return false;
            foreach (var s in _catalog.sequences)
            {
                if (s != null && s.id == id)
                {
                    seq = s;
                    return true;
                }
            }
            return false;
        }

        public bool TryStart(string sequenceId, out DialoguePlayback playback)
        {
            playback = null;
            if (!TryGetSequence(sequenceId, out var seq)) return false;
            if (seq.once && HasSeen(seq.id)) return false;
            playback = new DialoguePlayback(seq, Format);
            return true;
        }

        public bool TryStartByTrigger(string trigger, string stageId, out DialoguePlayback playback)
        {
            playback = null;
            if (_catalog?.sequences == null || string.IsNullOrEmpty(trigger)) return false;
            foreach (var s in _catalog.sequences)
            {
                if (s == null || s.trigger != trigger) continue;
                if (!string.IsNullOrEmpty(s.stageId) && s.stageId != stageId) continue;
                if (s.once && HasSeen(s.id)) continue;
                playback = new DialoguePlayback(s, Format);
                return true;
            }
            return false;
        }

        public void Complete(DialoguePlayback playback)
        {
            if (playback?.Sequence == null) return;
            if (playback.Sequence.once)
                MarkSeen(playback.Sequence.id);
        }
    }

    public sealed class DialoguePlayback
    {
        readonly Func<string, string> _format;
        int _index;

        public DialoguePlayback(DialogueSequence sequence, Func<string, string> format)
        {
            Sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            _format = format ?? (t => t);
            _index = 0;
        }

        public DialogueSequence Sequence { get; }
        public int Index => _index;
        public int Count => Sequence.lines?.Count ?? 0;
        public bool IsComplete => _index >= Count;
        public string SequenceId => Sequence.id;

        public bool TryCurrent(out string speaker, out string text)
        {
            speaker = null;
            text = null;
            if (IsComplete || Sequence.lines == null) return false;
            var line = Sequence.lines[_index];
            speaker = line?.speaker ?? "System";
            text = _format(line?.text ?? "");
            return true;
        }

        public string CurrentDisplay()
        {
            if (!TryCurrent(out var speaker, out var text)) return "";
            return $"{speaker}: {text}";
        }

        public bool Advance()
        {
            if (IsComplete) return false;
            _index++;
            return !IsComplete;
        }
    }
}
