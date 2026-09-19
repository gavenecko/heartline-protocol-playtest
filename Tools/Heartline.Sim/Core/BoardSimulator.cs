using System;
using System.Collections.Generic;

namespace Heartline.Sim
{
    /// <summary>6×8 match-3: orthogonal ≥3, cascades, Prism wild.</summary>
    public sealed class BoardSimulator
    {
        public const int Width = 6;
        public const int Height = 8;

        readonly GemColor[,] _cells = new GemColor[Width, Height];
        readonly Random _rng;
        readonly float _prismChance;

        public int LastCombo { get; private set; }
        public int LastCascadeSteps { get; private set; }
        public int LastGemsCleared { get; private set; }
        public Dictionary<GemColor, int> LastClearedByColor { get; private set; } = new();

        static readonly GemColor[] BaseColors =
        {
            GemColor.Crimson, GemColor.Azure, GemColor.Verdant, GemColor.Amber, GemColor.Violet
        };

        public BoardSimulator(int? seed = null, float prismChance = 0.04f)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
            _prismChance = prismChance;
            FillInitialNoMatches();
        }

        public GemColor Get(int x, int y) => _cells[x, y];
        public void Set(int x, int y, GemColor c) => _cells[x, y] = c;
        public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        public bool AreAdjacent(int x0, int y0, int x1, int y1)
            => Math.Abs(x0 - x1) + Math.Abs(y0 - y1) == 1;

        public bool TrySwap(int x0, int y0, int x1, int y1, out MatchResolveResult result)
        {
            result = default;
            if (!InBounds(x0, y0) || !InBounds(x1, y1) || !AreAdjacent(x0, y0, x1, y1))
                return false;

            Swap(x0, y0, x1, y1);
            if (FindMatches().Count == 0)
            {
                Swap(x0, y0, x1, y1);
                return false;
            }

            result = ResolveCascades();
            return true;
        }

        public MatchResolveResult ResolveCascades()
        {
            var total = new Dictionary<GemColor, int>();
            int combo = 0, steps = 0, gems = 0;

            while (true)
            {
                var matches = FindMatches();
                if (matches.Count == 0) break;
                steps++;
                combo++;
                foreach (var (x, y) in matches)
                {
                    var c = _cells[x, y];
                    if (c == GemColor.Empty) continue;
                    if (!total.ContainsKey(c)) total[c] = 0;
                    total[c]++;
                    gems++;
                    _cells[x, y] = GemColor.Empty;
                }
                CollapseAndRefill();
            }

            LastCombo = combo;
            LastCascadeSteps = steps;
            LastGemsCleared = gems;
            LastClearedByColor = total;
            return new MatchResolveResult(combo, steps, gems, total);
        }

        public HashSet<(int x, int y)> FindMatches()
        {
            var matched = new HashSet<(int, int)>();
            for (int y = 0; y < Height; y++)
            {
                int x = 0;
                while (x < Width)
                {
                    var run = CollectRunH(x, y);
                    if (run.Count >= 3) foreach (var p in run) matched.Add(p);
                    x += Math.Max(1, run.Count);
                }
            }
            for (int x = 0; x < Width; x++)
            {
                int y = 0;
                while (y < Height)
                {
                    var run = CollectRunV(x, y);
                    if (run.Count >= 3) foreach (var p in run) matched.Add(p);
                    y += Math.Max(1, run.Count);
                }
            }
            return matched;
        }

        List<(int x, int y)> CollectRunH(int startX, int y)
        {
            var cells = new List<(int, int)>();
            var anchor = _cells[startX, y];
            if (anchor == GemColor.Empty) return cells;
            for (int x = startX; x < Width; x++)
            {
                var c = _cells[x, y];
                if (c == GemColor.Empty) break;
                if (!ColorsMatch(anchor, c)) break;
                if (anchor == GemColor.Prism && c != GemColor.Prism) anchor = c;
                cells.Add((x, y));
            }
            return cells;
        }

        List<(int x, int y)> CollectRunV(int x, int startY)
        {
            var cells = new List<(int, int)>();
            var anchor = _cells[x, startY];
            if (anchor == GemColor.Empty) return cells;
            for (int y = startY; y < Height; y++)
            {
                var c = _cells[x, y];
                if (c == GemColor.Empty) break;
                if (!ColorsMatch(anchor, c)) break;
                if (anchor == GemColor.Prism && c != GemColor.Prism) anchor = c;
                cells.Add((x, y));
            }
            return cells;
        }

        static bool ColorsMatch(GemColor a, GemColor b)
            => a == GemColor.Prism || b == GemColor.Prism || a == b;

        void CollapseAndRefill()
        {
            for (int x = 0; x < Width; x++)
            {
                int write = 0;
                for (int y = 0; y < Height; y++)
                {
                    if (_cells[x, y] != GemColor.Empty)
                    {
                        _cells[x, write] = _cells[x, y];
                        if (write != y) _cells[x, y] = GemColor.Empty;
                        write++;
                    }
                }
                for (int y = write; y < Height; y++)
                    _cells[x, y] = RollGem();
            }
        }

        void FillInitialNoMatches()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                GemColor c;
                int guard = 0;
                do { c = RollGem(); guard++; }
                while (guard < 40 && WouldCreateMatchAt(x, y, c));
                _cells[x, y] = c;
            }
        }

        bool WouldCreateMatchAt(int x, int y, GemColor c)
        {
            if (x >= 2 && ColorsMatch(_cells[x - 1, y], c) && ColorsMatch(_cells[x - 2, y], c)
                && _cells[x - 1, y] != GemColor.Empty) return true;
            if (y >= 2 && ColorsMatch(_cells[x, y - 1], c) && ColorsMatch(_cells[x, y - 2], c)
                && _cells[x, y - 1] != GemColor.Empty) return true;
            return false;
        }

        GemColor RollGem()
        {
            if (_rng.NextDouble() < _prismChance) return GemColor.Prism;
            return BaseColors[_rng.Next(BaseColors.Length)];
        }

        void Swap(int x0, int y0, int x1, int y1)
        {
            (_cells[x0, y0], _cells[x1, y1]) = (_cells[x1, y1], _cells[x0, y0]);
        }

        public void LoadFrom(GemColor[,] source)
        {
            if (source.GetLength(0) != Width || source.GetLength(1) != Height)
                throw new ArgumentException("Board must be 6×8");
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                _cells[x, y] = source[x, y];
        }

        public string Dump()
        {
            var sb = new System.Text.StringBuilder();
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    sb.Append(Short(_cells[x, y]));
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        static char Short(GemColor c) => c switch
        {
            GemColor.Crimson => 'C',
            GemColor.Azure => 'A',
            GemColor.Verdant => 'V',
            GemColor.Amber => 'M',
            GemColor.Violet => 'I',
            GemColor.Prism => 'P',
            _ => '.'
        };
    }

    public readonly struct MatchResolveResult
    {
        public readonly int Combo;
        public readonly int CascadeSteps;
        public readonly int GemsCleared;
        public readonly Dictionary<GemColor, int> ClearedByColor;

        public MatchResolveResult(int combo, int steps, int gems, Dictionary<GemColor, int> byColor)
        {
            Combo = combo;
            CascadeSteps = steps;
            GemsCleared = gems;
            ClearedByColor = byColor ?? new Dictionary<GemColor, int>();
        }
    }
}
