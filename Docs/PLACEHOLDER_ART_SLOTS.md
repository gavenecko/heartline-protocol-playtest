# Placeholder Art Slots — Exact Filenames

**Version:** 1.0 · **Date:** 2026-09-19 (America/Denver)  
**Root:** `Assets/_Project/Art/`  
**Purpose:** Drop finals using these paths/names for plug-and-play. Folders exist with `.gitkeep`.

All paths relative to repo root. **SFW-only.** Characters **18+** (Juniper **21**).

---

## Characters (slice playable 4)

### Mira Solenne → `Assets/_Project/Art/Characters/mira_solenne/`

| File | Size | Use |
|------|------|-----|
| `t0_bust.png` | **2048×2048** transparent | Hub / Team / Codex / Room / dialogue |
| `t0_fullbody.png` | **2048×4096** transparent | Agent detail / gacha reveal / key art crop |
| `t0_sd.png` | **~512 px tall** transparent | Sync Match lane / Burst flash |

### Ivy Cross → `Assets/_Project/Art/Characters/ivy_cross/`

| File | Size |
|------|------|
| `t0_bust.png` | 2048×2048 |
| `t0_fullbody.png` | 2048×4096 |
| `t0_sd.png` | ~512 tall |

### Juniper Tate → `Assets/_Project/Art/Characters/juniper_tate/`

| File | Size |
|------|------|
| `t0_bust.png` | 2048×2048 |
| `t0_fullbody.png` | 2048×4096 |
| `t0_sd.png` | ~512 tall |

### Wren Halcyon → `Assets/_Project/Art/Characters/wren_halcyon/`

| File | Size |
|------|------|
| `t0_bust.png` | 2048×2048 |
| `t0_fullbody.png` | 2048×4096 |
| `t0_sd.png` | ~512 tall |

**Expected full paths (12):**

```
Assets/_Project/Art/Characters/mira_solenne/t0_bust.png
Assets/_Project/Art/Characters/mira_solenne/t0_fullbody.png
Assets/_Project/Art/Characters/mira_solenne/t0_sd.png
Assets/_Project/Art/Characters/ivy_cross/t0_bust.png
Assets/_Project/Art/Characters/ivy_cross/t0_fullbody.png
Assets/_Project/Art/Characters/ivy_cross/t0_sd.png
Assets/_Project/Art/Characters/juniper_tate/t0_bust.png
Assets/_Project/Art/Characters/juniper_tate/t0_fullbody.png
Assets/_Project/Art/Characters/juniper_tate/t0_sd.png
Assets/_Project/Art/Characters/wren_halcyon/t0_bust.png
Assets/_Project/Art/Characters/wren_halcyon/t0_fullbody.png
Assets/_Project/Art/Characters/wren_halcyon/t0_sd.png
```

---

## UI chrome → `Assets/_Project/Art/UI/Chrome/`

| File | Notes |
|------|-------|
| `ui_nav_bottom.png` | 5-tab bar / 9-slice |
| `ui_currency_sta.png` | 128² Stamina |
| `ui_currency_he.png` | 128² Heart Energy |
| `ui_currency_lc.png` | 128² Luma Coins |
| `ui_currency_pc.png` | 128² Pulse Crystals |
| `ui_currency_ticket.png` | 128² Echo Tickets |
| `ui_currency_marks.png` | 128² Arena Marks |
| `ui_frame_n.png` | N rarity |
| `ui_frame_r.png` | R |
| `ui_frame_sr.png` | SR |
| `ui_frame_ssr.png` | SSR |
| `ui_frame_ur.png` | UR |
| `ui_role_striker.png` | 128² |
| `ui_role_breaker.png` | 128² |
| `ui_role_guardian.png` | 128² |
| `ui_role_support.png` | 128² |
| `ui_role_controller.png` | 128² |
| `ui_role_synergist.png` | 128² |
| `ui_gem_crimson.png` | `#E23B4A` |
| `ui_gem_azure.png` | `#2F7BFF` |
| `ui_gem_verdant.png` | `#2DBE6A` |
| `ui_gem_amber.png` | `#F0A12A` |
| `ui_gem_violet.png` | `#8B5CF6` |
| `ui_gem_prism.png` | `#F5F7FF` |
| `ui_btn_primary.png` | 9-slice CTA |
| `ui_btn_secondary.png` | 9-slice |
| `ui_sync_gauge_fill.png` | Sync Cyan fill |
| `ui_panel_modal.png` | Modal 9-slice |
| `ui_badge_sfw.png` | SFW / age-gate badge |

Optional atlas: `ui_chrome_atlas.png` + `ui_chrome_atlas.json` **in addition to** (not instead of) named singles until Addressables remap lands.

---

## Room BGs → `Assets/_Project/Art/Environment/Rooms/`

| File | Size |
|------|------|
| `room_player_suite_evening.png` | **2160×3840** (9:16) |
| `room_agent_loft_default.png` | **2160×3840** (9:16) |

Optional later (not this kickoff):  
`room_rooftop_garden_night.png` · `room_cafe_shift.png` · `room_rain_window_lounge.png` · `room_clinic_guest.png` · `room_neon_date_overlook.png` · `room_sleepover_morning.png`

---

## Gem VFX → `Assets/_Project/Art/VFX/Gems/`

| File | Notes |
|------|-------|
| `gem_crimson_match.png` | Match pop |
| `gem_azure_match.png` | |
| `gem_verdant_match.png` | |
| `gem_amber_match.png` | |
| `gem_violet_match.png` | |
| `gem_prism_match.png` | |
| `gem_prism_sweep.png` | Row/col Prism sweep |
| `gem_cascade_trail.png` | Thin cascade trail |

---

## Import notes (eng)

1. Prefer PNG sRGB; generate Unity `.meta` on import (do not hand-author meta in git unless needed).  
2. Busts: Sprite, mesh type Full Rect, pivot center; enable alpha is transparency.  
3. Fullbody: same; UI may use Preserve Aspect.  
4. SD: PPU consistent with board (document PPU in Addressables entry when hooked).  
5. Room BGs: default Sprite / Texture 2D non-compressed mid quality for mobile.  
6. Do **not** invent NSFW paths under these folders.

---

*Slots locked for art kickoff PR · Heartline Protocol*
