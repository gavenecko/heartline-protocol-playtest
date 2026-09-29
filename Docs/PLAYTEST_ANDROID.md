# Android playtest guide

Heartline Protocol builds itself into an installable **Android APK** on GitHub — no Unity install needed on
your side. This page covers the one-time setup, installing on your phone, what to test, and how to send a
useful bug report.

---

## 0. One-time: enable the workflow file

The workflow is committed at `Tools/ci/android-build.yml` because the token that opened the PR wasn't allowed
to write to `.github/workflows/` (it lacks GitHub's `workflow` scope). Move it into place once, either:

- **Locally:** `git mv Tools/ci/android-build.yml .github/workflows/android-build.yml && git commit -m "ci: enable Android playtest build" && git push`
- **On github.com:** open `Tools/ci/android-build.yml` → copy its contents → **Add file → Create new file** named
  `.github/workflows/android-build.yml` → paste → commit (you can delete the `Tools/ci` copy afterwards).

## 1. One-time setup: add 3 repository secrets

The workflow `.github/workflows/android-build.yml` needs a Unity license to run the Unity editor in CI
(via [GameCI](https://game.ci/docs/github/activation)). Add these under
**GitHub → gavenecko/heartline-protocol → Settings → Secrets and variables → Actions → New repository secret**:

| Secret name | What to put in it |
|---|---|
| `UNITY_LICENSE` | The **full contents** of your Unity license file `Unity_lic.ulf` (see below). |
| `UNITY_EMAIL` | The email address of your Unity account. |
| `UNITY_PASSWORD` | Your Unity account password. (GameCI recommends a password **without special characters**.) |

**Getting `Unity_lic.ulf` (free Personal license):**
1. Install **Unity Hub** on any computer and sign in with your Unity account.
2. **Unity Hub → Preferences (Settings) → Licenses → Add → Get a free personal license.** Actually click *Add*,
   even if a license already shows; that's what writes the file.
3. Open the file and copy everything in it:
   - Windows: `C:\ProgramData\Unity\Unity_lic.ulf`
   - macOS: `/Library/Application Support/Unity/Unity_lic.ulf`
   - Linux: `~/.local/share/unity3d/Unity/Unity_lic.ulf`
4. Paste it as the value of `UNITY_LICENSE`.

If any of the three secrets is missing, the build fails right away with a message naming the missing secret(s).

### Optional: a stable signing key (so updates keep your save)

Without a keystore, each CI build is signed with a throw-away Unity **debug key**. Android refuses to install an
APK signed with a different key over the existing app. You'd see *"App not installed"* / *"package conflicts
with an existing package"*, and have to **uninstall first, which deletes your save.**

To keep saves across builds, create one keystore and add 4 more (optional) secrets:

```bash
keytool -genkeypair -v -keystore heartline-playtest.keystore -alias heartline \
  -keyalg RSA -keysize 2048 -validity 10000
base64 -w0 heartline-playtest.keystore   # macOS: base64 -i heartline-playtest.keystore
```

| Secret | Value |
|---|---|
| `ANDROID_KEYSTORE_BASE64` | the base64 output |
| `ANDROID_KEYSTORE_PASS` | keystore password |
| `ANDROID_KEYALIAS_NAME` | `heartline` (the alias you used) |
| `ANDROID_KEYALIAS_PASS` | key password (same as keystore password if you weren't asked separately) |

Keep the `.keystore` file somewhere safe and never commit it.

---

## 2. Getting a build

A build runs automatically on **every push/merge to `main`**. To build on demand:
**Actions → "Android playtest build" → Run workflow** (pick the branch). A cold build takes roughly
30–60 min (IL2CPP, ARM64 + ARMv7). Later builds reuse the cached `Library/` and finish faster.

Each successful build:
- uploads the APK as a workflow **artifact** (`HeartlineProtocol-playtest`, kept 14 days), and
- replaces the **`playtest-latest` pre-release** with the new `HeartlineProtocol-playtest.apk` attached.

Stable phone link (the repo is private, so you must be **signed in to GitHub in your phone's browser**):

- Release page: <https://github.com/gavenecko/heartline-protocol/releases/tag/playtest-latest>
- Direct APK: <https://github.com/gavenecko/heartline-protocol/releases/download/playtest-latest/HeartlineProtocol-playtest.apk>

The release title shows the version (`0.1.<run number>`) and commit, so you can tell which build you have.

> Adult art is never included. The workflow deletes any `Adult` art folders under `StreamingAssets`/`Resources`
> before building, so main's playtest APK stays SFW even if adult packs land in the repo later.

---

## 3. Installing on Android

1. On the phone, open the release page above in **Chrome** (or your usual browser) while signed in to GitHub.
   The GitHub mobile *app* can't install APKs, so use the browser.
2. Tap **`HeartlineProtocol-playtest.apk`** under *Assets*. If Chrome warns that the file might be harmful,
   tap **Download anyway**.
3. Open the downloaded file (notification or *Downloads*). The first time, Android asks you to allow installs from
   this source: **Settings → "Allow from this source"** for Chrome (or *Settings → Apps → Special app access →
   Install unknown apps → Chrome → Allow*).
4. Tap **Install**, then **Open**. If Play Protect asks, choose **Install anyway**. It's an unsigned dev build it
   hasn't seen before.
5. **Updating:** download the newer APK and install it over the old one. If Android says *App not installed*,
   the signing key changed (see the optional keystore above): uninstall the old app first (this wipes the save).

Requirements: Android 7.0 (API 24) or newer, ARM64 or ARMv7. Portrait only.

---

## 4. Playtest checklist

Go through these on a real phone. Anything odd (wrong/missing art, buttons that don't respond, text cut off or
under the notch, freezes, errors in the LOG overlay) is worth reporting.

- [ ] **Boot / age gate:** first launch shows the 18+ gate. *I am 18+* → Title. *Exit* closes the app.
- [ ] **Title:** key visual shows; *SFW Mode* / *Adult Content…* show the blocked messages; *New Game* → Hub.
- [ ] **FTUE:** after New Game, Hub status shows Mira's first dialogue line; tap the status text to advance.
- [ ] **Hub:** currency icons + values (STA/HE/LC/PC), featured bust, all nav buttons respond, nothing hidden
      behind the notch or gesture bar. *Mail* grants +500 LC once. *Debug: refill STA/HE* works.
- [ ] **Stages:** list scrolls with a finger; locked stages show the unlock reason; tapping 1-1 starts a battle
      (and a quick tap on a row isn't mistaken for a scroll).
- [ ] **Battle:** gem chips render with art; tap one gem then a neighbour to swap; invalid swaps say so;
      *Assist Aim*, Bursts (*Mira/Ivy/Juni/Wren*), *End Turn*, *Retreat* (tap twice) all work.
- [ ] **Results:** on victory the results panel appears over the board with rewards/XP bars; *Continue → Hub*.
      Clearing 1-1 adds Ivy + Juniper.
- [ ] **Team:** slot cycling, *Apply lane advice*, *Promote*, bust + rarity frame update.
- [ ] **Gacha:** free 10-pull (once), ticket and crystal pulls; reveal shows frame + fullbody/bust.
- [ ] **Jobs:** assign Café/Pier/Clinic, *Debug: complete now*, *Claim All*.
- [ ] **Daily Missions:** progress from stage clear / room talk / job; claim buttons.
- [ ] **Room:** background art; *Talk / Gift / Date / Buy Tea / Sleepover* change BG + text; Aff/HE update.
      Leave and re-enter after *Sleepover* to get the morning bonus.
- [ ] **Room → Cycle guest:** once Velvet/Rook are unlocked (see Tower/Arena), *Cycle guest* shows their
      **idle video loop** (or a still if the video can't play).
- [ ] **Codex:** Prev/Next through agents, rarity frames, *Toggle Fullbody*; secret agents stay hidden until
      unlocked; unlocked **Velvet / Rook play their idle videos**.
- [ ] **Arena:** *Attack next defense team* (3/day); 3 lifetime wins unlock **Rook**.
- [ ] **Tower:** *Stub: Clear next floor* ×10 unlocks **Velvet** at F10; *Fail* snaps back to the checkpoint.
- [ ] **Save persistence:** play a bit, then **swipe the app away** from recents (and try a phone reboot).
      Relaunch: it should skip the age gate and land in the Hub with progress, currencies and unlocks intact.
      Also background the app mid-battle, come back, and check it resumes.
- [ ] **Title → Debug: Reset Save:** wipes progress and returns to the age gate.

---

## 5. Reporting bugs with the LOG overlay

Playtest APKs are **development builds**, and they include an on-screen log:

- **Open / close:** tap the small **`LOG`** button in the **top-right corner**, or **tap with three fingers**.
  The button turns red and shows a count, e.g. `LOG (2)`, when new errors happen while it's closed.
- It lists recent **warnings, errors and exceptions** (with stack traces), plus which scene they happened in.
  *Warn+Err / All logs* toggles normal log lines too. *Clear* empties it.
- **Copy** puts a full report on the clipboard: app version, device model, Android version, GPU, screen/safe
  area, and the log. Paste it straight into a GitHub issue or chat along with:
  1. what you did (steps),
  2. what you expected vs what happened,
  3. a screenshot or screen recording if it's visual.

The overlay doesn't exist in non-development (release) builds.
