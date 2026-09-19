# HEARTLINE PROTOCOL — Tech Architecture (Production + Vertical Slice)

**Document type:** Engineering architecture & day-1 bootstrap  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Build-ready  
**Sources:** GDD · Art Bible · Roster Kits · Vertical Slice Plan  

**Binding policy:** Adults 18+ only. Age-gate and content flags are first-class architecture, not afterthoughts.

---

## 1. Engine Decision (LOCKED)

| Decision | Call |
|----------|------|
| **Production engine** | **Unity 6 LTS** (mobile-first, **portrait** 1080×1920 logical) |
| **Render** | URP 2D / 2D Renderer + UI Toolkit or uGUI (pick **uGUI** for slice speed; UI Toolkit optional post-slice) |
| **Data** | ScriptableObjects + JSON tables (Addressables) |
| **Optional web prototype** | **Not production.** If needed for pitch decks only: Unity WebGL *or* a disposable React board mock — **do not** dual-maintain combat logic. Slice and live ship Unity. |
| **Backend (live)** | HTTPS game API (Node/Go/C# ASP.NET — **recommend C# ASP.NET** for shared models with Unity) + Postgres + Redis + CDN |
| **Slice offline** | **Yes** — local economy, local RNG gacha, no auth required |

### Rationale — why Unity 6 LTS
1. Mobile portrait gacha + Spine/Live2D + Addressables is a known path.  
2. LTS stability for 18–24 month live ops.  
3. Team hiring / asset store / tooling density vs Unreal for 2D UI-heavy.  
4. Single codebase → iOS / Android / Steam later (landscape UI scale flag).  
5. GDD already assumed Unity-or-Unreal; this doc **ends the waffle: Unity**.

---

## 2. Client Module Map

```
AppBootstrap
 ├── AgeGateModule
 ├── ContentFlagsModule          # SFW / Adult / Region
 ├── SaveModule                  # local (+ cloud later)
 ├── AnalyticsModule
 ├── HubModule                   # Atelier Tower navigation
 ├── SyncMatchModule             # Board + Field + skills
 ├── RoomModule                  # Affection Aff1–3 (slice) / Aff1–10 (live)
 ├── GachaModule                 # stub local → live authoritative
 ├── MetaModule                  # Jobs, Mail, Missions, Shop shell, Team
 └── DebugModule                # slice only / #if DEVELOPMENT_BUILD
```

| Module | Responsibilities | Slice | Live |
|--------|------------------|-------|------|
| **Hub** | Floor graph, agent presence, entry points | Lobby/Suite/Room/doors | Full floors, furniture |
| **Sync Match** | Board sim, gem gen, gauges, lanes, turns, VFX hooks | 4 agents, Ch.1 | Full roster, modifiers |
| **Room** | Talk/Gift/Date/Sleepover, Aff XP, unlocks | Aff1–3 SFW | Aff1–10 + gated NSFW loader |
| **Gacha stub** | Banner UI, rates copy, local pull | Beginner only | Authoritative pulls + pity |
| **Meta** | Team, Jobs, Mail, Dailies, currencies HUD | Minimal | Shop, Pass, Arena hooks |

---

## 3. Data-Driven Content

### ScriptableObjects (authoring in Editor)

| SO Type | Examples | Fields (core) |
|---------|----------|---------------|
| `AgentDefinition` | Mira, Ivy, Juniper, Wren | id, age, rarity, role, gems[], lanes[], skills[], giftsPreferred[], bondIds[] |
| `SkillDefinition` | Soft Pulse, Prove It… | id, type Basic/Burst/Passive/Bond, multipliers, targeting |
| `StageDefinition` | `stage_1_01` … `1_10` | stamina, waves[], enemyTags[], teachingTips[], dialogueBefore/After |
| `EnemyDefinition` | Swarmling, Lease Reclaimer | hp, atk, lane, tags, telegraphs |
| `DialogueSequence` | prologue_mira_meet | lines[] speaker/text/voNote/flags |
| `BannerDefinition` | beginner_banner | pool[], rates, pity rules display |
| `GiftDefinition` | sunwell_tea_t1 | tier, affXp, preferredAgents[] |
| `JobDefinition` | cafe_shift | duration, yields[] |
| `AffRankReward` | mira_aff_2 | unlocks flags, mail |

### JSON tables (exported / hot-tunable)

```
/StreamingAssets/Tables/
  agents.json          # optional runtime override
  stages_ch1.json
  economy_constants.json   # stamina regen, costs
  localization_en.json
  content_flags_default.json
```

**Rule:** Combat balance numbers live in SO + JSON; code reads interfaces `IAgentData`, never hardcodes Mira’s ATK.

**Age field:** Every `AgentDefinition.age >= 18`. CI validator fails build if age < 18 or missing.

---

## 4. Server Needs — Slice vs Live

| Concern | Vertical Slice | Live / Soft Launch |
|---------|----------------|--------------------|
| Auth | None (device save) | OAuth / platform / email |
| Gacha | Local RNG (seeded, logged) | **Authoritative** pull API; client shows reveal only |
| Currency | Local wallet | Server wallet; anti-cheat reconcile |
| IAP | None | Store + receipt validate |
| NSFW packs | Not present | CDN signed URLs after age + consent |
| Leaderboards / Arena | None | Async defense API |
| Cloud save | None | Conflict-resolve last-write + merge currencies server-side |
| Analytics | Local log + optional Firebase | Full event pipeline |

**Slice can be 100% offline.** Live must never trust client for crystals, pity counters, or pull results.

---

## 5. Folder Structure Proposal (Unity project)

```
HeartlineProtocol/
├── Assets/
│   ├── _Project/
│   │   ├── Art/                 # final + PH_ prefixes
│   │   │   ├── Characters/
│   │   │   ├── UI/
│   │   │   ├── VFX/
│   │   │   └── Environment/
│   │   ├── Audio/
│   │   ├── AddressableAssets/
│   │   ├── Data/
│   │   │   ├── Agents/
│   │   │   ├── Stages/
│   │   │   ├── Skills/
│   │   │   ├── Dialogue/
│   │   │   └── Tables/
│   │   ├── Prefabs/
│   │   ├── Scenes/
│   │   ├── Scripts/
│   │   │   ├── App/
│   │   │   ├── Hub/
│   │   │   ├── SyncMatch/
│   │   │   ├── Room/
│   │   │   ├── Gacha/
│   │   │   ├── Meta/
│   │   │   ├── Save/
│   │   │   ├── ContentFlags/
│   │   │   ├── Analytics/
│   │   │   └── Shared/
│   │   ├── Settings/            # URP, Addressables profiles
│   │   └── Tests/               # EditMode + PlayMode
│   ├── Plugins/
│   ├── ThirdParty/
│   └── Resources/               # avoid; prefer Addressables
├── ProjectSettings/
├── Packages/manifest.json
├── Tools/                       # CI scripts, table validators
├── Docs/                        # copies or links to workspace GDDs
└── Builds/
```

**NSFW (live only):** separate Addressables group `Content_Adult` — **not imported in slice branch**.

---

## 6. Scene List

| Scene | Purpose | Load |
|-------|---------|------|
| `00_Boot` | Splash, version, AgeGate, flag load, save migrate | First |
| `01_Title` | New Game / Continue / Settings | After boot |
| `02_Hub` | Atelier Tower additive UI + 2D backdrop | Primary |
| `03_SyncMatch` | Combat (additive or single) | From stage select |
| `04_Room` | Mira Room (reusable RoomController) | From Hub |
| `05_Gacha` | Banner reveal theater | Modal or scene |
| `99_Debug` | Cheat menu (dev) | Dev only |

Dialogue can be overlay canvases inside Hub / Match / Room — no separate scene required for slice.

---

## 7. Save System

### Slice
- Path: `Application.persistentDataPath/heartline_save_v1.dat`  
- Format: JSON → AES or XOR+HMAC lightweight obfuscation (deter casual edit, not military)  
- Schema version int; migrator chain `v1→v2`  
- Autosave: on stage clear, Aff change, pull, app pause  

### Contents (slice)
```json
{
  "schema": 1,
  "playerIdLocal": "uuid",
  "ageGatePassed": true,
  "contentFlags": { "sfwMode": true, "adultUnlocked": false },
  "story": { "ch1Stage": "1-7", "ch1Complete": false },
  "agents": [{ "id": "mira", "level": 12, "affRank": 2, "affXp": 40 }],
  "wallet": { "stamina": 84, "heartEnergy": 35, "lumaCoins": 12000, "crystals": 1600, "tickets": 10 },
  "gacha": { "beginnerPullsUsed": 10, "pitySoft": 10 },
  "jobs": [{ "id": "cafe", "completeAt": "iso8601" }],
  "analyticsSessionId": "..."
}
```

### Live additions
- Server `accountId`, inventory eTags, pull receipts, cloud revision.

---

## 8. Age-Gate / Content Flags Architecture

```
ContentFlags
  ageGatePassed: bool          # required for New Game
  sfwMode: bool                # default true
  adultConsent: bool           # default false; requires ageGate + explicit opt-in
  regionAllowsAdult: bool      # from storefront / geo config
  buildIncludesAdultGroup: bool # compile / Addressables define

CanShowNSFW = ageGatePassed && adultConsent && !sfwMode
              && regionAllowsAdult && buildIncludesAdultGroup
```

| Build | `buildIncludesAdultGroup` | Notes |
|-------|---------------------------|-------|
| Slice / Store SFW | **false** | No adult assets in binary |
| Soft-launch Mature | true | Still requires runtime consent |

**Room Intimate action:** compiled out or permanently hidden when `!CanShowNSFW`.  
**CI:** scan for `NSFW_` asset paths on `slice` and `store_sfw` branches → fail if found.

---

## 9. Analytics Events List

Emit at least to debug sink; wire Firebase/Amplitude later without renaming.

| Event | When | Key props |
|-------|------|-----------|
| `app_boot` | Boot | app_ver, platform |
| `age_gate_result` | Confirm/deny | passed |
| `ftue_step` | Each tutorial beat | step_id, t_ms |
| `stage_start` | Enter fight | stage_id, team[], stamina_cost |
| `stage_clear` | Win | stage_id, stars, turns, duration_ms |
| `stage_fail` | Lose | stage_id, reason |
| `burst_used` | Burst fire | agent_id, stage_id |
| `room_enter` | Open Room | agent_id |
| `room_action` | Talk/Gift/Date/Sleepover | action, agent_id, aff_xp_delta, aff_rank |
| `aff_rank_up` | Rank change | agent_id, new_rank |
| `gacha_open` | Banner UI | banner_id |
| `gacha_pull` | Pull resolve | banner_id, count, results_rarity[] |
| `job_start` / `job_claim` | Jobs | job_id, yields |
| `currency_delta` | Any wallet change | currency, delta, source |
| `ch1_complete` | Boss clear | duration_session_ms |
| `session_end` | Pause/quit | play_minutes |

**PII:** none in slice. No email/device advertising IDs until privacy policy + consent live.

---

## 10. CI / Build Pipeline Sketch

```
PR opened
  → Unity EditMode tests (validators: age≥18, stage IDs, SO null refs)
  → Script compile (all defines)
  → Optional PlayMode smoke: boot→1-1 clear (editor)
merge main
  → Nightly: Android IL2CPP dev APK + Windows portrait zip
  → Tag `slice-x.y.z`: store-unsigned APK + iOS archive (manual sign OK early)
```

Tools: **Unity Builder** on GitHub Actions / GameCI, or local `Tools/build.sh`.  
Artifacts: `Builds/Android/`, `Builds/iOS/`, crash symbol upload later.

---

## 11. Third-Party SDKs

| SDK | Slice | Live |
|-----|-------|------|
| Unity Addressables | Yes | Yes |
| Unity Analytics **or** Firebase Analytics | Optional stub | Yes |
| TextMeshPro | Yes | Yes |
| Spine / Live2D | Optional (static sprites OK) | Yes for hub |
| **IAP** (Unity IAP) | **Later** | Soft launch |
| **Auth** (platform + custom) | **Later** | Soft launch |
| Crashlytics | Optional | Yes |
| Push notifications | No | Post-soft |
| Ads | **Never for romance core** — not planned |

Do not add SDKs that force ATT/privacy complexity before first playtest.

---

## 12. Repo Bootstrap Checklist (Day 1)

Engineers run this in order on day 1:

1. [ ] Install **Unity 6 LTS** (version pinned in `ProjectSettings/ProjectVersion.txt` once created) + Android Build Support + iOS Build Support (Mac)  
2. [ ] Clone repo; open project; confirm URP 2D template or convert  
3. [ ] Create folder tree per §5; empty `.gitkeep` where needed  
4. [ ] Add assemblies: `Heartline.App`, `Heartline.SyncMatch`, `Heartline.Hub`, `Heartline.Room`, `Heartline.Shared` (asmdef)  
5. [ ] Import TMP essentials; create gem color materials from art bible hex  
6. [ ] Create scenes `00_Boot` … `05_Gacha`; add to Build Settings  
7. [ ] Author SO stubs: 4 agents, 11 stages, economy_constants  
8. [ ] Implement `ContentFlags` + AgeGate stub scene flow  
9. [ ] Wire local SaveModule round-trip test  
10. [ ] Add EditMode test `AgentAgeValidator` (fail if age < 18)  
11. [ ] Copy design docs into `Docs/` or submodule link from `/workspace`  
12. [ ] Run empty Android build once (prove toolchain)  
13. [ ] Read vertical slice + sprint backlog; pull **HP-001**  

**Day 1 exit:** Hub empty scene loads, AgeGate → Title → Hub stub, save file writes, validators green.

---

## 13. Performance Budgets (mobile)

| Budget | Target |
|--------|--------|
| Hub RAM | ≤ 400 MB mid-tier |
| Match RAM | ≤ 500 MB |
| Hub FPS | 30 stable (60 on flagship OK) |
| Match FPS | 30 min; board input <100 ms latency |
| Cold start to Title | ≤ 8 s on mid Android |
| Addressable first Hub download (live) | ≤ 50 MB base |

---

## 14. Security Notes (live foresight)

- Never embed master RNG seed that equals server seed.  
- Pity counters server-side.  
- Heart Energy / Stamina: server timestamp regen on live; local OK for slice.  
- Cheat DebugModule stripped from release (`DEVELOPMENT_BUILD` only).

*— End of Tech Architecture —*
