# Idle loops (pseudo-Live2D)

## Primary: Grok Imagine video (ship these)

User-generated image→video idles from locked fullbodies. White BG — use in **framed** Codex/portrait slots (not raw over Room BGs).

| Agent | Folder | Files |
|-------|--------|-------|
| Velvet Noir | `Assets/_Project/Art/Characters/velvet_noir/Idle/` | `velvet_idle_imagine.mp4` (master), `velvet_idle_loop.mp4` / `.webm`, `velvet_idle_poster.png` |
| Rook Hale | `Assets/_Project/Art/Characters/rook_hale/Idle/` | `rook_idle_imagine.mp4` (master), `rook_idle_loop.mp4` / `.webm`, `rook_idle_poster.png` |

### Specs
- **720×1280** (9:16), **24 fps**, **~6.0s**, H.264, no audio
- Seamless loop (Imagine minimum length)
- White background — fine for framed UI; for Room overlay use green-screen regen or chroma key later

### Fallback
`*_idle_loop_mechanical.gif` — earlier sine breath/sway from still fullbodies (identity-locked, subtler).

### AI still drafts
`Idle/ai_draft/` — Imagine still keyframe attempts (identity drift); not used in-game.

## Unity
`VideoPlayer` → `velvet_idle_loop.mp4` / `rook_idle_loop.mp4`, loop = true. Prefer MP4 over GIF.
