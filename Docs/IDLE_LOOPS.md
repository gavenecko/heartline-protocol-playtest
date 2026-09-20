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

## Unity play notes

### Runtime load (no Addressables / no VideoClip import required)
`IdleVideo` + `ArtCatalog.IdleLoopRelativePath` resolve:

1. `StreamingAssets/Art/Characters/{folder}/Idle/{id}_idle_loop.mp4` (player builds — **shipped copies**)
2. `Assets/_Project/Art/Characters/{folder}/Idle/{id}_idle_loop.mp4` (Editor / repo)

Then `VideoPlayer.source = Url` with `file://…`, `isLooping = true`, `audioOutputMode = None`, render to a 720×1280 `RenderTexture` assigned to the portrait `RawImage`.

Prefer **MP4** over GIF/WebM for Unity VideoPlayer.

### Where it plays
| Screen | Behavior |
|--------|----------|
| **Codex** | Unlocked **velvet** / **rook** → looping idle over the bust slot. Locked / other agents → still `t0_bust` (secrets stay hidden via `VisibleAgents`). Missing MP4 → bust, then idle poster PNG. |
| **Room** | Default Mira still bust. **Cycle guest** cycles Mira → unlocked Velvet → unlocked Rook. Velvet/Rook show idle loops; Talk/Gift/Date/Sleepover remain Mira-only in the slice. |

### Code map
- `ArtCatalog` — idle relative paths + 720×1280 constants
- `IdleVideo` — URL resolve + `VideoPlayer` on the portrait GO
- `ArtLoader.ApplyPortraitOrIdle` — prefer idle → bust → poster
- `CodexController` / `RoomController` — hold `VideoPlayer` + `RenderTexture`, dispose on disable

### Limitations
- Room affection / actions are still Mira-hardcoded; guest cycle is portrait/idle preview only.
- White-BG idles look best in framed Codex; Room shows them over suite BG without chroma key yet.
- WebM / mechanical GIF are not wired — MP4 only.
