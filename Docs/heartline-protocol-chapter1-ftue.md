# HEARTLINE PROTOCOL — Chapter 1 + FTUE Script

**Document type:** Full Day-0 / Chapter 1 production script  
**Version:** 1.0 · **Date:** 2026-09-19 · **Status:** Build-ready  
**Sources:** GDD · Art Bible · Roster Kits · Vertical Slice  

**Content policy:** All speakers are adults **18+** (Mira 24, Ivy 23, Juniper 21, Wren 26). **NSFW: none in Chapter 1.** System footnote only: Aff ranks 8+ may unlock optional adult scenes in future builds when Adult Content is enabled — not present here. No erotic prose.

**Format key:** `Speaker | Line | [VO note]`  
Player Handler is addressed as “you”; dialogue may use `{Handler}` token for typed name (default “Handler”).

---

## 0. System Footnote (UI help string — not voiced)

> Affection currently unlocks through Rank 3 in this build (Talk, Gifts, Dates). Ranks 8 and 10 are reserved for optional adult scene packs in later builds, only if you pass the age gate and enable Adult Content. Main story never requires them.

---

## 1. Day-0 Tutorial Beats (minute-by-minute)

| Time | Beat ID | What happens | Skippable tip? |
|------|---------|--------------|----------------|
| 0:00 | `boot` | Splash → Age Gate 18+ | No |
| 0:30 | `title_new` | New Game → name entry optional | — |
| 1:00 | `pro_01` | Rain on Lumina Quay; lease letter | — |
| 1:45 | `pro_02` | Mira in lobby — meet-cute partner tone | — |
| 2:30 | `tip_hub` | Hub icons highlight: Stages / Room / Team | Yes after 1st |
| 3:00 | `stage_1_1` | Tutorial fight; forced Mira Burst | Coach overlays |
| 4:30 | `rec_ivy_jun` | Ivy + Juniper join | — |
| 5:00 | `tip_team` | Lane assign Front/Mid/Back | Yes |
| 5:30–11:00 | `1_2`…`1_5` | Story stages + Wren unlock after 1-5 | Per-tip |
| 11:00 | `room_unlock` | Room tutorial with Mira | — |
| 12:00–17:00 | `1_6`…`1_9` | Combat depth + Prism + telegraphs | Yes |
| 17:00 | `aff_gift` | Gift tutorial if Aff≥2 else Talk push | Yes |
| 18:00–21:00 | `1_10` boss | Lease Reclaimer | — |
| 21:00 | `ch1_clear` | Chapter clear fanfare + mail | — |
| 22:00 | `banner_soft` | Beginner Banner intro (no hard sell) | Yes dismiss |
| 23:00 | `job_cafe` | Assign Café Shift | Yes |
| 24:00 | `session_end` | Soft logout tease / sleepover assign | — |

**Branch notes:** If player skips tips, coach text still available from pause → Help. If Heart Energy <5 at Room unlock, grant +20 once (`ftue_he_topup`). If Stamina blocks before boss, grant Stamina Pot ×2 via mail once.

---

## 2. Dialogue Scripts

### 2.1 Age Gate (UI)

| Speaker | Line | VO |
|---------|------|----|
| System | Heartline Protocol is intended for adults. You must be 18 or older to play. | — |
| System | I confirm I am 18 years of age or older. [Confirm] / [Exit] | — |

### 2.2 Prologue — Quay Arrival (`pro_01`)

| Speaker | Line | VO |
|---------|------|----|
| Narration | Lumina Quay wears rain like jewelry. Neon leases flicker over wet glass. | Soft rain bed |
| Narration | The envelope in your hand is heavy: *Atelier Tower — Handler Lease Transfer.* | — |
| Narration | Corporate Heartline Corps call people like you freelancers. The Grid calls you a sync point. | — |
| Narration | Tonight, the Tower calls you home. | — |

### 2.3 Prologue — Mira Meet (`pro_02`)

| Speaker | Line | VO |
|---------|------|----|
| Mira | You’re early. Or the Breach is early. Hard to tell with Quay weather. | Warm mezzo, wry |
| Mira | Mira Solenne. Independent agent. Temporary — well, *formerly* temporary — lease liaison for this building. | — |
| Mira | Congrats, {Handler}. The Tower’s yours on paper. The Breaches don’t care about paper. | Soft command |
| Mira | I’m staying. Someone has to keep the Grid from eating the lobby. And… someone should make sure you eat. | Smile in voice |
| Mira | First Sync is downstairs. I’ll walk you through it. Try not to look impressed when I’m brilliant. | Tease |

