# Heartline Protocol — Art Kickoff

**Document type:** Art production kickoff (systems freeze → commission)  
**Version:** 1.0 · **Date:** 2026-09-19 (America/Denver)  
**Slice:** `0.5.0-slice-freeze` · systems from PRs **#1–#5** merged on `main`  
**Sources:** `Docs/heartline-protocol-art-bible.md` · `Docs/heartline-protocol-launch-roster-kits.md` · README

**Binding content policy:** **SFW-only for now.** All characters are adults **18+**. **Juniper Tate is 21** (youngest roster member — clearly adult; never child-coded). No NSFW assets, folders, or commission briefs in this kickoff.

---

## 1. Systems slice frozen enough for art

Code/systems for the vertical slice are **frozen enough to commission T0 art** without blocking on Live2D, Aff4+, or live server work.

| System (PRs 1–5) | Status for art |
|------------------|----------------|
| Runtime UI bootstrap + portrait 1080×1920 | **Frozen** — chrome pack targets this frame |
| Sync Match board / gems / Sync Gauge / lanes | **Frozen** — gem VFX + SD combat sprites |
| Hub / Stage / Battle / Results | **Frozen** — UI chrome + role chips |
| Room Aff 1–3 SFW (Talk / Gift / Date / Sleepover) | **Frozen** — Room BGs + bust/fullbody |
| Economy / gacha / jobs / growth / dailies | **Frozen** — currency icons in chrome pack |
| Arena / Tower / Codex stubs | **Frozen** — Codex uses busts; no unique art yet |
| Live2D / VO / Aff4+ / NSFW / IAP | **Out of this kickoff** |

Art may proceed against placeholders documented in `Docs/PLACEHOLDER_ART_SLOTS.md`. Drop files into the paths under `Assets/_Project/Art/` — plug-and-play.

---

## 2. Commission order (priority)

| Priority | Package | Why first |
|----------|---------|-----------|
| **P0** | **Mira Solenne** T0 bust + fullbody + SD | Starter, FTUE face, Room hero |
| **P0** | **UI chrome pack** (nav, frames, currency, gem chips) | Unblocks playable build polish |
| **P1** | **Ivy Cross** + **Juniper Tate** T0 sets | Unlock after 1-1; Front lane pair |
| **P1** | **2 Room BGs** (Player Suite evening · Agent Room loft) | Room loop readability |
| **P2** | **Wren Halcyon** T0 set | Unlock after 1-5; healer / gift economy face |
| **P2** | **Gem match VFX note** (direction only; can be eng placeholders) | Combat readability |

Full one-page briefs: `Docs/ART_COMMISSION_BRIEFS_SLICE.md`.

---

## 3. Resolutions (masters)

| Asset | Resolution | Notes |
|-------|------------|-------|
| Bust portrait | **2048×2048** PNG, transparent | UI circle-ready; safe crop center |
| Fullbody T0 | **2048×4096** PNG, transparent | Headroom + feet; portrait crop friendly |
| SD combat | **~512 px tall** PNG, transparent | Chibi or trimmed; Burst pose optional sheet later |
| Room BG | **2160×3840** (9:16) master | Downres 1080×1920 for mobile |
| UI chrome @1x | Design **1080×1920**; export @2x/@3x | Safe: top 88 / bottom 120 / side 24 |
| Skill / gift icons | 256² / 128² | Optional in this kickoff — not required for P0 |
| Gem VFX | Engine sprites / atlas TBD | Color-true to gem hexes (see art bible) |

---

## 4. Naming conventions

- **Agent folders:** `snake_case` full name — `mira_solenne`, `ivy_cross`, `juniper_tate`, `wren_halcyon`
- **Tier prefix:** `t0_` for Base obtain look (this kickoff only; T1–T3 later)
- **Files:** `{tier}_{kind}.png` — `t0_bust.png`, `t0_fullbody.png`, `t0_sd.png`
- **Rooms:** `room_{slug}.png` — e.g. `room_player_suite_evening.png`
- **UI:** `ui_{component}.png` under `Art/UI/Chrome/`
- **Gems:** `gem_{color}_match.png` / `_cascade.png` under `Art/VFX/Gems/`
- **No spaces**; lowercase; no NSFW path segments in this kickoff

---

## 5. Folder layout (`Assets/_Project/Art/`)

```
Assets/_Project/Art/
├── Characters/
│   ├── mira_solenne/     # t0_bust · t0_fullbody · t0_sd
│   ├── ivy_cross/
│   ├── juniper_tate/
│   └── wren_halcyon/
├── Environment/
│   └── Rooms/            # room_*.png
├── UI/
│   └── Chrome/           # ui_*.png / atlas packs
└── VFX/
    └── Gems/             # gem_* match FX
```

Empty folders are kept with `.gitkeep`. Exact filenames: `Docs/PLACEHOLDER_ART_SLOTS.md`.

---

## 6. Do’s / Don’ts (from art bible)

| Do | Don't |
|----|-------|
| Adult proportions (18+), consistent head:body, readable silhouettes at SD **and** fullbody | Childlike faces, school-uniform tropes, or any underage-coded styling |
| Gem colors as UI truth (board ↔ Burst FX ↔ role chips) | Rainbow noise that breaks colorblind / combat readability |
| Warm hub light vs cold Sector Breach storm light as emotional contrast | Flat gray sci-fi or pure pastel slice-of-life with no noir bite |
| Adult anime-influenced ~1:6.5–1:7.5 head:body; mature bone structure | Chibi heads on fullbody heroes (chibi = SD only) |
| Juniper clearly **21+** adult guard — earnest, not “petite child” coding | Kid styling, child props, or “kid’s meal” gag visuals |
| Faction-readable outfits at T0 with minimal VFX | Over-armoring T0 so silhouettes muddy at SD size |
| SFW Room / bust / fullbody only in this commission wave | NSFW, lingerie “default,” or Aff8+ intimate CGs |

**Keywords:** neon-noir · empathic sync · coastal megacity · apartment warmth · gem clarity

**Palette anchors:** Crimson `#E23B4A` · Azure `#2F7BFF` · Verdant `#2DBE6A` · Amber `#F0A12A` · Violet `#8B5CF6` · Prism `#F5F7FF` · Neon Magenta `#FF2D8A` · Sync Cyan `#00D4FF` · Quay Indigo `#12182B` · Atelier Gold `#E8C87A`

---

## 7. Content / delivery rules

- **SFW-only for now** — storefront, in-app slice, and all commissioned files.
- **All characters 18+**; ages on briefs are binding (Mira 24 · Ivy 23 · Juniper **21** · Wren 26).
- Deliver PNG masters + layered source (PSD/CLIP) if available; name layers for hair / face / outfit / prop.
- Color profile: sRGB. No watermark on finals.
- Reject / revise triggers: underage coding, wrong gem accent, silhouette collapse at SD, non-circle-safe bust crop.

---

## Related docs

- `Docs/ART_COMMISSION_BRIEFS_SLICE.md` — one-page briefs  
- `Docs/PLACEHOLDER_ART_SLOTS.md` — exact drop paths  
- `Docs/COMPLETION_CHECKLIST.md` — slice → MVP → soft launch → live  
- `Docs/heartline-protocol-art-bible.md` — full art bible  

---

*Heartline Protocol art kickoff · 0.5.0-slice-freeze · 2026-09-19*
