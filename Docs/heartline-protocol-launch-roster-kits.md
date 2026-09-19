# HEARTLINE PROTOCOL — Launch Roster Combat / Romance Kits

**Document type:** Design kit bible (combat + affection + meta)  
**IP:** Heartline Protocol · Lumina Quay · Atelier Tower  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Pre-production balance draft  
**Sources:** `/workspace/waifu-gacha-gdd.md` · `/workspace/heartline-protocol-art-bible.md`

**Binding content policy:** All agents are adults **18+** (ages from GDD). NSFW content is listed as **unlock ranks / scene pack IDs only** — no erotic prose. Mechanical numbers are approximate designer targets coherent with Sync Match RPG (3 lanes, gem colors, Aff 1–10, star caps).

---

## Mechanical Quick Reference (from GDD)

| Rule | Value |
|------|-------|
| Squad | 4 active + 1 Support (50% stats, no Aff) |
| Lanes | Front / Mid / Back (max 2 agents per lane) |
| Gems | Crimson · Azure · Verdant · Amber · Violet + Prism (wild) |
| Basic | Auto if gauge ≥20% |
| Burst | Manual at 100%; CD = gauge empty + **1 turn lock** |
| Bond Skill | Both Bond partners in squad; **once per battle** |
| Max active Bond bonuses | **2** per fight |
| Star → max Lv | ★40 · ★★50 · ★★★60 · ★★★★70 · ★★★★★80 · Awakened 90 |
| Base stars | R ★★ · SR ★★★ · SSR ★★★★ · UR ★★★★★ |
| Aff ranks | 1–10 (NSFW set A @8, set B @10 if Adult ON) |
| Evolve | T0 Base · T1 Ascendant · T2 Heartbound (Aff5) · T3 Resonance (Aff8) |

**Stat scaling convention (this doc):** Lv1 / Lv60 shown for all; R ≈ 0.72× SSR budget, SR ≈ 0.88×, SSR = 1.0×, UR ≈ 1.12× at same level band. Promote/Awaken push past Lv60 toward rarity caps.

---

# Team Archetypes (5 Launch Comps)

### 1. Tutorial Tower Family (F2P early)
| Lane | Agents |
|------|--------|
| Front | Juniper Tate (R Guardian), Ivy Cross (R Striker) |
| Mid | Mira Solenne (SSR Synergist) |
| Back | Wren Halcyon (SR Support) |
**Fantasy:** Survive chapters 1–3 while Mira accelerates gauge share. Jobs/Room focus on Mira Aff.  
**Bond priority:** Mira↔Juniper or Mira↔Ivy (mentorship).

### 2. Circuit Spotlight Crush
| Lane | Agents |
|------|--------|
| Front | Kade Virell (SSR Striker) |
| Mid | Luce Amaranth (SR Breaker), Hex Montal (SR Synergist) |
| Back | Wren or Nyra |
**Fantasy:** Amber/Crimson board → shred then nuke. Luce Breaks, Kade finishes, Hex feeds Prism fill.  
**Bond:** Kade↔Luce (lane crush) mandatory.

### 3. Clinic Lattice (sustain Elite / Tower)
| Lane | Agents |
|------|--------|
| Front | Ora Finch (SSR Guardian) |
| Mid | Nyra Quell (SSR Support), Sable Rin (SR Controller) |
| Back | Wren Halcyon (SR Support) |
**Fantasy:** Double-Support + Guard holds Abyss checkpoints; Sable converts bad boards.  
**Bond:** Nyra↔Wren healing lattice.

### 4. Chorus Whisper Control
| Lane | Agents |
|------|--------|
| Front | Ora or Juniper |
| Mid | Sable Rin, Helene Voss (SSR Controller), Hex |
| Back | Mira |
**Fantasy:** Violet/Prism convert meta; stall enemy Burst windows; Mira shares gauge into Controllers.  
**Bond:** Sable↔Hex; Helene↔Mira origin thread.

### 5. Limited Debut Devastation (post-Aveline)
| Lane | Agents |
|------|--------|
| Front | Aveline Roux (UR Breaker) |
| Mid | Kade or Luce, Mira |
| Back | Nyra |
**Fantasy:** UR Break Power deletes armored bosses; Mira Sync + Nyra keep Aveline alive on Front.  
**Bond:** Mira↔Aveline independent mythic link (Matrix unlock via limited event duo stage).

---

# Balance Spreadsheet Stub — Rarity Power Budgets

| Budget axis | R (★★) | SR (★★★) | SSR (★★★★) | UR (★★★★★) |
|-------------|--------|----------|------------|------------|
| **Base ATK index** (Lv60) | 0.72 | 0.88 | 1.00 | 1.12 |
| **HP index** | 0.78 | 0.90 | 1.00 | 1.08 |
| **Burst multiplier band** | 180–240% ATK | 240–320% | 320–420% | 400–520% |
| **Utility weight** (heal/shield/break/control) | Light | Medium | Full signature | Signature + unique field rule |
| **Passive power** | Niche / growth | Strong niche | Role-defining | Role-defining + Prism synergy |
| **Bond Skill potency** | +6–8% style | +8–10% | +10–12% | +12–15% or unique convert |
| **Fragment / dupe value** | High (easy stars) | Medium | High (pity chase) | Highest (limited only) |
| **Board dependence** | Single color OK | 1–2 colors | Dual preferred | Dual + Prism payoff |
| **F2P accessibility** | Standard + weekly shop | Soft pity / events | Beginner + hard pity | Limited banner only |
| **Star soft-cap expectation (M1 F2P)** | ★★★–★★★★ | ★★★–★★★★ | ★★★★ | ★★★★★ (if pulled) |

**Role budget modifiers (multiply Burst/utility):** Striker +ATK/−utility · Breaker +Break Power · Guardian +Shield/HP · Support +Heal · Controller +crowd-control duration · Synergist +gauge share / Bond enable.

**Anti-power-creep rule:** New limited SSRs should beat launch SSR in **one** niche, not all axes. UR may own a unique mechanic (e.g., Aveline armor ignore on Break stacks) without raising raw ATK beyond 1.15× SSR forever.

---

# Voice Line Categories Checklist (not full scripts)

Record / localize for each agent (JP+EN for SSR/UR; EN for R/SR at launch):