### 2.4 Hub Tip (`tip_hub`)

| Speaker | Line | VO |
|---------|------|----|
| Mira | Stages burn **Stamina**. The Room burns **Heart Energy** — that’s for talking, not punching storms. | Tutorial calm |
| Mira | Team desk is where we pretend we have a plan. Jobs pay coins while we sleep. Banner’s… later. Softly. | — |
| System | Tip: Tap **Sectors** to deploy. Tap **Room** when you want conversation over combat. | — |

### 2.5 Pre-`1-1` (`dlg_1_1_pre`)

| Speaker | Line | VO |
|---------|------|----|
| Mira | Breach static in the lobby. Small one. Perfect for a first Sync. | — |
| Mira | Match gems that match me — Verdant, or Prism if you’re lucky. Fill my gauge. When it hits full, fire my Burst. | Coach |
| Mira | I’ll do the heroic part. You do the clever part. | — |

### 2.6 Post-`1-1` + Recruit (`rec_ivy_jun`)

| Speaker | Line | VO |
|---------|------|----|
| Ivy | Handler! I mean — {Handler}! Ivy Cross. Independent. Mostly. I washed out of Handler track but I can still hit things. | Eager bright |
| Juniper | Juniper Tate. Door guard. Age twenty-one, licensed, insured, and very done with people calling me “kid.” | Earnest, firm on age |
| Juniper | The lobby is secure. Mostly. Please don’t unlock the back stair without telling me. | — |
| Mira | They’re ours. Front line needs a Striker and a Guardian. Ivy, Juniper — lanes. {Handler} — trust them. | — |
| Ivy | I won’t drop the beat. Or the shield. Or — Juniper has the shield. I’ll drop the enemies. | Nervous laugh |
| Juniper | Correct. | Deadpan |

### 2.7 Team / Lane Tip (`tip_team`)

| Speaker | Line | VO |
|---------|------|----|
| System | Assign up to 2 agents per lane: **Front** takes more hits, hits harder physically. **Back** safer for heals. **Mid** balanced. | — |
| Mira | Put Juniper Front. Ivy Front or Mid. I’m Mid — I share the beat. | — |

### 2.8 Between stages flavor (short)

**After `1-2`:**  
| Mira | Cascades are the Grid saying yes. Chase them, but don’t tunnel so hard you miss a Burst window. | |

**After `1-3`:**  
| Juniper | Front is loud. That’s fine. Loud means they’re hitting me, not you. | |

**After `1-4`:**  
| Ivy | Back-lane snipers are rude. Burst feels good when rude things fall down. | |

### 2.9 Wren Join — after `1-5` (`dlg_wren_join`)

| Speaker | Line | VO |
|---------|------|----|
| Wren | Vitals look… dramatic. I’m Wren Halcyon — Tidewell night shift, Tower moonlighter. Who’s bleeding? | Gentle soprano, sleepy humor |
| Mira | Nobody yet. That’s the goal. | — |
| Wren | Then I’m early. Perfect. I’ll take Back lane and the kettle. Also: I craft tea gifts if you ever need Affinity that isn’t awkward silence. | Soft smile |
| System | **Wren Halcyon** joined the squad. Role: Support. Attunement: Verdant. | — |
| Wren | Drink water between Breaches. I’m contractually obligated to say that. | |

### 2.10 Room Tutorial with Mira (`room_tutorial`)

*Trigger: first Hub return after Wren join (or Aff UI forced if player ignores Room until after `1-6`).*

| Speaker | Line | VO |
|---------|------|----|
| Mira | Door’s unlocked. That’s rare. Come in. | Soft |
| Mira | This is the Room. Not a briefing. Not a Corp eval. Just… us, and the window, and Quay pretending to sleep. | — |
| System | **Talk** (5 Heart Energy): +20 Affection XP. Dialogue changes with Aff Rank. | — |
| Mira | Talk first. Gifts later — I like Sunwell Tea, for the record. Not Corp pens. Never Corp pens. | Preferred gift tease |
| Mira | When we get Close — Aff Rank 3 — I’ll let you drag me on a Date. City outing. Soft shoes. No Breach optional. | Warm |
| System | Sleepover: assign Mira overnight for +30 Aff XP in the morning. Costs no Heart Energy. | — |

**Talk branch Aff1 (`talk_mira_aff1`):**

| Speaker | Line | VO |
|---------|------|----|
| Mira | I fled Aurelia with blueprints and a conscience that still files reports at 3 a.m. | Quiet |
| Mira | Staying here isn’t charity. It’s the first lease that felt like a choice. | — |
| Mira | Sync rates spike when you listen. Annoying scientific fact. | Tease |
| System | Affection XP +20. | — |

