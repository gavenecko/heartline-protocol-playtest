# Idle loops (pseudo-Live2D)

Still-image gen cannot hold identity across true keyframes, so ship **mechanical breath/sway** loops from locked `t0_fullbody.png` assets.

## Deliverables

| Agent | Folder | Files |
|-------|--------|-------|
| Velvet Noir | `Assets/_Project/Art/Characters/velvet_noir/Idle/` | `velvet_idle_loop.webm`, `.gif`, `.mp4`, `velvet_idle_k0–k3.png` |
| Rook Hale | `Assets/_Project/Art/Characters/rook_hale/Idle/` | `rook_idle_loop.webm`, `.gif`, `.mp4`, `rook_idle_k0–k3.png` |

## Technique

1. Pad fullbody to 720×960 white canvas, enlarge 8%.
2. 20 frames: sine breath scale (~1.0–1.018) + sway (±8px X, −4px Y).
3. Ping-pong for seamless loop (~2.7s cycle).
4. Export WebM (VP9), MP4 (H.264), GIF (12fps palette).

## AI draft keyframes

`Idle/ai_draft/` holds Imagine attempts. Rook stayed closer; Velvet outfit/hair drifted. Use mechanical loops in-game until Cubism/Spine or video gen is available.

## Unity note

Play with `VideoPlayer` (WebM/MP4) or `Image` + GIF plugin on Room/Codex bust slots. Prefer WebM/MP4 over GIF for size.