| Category | Trigger | Notes |
|----------|---------|-------|
| Login / Hub greet | Daily first Hub open | Variants by Aff rank band (1–3 / 4–7 / 8–10) |
| Stamina empty | Attempt stage at 0 Stamina | Light comic beat |
| Pre-battle ready | Squad confirm | Role flavor |
| Match cascade praise | 5+ / 8+ combo | Short barks |
| Basic Skill | Gauge ≥20% auto | Soft SFX + one line |
| Burst Skill | Manual Burst | Signature shout; longer for SSR/UR |
| Bond Skill | Duo fire | Name partner or “Sync lock” |
| Enemy KO / Wave clear | Wave end | |
| Low HP (self) | HP <30% | |
| Ally KO | Squadmate down | Guardian/Support variants heavier |
| Victory | Stage clear | ★ rating variants optional |
| Defeat | Party wipe | |
| Aff Rank up | Aff 2/3/5/8/10 | Unlock list VO |
| Gift liked / loved / disliked | Gift result | Preferred = 1.5× Aff |
| Date start / end | Aff≥3 date | |
| Sleepover morning | Overnight assign | |
| Job complete | Agent returns from Job | Idle affinity jobs get unique lines |
| Gacha obtain (self) | Pull reveal | Rarity stinger length scales |
| Evolve T1 / T2 / T3 | Visual unlock | |
| Intimate Scene unlock flag | Aff 8 / 10 + Adult ON | **Title card VO only** — no erotic script in base VO sheet |
| Birthday | Calendar | |
| Seasonal / Event | Bond Festival etc. | Hex priority event pack |

---

# Agent Kits (12)

---

## 1. Mira Solenne

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Pulse Anchor* / Mira Solenne |
| **Age** | 24 |
| **Rarity / Role / Faction** | SSR · Synergist · Independent |
| **Preferred lanes** | Mid (primary), Back |
| **Gem attunement** | Verdant + Prism |
| **Height / Voice** | 168 cm · Warm mezzo, soft command |
| **Base stars / max Lv reminder** | ★★★★ → Lv70; Awaken → Lv90 |

### Identity
Mira fled Aurelia with Grid blueprints and a conscience still half-corporate; she manages the Handler’s schedule like a co-conspirator roommate. Earnest without being naive, she teases when Sync rates spike and goes quiet when Breach static hits the Tower. She is the tutorial’s emotional north star and the roster’s social glue.

**Playstyle fantasy:** *Share the beat—everyone Bursts sooner because you’re on the board.*  
**Signature prop:** Folded Grid blueprint tablet.  
**Preferred gifts:** T1 Sunwell Tea · T2 Custom Midnight Playlist · T3 Void-Safe Blueprint Sleeve.  
**Disliked:** Cheap Corp swag pens (Aff XP −50% / sour VO).

### Base stats (Lv1 / Lv60 SSR-scaled)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 980 | 8,420 |
| ATK | 210 | 1,820 |
| DEF | 95 | 780 |
| Sync Rate | 12 | 48 |
| Crit Rate | 5% | 12% |
| **Gauge Share Power** | 8% | 18% |

### Skills
- **Passive — Heartline Relay:** Allies in Mira’s lane gain +10% Sync fill from gem matches. When Mira matches Verdant or Prism, all allies gain +4% gauge (once per cascade step, max 3/turn).
- **Basic (≥20%) — Soft Pulse:** Heal lowest-HP ally for **45% ATK** + grant **8% gauge** to one random ally (prefers Bond partner).
- **Burst (100%) — Protocol Overlap:** Mid/Back targeting. All allies +**22% Sync fill** instantly; Mira’s lane allies deal **+18% skill damage for 2 turns**. Damage pulse **180% ATK** split to Mid enemies. *CD: gauge empty + 1 turn lock.*
- **Bond Skill — Shared Lease (Ora Finch *or* Helene Voss):** Once/battle: both partners refill **35% gauge** and cleanse 1 debuff from Front ally.

### Growth & collection
- **Promote mats:** Synergist Thread · Independent Crest · Verdant Core Dust.  
- **Evolve fragments:** T1 30 · T2 60 + Aff5 · T3 100 + Aff8 + Awakening Trial stub.  
- **Awakening hook:** Relive the night she stole the blueprints—protect the Handler’s “ghost lease” boss.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Captain Ora Finch | +10% DEF both; Aff XP +15% when both in Room week |
| Professor Helene Voss | +8% Sync fill squad; Violet gems count 0.25× toward Mira gauge |
| Ivy Cross | Ivy ATK +12%; Mira Basic heal +10% |
| Aveline Roux | Prism matches +10% Break Power to Breakers in squad |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Leaseholder* | Mira admits why she chose your Tower over another Corp contract. |
| **5** | *Blueprint Between Us* | You decode a redacted Grid page together; T2 Heartbound unlocks. |
| **8** | *Resonance Clause* | She stops calling you only “Handler” in private; T3 + scene gate. |

**NSFW packs (IDs only):** Aff8 `NSFW_MIRA_A` · Aff10 `NSFW_MIRA_B`

### Room / hub
- **Idle Job affinity:** Night Broadcast (+Aff XP), Café Shift.  
- **Aff9 furniture theme:** *Blueprint Loft* (drafting table, magenta pulse lamps).

### Meta notes
Best early/mid glue unit; still relevant late as gauge engine. **Extremely F2P-friendly** (starter). Banner: **Beginner + Standard**.

---

## 2. Kade Virell

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Spotlight Edge* / Kade Virell |
| **Age** | 27 |
| **Rarity / Role / Faction** | SSR · Striker · Crimson Circuit |
| **Preferred lanes** | Front (primary), Mid |
| **Gem attunement** | Crimson + Amber |
| **Height / Voice** | 175 cm · Confident contralto, arena mic energy |
| **Base stars / max Lv reminder** | ★★★★ → Lv70; Awaken → Lv90 |

### Identity
Undefeated Circuit celebrity who treats Breaches like pay-per-view—until loyalty costs her a title bout. Cocky armor hides a Handler-shaped soft spot she refuses to admit on stream. She collects trophies, grudges, and exact Sync percentages.

