# HEARTLINE PROTOCOL — Vertical Slice Prototype Plan

**Document type:** First playable build scope (pre-MVP)  
**IP:** Heartline Protocol · Lumina Quay · Atelier Tower  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Build-ready  
**Sources:** `/workspace/waifu-gacha-gdd.md` · `/workspace/heartline-protocol-art-bible.md` · `/workspace/heartline-protocol-launch-roster-kits.md`

**Binding content policy:** All agents **18+**. Slice is **SFW-only** (Room Aff 1–3). NSFW Aff8+ exists later as a system footnote only — no adult scenes, packs, or erotic prose in this build.

**Engine call (locked):** **Unity 6 LTS**, mobile-first portrait. See tech architecture doc.

---

## 1. Slice Fantasy (one sentence)

*In ~20 minutes you inherit Atelier Tower, clear Chapter 1 Breaches with a found-family squad of four adult agents, and come home to deepen a SFW bond with Mira — proving the dual fantasy of Sync Match competence and apartment warmth before any live economy or NSFW systems exist.*

---

## 2. In-Scope vs Out-of-Scope

### In-scope (must ship in slice)

| Area | Spec |
|------|------|
| **Agents** | **Mira Solenne** (SSR Synergist, starter), **Ivy Cross** (R Striker), **Juniper Tate** (R Guardian), **Wren Halcyon** (SR Support, free unlock mid-Ch.1) |
| **Story** | Prologue + **Chapter 1** only (10 combat stages + 1 boss + dialogue) |
| **Combat** | Sync Match RPG: 6×8 board, 5 gem colors + Prism, 3-lane Field, Basic/Burst/Passive (Bond Skill stubbed or single Mira↔Ivy pair) |
| **Hub** | Atelier Tower lobby + Player Suite shell + one Agent Room (Mira’s) |
| **Room** | Aff ranks **1–3** SFW only: Talk, Gift (T1), Date unlock at Aff3; Sleepover morning bonus |
| **Currencies (local)** | Stamina, Heart Energy, Luma Coins, Pulse Crystals (grant-only), Standard/Beginner Tickets (grant-only) |
| **Gacha** | **Beginner Banner soft intro** UI + local RNG stub (no IAP, no live pity server) |
| **Jobs** | 1 slot: Café Shift (2h) → Luma Coins (accelerated timer OK for playtest) |
| **Meta** | Daily missions stub (3 fixed), mail inbox (scripted rewards), settings (SFW Mode locked ON, age-gate stub) |
| **Save** | Local JSON / PlayerPrefs+encrypted blob; reset debug menu |
| **Platforms** | Android + iOS portrait primary; Editor Play Mode; optional Windows portrait window for desk playtest |
| **Art** | Placeholders per policy below; 1–2 keyed Mira busts if available |

### Out-of-scope (explicitly cut)

- Chapters 2–6, Arena, Abyss/Tower, Drift Events, Circle/guild, friends Support borrow  
- Full 12-agent roster, Bond Matrix UI (data stub only for Mira↔Ivy), Evolve T2/T3, Awakening  
- Live authoritative server, IAP, Heartline Pass, Skin Atelier, Chroma Keys  
- NSFW scenes, Adult toggle ON path, Aff 4–10 content, Intimate Scene action  
- Live2D production polish, full VO, localization packs beyond EN strings table  
- PvP, multiplayer, cloud save, account auth  
- Furniture crafting / Decor shop depth (1–2 static props max)

---

## 3. Player Journey (15–25 minutes)

