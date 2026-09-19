# Heartline Protocol — Vertical Slice (Unity 6 LTS)

Offline / SFW prototype of **Heartline Protocol**: Sync Match (match-3 + lanes + Sync Gauge bursts) + Hub / Stage Select + Mira Room Aff 1–3 + local economy stubs.

**Content policy:** All agents are adults **18+**. Juniper is **21**. Slice Aff caps at **3**. No NSFW assets or systems.

## Open in Unity

1. Install **Unity 6 LTS** (project pins `6000.0.32f1` in `ProjectSettings/ProjectVersion.txt` — use closest 6000.0 LTS if needed).
2. Open folder: `/workspace/heartline-protocol` (or this repo root).
3. Let Unity import TMP / uGUI packages from `Packages/manifest.json`.
4. Open `Assets/_Project/Scenes/00_Boot.unity` and press Play.
5. Wire scene UI (first open): each scene has a camera stub — add Canvas + TMP + buttons and bind the listed controllers (see below).

Portrait target: **1080×1920**.

## Play path (design intent)

1. **00_Boot** — Age gate (18+) → Title  
2. **01_Title** — New Game (grants Mira)  
3. **02_Hub** — currencies (Stamina / Heart Energy / Luma Coins / Pulse Crystals)  
4. **03_StageSelect** — Chapter 1 stages `1-1`…`1-10` + `1-B`  
5. **08_Battle** — 6×8 board, Assist Aim swap, Burst buttons, End Turn  
6. After `1-1`: Ivy + Juniper unlock · After `1-5`: Wren unlock  
7. **04_Room** — Mira Talk / Gift / Date / Sleepover (Aff 1–3 SFW)  
8. **06_Gacha** / **07_Jobs** — Beginner soft banner + Café Shift stubs  

## What’s implemented vs stubbed

| Area | Status |
|------|--------|
| Board 6×8, match≥3, cascades, 5 colors + Prism | **Done** (pure C# + Tools harness) |
| Sync Gauge fill / Burst @100% + lock | **Done** |
| 3-lane Front/Mid/Back modifiers | **Done** |
| Mira / Ivy / Juniper / Wren JSON kits | **Done** (`StreamingAssets/Tables/agents.json`) |
| Ch.1 stage table | **Done** (`stages_ch1.json`) |
| Battle / Hub / Room / Jobs / Gacha controllers | **Done** (code); scenes need Canvas wiring in Editor |
| Local save JSON | **Done** |
| Age gate + SFW flags | **Done** |
| Live2D / final art / VO | **Stub** (PH palette hex only) |
| Live server / IAP / Aff4+ / NSFW | **Out of slice** |
| Bond skill Mira↔Ivy | **Data stub** |

## Verify combat math without Unity (Linux / CI)

```bash
./Tools/run_sim_tests.sh
# or:
cd Tools/Heartline.Sim && dotnet run -c Release
```

Expected: **45 passed, 0 failed** (board matches, Prism wild, gauge burst + lock, combo curve, lane caps, agent ages ≥18, Aff thresholds, Room talk/gift/date).

## Design docs

Copied under `Docs/` (BUILD_READY, vertical slice, tech arch, FTUE, backlog, roster kits, art bible, economy).

## Module map (asmdefs)

`Heartline.Shared` · `Heartline.SyncMatch` · `Heartline.App` · `Heartline.Hub` · `Heartline.Room` · `Heartline.Save` · `Heartline.ContentFlags` · `Heartline.Analytics` · `Heartline.Gacha` · `Heartline.Meta`

## Palette (art bible)

Crimson `#E23B4A` · Azure `#2F7BFF` · Verdant `#2DBE6A` · Amber `#F0A12A` · Violet `#8B5CF6` · Prism `#F5F7FF` · Neon Magenta `#FF2D8A` · Sync Cyan `#00D4FF` · Quay Indigo `#12182B` · Atelier Gold `#E8C87A`

---
Heartline Protocol vertical slice · 2026-09-19