**Playstyle fantasy:** *Front-lane superstar: match red-gold, delete the boss bar.*  
**Signature prop:** Trophy-blade gauntlet.  
**Preferred gifts:** T1 Energy Shot Can · T2 Signed Vinyl Bootleg · T3 Championship Ribbon Charm.  
**Disliked:** Participation trophies / cheap plastic medals.

### Base stats (Lv1 / Lv60)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 920 | 7,680 |
| ATK | 265 | 2,350 |
| DEF | 80 | 640 |
| Sync Rate | 10 | 42 |
| Crit Rate | 8% | 22% |
| **Crit Damage** | 150% | 175% |

### Skills
- **Passive — Arena Favor:** Front lane: +15% physical Burst damage (stacks with lane +15%). Crits refund **5% gauge** (max 2/turn).
- **Basic (≥20%) — Spotlight Cut:** Single Front enemy **95% ATK**; if Crit, +1 Amber gem convert on board (random).
- **Burst (100%) — Title Defense:** Front focus. **380% ATK** to primary Front target + **120% ATK** splash Mid; if target Broken, +30% multiplier. *CD: empty + 1 lock.*
- **Bond Skill — Lane Crush (Luce Amaranth):** Once/battle: apply **Break 25%** to all Front enemies; Kade & Luce +20% Crit Rate for 2 turns.

### Growth & collection
- **Promote mats:** Striker Edge Oil · Circuit Crest · Crimson Core Dust.  
- **Evolve frags:** T1 30 · T2 60 · T3 100.  
- **Awakening:** Solo exhibition vs holographic “past champions” under Breach lights.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Luce Amaranth | +12% ATK both; Break duration +1 turn |
| Hex Montal | Prism → Crimson convert chance +8% for Kade fills |
| Mira Solenne | Kade Aff XP +10% from gifts if Mira also Aff≥3 |
| Ora Finch | Mutual +8% Front survivability |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Autograph Only* | She keeps you backstage but pretends it’s PR. |
| **5** | *Off-Air* | A loss scare forces honesty about why she stays at the Tower. |
| **8** | *Undefeated Heart* | She cancels a stream to keep a promise to you. |

**NSFW:** `NSFW_KADE_A` (Aff8) · `NSFW_KADE_B` (Aff10)

### Room / hub
- **Jobs:** Corp Contract, Night Broadcast.  
- **Aff9 theme:** *Arena Spotlight Suite* (trophy shelf, racing-stripe couch).

### Meta notes
Carry DPS mid→late; needs Breaker partner for bosses. Medium F2P (standard SSR pity). Banner: **Standard**; mini rate-up candidate.

---

## 3. Dr. Nyra Quell

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Cortisol Quiet* / Dr. Nyra Quell |
| **Age** | 31 |
| **Rarity / Role / Faction** | SSR · Support · Tidewell Clinic |
| **Preferred lanes** | Mid, Back (primary) |
| **Gem attunement** | Verdant + Azure |
| **Height / Voice** | 176 cm · Calm alto, clinical warmth |
| **Base stars / max Lv reminder** | ★★★★ → Lv70; Awaken → Lv90 |

### Identity
Trauma specialist who Syncs by lowering squad cortisol—literally soothing the Grid’s panic noise. Past trial gone wrong makes her over-careful with consent and dosages. Soft humor under mint-white professionalism.

**Playstyle fantasy:** *Keep everyone standing; turn Verdant boards into full-party insurance.*  
**Signature prop:** Sync-cortisol scanner wand.  
**Preferred gifts:** T1 Chamomile Ampoule · T2 Hand-Thrown Ceramic Mug · T3 Calibrated Empathy Lens.  
**Disliked:** Energy drinks that “ignore sleep science.”

### Base stats (Lv1 / Lv60)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 1,050 | 9,100 |
| ATK | 195 | 1,640 |
| DEF | 100 | 820 |
| Sync Rate | 11 | 46 |
| Crit Rate | 5% | 10% |
| **Heal Power** | 15% | 35% |

### Skills
- **Passive — Steady Vitals:** Healing received by Back-lane allies +12%. On cleanse, Nyra gains **6% gauge**.
- **Basic (≥20%) — Triage Pulse:** Heal lowest ally **60% ATK × Heal Power**; small **28% ATK** Azure shield to Front for 1 turn.
- **Burst (100%) — Quiet Protocol:** All-ally heal **160% ATK × Heal Power**; cleanse 2 debuffs; grant **Regen 8% max HP / turn for 2 turns**. Back-lane heal duration +15% (lane rule). *CD: empty + 1 lock.*
- **Bond Skill — Clinic Lattice (Wren Halcyon):** Once/battle: full-squad heal **100% ATK × Heal Power** + immunity to next debuff (1 turn).

### Growth & collection
- **Promote mats:** Support Serum · Tidewell Crest · Verdant/Azure Dust.  
- **Evolve frags:** T1 30 · T2 60 · T3 100.  
- **Awakening:** Reconstruct the failed trial as a simulated multi-phase “panic storm” boss.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Wren Halcyon | Heal Power +10% both; gift craft cost −1 Heart Energy once/day |
| Ora Finch | Ora shields +12%; Nyra Aff XP +10% on Talk |
| Mira Solenne | Squad Sync fill +6% |
| Helene Voss | Cleanse also grants +5% DEF 2 turns |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Intake Form* | She runs a “voluntary” vitals check that becomes a real conversation. |
| **5** | *Trial Notes* | She shows you redacted clinic files—trust as medicine. |
| **8** | *Discharge Summary* | She admits she Syncs with you off-instruments. |

**NSFW:** `NSFW_NYRA_A` · `NSFW_NYRA_B`

### Room / hub
- **Jobs:** Café Shift (tea prep), Data Courier.  
- **Aff9 theme:** *Clinic-Calm Guest Room* (mint LEDs, soft navy throws).

### Meta notes
Premier sustain for Elite/Abyss; never rotates out of hard content. F2P via standard pity. Banner: **Standard**.

---

## 4. Sable Rin

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Whisper Key* / Sable Rin |
| **Age** | 22 |
| **Rarity / Role / Faction** | SR · Controller · Null Chorus |
| **Preferred lanes** | Mid (primary) |
| **Gem attunement** | Violet + Azure |
| **Height / Voice** | 162 cm · Soft-spoken hush, ASMR-adjacent |
| **Base stars / max Lv reminder** | ★★★ → Lv60; promote toward ★★★★★ Lv80 |

