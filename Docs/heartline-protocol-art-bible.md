# HEARTLINE PROTOCOL — Art Bible + UI Mock Brief

**Document type:** Production art direction & Figma kickoff brief  
**IP:** Heartline Protocol (original) · Lumina Quay · Atelier Tower  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Pre-production  
**Source GDD:** `/workspace/waifu-gacha-gdd.md`  

**Binding content policy:** All romanceable / collectible characters are adults **18+** (ages from GDD). No underage depiction or implication. Adult / NSFW art is specified at **pipeline, folder, and gating** level only — no erotic prose and no explicit sexual illustration descriptions in this document. Storefront and default builds remain SFW; mature packs are opt-in, age-gated, and CDN-flagged.

---

# A. Art Bible

## 1. Visual Thesis

**Heartline Protocol** reads as neon-noir coastal megacity romance: rain-slick glass, warm apartment gold against cold breach cyan, and adult anime-influenced character art that feels collectible without looking generic “gacha pink sludge.” Combat is crystal-clear gem language; home is soft practical luxury—lived-in Atelier Tower rooms where agents exist as partners, not props. The brand promise is dual: stylish Sync Match readability by day, intimate found-family glow by night.

**Keywords:** neon-noir · empathic sync · coastal megacity · apartment warmth · gem clarity  

| Do | Don't |
|----|-------|
| Adult proportions (18+), consistent head:body, readable silhouettes at SD and fullbody | Childlike faces, school-uniform tropes, or any underage-coded styling |
| Gem colors as UI truth (match board ↔ Burst FX ↔ role chips) | Rainbow noise that breaks colorblind / combat readability |
| Warm hub light vs cold Sector Breach storm light as emotional contrast | Flat gray sci-fi or pure pastel slice-of-life with no noir bite |

---

## 2. World Look

### Lumina Quay (neon-noir coastal megacity)
Vertical city on a bay: stacked transit spines, holographic lease boards, rain channels cut into plazas. Architecture language = glass curtain walls + brushed alloy + biolume signage in gem accent hues. Night is primary marketing hour—reflections double every neon run; day is overcast pearl with sharp speculars. Fog banks roll off the water; Breach storms tint the skyline with prismatic static.

**Lighting:** Key = cool cyan/magenta neon bounce; fill = warm sodium from street canopies; rim = Prism white on hero agents. Avoid pure black voids—use deep indigo `#0B1020` with readable midtones.

**Time-of-day moods:**
- **Dawn:** Soft amber on wet asphalt; hub “morning Affection” energy.
- **Day:** Desaturated coastal gray-blue; corporate Aurelia clarity.
- **Dusk:** Magenta–gold transition; banner / marketing hero hour.
- **Night:** Full neon-noir; default combat and key art.
- **Breach:** Unstable prismatic overlays, scanline flicker, gem-colored lightning.

### Atelier Tower (apartment hub)
Mid-rise residential block the player inherits: lobby with Heartline lease desk, player suite, expandable agent rooms, rooftop garden, basement training. Material palette = warm wood veneer, matte plaster, soft LEDs, textile throws—**human warmth inside the machine city**. Windows always show Quay neon as a living backdrop. Furniture is modular “Decor Coin” sets: loft industrial, clinic-clean, circuit-arena chic, chorus-hack loft—faction-tinged without breaking tower unity.

### Sector Breach environments
Combat stages are emotional-data storms: fractured plazas, flooded metro, Corp atriums mid-collapse, Null server gardens. Geometry stays readable for 3-lane Field overlays. VFX priority: gem match pops in foreground; environment particles secondary and never occlude Burst telegraphs.

---

## 3. Color System

### Gem colors (board + attunement)
| Gem | Hex (primary) | UI use |
|-----|---------------|--------|
| **Crimson** | `#E23B4A` | Striker accents, match FX, HP loss tint |
| **Azure** | `#2F7BFF` | Guardian shields, cool UI links |
| **Verdant** | `#2DBE6A` | Support heals, success states |
| **Amber** | `#F0A12A` | Breaker shred, warning soft |
| **Violet** | `#8B5CF6` | Controller convert, modal chrome |
| **Prism** | `#F5F7FF` → iridescent shift | Wild gem, UR rarity shimmer, pity sparkle |