**Talk branch Aff2 (`talk_mira_aff2`) — after rank-up:**

| Speaker | Line | VO |
|---------|------|----|
| Mira | Friendly is a soft word for “I’d cover your shift without billing you.” | — |
| Mira | Don’t tell Ora I said that. She’s still deciding if we’re a security incident. | Smile |
| Mira | …Thanks for not making the Tower feel like another Corp floor. | Soft sincere |

**Gift success preferred (`gift_mira_tea`):**

| Speaker | Line | VO |
|---------|------|----|
| Mira | Sunwell Tea. You listened. Dangerous habit. | Liked VO |
| System | Preferred gift! Aff XP ×1.5. | — |

**Date Aff3 (`date_mira_01`) — unlocks at Aff3:**

| Speaker | Line | VO |
|---------|------|----|
| Mira | Pier lights. No badges. If a Breach opens, we run *toward* coffee first. Deal? | Date start |
| Mira | I used to think Sync was a product SKU. Then you matched Verdant like you meant it. | — |
| Mira | Rank Three. Close. Don’t panic — I’m not drafting a marriage lease. Just… a longer conversation. | Warm laugh |
| Mira | Tomorrow’s Breaches can wait ten more minutes. Sit. | Date end |
| System | Aff XP +100. Date available 1/day/agent. | — |

**Aff rank-up toasts:**

| Rank | System line |
|------|-------------|
| 2 | Aff Rank Up — **Friendly** with Mira. Gifts unlocked. |
| 3 | Aff Rank Up — **Close** with Mira. Dates unlocked. Bond Matrix slot teased. |

### 2.11 Pre-Boss (`dlg_1_10_pre`)

| Speaker | Line | VO |
|---------|------|----|
| Juniper | Lease Reclaimer construct on sensors. Corp-shaped. Rude-shaped. | |
| Ivy | We clear this, Chapter One’s ours. I’m not washing out again. | |
| Wren | Shields up, egos measured, tea after. | |
| Mira | {Handler} — full squad. Prism if it shows. Burst windows matter. I’m with you. | Steady |

### 2.12 Chapter Clear (`ch1_clear`)

| Speaker | Line | VO |
|---------|------|----|
| Mira | Lobby’s quiet. That’s us. That’s *you*. | Pride |
| Narration | Chapter 1 complete. Atelier Tower holds — for tonight. | |
| Mira | There’s more Grid under this city than one chapter can hold. But the lease feels real now. | |
| Mail | Subject: Welcome, Handler — Luma Coins and a beginner’s courtesy. — Tower Systems | |

### 2.13 Beginner Banner Soft Intro (`banner_soft`)

**Tone: informational, no urgency timers, no “LAST CHANCE”, no hard sell.**

| Speaker | Line | VO |
|---------|------|----|
| Mira | That’s the Pulse Banner desk. Beginner track. Discounted. Guarantees on the wall — we publish pity because lying is a Corp sport. | Calm |
| System | Beginner Banner: 20% off display, guaranteed SR by 10 pulls, SSR by 30. Max 40 pulls. Rates visible in UI. | — |
| Mira | You already have me. Pulling me again becomes fragments — still useful. Or don’t pull tonight. The Tower doesn’t charge rent in tickets. | Anti-pressure |
| Mira | If you try a free sample ten-pull, celebrate whatever arrives. If it’s quiet, we train Ivy harder. Either way, dinner exists. | Soft smile |
| System | [Try Free 10-Pull] [Not Now] — Not Now has no penalty. | — |

### 2.14 Job Intro (`job_cafe`)

| Speaker | Line | VO |
|---------|------|----|
| Wren | Café Shift, two hours. Coins. I can work it if you prefer Mira broadcasting conspiracy at midnight instead. | |
| System | Assign an agent to a Job. Claim rewards when the timer ends. | — |
| Mira | Overnight, leave someone in the Room if you want morning Affection. Consent-based. Even sleepovers ask. | |

### 2.15 Session End Tease

| Speaker | Line | VO |
|---------|------|----|
| Mira | Stamina regenerates. Heart Energy too. I’m not going anywhere. | Soft |
| Juniper | Door’s locked. From the outside. On purpose. | |
| System | Thanks for playing the Heartline Protocol vertical slice prototype. | — |

---

## 3. Stage List — Chapter 1 (teaching goals)