### Identity
Infiltrator who “whispers” the board into obedience; allergic to NDAs and bright conference rooms. Speaks in half-sentences, then drops perfect tactical reads. Loyalty is quiet and absolute once earned.

**Playstyle fantasy:** *Rewrite enemy gems mid-cascade and stall their Burst.*  
**Signature prop:** Whisper-key deck.  
**Preferred gifts:** T1 Black Tea Sachet · T2 Noise-Cancel Earmuffs · T3 Unsigned Anti-NDA Patch Kit.  
**Disliked:** Corporate onboarding gift baskets.

### Base stats (Lv1 / Lv60 SR-scaled ≈0.88×)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 860 | 7,400 |
| ATK | 200 | 1,720 |
| DEF | 88 | 720 |
| Sync Rate | 13 | 50 |
| Crit Rate | 5% | 11% |
| **Control Power** | 10% | 28% |

### Skills
- **Passive — Board Whisper:** Matching Violet converts 1 random enemy-attuned gem to Violet (30% chance / cascade step, max 2/turn).
- **Basic (≥20%) — Soft Delay:** Mid enemy **70% ATK** + **Delay next skill 15%** (Control Power scales delay up to ~25%).
- **Burst (100%) — Chorus Mute:** Convert **5 gems** toward Violet/Azure mix; stun Mid enemy **1 turn** (Boss: −30% ATK 1 turn instead); allies Mid +12% damage taken reduction 1 turn. *CD: empty + 1 lock.*
- **Bond Skill — Signal Braid (Zara “Hex” Montal):** Once/battle: convert a **3×3** block to Prism/Violet mix; Hex & Sable +25% gauge.

### Growth & collection
- **Promote mats:** Controller Glitch Chip · Chorus Crest · Violet Dust.  
- **Evolve frags:** T1 25 · T2 50 · T3 80 (SR curve).  
- **Awakening:** Breach server-garden stealth puzzle + anti-Corp firewall boss.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Hex Montal | Gem convert +1 extra; Aff XP +20% during Bond Festival |
| Helene Voss | Stun/delay duration +0.5 turn equivalent |
| Mira Solenne | Mira Prism matches also fill Sable 0.5× |
| Juniper Tate | Juniper taunt weight +10% while Sable Mid |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Unlogged* | She shows you a dead-drop alley where Chorus leaves jokes, not just exploits. |
| **5** | *Key Exchange* | Shared passphrase; she admits she erased a file that named you. |
| **8** | *Open Channel* | First unencrypted call—voice shaking, still soft. |

**NSFW:** `NSFW_SABLE_A` · `NSFW_SABLE_B` (SR packs smaller; still Aff8/10)

### Room / hub
- **Jobs:** Data Courier, Night Broadcast.  
- **Aff9 theme:** *Hack Loft* (cable canopy, violet static lamps).

### Meta notes
Best early Controller; scales into convert comps. Very F2P (SR rates). Banner: **Standard**.

---

## 5. Captain Ora Finch

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Tower Aegis* / Captain Ora Finch |
| **Age** | 29 |
| **Rarity / Role / Faction** | SSR · Guardian · Aurelia Corp |
| **Preferred lanes** | Front (primary) |
| **Gem attunement** | Azure + Crimson |
| **Height / Voice** | 178 cm · Firm alto, salute softening |
| **Base stars / max Lv reminder** | ★★★★ → Lv70; Awaken → Lv90 |

### Identity
By-the-book security lead assigned to surveil the Handler—then chooses your side, paperwork be damned. Humor is dry; shields are not. Torn between Aurelia duty and Tower found-family.

**Playstyle fantasy:** *Stand Front, take the hit, make the squad fearless.*  
**Signature prop:** Deployable Tower riot-shield module.  
**Preferred gifts:** T1 Black Coffee Brick · T2 Polished Boot Care Kit · T3 Engraved Tower Challenge Coin.  
**Disliked:** Novelty “world’s okayest boss” mugs.

### Base stats (Lv1 / Lv60)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 1,180 | 10,200 |
| ATK | 185 | 1,520 |
| DEF | 130 | 1,050 |
| Sync Rate | 9 | 38 |
| Crit Rate | 5% | 9% |
| **Shield Power** | 12% | 32% |

### Skills
- **Passive — Assigned Watch:** Front: damage share **12%** from Mid allies. Taunt weight high (lane rule amplified +10%).
- **Basic (≥20%) — Intercept:** Shield self **80% ATK × Shield Power**; counterchip Front enemy **40% ATK**.
- **Burst (100%) — Aegis Protocol:** Party shield **140% ATK × Shield Power** (Front +25% more); Ora taunts 2 turns; Crimson match next turn fills Ora +50%. *CD: empty + 1 lock.*
- **Bond Skill — Loyalty Seal (Mira Solenne):** Once/battle: Mira & Ora gain barrier = **20% max HP**; cleanse fear/taunt-down effects.

### Growth & collection
- **Promote mats:** Guardian Plate Fragment · Aurelia Crest · Azure Dust.  
- **Evolve frags:** T1 30 · T2 60 · T3 100.  
- **Awakening:** Defend Atelier Tower lobby vs Corp retrieval squad (multi-lane hold).

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Mira Solenne | +10% DEF; Heart Energy Talk bonus +5 Aff XP |
| Ivy Cross | Ivy damage taken −10% if Ora Front |
| Helene Voss | Conflict-flavor: +8% Control resist; story flag |
| Juniper Tate | Dual Guardian: party Front damage taken −6% |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *After-Action* | She files a false-light report to keep auditors off your floor. |
| **5** | *Conflict of Interest* | Aurelia recall order vs staying—she shows you the unsigned form. |
| **8** | *Civilian Priority* | She salutes you as partner, not asset. |

**NSFW:** `NSFW_ORA_A` · `NSFW_ORA_B`

### Room / hub
- **Jobs:** Corp Contract, Café Shift (lobby presence).  
- **Aff9 theme:** *Security Loft* (armor rack, silver-trim blinds).

### Meta notes
Best launch tank; enables glass Strikers. F2P standard. Banner: **Standard**; story spotlight Ch.2–3.

---