### Role accent colors (chips, frames, lane tags)
| Role | Accent | Hex |
|------|--------|-----|
| Striker | Red | `#E23B4A` |
| Breaker | Orange | `#F97316` |
| Guardian | Blue | `#3B82F6` |
| Support | Green | `#22C55E` |
| Controller | Purple | `#A855F7` |
| Synergist | Pink | `#EC4899` |

### Master palette tables

**Primary**
| Token | Hex | Notes |
|-------|-----|-------|
| Neon Magenta | `#FF2D8A` | Brand pulse, gacha CTAs |
| Sync Cyan | `#00D4FF` | Sync Gauge, Heartline Grid |
| Quay Indigo | `#12182B` | App chrome base |
| Atelier Gold | `#E8C87A` | Hub warmth, Aff rank-up |

**Secondary**
| Token | Hex |
|-------|-----|
| Corp Silver | `#C5CDD8` |
| Circuit Crimson | `#B91C1C` |
| Clinic Mint | `#6EE7B7` |
| Chorus Violet | `#7C3AED` |
| Breach Prism | `#E0E7FF` |

**Neutrals**
| Token | Hex |
|-------|-----|
| Ink 900 | `#0B1020` |
| Ink 700 | `#1E293B` |
| Mist 400 | `#94A3B8` |
| Mist 200 | `#E2E8F0` |
| Paper | `#F8FAFC` |

**Semantic**
| Token | Hex |
|-------|-----|
| Danger | `#EF4444` |
| Success | `#22C55E` |
| Warning | `#F59E0B` |
| Info | `#38BDF8` |
| Locked / NSFW blur veil | `#0B1020` @ 72% + gaussian |

Rarity frames: N gray · R green · SR blue · SSR purple-gold · UR prismatic animated border.

---

## 4. Typography

| Use | Suggestion (free / common) | Fallback / alt |
|-----|----------------------------|----------------|
| **Display** (titles, banners, agent names) | **Orbitron** or **Exo 2** (Google Fonts) | **Rajdhani** Bold; custom logotype later |
| **UI body** (menus, dialogue, settings) | **Inter** or **Noto Sans** (+ CJK: Noto Sans JP/SC/KR) | **Source Sans 3** |
| **Numbers** (damage, pity, currencies) | **IBM Plex Mono** or **JetBrains Mono** tabular | **Roboto Mono**; enable tabular lining figures |

Rules: Display for 24–48 sp titles only; UI never below 12 sp on mobile (14 sp preferred); damage numbers outline + drop shadow for board contrast; JA/ZH/KO use Noto CJK with same weight ladder (Regular / Medium / Bold). Licensing: prefer OFL/Google Fonts for MVP; swap to licensed brand fonts post-funding without redesigning hierarchy.

---

## 5. Character Art Standards

### Proportions & face
- Adult anime-influenced: roughly **1:6.5–1:7.5** head:body for fullbody; bust crops at mid-chest with adult anatomy signed off in model sheets.
- Faces: mature bone structure; no chibi heads on fullbody heroes (chibi reserved for SD combat / emotes only).
- Hands/feet: production-detail (readable fingers, articulated shoes); not hyper-real but not mittens.
- Heights from bios respected; Juniper (21, youngest roster) remains clearly adult—never “petite child” coding.

### Outfit language by faction
| Faction | Look |
|---------|------|
| **Independent / Tower** | Layered street-atelier: soft coats, heartline pin, practical boots; found-family cohesion |
| **Crimson Circuit** | Arena glamour: cropped jackets, racing stripes, spotlight gloss, trophy accents |
| **Tidewell Clinic** | Med-tech chic: clean coats, vitals LEDs, mint/white with calm teal linings |
| **Aurelia Corp** | Contract polish: structured blazers, silver trim, ID lanyards, PR-ready symmetry |
| **Null Chorus** | Underground hack loft: cables, asymmetric cuts, violet static, anti-logo patches |