| Min | Beat | Player does | Systems touched |
|-----|------|-------------|-----------------|
| **0:00–1:00** | Boot / age gate stub | Confirm 18+; title → New Game | AgeGate, Save |
| **1:00–3:00** | Prologue cinematic + Mira meet | Tap through dialogue; receive Mira + lease of Tower | Hub intro, Dialogue |
| **3:00–5:00** | Combat tutorial stage `1-1` | Match gems; fire Mira Burst once; clear | Sync Match coach |
| **5:00–7:00** | Recruit Ivy + Juniper (story grant) | Squad edit: Front Ivy+Juniper, Mid Mira | Team Builder |
| **7:00–12:00** | Stages `1-2` … `1-6` (with tips) | Spend Stamina; learn lanes, colors, cascades | Stages, Stamina |
| **12:00–14:00** | Wren joins + Room unlock | Visit Mira’s Room; Talk → Aff1→2 path start | Room, Heart Energy |
| **14:00–18:00** | Stages `1-7` … `1-9` + Elite teach | Role tags; Assist Aim optional | Combat depth |
| **18:00–20:00** | Gift + optional Date (if Aff3) | Gift T1 tea; see Aff XP bar | Room Aff 2–3 |
| **20:00–22:00** | Boss `1-10` Chapter clear | Full squad Burst window | Boss telegraph |
| **22:00–24:00** | Beginner Banner soft intro | Watch rates UI; **optional** free 10-pull grant (1×) | Gacha stub |
| **24:00–25:00** | Job assign + logout tease | Café Shift; morning Aff tease copy | Jobs, retention hook |

**Target session length:** **18–22 min** median for designers; **≤25 min** hard cap for playtest script. Skip buttons on all tips after first completion.

---

## 4. Content List

### Agents (minimum 4)

| Agent | Age | Rarity | Role | Unlock | Slice notes |
|-------|-----|--------|------|--------|-------------|
| **Mira Solenne** | 24 | SSR | Synergist | Prologue grant | Tutorial anchor; Room focus; Verdant+Prism |
| **Ivy Cross** | 23 | R | Striker | After `1-1` | Crimson DPS; Front |
| **Juniper Tate** | 21 | R | Guardian | After `1-1` | Azure tank; Front; adult 21+ never child-coded |
| **Wren Halcyon** | 26 | SR | Support | After `1-5` story | Verdant heal; Back; gift craft teaser |

Optional NPC-only cameos (no kit): Ora Finch radio chatter, Helene one-liner in mail — **not playable**.

### Chapter 1 stages (10 + boss = **11 fights**)

| ID | Name | Type | Stamina | Enemy tags | Teaching goal |
|----|------|------|---------|------------|---------------|
| `1-1` | Lease Breach — Lobby | Story | 0 (tutorial) | Swarm / Soft | Match-3, Sync Gauge, Mira Burst |
| `1-2` | Rain Plaza Static | Story | 6 | Swarm | Cascades + combo meter |
| `1-3` | Metro Flood Gate | Story | 6 | Armored-light | Lane assign matter (Front take hits) |
| `1-4` | Signage Canyon | Story | 6 | Ranged Back | Target Back threats; Ivy Burst |
| `1-5` | Clinic Shuttle Dock | Story | 6 | Debuff | Wren join; heal Basic/Burst |
| `1-6` | Quay Bridge Mid | Story | 6 | Mixed | Full 4-squad; color attunement tip |
| `1-7` | Corp Kiosk Ambush | Elite-lite | 10 | Armor | Break via sustained Burst (no Breaker yet — DPS race teach) |
| `1-8` | Rooftop Garden Sparks | Story | 6 | Swarm+ | Prism wild gem intro |
| `1-9` | Basement Training Echo | Story | 6 | Mirror | Enemy Burst telegraph; Assist Aim |
| `1-10` | **Boss: Lease Reclaimer** | Boss | 15 | Boss / Multi-phase | Phase 2 lane shift; all systems |
| `1-B` | (optional grind) Lobby Sweep | Grind stub | 6 | Swarm | Post-clear Stamina sink (1 node) |

Dialogue-only nodes between stages: ~6 short scenes (see FTUE script doc).

### Room Aff content (SFW Aff 1–3 only)

| Aff | Unlock | Slice delivery |
|-----|--------|----------------|
| 1 Acquaintance | Base Room + Talk | Auto on Room unlock |
| 2 Friendly | Gift + VO set 2 | Talk/Gift XP |
| 3 Close | Date (1 outing script) | Date action; Bond Matrix *slot opens* (UI teaser, no duo stage) |

