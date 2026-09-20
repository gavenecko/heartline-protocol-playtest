# UI Art Wiring (chrome + Room BGs)

**Date:** 2026-09-20 (America/Denver) · branch `feat/wire-art-ui-slots`

Extends `ArtCatalog` / `ArtLoader` so the vertical slice looks art-driven without Addressables.

## Wired

| Slot | Source | Where |
|------|--------|-------|
| Currency icons | `ui_currency_sta/he/lc/pc/ticket/marks` | Hub currency strip |
| Gem chips | `ui_gem_{crimson,azure,verdant,amber,violet,prism}` | Battle board cells (gameplay unchanged) |
| Rarity frames | `ui_frame_n/r/sr/ssr/ur` | Team / Codex / Gacha reveal |
| Room BGs by action | Talk→suite (+loft), Gift→cafe (+rain), Date→neon (+rooftop), Sleepover→morning | `RoomController` via `ApplyRoomBgForAction` |
| Fullbody | `t0_fullbody` | Codex **Toggle Fullbody**; Gacha reveal prefers fullbody |
| Buttons/panels | `ui_buttons_panels_sheet` | Soft-applied on Hub nav + Room primary + Gacha pull buttons |
| Nav / KV | existing | Title / Hub |

## Load path

Same as busts: `Resources.Load` → disk `StreamingAssets/Art` → `Assets/_Project/Art` → `Assets/Resources/Art`.

Chrome singles + buttons sheet are mirrored under `Assets/Resources/Art/UI/Chrome/`. Extra Room BGs already mirrored under `Resources/Art/Environment/Rooms/`. No StreamingAssets duplicates for chrome (Editor/_Project + Resources cover offline play).

## Still stubbed

- Gem cascade / match pop VFX (`Art/VFX/Gems`) not hooked
- Live2D / Spine; Addressables remap
- Individual `ui_btn_primary` 9-slices (sheet soft-apply only)
- Role icons / sync gauge fill chrome