### Evolution tiers T0–T3
| Tier | Name | Visual rules |
|------|------|--------------|
| **T0 Base** | Obtain look | Clean silhouette; faction readable; minimal VFX |
| **T1 Ascendant** | ★★★ + Ch.1 | Outfit complexity +1 (armor plates / clinic LEDs / circuit trim); soft aura idle |
| **T2 Heartbound** | Aff 5 + frags | Romantic sync motifs (heartline filigree); stronger idle particles; alternate colorway accents |
| **T3 Resonance** | Aff 8 + awakening | Alternate outfit silhouette shift; combat Burst VFX upgrade; prismatic rim on UR/SSR |

Skins are cosmetic overlays—never replace T3 combat readability.

### Asset checklist per agent
| Asset | Spec |
|-------|------|
| Bust portrait | 2048×2048, transparent, UI circle-ready |
| Fullbody | 2048×4096, T0–T3 variants |
| Live2D / Spine (hub + Room) | Face + torso + hair physics; Room idle + talk + gift react (SFW mocap scope) |
| SD combat sprite | ~512 tall chibi or trimmed live; Burst pose sheet |
| Skill icons | 256×256 × Basic / Burst / Passive / Bond |
| Gift icons | 128×128 preferred + signature |
| Aff CG list (counts only) | Story SFW CGs per Aff gates; optional NSFW packs per SSR/UR (see §A.8)—**counts only, no content description** |

---

## 6. Launch Roster Visual Briefs (12 agents · all 18+)

Gem attunements invented where GDD omitted; Bond motifs use known partners.

### 1. Mira Solenne — 24 · SSR · Synergist · Independent
- **Silhouette:** Soft S-curve; open-handed “Handler partner” stance.  
- **Hair / eyes / palette:** Warm chestnut waves; amber-gold eyes; cream + magenta + indigo.  
- **Outfit:** Atelier coat over heartline-synced bodice; Tower pin.  
- **Pose personality:** Inviting eye contact; half-step toward player.  
- **Prop:** Folded Grid blueprint tablet.  
- **Gems:** Verdant + Prism.  
- **Bond motif:** Shared pulse ribbon with handler; duo glow with Ora / Helene arcs.

### 2. Kade Virell — 27 · SSR · Striker · Crimson Circuit
- **Silhouette:** Sharp inverted triangle; championship stance.  
- **Hair / eyes / palette:** Black with crimson underlights; scarlet eyes; black / Circuit Crimson / gold.  
- **Outfit:** Arena jacket open, racing stripe harness, spotlight gloss.  
- **Pose:** Chin-up challenge; one hand on hip.  
- **Prop:** Trophy-blade gauntlet.  
- **Gems:** Crimson + Amber.  
- **Bond:** Lane-crush flare with **Luce Amaranth**.

### 3. Dr. Nyra Quell — 31 · SSR · Support · Tidewell Clinic
- **Silhouette:** Tall calm column; centered weight.  
- **Hair / eyes / palette:** Silver-ash bob; cool teal eyes; white / mint / soft navy.  
- **Outfit:** Long clinic coat, vitals LED cuff, soft scarf.  
- **Pose:** Measuring calm—clipboard or open palm heal gesture.  
- **Prop:** Sync-cortisol scanner wand.  
- **Gems:** Verdant + Azure.  
- **Bond:** Healing lattice with **Wren Halcyon**.

### 4. Sable Rin — 22 · SR · Controller · Null Chorus
- **Silhouette:** Narrow vertical; hooded edges.  
- **Hair / eyes / palette:** Ink-black with violet tips; violet eyes; charcoal / Chorus Violet / cyan glitch.  
- **Outfit:** Asymmetric hacker wrap, cable jewelry, anti-NDA patch.  
- **Pose:** Finger-to-lips “whisper the board.”  
- **Prop:** Whisper-key deck.  
- **Gems:** Violet + Azure.  
- **Bond:** Signal braid with **Zara “Hex” Montal**.

### 5. Captain Ora Finch — 29 · SSR · Guardian · Aurelia Corp
- **Silhouette:** Broad-shouldered upright shield stance.  
- **Hair / eyes / palette:** Steel-blonde tight braid; ice-blue eyes; Corp Silver / navy / Azure.  
- **Outfit:** Structured security blazer-armor hybrid, ID lanyard.  
- **Pose:** Salute softening into protect.  
- **Prop:** Tower riot-shield module (deployable FX).  
- **Gems:** Azure + Crimson.  
- **Bond:** Loyalty seal with **Mira**; duty tension with Aurelia cast.