| ID | Name | Stamina | Enemy tags | Teaching goal | Dialogue hooks |
|----|------|---------|------------|---------------|----------------|
| `1-1` | Lease Breach — Lobby | 0 | `swarm`, `soft` | Match ≥3, gauge fill, **force Mira Burst** | `dlg_1_1_pre` |
| `1-2` | Rain Plaza Static | 6 | `swarm` | Cascades + combo ×1.2+ | flavor Mira |
| `1-3` | Metro Flood Gate | 6 | `armored_light`, `front_bias` | Lane: Juniper Front soaks | Juniper line |
| `1-4` | Signage Canyon | 6 | `ranged`, `back` | Priority Back threats; Ivy Burst | Ivy line |
| `1-5` | Clinic Shuttle Dock | 6 | `debuff`, `dot` | Heal pressure → **Wren unlock** | `dlg_wren_join` |
| `1-6` | Quay Bridge Mid | 6 | `mixed` | Full 4; attunement colors matter | tip_colors |
| `1-7` | Corp Kiosk Ambush | 10 | `armor`, `elite_lite` | Sustained Burst DPS race | Mira warn |
| `1-8` | Rooftop Garden Sparks | 6 | `swarm`, `prism_spawn` | **Prism wild** intro | tip_prism |
| `1-9` | Basement Training Echo | 6 | `mirror`, `telegraph` | Enemy Burst telegraph; Assist Aim | tip_telegraph |
| `1-10` | Boss: Lease Reclaimer | 15 | `boss`, `multi_phase`, `lane_shift` | Phase 2 Mid pressure; all tools | `dlg_1_10_pre/post` |
| `1-B` | Lobby Sweep (optional) | 6 | `swarm` | Post-clear sink | none |

**Boss phases (`1-10`):**  
- P1: Front armor node + swarm adds.  
- P2 @50% HP: lane shift — Mid beam telegraph; Wren cleanse value; Mira gauge share carry.

**Star ratings:** 3★ if clear with ≥2 agents alive and turns remaining above threshold (tune in playtest).

---

## 4. Room Tutorial Checklist (implementation)

1. Force highlight Room after `1-5` clear if never entered.  
2. Lock Intimate / Adult buttons (absent).  
3. First Talk free once if HE <5 (`ftue_he_topup`).  
4. Grant T1 Sunwell Tea ×2 in mail when Aff2 unlocks.  
5. Date button greyed with “Requires Aff 3” until rank.  
6. Sleepover assign available from Aff1.

---

## 5. Beginner Banner Soft Intro — UX Rules

- No countdown timers.  
- No red “BUY” — Crystals shown as balance only; free 10-pull uses `BeginnerSampleTicket` grant.  
- Rates panel one tap away, pre-expanded first visit.  
- Dismiss **Not Now** = `gacha_open` with `action=dismiss`; no nag more than 1× per session.  
- After pull, return Hub with Mira line acknowledging result rarity band (N/R/SR/SSR) — still soft.

---

## 6. Branch Notes / Skippable Tips

| Tip ID | First show | Skip behavior |
|--------|------------|---------------|
| `tip_hub` | Enter Hub | Don’t show again checkbox |
| `tip_match` | `1-1` | Cannot skip mid-forced Burst; skip later coaches |
| `tip_team` | Post recruit | Skippable |
| `tip_colors` | `1-6` | Skippable |
| `tip_prism` | `1-8` | Skippable |
| `tip_telegraph` | `1-9` | Skippable |
| `tip_room` | Room enter | Skippable after first Talk |
| `tip_banner` | Banner soft | Always dismissible |

**Assist Aim:** Toggle in Settings + offered on first fail of `1-9` or `1-10`. Highlights valid ≥3 swap.

**Name token:** If player skips naming, `{Handler}` → “Handler”.

**Death / fail:** Mira: “Reset isn’t failure — it’s a second Sync.” Stamina kept except `1-1`.

---

## 7. VO Priority (slice)

| Priority | Content |
|----------|---------|
| P0 | Mira key FTUE lines, Burst shout, Aff rank-ups |
| P1 | Ivy/Juniper/Wren recruit + Burst |
| P2 | System tips (TTS or silent OK) |
| P3 | Full stage banter |

EN text ship-mandatory; JP optional post-slice.

---

## 8. NSFW System Footnote (designers / settings — not in Ch.1 script body)

Chapter 1 and the vertical slice contain **zero** adult scenes. Live product may gate optional packs `NSFW_*_A/B` at Aff **8** / **10** behind age gate + Adult ON + per-scene confirm. Romance combat bonuses never require those packs.

*— End of Chapter 1 + FTUE Script —*