## 6. Luce Amaranth

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Vinyl Shred* / Luce Amaranth |
| **Age** | 25 |
| **Rarity / Role / Faction** | SR · Breaker · Crimson Circuit |
| **Preferred lanes** | Mid (primary), Front |
| **Gem attunement** | Amber + Crimson |
| **Height / Voice** | 166 cm · Bright belter, laugh in the mix |
| **Base stars / max Lv reminder** | ★★★ → Lv60+ |

### Identity
Explosive midlaner who collects vinyl and grudges in equal stacks. Worships the spotlight but idolizes Kade’s discipline more than she’ll say. Chaos with a metronome.

**Playstyle fantasy:** *Shred armor, paint the board Amber, set up the finisher.*  
**Signature prop:** Vinyl disc chakram.  
**Preferred gifts:** T1 Instant Ramen Brick · T2 Rare Pressing Sleeve · T3 Custom Shredder Pick Set.  
**Disliked:** Classical compilation CDs “for focus.”

### Base stats (Lv1 / Lv60 SR)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 880 | 7,550 |
| ATK | 230 | 1,980 |
| DEF | 82 | 660 |
| Sync Rate | 11 | 44 |
| Crit Rate | 6% | 14% |
| **Break Power** | 14% | 36% |

### Skills
- **Passive — Groove Crack:** Amber matches grant +8% Break Power for 2 turns (refresh). Broken targets take +10% damage from Crimson Circuit allies.
- **Basic (≥20%) — Needle Drop:** Mid enemy **85% ATK** + **Break 12%** (× Break Power scaling → ~18% at build).
- **Burst (100%) — Full Spin Shred:** Mid/Front: **Break 35%** to primary + **Break 15%** splash; damage **260% ATK**; spawn 3 Amber gems. *CD: empty + 1 lock.*
- **Bond Skill — Lane Crush (Kade Virell):** See Kade—shared once/battle Break + Crit window.

### Growth & collection
- **Promote mats:** Breaker Solvent · Circuit Crest · Amber Dust.  
- **Evolve frags:** T1 25 · T2 50 · T3 80.  
- **Awakening:** Club-floor Breach where speakers become armor nodes.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Kade Virell | +12% ATK; Break + Crit package |
| Hex Montal | Amber/Prism cascade Aff XP event bonus |
| Ivy Cross | Rookie copy: Ivy Break on Basic +5% |
| Aveline Roux | Independent Breakers: Break Power +8% |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *B-Side* | She plays you a track she never released after a bad bout. |
| **5** | *Double Bill* | Joint Circuit charity show; she credits you on mic. |
| **8** | *Pressed Heart* | Limited vinyl with a private etching—not for sale. |

**NSFW:** `NSFW_LUCE_A` · `NSFW_LUCE_B`

### Room / hub
- **Jobs:** Night Broadcast, Corp Contract.  
- **Aff9 theme:** *Vinyl Circuit Den* (wall pressings, orange neon).

### Meta notes
Essential Breaker until Aveline; pairs with Kade forever. F2P SR. Banner: **Standard**.

---

## 7. Wren Halcyon

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Night Shift* / Wren Halcyon |
| **Age** | 26 |
| **Rarity / Role / Faction** | SR · Support · Tidewell |
| **Preferred lanes** | Back (primary), Mid |
| **Gem attunement** | Verdant |
| **Height / Voice** | 164 cm · Gentle soprano, sleepy humor |
| **Base stars / max Lv reminder** | ★★★ → Lv60+ |

### Identity
Night-shift nurse who moonlights as Room caretaker and the Tower’s best T1 gift crafter. Remembers everyone’s tea order and Aff thresholds. Soft spine, steel when patients code.

**Playstyle fantasy:** *Single-color Verdant machine—heals, crafts, comforts.*  
**Signature prop:** Apothecary gift kit.  
**Preferred gifts:** T1 Herbal Sachet (meta: she *makes* these) · T2 Knitted Cardigan Sample · T3 Named Night-Shift Lantern.  
**Disliked:** Loud party horns / sleep-disrupt gadgets.

### Base stats (Lv1 / Lv60 SR)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 940 | 8,100 |
| ATK | 175 | 1,480 |
| DEF | 92 | 760 |
| Sync Rate | 12 | 47 |
| Crit Rate | 5% | 9% |
| **Heal Power** | 12% | 30% |

### Skills
- **Passive — Caretaker’s Reserve:** Verdant matches heal Wren’s lowest ally for **3% max HP** (max 2/turn). Shop T1 gift craft time −20% when Wren assigned to Café Job.
- **Basic (≥20%) — Sip & Suture:** Heal **55% ATK × Heal Power**; grant **Heartline Regen** 4% HP for 1 turn.
- **Burst (100%) — Ward Round:** All-ally heal **130% ATK × Heal Power**; cleanse 1; Back allies +1 turn buff duration. *CD: empty + 1 lock.*
- **Bond Skill — Clinic Lattice (Nyra Quell):** Shared big heal + debuff immunity (see Nyra).

### Growth & collection
- **Promote mats:** Support Serum · Tidewell Crest · Verdant Dust.  
- **Evolve frags:** T1 25 · T2 50 · T3 80.  
- **Awakening:** Overnight clinic Breach—save NPCs while meter manages sleep debt.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Nyra Quell | Heal lattice; dual Support overheal → small shields |
| Mira Solenne | Room gift Aff XP +10% account-wide when Wren crafts |
| Juniper Tate | Juniper HP +8% |
| Ivy Cross | Post-KO revive chance tutorial flag (once/battle 20% HP)—story-limited tuning |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Chart Notes* | She leaves anonymous care packages labeled with your handwriting practice. |
| **5** | *Shift Change* | You cover “lobby vitals” so she can sleep; she cooks breakfast badly on purpose. |
| **8** | *Off-Duty* | She clocks out of Clinic identity and into Tower partner. |

**NSFW:** `NSFW_WREN_A` · `NSFW_WREN_B`

### Room / hub
- **Jobs:** Café Shift (**best**), Night Broadcast.  
- **Aff9 theme:** *Apothecary Nook* (herb wall, rose-mint textiles).

### Meta notes
Best F2P healer; gift economy MVP. Banner: **Standard**; often free-event SR.

---