### 6. Luce Amaranth — 25 · SR · Breaker · Crimson Circuit
- **Silhouette:** Explosive forward lean; midlane wedge.  
- **Hair / eyes / palette:** Hot pink undercut; amber eyes; magenta / orange / vinyl black.  
- **Outfit:** Cropped circuit jacket, shredder gauntlets, vinyl-collector pins.  
- **Pose:** Grin + fist pump mid-burst.  
- **Prop:** Vinyl disc chakram.  
- **Gems:** Amber + Crimson.  
- **Bond:** Spotlight duo with **Kade**.

### 7. Wren Halcyon — 26 · SR · Support · Tidewell
- **Silhouette:** Soft rounded readiness; nurse bag hip weight.  
- **Hair / eyes / palette:** Soft brown bun; green eyes; mint / cream / rose.  
- **Outfit:** Night-shift scrubs chic + Room caretaker cardigan.  
- **Pose:** Offering tea / gift craft smile.  
- **Prop:** Apothecary gift kit.  
- **Gems:** Verdant.  
- **Bond:** Clinic care loop with **Nyra**.

### 8. Ivy Cross — 23 · R · Striker · Independent
- **Silhouette:** Eager forward crouch; growth-arc posture.  
- **Hair / eyes / palette:** Copper ponytail; determined hazel; Tower cream / crimson accents.  
- **Outfit:** Rookie field jacket, mismatched training pads.  
- **Pose:** “Ready!” fist at chest.  
- **Prop:** Practice sync baton.  
- **Gems:** Crimson.  
- **Bond:** Mentorship glow with Mira / Ora.

### 9. Professor Helene Voss — 34 · SSR · Controller · Aurelia (ex)
- **Silhouette:** Elegant long coat sweep; mentor gravity.  
- **Hair / eyes / palette:** Deep burgundy pinned waves; violet-gray eyes; academic black / gold / violet.  
- **Outfit:** Ex-Corp theorist coat, chalk-dust gloves, Grid monocle HUD.  
- **Pose:** Lecturing finger; knowing half-smile.  
- **Prop:** Heartline theory slate.  
- **Gems:** Violet + Prism.  
- **Bond:** Origin-secret thread with Mira; intellectual sync with Null Controllers.

### 10. Zara “Hex” Montal — 28 · SR · Synergist · Null Chorus
- **Silhouette:** Dance-ready open stance; headphone halo.  
- **Hair / eyes / palette:** Neon teal dreads/locks; pink eyes; black / teal / hot pink.  
- **Outfit:** DJ-signal witch layers, LED sleeves, festival harness.  
- **Pose:** Drop-the-beat arms; hub dance emote hero.  
- **Prop:** Hex-pad controller.  
- **Gems:** Prism + Violet.  
- **Bond:** Festival waveform with **Sable**; Bond Festival event queen art.

### 11. Juniper Tate — 21 · R · Guardian · Independent
- **Silhouette:** Compact adult guard stance (clearly 21+); door-frame weight.  
- **Hair / eyes / palette:** Sandy short cut; bright blue eyes; Tower denim / silver / soft gold.  
- **Outfit:** Earnest door-guard vest, radio, comic-relief patch.  
- **Pose:** Salute with slightly too-serious face.  
- **Prop:** Atelier lobby keycard lance.  
- **Gems:** Azure.  
- **Bond:** Tower family shield with Mira / Ivy.

### 12. Aveline Roux — 30 · UR (limited debut) · Breaker · Independent
- **Silhouette:** Calm wide base; chef-to-catastrophe contrast.  
- **Hair / eyes / palette:** Platinum with ember tips; deep amber eyes; charcoal / ember / prism white.  
- **Outfit:** Freelancer coat over kitchen-apron remnant; devastation plating on Field.  
- **Pose:** Quiet ladle → cut to field crush (key art dual pose).  
- **Prop:** Seasoned cleaver-breaker (combat stylized; Room kitchen knife SFW).  
- **Gems:** Amber + Prism.  
- **Bond:** Mythic independent link; Room domestic motif vs Field ruin motif.

---

## 7. VFX & Combat Readability

