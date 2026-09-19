# HEARTLINE PROTOCOL — Sprint Zero + First 2 Sprints Backlog

**Document type:** Ticket-ready engineering / content / art stories  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Build-ready  
**Scope:** Vertical slice only (Mira, Ivy, Juniper, Wren · Ch.1 · Room Aff1–3 SFW)  
**Engine:** Unity 6 LTS (locked)

**Estimate key:** S ≤1d · M 2–3d · L 4–5d (per person).  
**Priority:** P0 must for slice playtest · P1 should · P2 nice.

---

# Epic A — Project Bootstrap & Pipeline

### HP-001 · Unity 6 LTS project scaffold
- **Priority:** P0 · **Est:** M · **Deps:** none  
- **Story:** As an engineer, I can open a pinned Unity 6 LTS project with URP 2D, asmdefs, and folder tree from the tech architecture doc.  
- **AC:**
  - [ ] `ProjectVersion.txt` pinned; README lists exact Unity version  
  - [ ] Folders per tech arch §5 exist  
  - [ ] Assemblies: App, SyncMatch, Hub, Room, Shared compile empty  
  - [ ] Scenes 00_Boot–05_Gacha in Build Settings  
  - [ ] Empty Android build succeeds once  

### HP-002 · CI smoke + AgentAgeValidator
- **Priority:** P0 · **Est:** S · **Deps:** HP-001  
- **AC:**
  - [ ] EditMode test fails if any AgentDefinition.age < 18 or null  
  - [ ] PR script compiles player code  
  - [ ] Documented `Tools/build.sh` or GameCI stub  

### HP-003 · ContentFlags + Age Gate flow
- **Priority:** P0 · **Est:** M · **Deps:** HP-001  
- **AC:**
  - [ ] Boot → Age Gate → Title; deny exits  
  - [ ] Flags: sfwMode true, adultUnlocked false, buildIncludesAdultGroup false  
  - [ ] Intimate/NSFW UI absent in slice  

### HP-004 · Local SaveModule v1
- **Priority:** P0 · **Est:** M · **Deps:** HP-001  
- **AC:**
  - [ ] Save/load wallet, story, agents, flags  
  - [ ] Survive app kill; Reset debug clears file  
  - [ ] Schema version field present  

### HP-005 · Analytics debug sink
- **Priority:** P1 · **Est:** S · **Deps:** HP-001  
- **AC:**
  - [ ] Events from tech arch §9 log to console + rolling file  
  - [ ] `ftue_step`, `stage_clear`, `room_action`, `ch1_complete` wired when features land  

---

# Epic B — Hub Shell & Navigation

### HP-010 · Hub scene navigation
- **Priority:** P0 · **Est:** M · **Deps:** HP-001, HP-003  
- **AC:**
  - [ ] Buttons: Sectors, Team, Room, Banner, Jobs, Mail, Settings  
  - [ ] Currency HUD: Stamina, HE, Coins, Crystals  
  - [ ] Greybox doors for Rooftop/Basement non-blocking  

### HP-011 · Team Builder + lanes
- **Priority:** P0 · **Est:** M · **Deps:** HP-010, HP-030  
- **AC:**
  - [ ] Equip 4 agents; max 2 per Front/Mid/Back  
  - [ ] Persist squad in save  
  - [ ] Support slot visible but locked  

### HP-012 · Mail + Daily Missions stub
- **Priority:** P1 · **Est:** S · **Deps:** HP-010, HP-004  
- **AC:**
  - [ ] Scripted welcome mails claimable  
  - [ ] 3 daily missions update and grant rewards  

### HP-013 · Jobs Café Shift
- **Priority:** P1 · **Est:** S · **Deps:** HP-010, HP-004  
- **AC:**
  - [ ] Assign 1 agent; timer; claim Luma Coins  
  - [ ] Debug complete-now button in DEV builds  

---

# Epic C — Sync Match Combat

### HP-020 · Board simulation 6×8
- **Priority:** P0 · **Est:** L · **Deps:** HP-001  
- **AC:**
  - [ ] 5 colors + Prism; match ≥3 orthogonal; cascades  
  - [ ] Combo multiplier 1.0→1.2→1.5→2.0 at 8+  
  - [ ] Input: swap adjacent; 5 swaps or 12s timer per player phase  

