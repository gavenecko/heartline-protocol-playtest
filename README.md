# Heartline Protocol — Vertical Slice (Unity 6 LTS)

Offline / SFW prototype of **Heartline Protocol**: Sync Match (match-3 + lanes + Sync Gauge bursts) + Hub / Stage Select + Mira Room Aff 1–3 + local economy stubs.

**Content policy:** All agents are adults **18+**. Juniper is **21**. Slice Aff caps at **3**. No NSFW assets or systems.

## Open in Unity (preferred)

1. Install **Unity 6 LTS** (project pins `6000.0.32f1` in `ProjectSettings/ProjectVersion.txt` — use closest 6000.0 LTS if needed).
2. Open folder: this repo root (`heartline-protocol`).
3. Let Unity import TMP / uGUI packages from `Packages/manifest.json`.
4. Open `Assets/_Project/Scenes/00_Boot.unity` and press Play.
5. **No Editor Canvas wiring required** — `SliceRuntimeBootstrap` builds Boot→Title→Hub→StageSelect→Battle→Room (and Team/Gacha/Jobs) UI at runtime with legacy uGUI Text.

Portrait target: **1080×1920**.

## Play path

1. **00_Boot** — Age gate (18+) → Title  
2. **01_Title** — New Game (grants Mira)  
3. **02_Hub** — STA / HE / Luma Coins / Pulse Crystals; nav to Stages, Mira Room, Team, Banner, Jobs, Mail, Settings  
4. **03_StageSelect** — Chapter 1 stages `1-1`…`1-10` + `1-B` (sequential unlock)  
5. **08_Battle** — 6×8 board (tap two adjacent gems), Assist Aim, Burst buttons, End Turn, Retreat  
6. After `1-1`: Ivy + Juniper unlock · After `1-5`: Wren unlock · After `1-10`: Ch.1 complete  
7. **04_Room** — Mira Talk / Gift / Date / Sleepover (Aff 1–3 SFW only)  
8. **06_Gacha** / **07_Jobs** — Beginner soft banner + Café Shift stubs  

Debug: Hub “refill STA/HE”; Jobs “complete now”.

## What’s implemented vs stubbed

| Area | Status |
|------|--------|
| Runtime UI bootstrap (Boot→…→Battle/Room) | **Done** (no Editor wiring needed) |
| Board 6×8, match≥3, cascades, 5 colors + Prism | **Done** (pure C# + Tools harness) |
| Sync Gauge fill / Burst @100% + lock | **Done** |
| 3-lane Front/Mid/Back modifiers | **Done** |
| Stage combat sizing + sequential unlock (`StageCombatMath`) | **Done** |
| Mira / Ivy / Juniper / Wren JSON kits | **Done** (`StreamingAssets/Tables/agents.json`) |
| Ch.1 stage table `1-1`…`1-10` + `1-B` | **Done** (`stages_ch1.json`) |
| Battle loop: input, gauges, Burst, lanes, rewards, wipe/retreat | **Done** (logic + runtime board UI) |
| Stages 1-1…1-5 playable in logic | **Done**; 1-6…1-10 + 1-B **data + logic ready** (same battle loop) |
| Room Aff 1–3 SFW Talk/Gift/Date/Sleepover | **Done** |
| Local save JSON + regen | **Done** |
| FTUE DialogueRunner + Ch.1 JSON (SFW 18+) | **Done** |
| Battle polish (phases, Assist tip, stars, retreat confirm) | **Done** |
| Team screen 4-slot + lane cycle + save | **Done** |
| Age gate + SFW flags | **Done** |
| Beginner banner free 10-pull / Café job | **Done** (local stubs) |
| Live2D / final art / VO | **Stub** (PH palette / letter gems) |
| Live server / IAP / Aff4+ / NSFW | **Out of slice** |
| Bond Matrix Mira↔Ivy | **Done** (once/battle +15% ATK if both in squad) |
| Full TMP scene prefabs | **Stub** (runtime legacy Text path) |

## Verify combat math without Unity (Linux / CI)

```bash
./Tools/run_sim_tests.sh
# or:
cd Tools/Heartline.Sim && dotnet run -c Release
```

Expected: **81 passed, 0 failed** (board, Prism, gauge+lock, combo, lanes, ages ≥18, Aff thresholds, Room talk/gift/date, stage HP/unlock, FTUE dialogue runner, star rating, Bond Matrix, squad logic).

## Design docs

Under `Docs/` (BUILD_READY, vertical slice, tech arch, FTUE, backlog, roster kits, art bible, economy).

## Module map (asmdefs)

`Heartline.Shared` · `Heartline.SyncMatch` · `Heartline.App` · `Heartline.Hub` · `Heartline.Room` · `Heartline.Save` · `Heartline.ContentFlags` · `Heartline.Analytics` · `Heartline.Gacha` · `Heartline.Meta`

## Palette (art bible)

Crimson `#E23B4A` · Azure `#2F7BFF` · Verdant `#2DBE6A` · Amber `#F0A12A` · Violet `#8B5CF6` · Prism `#F5F7FF` · Neon Magenta `#FF2D8A` · Sync Cyan `#00D4FF` · Quay Indigo `#12182B` · Atelier Gold `#E8C87A`

---
Heartline Protocol vertical slice · 2026-09-19