| System | Direction |
|--------|-----------|
| **Gem match FX** | Color-true pop + short radial; Prism = iridescent row/col sweep; cascade trails thin so board stays legible |
| **Sync Gauge fill** | Cyan heartline pulse along agent portrait rim; dual-attune = split gem colors on bar |
| **Burst telegraph** | 300–400 ms lane highlight + character flash; shape = role icon stamp; never surprise-frame lethal |
| **Lane indicators** | Front / Mid / Back floor glyphs; Front warmer, Back cooler; max 2 agents/lane readable as paired icons |
| **Damage numbers** | Tabular mono; Crit = larger + shake; Heal = Verdant float-up; Breaker shred = Amber chips off armor bar |
| **Bond Skill** | Dual-character silhouette clap once per battle; pink Synergist ribbon if Synergist present |

Priority stack: telegraph > gems > portraits > environment particles.

---

## 8. Room / Intimacy Art Pipeline

### Room BG list (8)
1. Player Suite evening  
2. Agent Room default loft  
3. Rooftop Garden night  
4. Café Shift corner (job-linked)  
5. Rain-window lounge  
6. Clinic-calm guest room  
7. Neon city date overlook  
8. Sleepover morning light  

**Furniture style:** Modular low-poly-to-mid illustration hybrid; Decor Coin sets tint by faction but share Atelier wood/metal kit. Snap-to-grid for player placement.

### CG resolution & aspect
- Still CG: **2160×3840** (9:16) master; downres to 1080×1920 for mobile.  
- Optional short loops: same aspect, ≤8 s, SFW or mature pack as flagged.  
- Gallery thumbs: 512×912 with blur veil when locked / SFW Mode.

### SFW vs age-gated NSFW pack structure (pipeline only)
```
/art/agents/{id}/
  sfw/
    portraits/ fullbody/ live2d/ cg_story/ cg_intimate_sfw/
  nsfw/                    # separate CDN pack; default NOT in base build
    cg_set_a/              # Aff Rank 8 unlock
    cg_set_b/              # Aff Rank 10 unlock
    meta.json              # consent flags, age_gate: 18
```
- Build flag + CDN entitlement; region kill-switch.  
- Unlock rules: Adult Scenes master ON + age gate passed + Aff threshold + per-scene confirm.  
- VIP never required. Abort = no Heart Energy loss.

### Consent UI chrome (non-graphic)
- Age gate modal (18+ confirm) before first Adult ON.  
- Master toggle: Settings → Content → Adult Scenes.  
- Per-scene: title card + content tags (text only) + Confirm / Cancel; Skip after first view.  
- SFW Mode: locks mature folders; softens UI flirt copy; Room actions show SFW path only.  
- Locked tiles: blurred thumb + Aff rank requirement chip—no preview of mature frames.

---

## 9. Marketing / Store Art

**Key visual composition:** Atelier Tower window foreground (warm) → Mira + 2–3 roster heroes mid → Lumina Quay neon bay behind; gem constellation / Heartline pulse motif; title lockup bottom-safe for age badge.

**Age badge:** Store **17+ / Mature** badge per platform guidelines—top-left or platform-mandated corner; never covered by logo. In-app 18+ gate separate from store badge.

**SFW storefront vs mature trailer rules:**
- Store screenshots / page: SFW only; Room stills without mature packs; combat + hub comedy.  
- Mature trailer / Discord / Patreon-style channels (where allowed): may show **that adult packs exist** (folder/gating messaging)—no explicit sexual illustration in ads that hit general store APIs.  
- Trailer audio: stylish synth + city rain; no suggestive VO that implies minors.

---

# B. UI Mock Brief (Figma Kickoff)

## 1. Device Targets

| Target | Spec |
|--------|------|
| **Primary** | Portrait mobile **1080×1920** logical; design at @1x then export @2x/@3x |
| **Safe areas** | Top 88 px (status + currency); bottom 120 px (nav + home indicator); side 24 px gutters |
| **PC / Steam** | Landscape scale: letterbox or reflow hub to 16:9; battle board keeps portrait panel + side Field; min width 1280 |
| **Touch** | Primary hit 48×48 dp; gacha pull CTA 56+ dp tall |

---

## 2. Navigation IA

**Bottom nav (5 tabs) + Hub radial shortcut for Jobs/Mail/Pass.**

