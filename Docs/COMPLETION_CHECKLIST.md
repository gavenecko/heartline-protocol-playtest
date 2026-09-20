# Heartline Protocol — Completion Checklist

**Version:** 1.0 · **Date:** 2026-09-19 (America/Denver)  
**Code baseline:** `main` after PR **#5** merged · art kickoff → **0.5.0-slice-freeze**  
**Policy:** SFW-only slice · all agents **18+** · Juniper **21**

Legend: **[x]** done · **[ ]** remaining · **[~]** stub / partial

---

## Phase 0 — Vertical Slice (systems)

Marked from README “What’s implemented vs stubbed” + PRs **#1–#5**.

### Done (code systems)

- [x] Runtime UI bootstrap (`SliceRuntimeBootstrap`) — PR #1
- [x] Board / Sync Gauge / lanes / Ch.1 stages — PR #1
- [x] Room Aff 1–3 SFW (Talk / Gift / Date / Sleepover) + Buy Tea sink — PR #1 / #3
- [x] Versioned local save (v5) + STA/HE timestamp regen — PRs #1–#5
- [x] Age gate + SFW flags on Title (Adult blocked) — PR #5
- [x] Beginner pity gacha + ticket/crystal spend + fragments — PR #3
- [x] Jobs ×3 + claim-all + preferred-agent bonus — PR #3
- [x] Stage select ★ / STA / unlock reasons — PR #2 / #3
- [x] FTUE dialogue sequencer + battle polish + team + bond matrix — PR #2
- [x] Agent XP / level caps by ★ + Promote — PR #4
- [x] Post-battle results (XP bars / LC / ★ / frags) — PR #4
- [x] Daily missions ×3 + claim (UTC reset) — PR #4
- [x] Arena async stub + Arena Marks — PR #5
- [x] Tower / Abyss 10/50 + checkpoints — PR #5
- [x] Agent Codex launch 12 teasers — PR #5
- [x] Sim tests green (board, pity, jobs, XP, arena, tower, ages×12, Aff, …)

### Remaining (slice → art / polish)

- [x] **T0 art drops** for Mira / Ivy / Juniper / Wren (bust + fullbody + SD) — AI drafts wired (PR #7–#10 + runtime)
- [x] **UI chrome pack** wired into Hub currency / nav / rarity frames / gem chips / buttons (feat/wire-art-ui-slots)
- [x] **Room BGs** wired — suite default + action maps (Talk/Gift/Date/Sleepover) + extras under Resources
- [~] **Gem match VFX** — board uses ui_gem_* chips (still no cascade/pop VFX)
- [ ] Live2D / Spine hub + Room (face / torso / hair) — stub
- [ ] VO lines (even stub bark set)
- [ ] Addressables / art import pipeline hooked to slots
- [ ] Editor Canvas / prefab polish (optional; bootstrap remains valid)

### Explicitly out of slice

- [ ] Live server / accounts / IAP
- [ ] Aff 4+ / NSFW packs / Adult path unlock
- [ ] Weekly Arena shop
- [ ] Full Tower 50 floors + live ranking
- [ ] Remaining 8 agents playable (Codex teasers only)

---

## Phase 1 — MVP

Playable product with art + core loops closed; still may be soft-offline.

- [ ] All **12 launch agents** obtain path (gacha + story grants) with T0 art
- [ ] Chapters **1–2** (or Ch.1 + prologue polish) with final-ish BGs
- [ ] Live2D or equivalent for **Mira Room** (min); others bust-fallback OK
- [ ] Gem VFX + Burst telegraphs meeting art bible readability
- [ ] Economy tuned vs `economy-7day` (F2P D7 targets)
- [ ] Beginner banner + standard banner; pity UX final copy
- [ ] Daily / weekly mission set (not stub-only)
- [ ] Settings: SFW Mode, age gate, accessibility (colorblind gem shapes)
- [ ] Soft tutorial metrics + analytics hooks (events fire locally)
- [ ] Crash-safe save + export/import debug
- [ ] Store SFW screenshots set (no mature)

---

## Phase 2 — Soft launch

Limited regions / TestFlight / closed Android; live backend light.

- [ ] Accounts + cloud save sync
- [ ] Receipted IAP (crystals / monthly) + restore
- [ ] Live pity / inventory authoritative server
- [ ] Push / mail ops tools
- [ ] Arena weekly season + Marks shop (first catalog)
- [ ] Tower floors **11–50** gated content
- [ ] Live-ops calendar stub (1 event banner)
- [ ] CDN for art packs; remote config for rates
- [ ] Crashlytics / perf budget on mid-tier devices
- [ ] Content freeze checklist + rollback plan
- [ ] Age-gate / regional compliance review (SFW default)

---

## Phase 3 — Live

- [ ] Global storefront (SFW) + regional Mature where allowed
- [ ] Aff 8 / 10 gates + optional NSFW CDN packs (pipeline only; opt-in)
- [ ] Full Bond Festival / limited UR cadence (Aveline debut pattern)
- [ ] Skins + T1–T3 evolve art for launch roster
- [ ] Live Arena ranking + anti-cheat
- [ ] Customer support tooling + refund SOP
- [ ] Localization pass (EN + 1 CJK min)
- [ ] Long-term economy / power creep review board

---

## Art kickoff gate (this PR)

- [x] `Docs/ART_KICKOFF.md` — freeze declaration + order + layout
- [x] `Docs/ART_COMMISSION_BRIEFS_SLICE.md` — Mira / Ivy / Juniper / Wren + UI + Rooms + gem VFX
- [x] `Docs/PLACEHOLDER_ART_SLOTS.md` — exact filenames
- [x] `Assets/_Project/Art/...` folder `.gitkeep` structure
- [ ] Artist kickoff meeting / commission PO issued *(human)*
- [ ] First Mira bust drop accepted into slots *(human)*

---

*Checklist tracks slice freeze → live. Update when PRs land.*  
*2026-09-20: UI chrome + Room action BGs + Codex fullbody/gacha reveal wired (feat/wire-art-ui-slots).*
