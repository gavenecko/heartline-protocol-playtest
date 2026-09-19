# HEARTLINE PROTOCOL — BUILD READY

**Master index & engineering gate**  
**Version:** 1.0 · **Date:** 2026-09-19 (America/Denver)  
**Status:** Pre-build package complete — ready for Sprint 0  

**Content policy (binding):** All romanceable / collectible characters are adults **18+**. Vertical slice is **SFW-only** (Room Aff 1–3). Optional Aff8+ adult packs are future / live-only and never required for story.

---

## 1. Document Index

| Doc | Path | Role |
|-----|------|------|
| **GDD** | `/workspace/waifu-gacha-gdd.md` | High concept, loops, combat, gacha, Aff, economy pillars, roster seed |
| **Art Bible + UI Brief** | `/workspace/heartline-protocol-art-bible.md` | Visual thesis, palette, typography, character standards, UI mocks |
| **Launch Roster Kits** | `/workspace/heartline-protocol-launch-roster-kits.md` | Combat/romance kits for 12 agents; team archetypes |
| **Vertical Slice Plan** | `/workspace/heartline-protocol-vertical-slice.md` | First playable scope, journey, AC, metrics, eng weeks |
| **Tech Architecture** | `/workspace/heartline-protocol-tech-architecture.md` | Unity 6 LTS, modules, data, save, flags, CI, day-1 bootstrap |
| **Chapter 1 + FTUE Script** | `/workspace/heartline-protocol-chapter1-ftue.md` | Minute-by-minute tutorial, dialogue, stages, Room, banner soft intro |
| **Economy 7-Day CSV** | `/workspace/heartline-protocol-economy-7day.csv` | F2P Day0–7 sources/sinks model |
| **Economy Notes** | `/workspace/heartline-protocol-economy-notes.md` | Assumptions, Aff thresholds, D7 targets |
| **Sprint Backlog** | `/workspace/heartline-protocol-sprint-zero-backlog.md` | HP-001… tickets for Sprint 0–2 |
| **This file** | `/workspace/BUILD_READY.md` | Index, build order, DoR, locked decisions |

---

## 2. Locked Decisions (no waffle)

| Topic | Decision |
|-------|----------|
| **Title** | **Heartline Protocol** (primary) |
| **Engine** | **Unity 6 LTS**, mobile-first **portrait** 1080×1920 |
| **Web** | Pitch-only optional; **not** a second production client |
| **Slice agents** | **Mira + Ivy + Juniper + Wren** only |
| **Slice story** | Prologue + **Chapter 1** (stages `1-1`…`1-10` + optional `1-B`) |
| **Room** | Aff **1–3 SFW** only; NSFW Aff8+ footnote only |
| **Slice networking** | **Offline** local economy + local gacha RNG |
| **Live networking** | Authoritative gacha/currency; CDN adult packs later |
| **Backend language (live)** | **C# ASP.NET** preferred for shared models |
| **UI framework (slice)** | **uGUI** + TextMeshPro |
| **Aff XP thresholds** | Aff2 @ **100**, Aff3 @ **300** cumulative |
| **Eng schedule** | **~7 weeks** to slice playtest (team 2–4) |
| **Beginner pulls in economy model** | Free 10 Day0 + optional 1600-crystal 10-pull Day2 |
| **Juniper** | Age **21**, adult proportions only — never child-coded |

---

## 3. Recommended Build Order

1. **Read** GDD → Art Bible → Roster Kits → Vertical Slice → Tech Arch.  
2. **Sprint 0:** Repo bootstrap checklist (tech arch §12); HP-001–005, HP-010, HP-030, HP-032, start HP-060.  
3. **Combat vertical:** HP-020→021→022 with Mira-only sandbox stage before full Ch.1.  
4. **Hub + Room:** HP-011, HP-040–042 in parallel with combat polish.  
5. **Content drip:** HP-050 scripts → HP-024 stages → HP-051 FTUE sequencer.  
6. **Meta soft:** HP-012, HP-013, HP-052 banner.  
7. **Playtest gate:** HP-071–072 against vertical slice acceptance checklist.  
8. **Only then** expand toward MVP (more agents, Ch.2, live server).

```
Bootstrap → Board MVP → 4 Kits → Ch.1 Data → Room Aff1–3 → FTUE Wire → Banner/Jobs → Device Playtest
```

---

## 4. Definition of Ready for Engineering

Engineering may start Sprint 0 when **all** are true:

- [x] GDD, Art Bible, Roster Kits available in `/workspace`  
- [x] Vertical slice in/out scope written with binary AC  
- [x] Tech architecture + day-1 bootstrap checklist written  
- [x] Ch.1 / FTUE scripts written (SFW, 18+)  
- [x] 7-day economy CSV + notes with D7 targets  
- [x] Sprint 0–2 backlog ticketized (HP-IDs)  
- [x] Engine and slice scope **locked** (this file §2)  
- [x] Content policy reaffirmed (18+, no NSFW in slice)  

**Definition of Ready for a single ticket:** see backlog “Definition of Ready (per story).”

**Definition of Done for vertical slice:** Vertical Slice doc §6 acceptance 100% PASS + §8 playtest bars (or signed waivers).

---

## 5. Open Decisions — Resolved Here

| Was open | Resolution |
|----------|------------|
| Unity vs Unreal? | **Unity 6 LTS** |
| Slice offline vs fake server? | **Offline** |
| Aff XP numbers? | **100 / 300** for ranks 2 / 3 |
| Free healer timing? | **Wren after stage 1-5** |
| Banner in slice? | **Soft intro + local Beginner only** |
| Live2D in slice? | **Optional; static PH OK** |
| IAP / Auth in slice? | **No — later** |
| Adult assets in repo slice branch? | **Forbidden** (`buildIncludesAdultGroup = false`) |

*No material open questions remain for Sprint 0 kickoff.*

---

## 6. Team Kickoff One-Pager

**Fantasy to protect:** Sync Match competence by day, apartment warmth with Mira by evening.  
**First playable:** ~20 min Ch.1 clear + Room Aff progress.  
**First ticket:** **HP-001**.  
**First playtest metric that matters:** ≥80% Ch.1 complete without soft-lock.

---

## 7. Contact / Ownership (fill at kickoff)

| Role | Name |
|------|------|
| Design lead | _TBD_ |
| Eng lead | _TBD_ |
| Art lead | _TBD_ |
| Producer | _TBD_ |

---

*— Heartline Protocol build-ready package · 2026-09-19 —*
