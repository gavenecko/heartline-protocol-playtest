# HEARTLINE PROTOCOL — 7-Day F2P Economy Notes

**Companion to:** `/workspace/heartline-protocol-economy-7day.csv`  
**Version:** 1.0 · **Date:** 2026-09-19  
**Persona:** Diligent F2P, clears FTUE Day0, plays ~30–45 min/day after, uses Room daily on Mira, does **one** Beginner paid 10-pull on Day2 from FTUE crystal bank, no IAP.

---

## 1. How to import the CSV

1. Google Sheets / Excel → Import → Upload `heartline-protocol-economy-7day.csv`.  
2. Filter by `Day` or pivot Sum of `*_in` / `*_out` columns.  
3. `DAYN_SUBTOTAL` rows are convenience aggregates (not double-count in pivots — exclude rows where Source contains `SUBTOTAL` when summing detail lines).  
4. Crystal spend on Day2 is shown as negative `Crystals_in` (−1600) because the sheet has no separate `Crystals_out` column — treat abs(negative) as sink.  
5. Ticket spend Day6: negative `Tickets_in` (−5).

---

## 2. Locked constants (from GDD)

| Constant | Value |
|----------|-------|
| Stamina cap | 120 (base) |
| Stamina regen | 1 / 6 min (full ~12h) |
| Heart Energy cap | 60 |
| HE regen | 1 / 8 min (full ~8h) |
| Stage costs | N6 / H10 / Elite15 / Boss20 — Ch.1 uses 0/6/10/15 |
| Talk | 5 HE → 20 Aff XP |
| Gift | 8 HE → 40–120 (preferred ×1.5) |
| Date | 15 HE → 100 Aff XP (Aff≥3, 1/day) |
| Sleepover | 0 HE → +30 morning |
| Pull | 160 crystals each; 10-pull 1600 |
| Beginner | SR@10, SSR@30, max 40; 20% off display |

---

## 3. Aff XP thresholds (slice design call)

GDD lists ranks but not XP/band. **Slice call:**

| Aff Rank | Cumulative XP | Name |
|----------|---------------|------|
| 1 | 0 | Acquaintance |
| 2 | **100** | Friendly |
| 3 | **300** | Close |
| 4 | 600 | Trusted (out of slice) |
| … | … | … |
| 8 | 2800 | Resonance (NSFW gate live — footnote only) |

**Mira D7 target:** Aff **3** securely (cumulative ≈ **1600–1800** XP with modeled Room play) — deep into Close, **not** Aff4+.

---

## 4. Assumptions

1. **Clear counts:** Day0 ≈ 7 story fights + tutorial; Day1 finishes boss + 2 grind; Days2–7 ≈ 8–10 normal equivalent clears/day (Stamina-limited).  
2. **Star quality:** Average ~2★ rewards (not min-max).  
3. **Jobs:** Café Shift 2h ≈ **1200** Luma Coins; 2 claims/day when playing evenings.  
4. **Missions:** Fixed daily ≈ **1200–1500** coins + 50 crystals + 1 ticket.  
5. **Gifts:** Preferred Mira tea craft/shop; HE is the real Room bottleneck, not gift stock after Day1 mail.  
6. **Beginner pulls:** Day0 **free sample 10-pull** (ticket grant). Day2 **one** 1600-crystal 10-pull from FTUE bank. Further pulls not assumed. Expected value: soft SR guarantee path; SSR by 30 if they continue — not required for D7 power.  
7. **No VIP / Pass / friend stam** required; +10 friend stam on Day5 is optional fluff.  
8. **Promote:** Soft coin sink 4–8k/day mid-week for Ivy/Juniper stars — not full ★★★★.  
9. **Slice offline:** Numbers are local; live will retune drop tables ±15%.

---

## 5. Wallet trajectory (approx end-of-day, diligent F2P)

| Day | Stamina feel | Luma Coins (bank est.) | Crystals bank | Tickets | Mira Aff |
|-----|--------------|------------------------|---------------|---------|----------|
| 0 | Comfortable | ~15–17k | ~1650 | ~12 | Aff1 (~40 XP) |
| 1 | Boss day | ~25k | ~1800 | ~14 | Aff2 |
| 2 | After 10-pull | ~30k | ~250 | ~16 | **Aff3** |
| 3–4 | Grind | 25–35k oscillating | ~300–400 | rising | Aff3 deep |
| 7 | Stable | ~30–40k | ~500–700 | ~10+ after spends | **Aff3** (≫300 XP) |

*Banks are order-of-magnitude from summing CSV detail rows minus shop sinks; use for design, not accounting audit.*

---

## 6. D7 targets (PASS bars for economy playtest)

| Target | F2P diligent | Notes |
|--------|--------------|-------|
| **Mira Aff Rank** | **3** | Must hit; Room daily required |
| **Team levels** | Ivy/Juniper/Wren/Mira **Lv 28–35** | Mira may lead ~35 |
| **Stars** | Ivy/Juniper ★★–★★★; Wren ★★★; Mira ★★★★ (base) | No Aff-gated promote beyond Aff2 |
| **Ch.1** | Complete by end Day1 | Day0 possible for skilled |
| **Beginner pulls used** | 10–20 | Free 10 + optional paid 10 |
| **Crystal bank** | >0 if they skipped Day2 pull; ~200–700 if they pulled | Never soft-lock story |
| **HE** | Room actions most days without pots | Caps respected |

---

## 7. Stress / whale contrast (not in CSV)

- **Whale:** Pass + crystal packs → more pulls, not higher Aff ceiling speed beyond HE regen (ethical pillar).  
- **Casual:** May reach Aff3 by D10–12; still fine — do not raise Aff XP costs.

---

## 8. Open tuning knobs

If Aff3 comes too fast: raise Aff2/3 thresholds to 120 / 360.  
If HE starving: +1 Talk free/day on Mira only through D3.  
If coin drought: Café yield 1500.  
**Call for slice ship:** keep thresholds **100 / 300** and Café **1200**.

*— End of Economy Notes —*