**No Aff4+.** Footnote in settings help: *“Affection ranks 8+ may unlock optional adult scenes in future builds when Adult Content is enabled — not available in this prototype.”*

### Other content

- Beginner Banner: pool = Mira (already owned → frags), Ivy, Juniper, Wren, + 4 phantom N fillers for reveal juice  
- Daily missions ×3: Clear 2 stages · Talk once · Spend 20 Stamina  
- Mail ×2: Welcome Coins · Wren’s herbal sachet ×3  

---

## 5. Systems — Implement vs Stub

| System | Implement (playable) | Stub (UI/data only or hardcoded) |
|--------|----------------------|----------------------------------|
| Sync Match board + Field | **Yes** | — |
| Agent skills Mira/Ivy/Juniper/Wren | **Yes** (kit numbers from roster kits, simplified VFX) | Bond Skill: Mira↔Ivy only; others “Coming soon” |
| Hub navigation | **Yes** (Lobby, Suite, Room, Stages, Banner, Jobs, Mail, Settings) | Rooftop/Basement greybox doors |
| Stamina / Heart Energy regen | **Yes** (local clock) | VIP cap bonuses |
| Room Talk/Gift/Date/Sleepover | **Yes** Aff1–3 | Intimate Scene button hidden |
| Gacha Beginner | **Yes** local RNG + pity display numbers | Live auth, IAP, medal spark |
| Jobs Café Shift | **Yes** 1 slot | Other jobs locked |
| Dialogue runner | **Yes** | Lip-sync / Live2D full |
| Team Builder + lane | **Yes** | Support borrow slot greyed |
| Save / Load / Reset | **Yes** | Cloud |
| Age gate + SFW flag | **Yes** (gate + flag architecture) | Adult CDN packs |
| Analytics events | **Yes** (debug log + optional Firebase stub) | Full funnel dashboards |
| Bond Matrix | — | **Stub** panel “Unlocks Aff3” |
| Promote / Level | **Yes** simplified (XP from stages, level cap 40 for R / 50 SR / 60 Mira soft) | Full promote mat economy |
| Arena / Events / Pass | — | **Out** |

---

## 6. Acceptance Criteria Checklist (binary)

### A. Boot & policy
- [ ] App launches to title in portrait without crash on target device/Editor  
- [ ] Age-gate requires explicit 18+ confirm before New Game  
- [ ] SFW Mode is ON and Adult Content controls are hidden or disabled  
- [ ] No NSFW assets referenced in Addressables / Resources for this build  

### B. FTUE / journey
- [ ] New Game grants Mira; Ivy+Juniper after `1-1`; Wren after `1-5`  
- [ ] Player can reach Ch.1 boss clear in ≤25 min with tip skips allowed after first see  
- [ ] All tip modals have Skip / Don’t show again  
- [ ] Chapter 1 clear sets `story.ch1.complete = true` and shows victory mail  

### C. Combat
- [ ] Board is 6×8; matches ≥3 orthogonal; cascades refill  
- [ ] Five gem colors + Prism wild behave per GDD  
- [ ] Sync Gauge fills from attuned colors; Burst fires only at 100% and empties + 1-turn lock  
- [ ] Lanes Front/Mid/Back apply documented modifiers  
- [ ] Stages `1-1`–`1-10` all clearable with starter four at default levels  
- [ ] Defeat returns to Hub without soft-lock; Stamina refund on tutorial `1-1` only  

### D. Room / Aff
- [ ] Talk costs 5 Heart Energy, +20 Aff XP  
- [ ] Gift costs 8, T1 grants 40 (preferred Mira tea 60)  
- [ ] Date unlocks at Aff3, costs 15, +100 XP, 1/day  
- [ ] Aff rank-ups 1→2 and 2→3 fire VO/text and unlock flags  
- [ ] No path to Aff4+ in data tables for slice  