## 8. Ivy Cross

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Rookie Edge* / Ivy Cross |
| **Age** | 23 |
| **Rarity / Role / Faction** | R · Striker · Independent |
| **Preferred lanes** | Front, Mid |
| **Gem attunement** | Crimson |
| **Height / Voice** | 170 cm · Eager bright mezzo |
| **Base stars / max Lv reminder** | ★★ → Lv50; invest to ★★★★ for mid-game |

### Identity
Handler-washout turned agent—chip on shoulder, heart on sleeve. Growth character who mirrors the player’s early climb. Collects training scars and Mira’s advice equally.

**Playstyle fantasy:** *Simple Crimson beatstick that grows into a real Front threat.*  
**Signature prop:** Practice sync baton.  
**Preferred gifts:** T1 Protein Bar · T2 Training Log Notebook · T3 First-Clear Tower Pin Replica.  
**Disliked:** “Participation” stickers.

### Base stats (Lv1 / Lv60 R-scaled ≈0.72×)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 780 | 6,200 |
| ATK | 195 | 1,620 |
| DEF | 70 | 520 |
| Sync Rate | 10 | 40 |
| Crit Rate | 6% | 16% |
| **Growth ATK** (per promote star) | — | +3% passive |

### Skills
- **Passive — Washout’s Grit:** Each promote star: +3% ATK. Below 50% HP: +10% Crit Rate.
- **Basic (≥20%) — Ready Strike:** Front/Mid **90% ATK**.
- **Burst (100%) — Prove It:** Front **220% ATK**; if Mira or Ora also in squad, +25% multiplier. *CD: empty + 1 lock.*
- **Bond Skill — Mentorship Glow (Mira *or* Ora):** Once/battle: Ivy gains **30% gauge** + **+20% ATK 2 turns**; mentor gains 10% gauge.

### Growth & collection
- **Promote mats:** Striker Edge Oil · Independent Crest · Crimson Dust (cheap nodes).  
- **Evolve frags:** T1 20 · T2 40 · T3 60 (R curve).  
- **Awakening:** Rematch the exam she failed—Handler simulation boss.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Mira Solenne | Mentorship; Aff XP +15% |
| Ora Finch | Damage taken −10% Front |
| Juniper Tate | Tower siblings: +6% ATK both |
| Luce Amaranth | Break on Ivy Crit 5% |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Remedial Sync* | Extra drills on the rooftop; she stops apologizing for existing. |
| **5** | *Field Jacket* | You mend her mismatched pads; she keeps the crooked stitch. |
| **8** | *Name on the Board* | First elite clear with her as MVP—she frames the ★ ticket. |

**NSFW:** `NSFW_IVY_A` · `NSFW_IVY_B`

### Room / hub
- **Jobs:** Data Courier, Corp Contract.  
- **Aff9 theme:** *Rookie Barracks Corner* (tape marks, training dummies mini).

### Meta notes
**Best early F2P DPS**; falls off late raw but Bond keeps her usable. Banner: **Standard** + weekly fragment shop.

---

## 9. Professor Helene Voss

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Theory Architect* / Professor Helene Voss |
| **Age** | 34 |
| **Rarity / Role / Faction** | SSR · Controller · Aurelia (ex) |
| **Preferred lanes** | Mid (primary), Back |
| **Gem attunement** | Violet + Prism |
| **Height / Voice** | 174 cm · Smoky academic contralto |
| **Base stars / max Lv reminder** | ★★★★ → Lv70; Awaken → Lv90 |

### Identity
Architect of early Heartline theory; left Aurelia when empathy became a product SKU. Cryptic mentor romance—answers questions with better questions. Knows why the Grid dreams.

**Playstyle fantasy:** *Prism intellect—convert, delay, reveal boss patterns.*  
**Signature prop:** Heartline theory slate.  
**Preferred gifts:** T1 Chalk Tin · T2 First-Edition Empathy Math · T3 Unredacted Grid Folio Page.  
**Disliked:** Motivational poster printouts.

### Base stats (Lv1 / Lv60)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 900 | 7,900 |
| ATK | 215 | 1,880 |
| DEF | 90 | 740 |
| Sync Rate | 12 | 49 |
| Crit Rate | 5% | 12% |
| **Control Power** | 14% | 34% |

### Skills
- **Passive — Origin Lemma:** Prism matches fill Helene at **1.5×** and grant squad +3% Sync fill. Enemy skill telegraphs last **+0.3s** while she’s alive (UI assist fantasy).
- **Basic (≥20%) — Chalk Delay:** Mid **75% ATK** + delay 20%; convert 1 gem to Prism (40%).
- **Burst (100%) — Empathic Proof:** Convert up to **8 gems** to Violet/Prism; inflict **Thought Bind** (cannot Burst 1 turn / Bosses: gauge lock 30%); deal **200% ATK** to all Mid. *CD: empty + 1 lock.*
- **Bond Skill — Origin Thread (Mira Solenne):** Once/battle: Mira & Helene dual Burst window—both gain **50% gauge**; next Prism clear grants full-squad +12% damage 1 turn.

### Growth & collection
- **Promote mats:** Controller Glitch Chip · Ex-Aurelia Crest · Violet/Prism Dust.  
- **Evolve frags:** T1 30 · T2 60 · T3 100.  
- **Awakening:** Lecture-hall Breach where equations become enemies—solve or shatter.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Mira Solenne | Origin thread; Sync +8% squad |
| Sable Rin | Convert count +1 |
| Ora Finch | Story tension; Control resist +8% |
| Hex Montal | Prism cascade score event bonus |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Office Hours* | She assigns you “homework” that is really a trust test. |
| **5** | *Unpublished* | Grid origin confession—soft SPOILER flag for Ch.5. |
| **8** | *Coauthor* | She adds your name to a sealed theory addendum. |

**NSFW:** `NSFW_HELENE_A` · `NSFW_HELENE_B`

### Room / hub
- **Jobs:** Data Courier, Night Broadcast (lectures).  
- **Aff9 theme:** *Ex-Corp Study* (slate wall, gold-violet lamps).

### Meta notes
Late-game control goddess; story-critical SSR. Medium pull priority. Banner: **Standard**; strong story rate-up Ch.4–5.

---