### HP-021 · Field lanes + turn loop
- **Priority:** P0 · **Est:** L · **Deps:** HP-020  
- **AC:**
  - [ ] Player match → Burst window → Enemy phase → ticks  
  - [ ] Lane modifiers per GDD applied to damage  
  - [ ] Enemy waves from StageDefinition  

### HP-022 · Sync Gauge + Basic/Burst/Passive
- **Priority:** P0 · **Est:** L · **Deps:** HP-021, HP-030  
- **AC:**
  - [ ] Attunement fill rates; Basic at ≥20%; Burst at 100% + empty + 1 lock  
  - [ ] Four kits numerically match roster kits (±tuning 10%)  
  - [ ] Mira↔Ivy Bond Skill once/battle  

### HP-023 · Combat coach / Assist Aim
- **Priority:** P0 · **Est:** M · **Deps:** HP-020  
- **AC:**
  - [ ] `1-1` forces first Mira Burst  
  - [ ] Assist Aim highlights legal match after setting/fail  
  - [ ] Tips skippable per FTUE doc  

### HP-024 · Stage runner Ch.1 data
- **Priority:** P0 · **Est:** M · **Deps:** HP-021, HP-040  
- **AC:**
  - [ ] Stages 1-1…1-10 + optional 1-B load from data  
  - [ ] Stamina deduct; rewards grant; stars 1–3  
  - [ ] Boss multi-phase Lease Reclaimer  

### HP-025 · Combat VFX placeholders
- **Priority:** P1 · **Est:** M · **Deps:** HP-022  
- **AC:**
  - [ ] Gem pop, Burst flash, damage numbers use art bible colors  
  - [ ] PH watermark on temp VFX textures  

---

# Epic D — Agents & Content Data

### HP-030 · Agent SO + JSON for 4
- **Priority:** P0 · **Est:** M · **Deps:** HP-001  
- **AC:**
  - [ ] Mira, Ivy, Juniper, Wren definitions complete (age, skills, gems, gifts)  
  - [ ] Validator green  
  - [ ] Unlock flags: prologue / post-1-1 / post-1-5  

### HP-031 · Leveling simplified
- **Priority:** P1 · **Est:** S · **Deps:** HP-030, HP-004  
- **AC:**
  - [ ] XP from stages; caps Lv40 R / 50 SR / 60 Mira soft  
  - [ ] HUD shows levels  

### HP-032 · Economy constants table
- **Priority:** P0 · **Est:** S · **Deps:** HP-001  
- **AC:**
  - [ ] Stamina/HE caps & regen; Room costs; stage costs in JSON/SO  
  - [ ] Matches economy notes  

---

# Epic E — Room & Affection

### HP-040 · Dialogue runner
- **Priority:** P0 · **Est:** M · **Deps:** HP-001  
- **AC:**
  - [ ] Speaker, line, advance, optional VO hook  
  - [ ] Loads sequences from FTUE script IDs  
  - [ ] `{Handler}` token  

### HP-041 · Room Aff 1–3 actions
- **Priority:** P0 · **Est:** L · **Deps:** HP-040, HP-032, HP-010  
- **AC:**
  - [ ] Talk/Gift/Date/Sleepover per GDD costs & XP  
  - [ ] Preferred gift ×1.5 for Mira tea  
  - [ ] Rank-ups to 2 and 3 fire rewards; **no Aff4+** in tables  
  - [ ] Date locked until Aff3  

### HP-042 · Room tutorial wire
- **Priority:** P0 · **Est:** S · **Deps:** HP-041, HP-040  
- **AC:**
  - [ ] Forced highlight after 1-5 if needed  
  - [ ] FTUE HE topup once  
  - [ ] Script `room_tutorial` plays  

---

# Epic F — Story / FTUE Wire

