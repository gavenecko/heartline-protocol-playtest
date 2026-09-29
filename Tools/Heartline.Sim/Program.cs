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
        TestBuildArtCoverage();
        TestAffRankThresholds();
        TestRoomTalkGift();
        TestStageCombatMath();
        TestStageUnlockOrder();
        TestDialogueRunner();
        TestStageStarRating();
        TestBondMatrix();
        TestSquadLogic();
        TestEconomyRegen();
        TestGachaPity();
        TestJobPayouts();
        TestGiftShopSink();
        TestStageUnlockReason();
        TestGrowthXpAndPromote();
        TestDailyMissions();
        TestArenaMarks();
        TestTowerFloorUnlock();

        Console.WriteLine();
        Console.WriteLine($"Passed: {_passed}  Failed: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    
    static void TestDialogueRunner()
    {
        var catalog = new DialogueCatalog
        {
            sequences = new System.Collections.Generic.List<DialogueSequence>
            {
                new DialogueSequence
                {
                    id = "title_new",
                    trigger = "title_new",
                    once = true,
                    lines = new System.Collections.Generic.List<DialogueLine>
                    {
                        new DialogueLine { speaker = "Mira", text = "Hello, {Handler}." }
                    }
                },
                new DialogueSequence
                {
                    id = "pre_1_1",
                    trigger = "pre_stage",
                    stageId = "1-1",
                    once = true,
                    lines = new System.Collections.Generic.List<DialogueLine>
                    {
                        new DialogueLine { speaker = "System", text = "Fight!" }
                    }
                }
            }
        };
        var runner = new DialogueRunner(catalog);
        runner.HandlerName = "Alex";
        Assert(runner.TryStart("title_new", out var pb), "dialogue: start title_new");
        Assert(pb.TryCurrent(out var sp, out var tx) && sp == "Mira" && tx.Contains("Alex"), "dialogue: handler token");
        Assert(!pb.Advance(), "dialogue: single line finishes");
        runner.Complete(pb);
        Assert(runner.HasSeen("title_new"), "dialogue: marked seen");
        Assert(!runner.TryStart("title_new", out _), "dialogue: once blocks replay");
        Assert(runner.TryStartByTrigger("pre_stage", "1-1", out var pre), "dialogue: trigger+stage");
        Assert(pre.SequenceId == "pre_1_1", "dialogue: stage-filtered id");
    }

    static void TestStageStarRating()
    {
        Assert(StageStarRating.Rate(1, 3, 8) == 1, "stars: 1 alive → 1");
        Assert(StageStarRating.Rate(2, 9, 8) == 2, "stars: 2 alive slow → 2");
        Assert(StageStarRating.Rate(3, 5, 8) == 3, "stars: 3 alive fast → 3");
        Assert(StageStarRating.ThresholdForStageType("Boss") == 12, "stars: boss threshold");
        Assert(StageStarRating.FormatStars(3) == "★★★", "stars: format 3");
    }

    static void TestBondMatrix()
    {
        Assert(BondMatrix.IsMiraIvyBondActive(new[] { "mira", "ivy", "wren" }), "bond: mira+ivy");
        Assert(!BondMatrix.IsMiraIvyBondActive(new[] { "mira", "wren" }), "bond: missing ivy");
        Assert(System.Math.Abs(BondMatrix.AtkMultiplier(true, false) - 1.15f) < 0.001f, "bond: atk buff");
        Assert(System.Math.Abs(BondMatrix.AtkMultiplier(true, true) - 1f) < 0.001f, "bond: consumed");
    }

    static void TestSquadLogic()
    {
        string f0 = "", f1 = "", m0 = "mira", m1 = "", b0 = "", b1 = "";
        var unlocked = new System.Collections.Generic.List<string> { "mira", "ivy", "juniper", "wren" };
        Assert(SquadLogic.TryAssign("juniper", "Front", 0, unlocked, ref f0, ref f1, ref m0, ref m1, ref b0, ref b1), "squad: assign juniper front");
        Assert(SquadLogic.TryAssign("ivy", "Front", 1, unlocked, ref f0, ref f1, ref m0, ref m1, ref b0, ref b1), "squad: assign ivy front");
        Assert(SquadLogic.TryAssign("wren", "Back", 0, unlocked, ref f0, ref f1, ref m0, ref m1, ref b0, ref b1), "squad: assign wren");
        Assert(SquadLogic.CountFilled(f0, f1, m0, m1, b0, b1) <= SquadLogic.MaxSquadSize, "squad: filled ≤ max");
        var next = SquadLogic.NextOption(f0, unlocked, SquadLogic.OccupiedExcept(f0, f0, f1, m0, m1, b0, b1));
        Assert(next != null, "squad: next option");
        Assert(SquadLogic.TryAssign("mira", "Mid", 0, unlocked, ref f0, ref f1, ref m0, ref m1, ref b0, ref b1), "squad: reassign mira");
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
        Assert(catalog.Agents.Count(a => !a.Secret) == 12, "agents: launch 12 in data");
        Assert(catalog.Agents.Count(a => a.Secret) == 2, "agents: 2 secret in data");
        Assert(catalog.Agents.Any(a => a.Id == "velvet" && a.Secret), "agents: velvet is secret");
        Assert(catalog.Agents.Any(a => a.Id == "rook" && a.Secret), "agents: rook is secret");
        Assert(catalog.Agents.Count == 14, "agents: roster 14 total");
        foreach (var a in catalog.Agents)
        {
            Assert(a.Age >= 18, $"agents: {a.Id} age {a.Age} ≥ 18");
            if (a.Id == "juniper")
                Assert(a.Age == 21, "agents: Juniper is 21");
        }
        var playable = new[] { "mira", "ivy", "juniper", "wren" };
        foreach (var id in playable)
            Assert(catalog.Agents.Any(a => a.Id == id), $"agents: playable {id}");
        Assert(catalog.Agents.Any(a => a.Id == "aveline" && a.Age == 30), "agents: aveline teaser adult");
        Assert(catalog.Agents.All(a => a.Age >= 18), "agents: no underage");
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

    
    static void TestStageCombatMath()
    {
        Assert(StageCombatMath.EnemyHp(new StageDefinition { Id = "1-1", Type = "Story" }) == 900, "stage: 1-1 hp 900");
        Assert(StageCombatMath.EnemyHp(new StageDefinition { Id = "1-5", Type = "Story" }) == 900 + 4 * 80, "stage: 1-5 hp scales");
        Assert(StageCombatMath.EnemyHp(new StageDefinition { Id = "1-7", Type = "EliteLite" }) == 1800, "stage: elite hp");
        Assert(StageCombatMath.EnemyHp(new StageDefinition { Id = "1-10", Type = "Boss" }) == 3500, "stage: boss hp");
        Assert(StageCombatMath.EnemyAtk(new StageDefinition { Id = "1-10", Type = "Boss" }) == 110, "stage: boss atk");
        Assert(StageCombatMath.StageOrdinal("1-1") == 0, "stage: ordinal 1-1");
        Assert(StageCombatMath.StageOrdinal("1-10") == 9, "stage: ordinal 1-10");
        Assert(StageCombatMath.PreviousStageId("1-5") == "1-4", "stage: prev of 1-5");
        Assert(StageCombatMath.PreviousStageId("1-B") == "1-10", "stage: prev of 1-B");
    }

    static void TestStageUnlockOrder()
    {
        var cleared = new HashSet<string>();
        Assert(StageCombatMath.IsUnlocked("1-1", cleared, false), "unlock: 1-1 open");
        Assert(!StageCombatMath.IsUnlocked("1-2", cleared, false), "unlock: 1-2 locked");
        cleared.Add("1-1");
        Assert(StageCombatMath.IsUnlocked("1-2", cleared, false), "unlock: 1-2 after 1-1");
        Assert(!StageCombatMath.IsUnlocked("1-B", cleared, false), "unlock: 1-B locked");
        cleared.Add("1-10");
        Assert(StageCombatMath.IsUnlocked("1-B", cleared, false), "unlock: 1-B after 1-10");
    }

    









    





    






    static void TestEconomyRegen()
    {
        int sta = 100;
        long last = 1_000_000;
        // 6 min = 360s per point → after 720s expect +2
        int gained = EconomyRegen.ApplyStamina(ref sta, ref last, 1_000_000 + 720);
        Assert(gained == 2 && sta == 102, "regen: stamina +2 over 12 min");
        Assert(last == 1_000_000 + 720, "regen: lastUtc advanced by points*interval");

        int he = 59;
        long hLast = 2_000_000;
        // 8 min = 480s → +1 then at cap snaps
        gained = EconomyRegen.ApplyHeart(ref he, ref hLast, 2_000_000 + 480);
        Assert(gained == 1 && he == 60, "regen: HE +1 to cap");
        Assert(hLast == 2_000_000 + 480, "regen: HE lastUtc at cap snap path");

        int full = 120;
        long fLast = 3_000_000;
        gained = EconomyRegen.ApplyStamina(ref full, ref fLast, 3_000_000 + 10_000);
        Assert(gained == 0 && full == 120 && fLast == 3_000_000 + 10_000, "regen: at cap no backlog");

        Assert(EconomyRegen.StaminaSecondsPerPoint == 360, "regen: STA 6 min");
        Assert(EconomyRegen.HeartSecondsPerPoint == 480, "regen: HE 8 min");
    }

    static void TestGachaPity()
    {
        // Hard pity: force SSR by pull 40
        int pitySr = 0, pitySsr = 39;
        var rng = new Random(1);
        var r = GachaMath.Roll(rng, ref pitySr, ref pitySsr);
        Assert(r == GachaRarity.SSR, "pity: hard@40 → SSR");
        Assert(pitySsr == 0 && pitySr == 0, "pity: counters reset on SSR");

        // Soft SR@10
        pitySr = 9; pitySsr = 5;
        r = GachaMath.Roll(new Random(2), ref pitySr, ref pitySsr);
        Assert(r == GachaRarity.SR || r == GachaRarity.SSR, "pity: soft@10 → ≥SR");
        if (r == GachaRarity.SR) Assert(pitySr == 0, "pity: SR resets SR counter");

        // Soft SSR@30
        pitySr = 0; pitySsr = 29;
        r = GachaMath.Roll(new Random(3), ref pitySr, ref pitySsr);
        Assert(r == GachaRarity.SSR, "pity: soft@30 → SSR");

        // Duplicate → fragments
        var unlocked = new List<string> { "mira" };
        var frags = new Dictionary<string, int>();
        int got = GachaMath.ApplyResult("mira", GachaRarity.SSR, unlocked, frags);
        Assert(got == GachaMath.FragSsr && frags["mira"] == 50, "gacha: dup mira → 50 frag");
        got = GachaMath.ApplyResult("ivy", GachaRarity.R, unlocked, frags);
        Assert(got == 0 && unlocked.Contains("ivy"), "gacha: new ivy unlock");

        int tix = 10, cry = 1600;
        Assert(GachaMath.TrySpendTenPull(ref tix, ref cry, true, out _) && tix == 0, "gacha: spend tickets");
        Assert(GachaMath.TrySpendTenPull(ref tix, ref cry, false, out _) && cry == 0, "gacha: spend crystals");
        Assert(!GachaMath.TrySpendTenPull(ref tix, ref cry, true, out _), "gacha: reject empty wallet");
    }

    static void TestJobPayouts()
    {
        var cafe = JobMath.Get("cafe_shift");
        Assert(cafe != null && cafe.YieldLumaCoins == 1200, "jobs: cafe base 1200");
        Assert(JobMath.ComputePayout(cafe, "wren") == 1440, "jobs: cafe preferred Wren +20%");
        Assert(JobMath.ComputePayout(cafe, "mira") == 1200, "jobs: cafe non-pref base");

        var pier = JobMath.Get("pier_patrol");
        Assert(pier != null && JobMath.ComputePayout(pier, "ivy") == 720, "jobs: pier Ivy bonus");
        var clinic = JobMath.Get("clinic_assist");
        Assert(clinic != null && JobMath.ComputePayout(clinic, "juniper") == 1080, "jobs: clinic Juni bonus");

        Assert(JobMath.Catalog.Length == 3, "jobs: 3 slice jobs");
        Assert(JobMath.IsReady(DateTime.UtcNow.AddSeconds(-1).ToString("o"), DateTime.UtcNow), "jobs: ready when past");
        Assert(!JobMath.IsReady(DateTime.UtcNow.AddMinutes(5).ToString("o"), DateTime.UtcNow), "jobs: not ready future");
    }

    static void TestGiftShopSink()
    {
        int lc = 500, stock = 0;
        Assert(GiftShop.TryBuy("sunwell_tea_t1", ref lc, ref stock, out _) && lc == 0 && stock == 1, "gift: buy tea 500 LC");
        Assert(!GiftShop.TryBuy("sunwell_tea_t1", ref lc, ref stock, out _), "gift: reject broke");
    }

    static void TestStageUnlockReason()
    {
        var cleared = new HashSet<string>();
        Assert(StageCombatMath.UnlockReason("1-2", cleared, false).Contains("1-1"), "unlock reason: 1-2 needs 1-1");
        Assert(StageCombatMath.UnlockReason("1-B", cleared, false).Contains("1-10"), "unlock reason: 1-B needs boss");
        cleared.Add("1-1");
        Assert(StageCombatMath.UnlockReason("1-2", cleared, false) == null, "unlock reason: null when open");
    }


    static void TestGrowthXpAndPromote()
    {
        Assert(GrowthMath.LevelCapForStars(1) == 40, "growth: ★ cap 40");
        Assert(GrowthMath.LevelCapForStars(2) == 50, "growth: ★★ cap 50");
        Assert(GrowthMath.LevelCapForStars(3) == 60, "growth: ★★★ cap 60");
        Assert(GrowthMath.LevelCapForStars(4) == 70, "growth: ★★★★ cap 70");
        Assert(GrowthMath.LevelCapForStars(5) == 80, "growth: ★★★★★ cap 80");
        Assert(GrowthMath.BaseStarsForRarity("R") == 2, "growth: R base ★★");
        Assert(GrowthMath.BaseStarsForRarity("SSR") == 4, "growth: SSR base ★★★★");
        Assert(GrowthMath.XpToNextLevel(1) == 52, "growth: xp curve L1→2 = 52");
        Assert(GrowthMath.XpToNextLevel(10) == 160, "growth: xp curve L10→11 = 160");

        int level = 5, xp = 0;
        int gained = GrowthMath.ApplyXp(ref level, ref xp, 200, 50);
        Assert(gained >= 1 && level > 5, "growth: apply xp levels up");
        Assert(xp < GrowthMath.XpToNextLevel(level), "growth: leftover xp below next");

        // Cap soft lock
        level = 50; xp = 0;
        gained = GrowthMath.ApplyXp(ref level, ref xp, 9999, 50);
        Assert(gained == 0 && level == 50 && xp == 0, "growth: at ★★ cap no overflow");

        // Promote ★★→★★★ costs
        Assert(GrowthMath.TryGetPromoteCost(2, out int fr, out int lc, out int aff) && fr == 50 && lc == 20_000 && aff == 0, "growth: ★★→★★★ cost");
        Assert(GrowthMath.TryGetPromoteCost(3, out fr, out lc, out aff) && fr == 100 && lc == 80_000 && aff == 2, "growth: ★★★→★★★★ Aff≥2");
        Assert(GrowthMath.TryGetPromoteCost(4, out fr, out lc, out aff) && aff == 4, "growth: ★★★★→★★★★★ Aff≥4");

        int stars = 2, luma = 25_000, frags = 60;
        Assert(GrowthMath.TryPromote(ref stars, ref luma, ref frags, affRank: 1, out int cap, out _), "growth: promote ivy ★★→★★★");
        Assert(stars == 3 && cap == 60 && frags == 10 && luma == 5_000, "growth: promote spends frags+LC");

        stars = 4; luma = 300_000; frags = 250;
        Assert(!GrowthMath.TryPromote(ref stars, ref luma, ref frags, affRank: 3, out _, out var err), "growth: block 5★ without Aff4");
        Assert(err == "aff", "growth: Aff gate error");

        Assert(GrowthMath.FormatStars(3) == "★★★", "growth: format stars");
        Assert(GrowthMath.XpBar(50, 100, 10).Contains("█") && GrowthMath.XpBar(50, 100, 10).Contains("░"), "growth: xp bar");
    }

    static void TestDailyMissions()
    {
        Assert(DailyMissionMath.Catalog.Length == 3, "daily: 3 missions");
        DailyMissionMath.Totals(out int luma, out int cry, out int tix);
        Assert(luma == 1200 && cry == 50 && tix == 1, "daily: totals 1200 LC + 50 PC + 1 tix");

        string day = "";
        var progress = new Dictionary<string, int>();
        var claimed = new List<string>();
        var now = new DateTime(2026, 9, 19, 12, 0, 0, DateTimeKind.Utc);
        Assert(DailyMissionMath.EnsureDay(ref day, progress, claimed, now), "daily: first ensure resets");
        Assert(day == "2026-09-19", "daily: utc date");

        DailyMissionMath.AddProgress(progress, "clear_stage", 1, 1);
        Assert(DailyMissionMath.IsComplete(progress, DailyMissionMath.Get("clear_stage")), "daily: clear complete");

        int walletLc = 0, walletPc = 0, walletTix = 0;
        Assert(DailyMissionMath.TryClaim("clear_stage", progress, claimed, ref walletLc, ref walletPc, ref walletTix, out _), "daily: claim clear");
        Assert(walletLc == 500 && claimed.Contains("clear_stage"), "daily: +500 LC");
        Assert(!DailyMissionMath.TryClaim("clear_stage", progress, claimed, ref walletLc, ref walletPc, ref walletTix, out _), "daily: no double claim");

        Assert(!DailyMissionMath.TryClaim("room_talk", progress, claimed, ref walletLc, ref walletPc, ref walletTix, out _), "daily: incomplete talk");
        DailyMissionMath.AddProgress(progress, "room_talk", 1, 1);
        DailyMissionMath.AddProgress(progress, "claim_job", 1, 1);
        Assert(DailyMissionMath.TryClaim("room_talk", progress, claimed, ref walletLc, ref walletPc, ref walletTix, out _), "daily: claim talk");
        Assert(DailyMissionMath.TryClaim("claim_job", progress, claimed, ref walletLc, ref walletPc, ref walletTix, out _), "daily: claim job");
        Assert(walletLc == 1200 && walletPc == 50 && walletTix == 1, "daily: full day payout");

        // Day roll resets
        var next = now.AddDays(1);
        Assert(DailyMissionMath.EnsureDay(ref day, progress, claimed, next), "daily: next day reset");
        Assert(progress.Count == 0 && claimed.Count == 0 && day == "2026-09-20", "daily: cleared on roll");
    }


    static void TestArenaMarks()
    {
        Assert(ArenaMath.FreeAttacksPerDay == 3, "arena: 3 free attacks");
        Assert(ArenaMath.MarksForRecord(2, 1) == 2 * ArenaMath.MarksWin + ArenaMath.MarksLoss, "arena: marks record");
        Assert(ArenaMath.FakeOpponents.Length == 3, "arena: 3 fake defense teams");

        string day = "";
        int used = 0, marks = 0;
        var now = new DateTime(2026, 9, 19, 15, 0, 0, DateTimeKind.Utc);
        Assert(ArenaMath.EnsureDay(ref day, ref used, now), "arena: day ensure");
        Assert(day == "2026-09-19", "arena: utc day");

        // Win vs Quay Rookie
        var opp = ArenaMath.PickOpponent(used);
        Assert(ArenaMath.TryAttack(ref used, ref marks, opp, out bool won, out int gained, out _), "arena: attack 1");
        Assert(won && gained == ArenaMath.MarksWin && marks == ArenaMath.MarksWin, "arena: win marks");
        Assert(used == 1 && ArenaMath.AttacksRemaining(used) == 2, "arena: 2 left");

        // Second win
        opp = ArenaMath.PickOpponent(used);
        Assert(ArenaMath.TryAttack(ref used, ref marks, opp, out won, out gained, out _), "arena: attack 2");
        // Third — Aurelia Scout loses
        opp = ArenaMath.PickOpponent(used);
        Assert(opp.HandlerName == "Aurelia Scout", "arena: third opp scout");
        Assert(ArenaMath.TryAttack(ref used, ref marks, opp, out won, out gained, out _), "arena: attack 3");
        Assert(!won && gained == ArenaMath.MarksLoss, "arena: loss marks");
        Assert(ArenaMath.AttacksRemaining(used) == 0, "arena: cap reached");
        Assert(!ArenaMath.TryAttack(ref used, ref marks, opp, out _, out _, out var err) && err == "cap", "arena: 4th blocked");

        string week = ArenaMath.WeekKeyUtc(now);
        Assert(week.StartsWith("2026-W"), "arena: week key");

        // Day roll resets attacks
        Assert(ArenaMath.EnsureDay(ref day, ref used, now.AddDays(1)), "arena: next day");
        Assert(used == 0 && day == "2026-09-20", "arena: attacks reset");
    }

    static void TestTowerFloorUnlock()
    {
        Assert(TowerMath.SliceFloorCount == 10 && TowerMath.FullFloorCount == 50, "tower: 10 of 50");
        Assert(TowerMath.CheckpointEvery == 5, "tower: CP every 5");
        Assert(TowerMath.DefaultStaminaCost(1) == 6 && TowerMath.DefaultStaminaCost(6) == 8, "tower: STA costs");
        Assert(TowerMath.IsCheckpoint(5) && TowerMath.IsCheckpoint(10) && !TowerMath.IsCheckpoint(4), "tower: checkpoint flags");
        Assert(TowerMath.IsFloorUnlocked(1, 0) && !TowerMath.IsFloorUnlocked(2, 0), "tower: only F1 unlocked at start");
        Assert(TowerMath.CanAttempt(1, 0) && !TowerMath.CanAttempt(2, 0), "tower: attempt F1 only");

        int sta = 120, highest = 0, cp = 0;
        for (int f = 1; f <= 5; f++)
        {
            Assert(TowerMath.TryClear(f, ref sta, ref highest, ref cp, 10, TowerMath.DefaultStaminaCost, out _), $"tower: clear F{f}");
        }
        Assert(highest == 5 && cp == 5, "tower: CP5 after clear");
        Assert(sta == 120 - 5 * 6, "tower: STA spent 1–5");

        // Clear 6–7 then fail 8 → snap to CP5
        Assert(TowerMath.TryClear(6, ref sta, ref highest, ref cp, 10, TowerMath.DefaultStaminaCost, out _), "tower: clear F6");
        Assert(TowerMath.TryClear(7, ref sta, ref highest, ref cp, 10, TowerMath.DefaultStaminaCost, out _), "tower: clear F7");
        Assert(highest == 7 && cp == 5, "tower: mid-band no new CP");
        Assert(TowerMath.TryFail(8, ref sta, ref highest, cp, 10, TowerMath.DefaultStaminaCost, out _), "tower: fail F8");
        Assert(highest == 5, "tower: fail snaps to CP");
        Assert(TowerMath.NextAttemptAfterFail(cp) == 6, "tower: next after fail = 6");
        Assert(TowerMath.CanAttempt(6, highest), "tower: resume at F6");

        // Climb to slice cap
        for (int f = 6; f <= 10; f++)
            Assert(TowerMath.TryClear(f, ref sta, ref highest, ref cp, 10, TowerMath.DefaultStaminaCost, out _), $"tower: clear F{f} b");
        Assert(highest == 10 && cp == 10, "tower: slice complete CP10");
        Assert(!TowerMath.CanAttempt(11, highest), "tower: F11 beyond slice");
        Assert(!TowerMath.TryClear(10, ref sta, ref highest, ref cp, 10, TowerMath.DefaultStaminaCost, out var doneErr) && doneErr == "complete", "tower: no re-clear at cap");
    }

    /// <summary>
    /// Player builds can only see Assets/Resources (Resources.Load) and StreamingAssets.
    /// Every texture the runtime UI asks ArtLoader for must exist under Assets/Resources.
    /// </summary>
    static void TestBuildArtCoverage()
    {
        var agentsPath = FindAgentsJson();
        if (agentsPath == null) { Assert(false, "art: agents.json found"); return; }
        var assets = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(agentsPath), "..", ".."));
        var resources = Path.Combine(assets, "Resources");
        bool Has(string resPath) =>
            resPath != null && new[] { ".png", ".jpg", ".jpeg" }.Any(e => File.Exists(Path.Combine(resources, resPath + e)));

        var catalog = AgentDefinition.LoadFromJson(File.ReadAllText(agentsPath));
        foreach (var a in catalog.Agents)
        {
            Assert(Heartline.Shared.ArtCatalog.TryGetFolder(a.Id, out _), $"art: {a.Id} mapped to a folder");
            Assert(Has(Heartline.Shared.ArtCatalog.BustResourcePath(a.Id)), $"art: {a.Id} bust in Resources");
            Assert(Has(Heartline.Shared.ArtCatalog.SdResourcePath(a.Id)), $"art: {a.Id} sd in Resources");
            Assert(Has(Heartline.Shared.ArtCatalog.FullbodyResourcePath(a.Id)), $"art: {a.Id} fullbody in Resources");
        }
        foreach (var id in new[] { "velvet", "rook" })
        {
            Assert(Has(Heartline.Shared.ArtCatalog.IdlePosterResourcePath(id)), $"art: {id} idle poster in Resources");
            var loop = Heartline.Shared.ArtCatalog.IdleLoopRelativePath(id);
            Assert(File.Exists(Path.Combine(assets, "StreamingAssets", loop)), $"art: {id} idle loop in StreamingAssets");
        }
        var rooms = new[]
        {
            Heartline.Shared.ArtCatalog.RoomSuiteEvening, Heartline.Shared.ArtCatalog.RoomAgentLoft,
            Heartline.Shared.ArtCatalog.RoomCafe, Heartline.Shared.ArtCatalog.RoomRainLounge,
            Heartline.Shared.ArtCatalog.RoomNeonOverlook, Heartline.Shared.ArtCatalog.RoomRooftop,
            Heartline.Shared.ArtCatalog.RoomSleepoverMorning, Heartline.Shared.ArtCatalog.RoomClinic
        };
        foreach (var r in rooms)
            Assert(Has(Heartline.Shared.ArtCatalog.RoomResourcePath(r)), $"art: room {r} in Resources");
        foreach (var action in new[] { "talk", "gift", "date", "sleepover", "morning" })
            Assert(Has(Heartline.Shared.ArtCatalog.RoomResourcePath(Heartline.Shared.ArtCatalog.RoomForAction(action))),
                $"art: room for {action} in Resources");
        var chrome = new[]
        {
            Heartline.Shared.ArtCatalog.NavBottom, Heartline.Shared.ArtCatalog.ButtonsPanelsSheet,
            Heartline.Shared.ArtCatalog.CurrencySta, Heartline.Shared.ArtCatalog.CurrencyHe,
            Heartline.Shared.ArtCatalog.CurrencyLc, Heartline.Shared.ArtCatalog.CurrencyPc,
            Heartline.Shared.ArtCatalog.CurrencyTicket, Heartline.Shared.ArtCatalog.CurrencyMarks
        };
        foreach (var c in chrome)
            Assert(Has(Heartline.Shared.ArtCatalog.ChromeResourcePath(c)), $"art: chrome {c} in Resources");
        foreach (var r in new[] { "N", "R", "SR", "SSR", "UR" })
            Assert(Has(Heartline.Shared.ArtCatalog.FrameResourcePath(r)), $"art: frame {r} in Resources");
        foreach (Heartline.Shared.GemColor g in Enum.GetValues(typeof(Heartline.Shared.GemColor)))
        {
            if (g == Heartline.Shared.GemColor.Empty) continue;
            Assert(Has(Heartline.Shared.ArtCatalog.GemResourcePath(Heartline.Shared.ArtCatalog.GemFileForColor(g))),
                $"art: gem {g} in Resources");
        }
        Assert(Has(Heartline.Shared.ArtCatalog.MarketingResourcePath(Heartline.Shared.ArtCatalog.KeyVisual)),
            "art: key visual in Resources");
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