## 10. Zara “Hex” Montal

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Signal Witch* / Zara “Hex” Montal |
| **Age** | 28 |
| **Rarity / Role / Faction** | SR · Synergist · Null Chorus |
| **Preferred lanes** | Mid, Back |
| **Gem attunement** | Prism + Violet |
| **Height / Voice** | 171 cm · Playful tenor-lean mezzo, DJ cadence |
| **Base stars / max Lv reminder** | ★★★ → Lv60+ |

### Identity
DJ / signal witch; hub dance emotes queen and Bond Festival mascot. Turns Sync into bass drops. Flirts at festival volume, cares at whisper volume.

**Playstyle fantasy:** *Prism battery—feed everyone’s gauges and remix bad boards.*  
**Signature prop:** Hex-pad controller.  
**Preferred gifts:** T1 Glowstick Pack · T2 Underground Set USB · T3 Festival Headliner Pass.  
**Disliked:** Corporate onboarding webinars-on-disc.

### Base stats (Lv1 / Lv60 SR)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 900 | 7,700 |
| ATK | 205 | 1,760 |
| DEF | 85 | 700 |
| Sync Rate | 14 | 52 |
| Crit Rate | 5% | 11% |
| **Gauge Share Power** | 10% | 22% |

### Skills
- **Passive — Drop Incoming:** Prism matches fill all allies at **0.35×** Hex’s Sync Rate. Dance emote in Hub after victory (cosmetic).
- **Basic (≥20%) — Tease Wave:** Ally with highest missing gauge +**15% gauge**; tiny **40% ATK** Violet poke Mid.
- **Burst (100%) — Hex Drop:** Board remix—reroll 6 gems biased to squad attunements; all allies +**18% gauge**; Mid enemies −12% DEF 2 turns. *CD: empty + 1 lock.*
- **Bond Skill — Signal Braid (Sable Rin):** Shared convert burst (see Sable).

### Growth & collection
- **Promote mats:** Synergist Thread · Chorus Crest · Prism Dust.  
- **Evolve frags:** T1 25 · T2 50 · T3 80.  
- **Awakening:** Rooftop festival Breach—keep the beat meter alive vs silence boss.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Sable Rin | Signal braid; convert + festival Aff |
| Kade Virell | Spotlight set: Kade gauge +8% on Hex Burst |
| Mira Solenne | Dual Synergist: Bond Skill cap exception flag (design note: still max 2 Bonds) |
| Luce Amaranth | Amber/Prism party comps |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Soundcheck* | Private set on the Tower roof; no crowd, on purpose. |
| **5** | *Guest List* | She adds you as co-headliner on a Chorus flyer. |
| **8** | *Afterparty Quiet* | Headphones shared—one track, no remix. |

**NSFW:** `NSFW_HEX_A` · `NSFW_HEX_B`

### Room / hub
- **Jobs:** Night Broadcast (**best**), Café Shift.  
- **Aff9 theme:** *Festival Loft* (LED sleeves display, hex-pad coffee table).

### Meta notes
Event queen; Prism engine for mid/late. F2P SR. Banner: **Standard** + Bond Festival rate-up.

---

## 11. Juniper Tate

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Door Guard* / Juniper Tate |
| **Age** | 21 |
| **Rarity / Role / Faction** | R · Guardian · Independent |
| **Preferred lanes** | Front (primary) |
| **Gem attunement** | Azure |
| **Height / Voice** | 160 cm · Earnest bright alto (**adult 21+**; never child-coded) |
| **Base stars / max Lv reminder** | ★★ → Lv50; promote for tankiness |

### Identity
Youngest roster member at **21**—earnest Atelier lobby guard, comic relief with real steel when Breaches hit the door. Takes the keycard lance too seriously; learns when to laugh.

**Playstyle fantasy:** *Budget Ora—taunt, shield, buy time for carries.*  
**Signature prop:** Atelier lobby keycard lance.  
**Preferred gifts:** T1 Instant Cocoa · T2 Comedy Radio Badge · T3 Official Tower Guard Cap (fitted adult size).  
**Disliked:** “Kid’s meal” novelty toys (Aff penalty + VO correction that she’s 21).

### Base stats (Lv1 / Lv60 R)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 1,000 | 8,000 |
| ATK | 140 | 1,150 |
| DEF | 110 | 880 |
| Sync Rate | 8 | 34 |
| Crit Rate | 4% | 8% |
| **Shield Power** | 8% | 22% |

### Skills
- **Passive — Lobby Protocol:** Front taunt weight +15%. First time per battle she drops below 40% HP: auto-shield **10% max HP**.
- **Basic (≥20%) — Keycard Jab:** **60% ATK** + self-shield **50% ATK × Shield Power**.
- **Burst (100%) — Hold the Door:** Party shield **100% ATK × Shield Power**; Juniper taunts 2 turns; Azure matches next turn heal her 5% max HP. *CD: empty + 1 lock.*
- **Bond Skill — Tower Family Shield (Mira *or* Ivy):** Once/battle: Front damage taken −20% for 2 turns; Mira/Ivy +10% gauge.

### Growth & collection
- **Promote mats:** Guardian Plate · Independent Crest · Azure Dust.  
- **Evolve frags:** T1 20 · T2 40 · T3 60.  
- **Awakening:** Overnight lobby siege—comic setup, serious final phase.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Mira Solenne | Family shield; Aff XP +10% |
| Ivy Cross | Sibling ATK/DEF +6% |
| Ora Finch | Dual Guard lesson: Shield Power +8% |
| Wren Halcyon | HP +8% |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Shift Brief* | She practices professional greetings; you coach the smile. |
| **5** | *Not a Kid* | Explicit affirmation of her adult agency after a patronizing NPC—tone clear, 21+. |
| **8** | *Keys to the Tower* | She cuts you a spare keycard—symbolic co-ownership. |

**NSFW:** `NSFW_JUNIPER_A` · `NSFW_JUNIPER_B` (adult 21+ packs; same Aff gates)

### Room / hub
- **Jobs:** Café Shift (lobby), Corp Contract.  
- **Aff9 theme:** *Guard Nook* (radio shelf, denim/silver decor).

### Meta notes
**Best early F2P tank**; Ora replaces her later, Bond keeps her for Tower family comps. Banner: **Standard** + login rewards candidate.

---

## 12. Aveline Roux

### Header