### E. Economy / meta
- [ ] Stamina cap 120; regen 1/6 min (debug ×10 toggle for playtest)  
- [ ] Heart Energy cap 60; regen 1/8 min  
- [ ] Beginner Banner shows published rates; one free 10-pull grant works offline  
- [ ] Café Job completes and grants Luma Coins  
- [ ] Local save survives app kill; Reset clears progress  

### F. Art / UX
- [ ] Placeholders labeled `PH_` never ship without watermark in slice builds  
- [ ] Damage numbers and gem colors meet art bible hex tokens  
- [ ] No underage-coded art on Juniper or any agent  

### G. Engineering hygiene
- [ ] Content driven from ScriptableObjects / JSON (agents, stages, skills)  
- [ ] Analytics events fire for funnel list in tech doc (at least log sink)  
- [ ] Build pipeline produces Android APK or iOS archive from CI or documented local script  

**Slice PASS** = all checkboxes true. Any fail = not playtest-ready.

---

## 7. Art Placeholders Policy

| Rule | Detail |
|------|--------|
| **Prefix** | All temp art: `PH_{type}_{name}` e.g. `PH_bust_mira`, `PH_bg_lobby` |
| **Watermark** | Diagonal “HEARTLINE SLICE / PLACEHOLDER” at 20% opacity on PH_* textures in Development builds |
| **Silhouette first** | Prefer readable color-blocked silhouettes over random stock anime faces |
| **Juniper** | Adult proportions only (art bible 1:6.5–1:7.5); reject any PH that reads underage |
| **Swap path** | Addressable keys stable; final art drops replace PH without code change |
| **Banned** | Scraped copyrighted character art; real-person faces; NSFW refs in slice repo |
| **Minimum keyed** | If budget allows: Mira T0 bust + Hub lobby BG + gem atlas — else all PH OK |

---

## 8. Success Metrics — Internal Playtest

| Metric | Pass bar | Measure |
|--------|----------|---------|
| **Session complete Ch.1** | ≥80% of playtesters | Funnel `ch1_boss_clear` |
| **Median clear time** | 15–25 min | Timestamp login→clear |
| **Tutorial drop before `1-2`** | ≤15% | Drop-off |
| **Room visited same session** | ≥70% | `room_enter` |
| **Aff ≥2 on Mira same session** | ≥50% | Aff rank |
| **Combat “confused” self-report** | ≤25% say 4–5 on 1–5 scale | Survey |
| **Would play again tomorrow** | ≥60% Yes | Survey |
| **Critical bugs** | 0 soft-locks / data loss | QA log |
| **Brand safety** | 0 underage / NSFW incidents | Review |

---

## 9. Estimated Engineering Weeks (team 2–4)

Assumes: 1 gameplay eng, 1 client/systems eng, optional 1 tech art/UI, optional 1 designer-scripter (content). Unity 6 LTS greenfield.

| Workstream | Weeks (calendar) | Notes |
|------------|------------------|-------|
| Repo bootstrap + Hub shell + navigation | 1.0 | Sprint 0 |
| Sync Match board + Field + 4 kits | 2.5–3.0 | Highest risk |
| Stages Ch.1 data + pacing + boss | 1.0 | Parallel after board MVP |
| Room Aff 1–3 + dialogue runner | 1.0 | |
| Economy local + Jobs + Banner stub | 0.75 | |
| FTUE wire + analytics + save | 0.75 | |
| Polish, device builds, playtest fixes | 1.0 | |
| **Total** | **≈ 6–8 weeks** | 3 eng → ~6–7; 2 eng → ~8; +1 UI speeds polish |

**Call:** Schedule **7 weeks** to vertical-slice playtest with a **2-week** content/art PH buffer overlapping.

---

## 10. Definition of Done (slice)

Vertical slice is done when acceptance checklist §6 is 100% PASS, internal playtest hits §8 bars (or documented waivers signed by design lead), and `BUILD_READY.md` build-order steps 1–4 are complete.

*— End of Vertical Slice Plan —*