### HP-050 · Prologue + Ch.1 dialogue content
- **Priority:** P0 · **Est:** M · **Deps:** HP-040  
- **Owner:** Design/Content  
- **AC:**
  - [ ] All scripts from `heartline-protocol-chapter1-ftue.md` in data  
  - [ ] Adults-only review signed  
  - [ ] No NSFW lines  

### HP-051 · FTUE sequencer Day0
- **Priority:** P0 · **Est:** M · **Deps:** HP-010, HP-024, HP-042, HP-050  
- **AC:**
  - [ ] Minute-by-minute beats unlock in order  
  - [ ] Grants: Mira, Ivy+Juniper, Wren at correct points  
  - [ ] `ch1_complete` on boss clear  

### HP-052 · Beginner Banner soft intro + local RNG
- **Priority:** P1 · **Est:** M · **Deps:** HP-010, HP-032  
- **AC:**
  - [ ] Rates UI; free 10-pull; Not Now no penalty  
  - [ ] Pity numbers displayed; local resolve logged  
  - [ ] No IAP  

---

# Epic G — Art Placeholders

### HP-060 · PH kit pipeline
- **Priority:** P0 · **Est:** M · **Deps:** HP-001  
- **Owner:** Art / Tech Art  
- **AC:**
  - [ ] PH_ busts for 4 agents; lobby BG; gem atlas; Room BG  
  - [ ] Watermark in Development builds  
  - [ ] Juniper adult silhouette sign-off  
  - [ ] Addressable keys stable for swap  

### HP-061 · UI chrome kit (portrait)
- **Priority:** P1 · **Est:** M · **Deps:** HP-010  
- **AC:**
  - [ ] HUD, buttons, rarity frames N–SSR using art bible hex  
  - [ ] TMP fonts Inter + Orbitron/Exo2  

### HP-062 · Mira keyed bust (optional stretch)
- **Priority:** P2 · **Est:** L · **Deps:** HP-060  
- **AC:**
  - [ ] Non-PH Mira T0 bust if schedule allows; else waive  

---

# Epic H — Polish, QA, Playtest

### HP-070 · Acceptance checklist automation aids
- **Priority:** P1 · **Est:** S · **Deps:** slice features  
- **AC:**
  - [ ] Debug menu: jump stage, set Aff, refill energies  
  - [ ] Checklist markdown ticked in playtest report  

### HP-071 · Device playtest build
- **Priority:** P0 · **Est:** M · **Deps:** all P0  
- **AC:**
  - [ ] Android APK (and/or iOS) for internal testers  
  - [ ] Vertical slice acceptance §6 binary pass  
  - [ ] Survey + metrics from vertical slice §8 collected  

### HP-072 · Bug bash buffer
- **Priority:** P0 · **Est:** M · **Deps:** HP-071  
- **AC:**
  - [ ] Zero soft-locks / save corruption  
  - [ ] Critical combat bugs fixed  

---

## Sprint Plan (suggested)

| Sprint | Days | Focus | Exit |
|--------|------|-------|------|
| **Sprint 0** | 5 | HP-001–005, HP-010, HP-030, HP-032, HP-060 start | Boot→Hub→Save; 4 agent data; PH pipeline |
| **Sprint 1** | 10 | HP-020–024, HP-011, HP-040–042, HP-050 | Playable board; Room Aff; dialogue in |
| **Sprint 2** | 10 | HP-051–052, HP-012–013, HP-025, HP-031, HP-061, HP-070–072 | Full Ch.1 FTUE; banner; playtest PASS |

**Stretch into Sprint 2 only if P0 green:** HP-062.

---

## Dependency Graph (critical path)

```
HP-001 → HP-003 → HP-010 → HP-051
HP-001 → HP-020 → HP-021 → HP-022 → HP-024 → HP-051
HP-001 → HP-030 → HP-022
HP-001 → HP-040 → HP-041 → HP-042 → HP-051
HP-051 → HP-071 → HP-072
```

---

## Definition of Ready (per story)

- Linked epic + ID  
- Priority + estimate  
- Clear AC checkboxes  
- Deps listed  
- Content stories reference FTUE/GDD section  
- Art stories reference art bible tokens  

*— End of Sprint Backlog —*
