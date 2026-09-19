# Heartline Protocol — Vertical Slice (Unity 6 LTS)

Offline / SFW prototype of **Heartline Protocol**: Sync Match (match-3 + lanes + Sync Gauge bursts) + Hub / Stage Select + Mira Room Aff 1–3 + local economy / gacha / jobs.

**Content policy:** All agents are adults **18+**. Juniper is **21**. Slice Aff caps at **3**. No NSFW assets or systems.

**Slice version:** `0.2.0-slice` · save schema **v3**

## Open in Unity (preferred)

1. Install **Unity 6 LTS** (project pins `6000.0.32f1` in `ProjectSettings/ProjectVersion.txt` — use closest 6000.0 LTS if needed).
2. Open folder: this repo root (`heartline-protocol`).
3. Let Unity import TMP / uGUI packages from `Packages/manifest.json`.
4. Open `Assets/_Project/Scenes/00_Boot.unity` and press Play.
5. **No Editor Canvas wiring required** — `SliceRuntimeBootstrap` builds Boot→Title→Hub→StageSelect→Battle→Room (and Team/Gacha/Jobs) UI at runtime with legacy uGUI Text.

Portrait target: **1080×1920**.

## Play path

1. **00_Boot** — Age gate (18+) → Title  
2. **01_Title** — New Game (grants Mira) · **Debug: Reset Save (dev)**  
3. **02_Hub** — STA / HE (regen timers) / Luma Coins / Pulse Crystals / Echo Tickets  
4. **03_StageSelect** — Ch.1 stages with ★ earned, STA cost, unlock reasons  
5. **08_Battle** — 6×8 board, Assist Aim, Burst, End Turn, Retreat  
6. After `1-1`: Ivy + Juniper · After `1-5`: Wren · After `1-10`: Ch.1 complete  
7. **04_Room** — Talk / Gift / Date / Sleepover + **Buy Tea (−500 LC)** sink  
8. **06_Gacha** — Beginner banner rates + soft/hard pity · free 10-pull once · Ticket/Crystal 10-pull · dup→fragments  
9. **07_Jobs** — Café / Pier Patrol / Clinic Assist · preferred-agent +20% · Claim All  

Debug: Hub “refill STA/HE”; Jobs “complete now”; Title “Reset Save”.

## Economy constants (aligned)

| Resource | Cap / rule |
|----------|------------|
| Stamina | 120 · regen 1 / 6 min |
| Heart Energy | 60 · regen 1 / 8 min |
| Pull | 160 PC · 10-pull 1600 · or 10 Echo Tickets |
| Beginner pity | SR soft@10 · SSR soft@30 · hard@40 |
| Café | 1200 LC / 2h · Wren preferred ×1.2 → 1440 |
| Gift shop | Sunwell Tea 500 LC |

Source of truth: `Docs/heartline-protocol-economy-notes.md` + `Assets/StreamingAssets/Tables/economy_constants.json`.

## What’s implemented vs stubbed

| Area | Status |
|------|--------|
| Runtime UI bootstrap | **Done** |
| Board / Sync Gauge / lanes / stages | **Done** |
| Room Aff 1–3 SFW | **Done** |
| Versioned save v3 + STA/HE timestamp regen | **Done** |
| Beginner pity + ticket/crystal spend + fragments | **Done** (local stub) |
| Jobs ×3 + claim-all + preferred bonus | **Done** |
| Stage select ★ / STA / unlock reasons | **Done** |
| Luma gift sink (Buy Tea) | **Done** |
| FTUE dialogue + battle polish + team + bond | **Done** |
| Live2D / final art / VO | **Stub** |
| Live server / IAP / Aff4+ / NSFW | **Out of slice** |

## Verify combat + economy math without Unity (Linux / CI)

```bash
./Tools/run_sim_tests.sh
# or:
cd Tools/Heartline.Sim && dotnet run -c Release
```

Expected: **111 passed, 0 failed** (board, Prism, gauge, combo, lanes, ages, Aff, Room, stages, FTUE, stars, Bond, squad, regen, pity, jobs, gift sink, unlock reasons).

## Design docs

Under `Docs/` (BUILD_READY, vertical slice, tech arch, FTUE, backlog, roster kits, art bible, economy).

## Module map (asmdefs)

`Heartline.Shared` · `Heartline.SyncMatch` · `Heartline.App` · `Heartline.Hub` · `Heartline.Room` · `Heartline.Save` · `Heartline.ContentFlags` · `Heartline.Analytics` · `Heartline.Gacha` · `Heartline.Meta`

## Palette (art bible)

Crimson `#E23B4A` · Azure `#2F7BFF` · Verdant `#2DBE6A` · Amber `#F0A12A` · Violet `#8B5CF6` · Prism `#F5F7FF` · Neon Magenta `#FF2D8A` · Sync Cyan `#00D4FF` · Quay Indigo `#12182B` · Atelier Gold `#E8C87A`

---
Heartline Protocol vertical slice · 2026-09-19