| Field | Value |
|-------|-------|
| **Codename / Real** | *Mythic Cleaver* / Aveline Roux |
| **Age** | 30 |
| **Rarity / Role / Faction** | UR · Breaker · Independent |
| **Preferred lanes** | Front (primary), Mid |
| **Gem attunement** | Amber + Prism |
| **Height / Voice** | 177 cm · Quiet low alto; kitchen calm → field steel |
| **Base stars / max Lv reminder** | ★★★★★ → Lv80; Awaken → Lv90 |

### Identity
Freelancer rumor made flesh: calm chef in the Room, devastation on the Field. Speaks sparingly; cooks generously; Breaks armor like bones for broth. Limited debut—mythic Independent who chose your Tower without a contract.

**Playstyle fantasy:** *UR Break god—Prism/Amber boards become boss funerals.*  
**Signature prop:** Seasoned cleaver-breaker (Room: SFW kitchen knife).  
**Preferred gifts:** T1 Fresh Bay Leaves · T2 Cast-Iron Care Oil · T3 Reserved Chef’s Table Ticket (Quay).  
**Disliked:** Meal kits with “mystery meat” branding.

### Base stats (Lv1 / Lv60 UR-scaled ≈1.12×)

| Stat | Lv1 | Lv60 |
|------|-----|------|
| HP | 1,020 | 8,900 |
| ATK | 280 | 2,480 |
| DEF | 95 | 780 |
| Sync Rate | 11 | 45 |
| Crit Rate | 7% | 16% |
| **Break Power** | 18% | 42% |

### Skills
- **Passive — Mise en Place:** When enemy Armor >50%, Aveline Break Power +15%. Prism matches grant **Break stacks** (max 3): each stack = +5% armor ignore on her Burst.
- **Basic (≥20%) — Prep Cut:** **100% ATK** + **Break 15%** (scales with Break Power).
- **Burst (100%) — Service Devastation:** Front primary **480% ATK** with armor ignore from stacks; splash Mid **160% ATK**; apply **Break 40%** to primary. If 3 stacks, cleanse own debuffs. *CD: empty + 1 lock.*
- **Bond Skill — Mythic Lease (Mira Solenne):** Once/battle: Aveline gains 3 Break stacks instantly; Mira shares **40% gauge** to Aveline; both +15% ATK 2 turns.

### Growth & collection
- **Promote mats:** Breaker Solvent · Independent Crest · Amber/Prism Cores (UR-tagged).  
- **Evolve frags:** T1 40 · T2 80 · T3 120 (UR curve; dupes = 80 frags).  
- **Awakening:** “Ghost kitchen” Breach—courses as phases; final plating is the boss kill.

### Bond Matrix
| Partner | Bonus |
|---------|-------|
| Mira Solenne | Mythic lease; Prism→Break feed |
| Luce Amaranth | Dual Breaker: Break Power +8% |
| Nyra Quell | Sustain for Front UR glass-adjacent |
| Kade Virell | After Break: Kade Burst +12% one shot |

### Affection story beats
| Aff | Episode | SFW summary |
|-----|---------|-------------|
| **2** | *Staff Meal* | She feeds the whole Tower; you do dishes; myth softens. |
| **5** | *Reservation for Two* | Quiet Quay dinner; she explains why freelancers vanish. |
| **8** | *House Knife* | She leaves her Room cleaver’s twin in your Suite—trust, not threat. |

**NSFW:** `NSFW_AVELINE_A` · `NSFW_AVELINE_B`

### Room / hub
- **Jobs:** Café Shift (**chef bonus**), Corp Contract.  
- **Aff9 furniture theme:** *Freelancer Kitchen* (ember-prism pendants, cast-iron rack).

### Meta notes
**Best late Breaker / boss delete**; defines limited banner power. Low F2P access (120 pity / spark). Banner: **Limited debut only** (not in permanent pool).

---

# Appendix A — Promote Material Tag Index

| Tag | Used by |
|-----|---------|
| Synergist Thread | Mira, Hex |
| Striker Edge Oil | Kade, Ivy |
| Support Serum | Nyra, Wren |
| Controller Glitch Chip | Sable, Helene |
| Guardian Plate Fragment | Ora, Juniper |
| Breaker Solvent | Luce, Aveline |
| Faction Crests | Per faction (Independent / Circuit / Tidewell / Aurelia / Chorus) |
| Gem Core Dust | Attunement colors |
| UR-tagged Cores | Aveline only at launch |

---

# Appendix B — NSFW Pack ID Registry (launch)

| Agent | Aff8 | Aff10 |
|-------|------|-------|
| Mira Solenne | NSFW_MIRA_A | NSFW_MIRA_B |
| Kade Virell | NSFW_KADE_A | NSFW_KADE_B |
| Dr. Nyra Quell | NSFW_NYRA_A | NSFW_NYRA_B |
| Sable Rin | NSFW_SABLE_A | NSFW_SABLE_B |
| Captain Ora Finch | NSFW_ORA_A | NSFW_ORA_B |
| Luce Amaranth | NSFW_LUCE_A | NSFW_LUCE_B |
| Wren Halcyon | NSFW_WREN_A | NSFW_WREN_B |
| Ivy Cross | NSFW_IVY_A | NSFW_IVY_B |
| Professor Helene Voss | NSFW_HELENE_A | NSFW_HELENE_B |
| Zara “Hex” Montal | NSFW_HEX_A | NSFW_HEX_B |
| Juniper Tate (21+) | NSFW_JUNIPER_A | NSFW_JUNIPER_B |
| Aveline Roux | NSFW_AVELINE_A | NSFW_AVELINE_B |

Unlock rules: Adult Scenes master ON + age gate + Aff threshold + per-scene confirm. VIP never required. No erotic prose in this document.

---

# Appendix C — Banner Placement Summary

| Placement | Agents |
|-----------|--------|
| **Beginner Banner** | Mira featured weight; SR pool includes Wren/Sable/Luce/Hex; R Ivy/Juniper |
| **Standard Pulse Pool** | All launch N–SSR (Mira, Kade, Nyra, Sable, Ora, Luce, Wren, Ivy, Helene, Hex, Juniper) |
| **Limited Debut** | **Aveline Roux (UR)** only — 21-day launch banner |

---

*— End of Launch Roster Kits —*  
*All characters 18+. Continuity: GDD §15 + Art Bible §A.6. Sync Match RPG mechanics per GDD §5.*