| Tab | Screen |
|-----|--------|
| **Hub** | Atelier Tower home |
| **Stages** | Sector select → pre-battle |
| **Agents** | Roster → detail / bonds |
| **Room** | Affection actions |
| **More** | Gacha · Shop · Social (Friends / Circle) |

Gacha also deep-linkable from Hub banner carousel. Shop/Social live under More as list, or as Hub radial nodes for FTUE discoverability.

---

## 3. Screen-by-Screen Wireframe Briefs

### Atelier Hub home
- **Top zone:** Currency chips (Pulse / Luma / Stamina / Heart Energy) + Mail + Pass claim.  
- **Hero zone:** Live2D/Spine tower lobby or suite; tappable agents; rooftop / basement hotspots.  
- **Mid widgets:** Daily missions strip; Atelier Income claim; Job slots (3+).  
- **Promo carousel:** Limited banner / Drift Event.  
- **Bottom:** Main nav.  
- **Must-show:** Idle income ready state; energy regen timers; one-tap Room entry on featured agent.

### Sector Stage select + pre-battle
- **Map / list:** Chapter nodes; stamina cost badges; ★ clearance.  
- **Stage detail sheet:** Enemy tags, recommended roles, rewards preview.  
- **Pre-battle:** 4 squad slots + 1 Support; **lane assign** Front/Mid/Back (max 2/lane); Bond Matrix hints.  
- **Must-show:** Gem attunement icons per agent; Assist Aim toggle; Start (Stamina confirm).

### Sync Match battle HUD
- **Top:** Wave / turn timer; enemy lane HP.  
- **Board center:** 6×8 gems; swap feedback.  
- **Bottom-left:** 4 agent Sync Gauges + Burst buttons.  
- **Bottom-right / overlay:** Lane Field miniatures.  
- **Must-show:** Combo multiplier; Burst telegraph; pause / auto-basic; damage numbers.  
- **No chrome over gem cells** during swap window.

### Agent detail (stats, evolve, bonds)
- **Left / top:** Fullbody T-tier art; rarity frame; role chip; faction badge.  
- **Tabs:** Stats · Skills · Evolve (T0–T3) · Bonds · Voice / Story.  
- **Must-show:** Level/star; fragment progress; Aff rank; gem attunements; Bond partners list; Evolve Core costs.

### Room (affection actions)
- **BG:** Selected Room BG + furniture.  
- **Agent Live2D:** Center; mood emoji.  
- **Action bar:** Talk (5) · Gift (8) · Date (15) · Intimate Scene (20 if unlocked) · Sleepover assign.  
- **Must-show:** Heart Energy cost + Aff XP preview; preferred gift highlight; consent chrome if mature path; Aff rank meter.

### Gacha banner + pity meter
- **Banner art:** Featured agent(s); rates link (legal sheet).  
- **Pull CTAs:** ×1 (160) · ×10 (1600); ticket variants.  
- **Pity panel (ethical clarity):** Soft pity start 60; hard SSR 90; featured spark rules; UR 120 if applicable; Banner Medals / 200 spark shop.  
- **Must-show:** Published rates; history; Beginner Banner remaining pulls; **no dark-pattern countdown on romance**.

### Results / rewards
- **Victory:** ★ rating; XP / coins / mats / frags; first-clear chests.  
- **Gacha reveal:** Rarity stingers → card flip; skip; new vs dupe→fragments.  
- **Aff rank-up:** Fullscreen motif; unlock list (story / evolve / scene pack flags).  
- **Must-show:** Claim-all; overflow fragment exchange CTA.

---

## 4. Component Library

| Component | Notes |
|-----------|-------|
| **Buttons** | Primary (magenta), Secondary (ghost cyan), Danger, Disabled; 12 radius; press scale 0.97 |
| **Rarity frames** | N–UR; UR animated prism stroke |
| **Currency chips** | Icon + abbreviated count; Pulse distinct from Luma; tap → shop |
| **Energy bars** | **Stamina** = Sync Cyan fill, lightning icon; **Heart Energy** = Magenta fill, heartline icon—never same hue |
| **Toast** | 3 s; success/warn/error; top-safe |
| **Modal** | Scrim 60%; 24 radius card; focus trap |
| **Age-gate** | 18+ DOB or confirm pattern per store law; blocks Adult ON |
| **Adult toggle** | Settings switch default OFF; requires age-gate; SFW Mode master reverse |

