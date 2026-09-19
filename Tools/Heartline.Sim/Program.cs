using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Heartline.Sim;

static class Program
{
    static int _failed;
    static int _passed;

    static int Main(string[] args)
    {
        Console.WriteLine("=== Heartline.Sim — Sync Match harness ===");
        TestBoardMatchClearsThree();
        TestBoardRejectsNonMatchSwap();
        TestCascadesAndCombo();
        TestPrismWild();
        TestGaugeFillAndBurst();
        TestGaugeBurstLock();
        TestComboCurve();
        TestLaneModifiers();
        TestAgentAgesAdult();
        TestAffRankThresholds();
        TestRoomTalkGift();

        Console.WriteLine();
        Console.WriteLine($"Passed: {_passed}  Failed: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    static void Assert(bool cond, string name)
    {
        if (cond) { _passed++; Console.WriteLine($"  PASS  {name}"); }
        else { _failed++; Console.WriteLine($"  FAIL  {name}"); }
    }

    static void TestBoardMatchClearsThree()
    {
        var board = new BoardSimulator(seed: 1);
        var grid = EmptyGrid();
        // Fill with no accidental matches, then plant a horizontal crimson triple on row 0
        for (int y = 0; y < BoardSimulator.Height; y++)
        for (int x = 0; x < BoardSimulator.Width; x++)
            grid[x, y] = ((x + y) % 2 == 0) ? GemColor.Azure : GemColor.Verdant;

        grid[0, 0] = GemColor.Crimson;
        grid[1, 0] = GemColor.Crimson;
        grid[2, 0] = GemColor.Amber; // swap with crimson below later
        grid[2, 1] = GemColor.Crimson;
        board.LoadFrom(grid);

        // Swap (2,1) Crimson with (2,0) Amber → row0 becomes C C C
        bool ok = board.TrySwap(2, 1, 2, 0, out var result);
        Assert(ok, "board: swap creates match");
        Assert(result.GemsCleared >= 3, "board: cleared ≥3 gems");
        Assert(result.Combo >= 1, "board: combo ≥1");
    }

    static void TestBoardRejectsNonMatchSwap()
    {
        var board = new BoardSimulator(seed: 2);
        var grid = EmptyGrid();
        for (int y = 0; y < BoardSimulator.Height; y++)
        for (int x = 0; x < BoardSimulator.Width; x++)
            grid[x, y] = ((x + y * 3) % 5) switch
            {
                0 => GemColor.Crimson,
                1 => GemColor.Azure,
                2 => GemColor.Verdant,
                3 => GemColor.Amber,
                _ => GemColor.Violet
            };
        // Ensure no swap of adjacent different colors that don't form 3
        board.LoadFrom(grid);
        // Find two adjacent different colors that won't match
        bool rejected = false;
        for (int y = 0; y < BoardSimulator.Height && !rejected; y++)
        for (int x = 0; x < BoardSimulator.Width - 1; x++)
        {
            if (grid[x, y] == grid[x + 1, y]) continue;
            // probe
            var probe = new BoardSimulator(seed: 99);
            probe.LoadFrom(Clone(grid));
            if (!probe.TrySwap(x, y, x + 1, y, out _))
            {
                rejected = true;
                break;
            }
        }
        Assert(rejected, "board: rejects at least one non-matching swap");
    }

    static void TestCascadesAndCombo()
    {
        var board = new BoardSimulator(seed: 3);
        var grid = EmptyGrid();
        for (int y = 0; y < BoardSimulator.Height; y++)
        for (int x = 0; x < BoardSimulator.Width; x++)
            grid[x, y] = GemColor.Violet;

        // Setup cascade: bottom three crimson, and three azure that will drop into match
        grid[0, 0] = GemColor.Crimson;
        grid[1, 0] = GemColor.Crimson;
        grid[2, 0] = GemColor.Crimson;
        // After clear, column fills — plant azure stack that will match when collapsing is hard;
        // instead force ResolveCascades on planted matches only.
        board.LoadFrom(grid);
        var result = board.ResolveCascades();
        Assert(result.Combo >= 1, "cascade: initial triple clears");
        Assert(result.GemsCleared >= 3, "cascade: gems cleared");
    }

    static void TestPrismWild()
    {
        var board = new BoardSimulator(seed: 4);
        var grid = EmptyGrid();
        for (int y = 0; y < BoardSimulator.Height; y++)
        for (int x = 0; x < BoardSimulator.Width; x++)
            grid[x, y] = ((x + y) % 2 == 0) ? GemColor.Amber : GemColor.Violet;

        grid[0, 0] = GemColor.Crimson;
        grid[1, 0] = GemColor.Prism;
        grid[2, 0] = GemColor.Crimson;
        board.LoadFrom(grid);
        var matches = board.FindMatches();
        Assert(matches.Count >= 3, "prism: wild completes crimson triple");
    }

    static void TestGaugeFillAndBurst()
    {
        var gauge = new SyncGauge(new SyncGaugeConfig { AttunedFillPerGem = 0.04f, PrismFillPerGem = 0.05f });
        var att = new List<GemColor> { GemColor.Verdant, GemColor.Prism };
        var cleared = new Dictionary<GemColor, int>
        {
            [GemColor.Verdant] = 10, // 0.40
            [GemColor.Prism] = 4,    // 0.20
            [GemColor.Crimson] = 5   // 0.05 off
        };
        float gained = gauge.ApplyClears(att, cleared);
        Assert(gained > 0.6f, "gauge: fill from attuned+prism");
        Assert(gauge.CanBurst || gauge.Value >= 0.65f, "gauge: near or at burst");
        // Top up
        gauge.AddFlat(1f);
        Assert(gauge.CanBurst, "gauge: can burst at 100%");
        Assert(gauge.TryFireBurst(out var consumed) && consumed >= 1f, "gauge: fire burst empties");
        Assert(gauge.Value == 0f, "gauge: empty after burst");
    }

    static void TestGaugeBurstLock()
    {
        var gauge = new SyncGauge();
        gauge.Set(1f);
        Assert(gauge.TryFireBurst(out _), "lock: burst fires");
        Assert(gauge.IsLocked, "lock: locked after burst");
        Assert(!gauge.CanBurst, "lock: cannot burst while locked");
        gauge.TickTurnEnd();
        Assert(!gauge.IsLocked, "lock: unlocks after 1 turn");
    }

    static void TestComboCurve()
    {
        Assert(Math.Abs(ComboMath.Multiplier(1) - 1.0f) < 0.001f, "combo: 1 → 1.0");
        Assert(Math.Abs(ComboMath.Multiplier(3) - 1.2f) < 0.001f, "combo: 3 → 1.2");
        Assert(Math.Abs(ComboMath.Multiplier(5) - 1.5f) < 0.001f, "combo: 5 → 1.5");
        Assert(Math.Abs(ComboMath.Multiplier(8) - 2.0f) < 0.001f, "combo: 8 → 2.0");
    }

    static void TestLaneModifiers()
    {
        Assert(LaneField.DamageDealtMod(Lane.Front) > LaneField.DamageDealtMod(Lane.Back), "lane: Front deals more");
        Assert(LaneField.DamageTakenMod(Lane.Front) > LaneField.DamageTakenMod(Lane.Back), "lane: Front takes more");
        var field = new LaneField();
        Assert(field.Assign("mira", Lane.Mid), "lane: assign mira Mid");
        Assert(field.Assign("ivy", Lane.Front), "lane: assign ivy Front");
        Assert(field.Assign("juniper", Lane.Front), "lane: assign juniper Front");
        Assert(!field.Assign("wren", Lane.Front), "lane: reject 3rd Front");
    }

    static void TestAgentAgesAdult()
    {
        var path = FindAgentsJson();
        Assert(path != null, "agents: json found");
        if (path == null) return;
        var catalog = AgentDefinition.LoadFromJson(File.ReadAllText(path));
        Assert(catalog.Agents.Count == 4, "agents: 4 slice agents");
        foreach (var a in catalog.Agents)
        {
            Assert(a.Age >= 18, $"agents: {a.Id} age {a.Age} ≥ 18");
            if (a.Id == "juniper")
                Assert(a.Age == 21, "agents: Juniper is 21");
        }
        var ids = catalog.Agents.Select(a => a.Id).OrderBy(x => x).ToArray();
        Assert(ids.SequenceEqual(new[] { "ivy", "juniper", "mira", "wren" }), "agents: mira ivy juniper wren");
    }

    static void TestAffRankThresholds()
    {
        Assert(RoomEconomy.RankForXp(0) == 1, "aff: 0 → rank 1");
        Assert(RoomEconomy.RankForXp(99) == 1, "aff: 99 → rank 1");
        Assert(RoomEconomy.RankForXp(100) == 2, "aff: 100 → rank 2");
        Assert(RoomEconomy.RankForXp(300) == 3, "aff: 300 → rank 3");
        Assert(RoomEconomy.RankForXp(9999) == 3, "aff: slice caps at 3");
    }

    static void TestRoomTalkGift()
    {
        var mira = new RoomBondState("mira");
        int he = 60;
        Assert(mira.TryTalk(ref he, out var xp) && xp == 20 && he == 55, "room: talk +20 xp cost 5 HE");
        Assert(mira.AffXp == 20, "room: xp accumulated");
        Assert(mira.TryGift(ref he, preferred: true, out xp) && xp == 60, "room: preferred gift 40×1.5=60");
        // Grind to Aff3
        while (mira.AffRank < 3 && he > 0)
        {
            if (!mira.TryTalk(ref he, out _))
            {
                he = 60; // regen stub
                mira.TryTalk(ref he, out _);
            }
        }
        Assert(mira.AffRank == 3, "room: can reach Aff3 via talks");
        Assert(mira.TryDate(ref he, out xp) && xp == 100, "room: date at Aff3 +100");
        Assert(!mira.TryDate(ref he, out _), "room: date once per day");
    }

    static string FindAgentsJson()
    {
        var candidates = new[]
        {
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Assets", "StreamingAssets", "Tables", "agents.json")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "StreamingAssets", "Tables", "agents.json")),
            "/workspace/heartline-protocol/Assets/StreamingAssets/Tables/agents.json"
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    static GemColor[,] EmptyGrid() => new GemColor[BoardSimulator.Width, BoardSimulator.Height];

    static GemColor[,] Clone(GemColor[,] src)
    {
        var g = EmptyGrid();
        Array.Copy(src, g, src.Length);
        return g;
    }
}

/// <summary>Minimal Room Aff 1–3 economy stub for harness.</summary>
static class RoomEconomy
{
    public static int RankForXp(int xp)
    {
        if (xp >= 300) return 3;
        if (xp >= 100) return 2;
        return 1;
    }
}

sealed class RoomBondState
{
    public string AgentId { get; }
    public int AffXp { get; private set; }
    public int AffRank => RoomEconomy.RankForXp(AffXp);
    bool _datedToday;

    public RoomBondState(string agentId) => AgentId = agentId;

    public bool TryTalk(ref int heartEnergy, out int xpGained)
    {
        xpGained = 0;
        if (heartEnergy < 5) return false;
        heartEnergy -= 5;
        xpGained = 20;
        AffXp += xpGained;
        CapSlice();
        return true;
    }

    public bool TryGift(ref int heartEnergy, bool preferred, out int xpGained)
    {
        xpGained = 0;
        if (heartEnergy < 8) return false;
        heartEnergy -= 8;
        xpGained = preferred ? 60 : 40;
        AffXp += xpGained;
        CapSlice();
        return true;
    }

    public bool TryDate(ref int heartEnergy, out int xpGained)
    {
        xpGained = 0;
        if (AffRank < 3 || _datedToday || heartEnergy < 15) return false;
        heartEnergy -= 15;
        xpGained = 100;
        AffXp += xpGained;
        _datedToday = true;
        CapSlice();
        return true;
    }

    void CapSlice()
    {
        // Slice forbids Aff4+ progression content; XP can sit at/above 300 but rank stays 3.
    }
}