---

## 5. Motion Notes (timing targets)

| Beat | Timing |
|------|--------|
| Gacha pull reveal (single) | 800–1200 ms to rarity lock; SSR/UR +400–800 ms flourish |
| 10-pull cascade | Stagger 80–100 ms per card; skip compresses to 200 ms total |
| Aff rank-up | 600 ms slam + 400 ms unlock list; haptics if available |
| Burst camera punch | 120 ms zoom + 80 ms settle; reduce-motion → opacity flash only |
| Gem match pop | 150–200 ms; cascade ≤120 ms per step |
| Hub agent tap | 200 ms Room crossfade |

---

## 6. Accessibility

- **Colorblind gem shapes:** Crimson◆ · Azure● · Verdant■ · Amber▲ · Violet★ · Prism◎ (shape + color always paired).  
- **Text scale:** Support 100–150% dynamic type; reflow currency chips.  
- **Reduce-motion:** System setting kills camera punch, gacha long flourishes, Live2D heavy sway.  
- **NSFW blur defaults:** Mature thumbs blurred until unlock + Adult ON; SFW Mode = permanent hide of mature folders.  
- **Contrast:** Body text ≥ 4.5:1 on Ink/Paper pairs; damage numbers outlined.

---

## 7. Deliverable Checklist — First Figma Sprint

**P0**
1. Design system: color, type, buttons, chips, frames, energy bars  
2. Hub home (empty + claimed + banner states)  
3. Battle HUD + gem shape set  
4. Pre-battle lane assign  
5. Gacha banner + pity panel (ethical)  
6. Age-gate + Adult toggle + SFW Mode settings  

**P1**
7. Agent detail (Stats / Evolve / Bonds tabs)  
8. Room actions + Aff meter  
9. Stage select + results  
10. Rarity pull reveal motion specs  

**P2 variants**
11. PC landscape hub reflow  
12. Colorblind simulation pass  
13. Locked NSFW gallery tile states (blur only)

---

# C. Close

## Asset Volume Estimate (aligned to GDD launch)

| Asset | Launch target | Art notes |
|-------|---------------|-----------|
| Playable agents | **10–12** | Full checklist × roster |
| Evolution art | ×4 tiers (T0–T3) per agent | Outfit + VFX escalation |
| Main chapters | **6** + prologue | BG + story SFW CG |
| Stages (all) | **~120** | Reuse env kits; elite/boss uniques |
| Room backgrounds | **8** | Listed in §A.8 |
| Furniture pieces | **60** | Decor Coin sets |
| SFW CG | **40+** | Story + Aff SFW intimate packs |
| NSFW scene packs | **1 per SSR/UR** (opt-in CDN) | Folder/gating only; Aff 8 / 10 |
| Skill / gift icons | Per agent + shared gift tiers | 256 / 128 |
| Banners | Standard + beginner + 1 limited | Key visuals |
| Voice key art support | JP+EN SSR; EN all | Mouth charts for Live2D |

**Rough illustration volume (order-of-magnitude):** ~12 bust + 12×4 fullbody tiers + 8 Room BG + 40 SFW CG + SD sheets + icons ≈ **mid-triple-digit** production tickets before skins/live-ops.

## Open Questions for Art Director

1. Live2D vs Spine as hub/Room primary—pipeline staffing and license preference?  
2. Combat SD: chibi vs trimmed Live2D Burst—readability vs cost on mid-tier Androids?  
3. UR prismatic frame: shader budget or pre-rendered border atlas?  
4. Faction furniture kits: strict set-bonuses or free mix for player expression?  
5. Key visual: Mira-solo Handler fantasy vs full launch ensemble for store first impression?  
6. CJK typography: Noto everywhere or region-specific display fonts for JA marketing?  
7. Mature pack delivery: separate APK/DLC vs in-client CDN entitlement only?  
8. Breach environment fidelity: hand-painted 2.5D vs modular 3D with 2D character overlay?

---

*— End of Art Bible + UI Mock Brief —*  
*All characters 18+. NSFW = pipeline / folder / gating only. Original IP: Heartline Protocol / Lumina Quay / Atelier Tower / GDD factions.*
