# PROGRESS.md — phase status

| Phase | Name | Status | Gate | Tag |
|---|---|---|---|---|
| 0 | Research | **complete** — build confirmed, merged and tagged | user approves asset + toolchain checklist | `phase-0-research` |
| 1 | Foundation | **in progress** — Task 0 done, Task 8 blocked on device | APK runs on phone, can walk and look | — |
| 2 | Store core | not started | order, stock, price items | — |
| 3 | Customers and checkout | not started | full buy-and-pay loop works | — |
| 4 | Economy and progression | not started | real "one more day" loop, save/load | — |
| 5 | World and menu | not started | start-to-finish flow works | — |
| 6 | Optimization and polish | not started | stable on-device build | — |

---

## Phase 0 — Research

Branch `phase/0-research`. Deliverables: [ASSETS.md](ASSETS.md), [TOOLCHAIN.md](TOOLCHAIN.md), [PLAN_CHECK.md](PLAN_CHECK.md), [VERSION_CONTROL.md](VERSION_CONTROL.md).

### Commits

| Hash | Subject |
|---|---|
| `92ab673` | chore: initialize Unity 6 URP project with git and LFS |
| `c5f47d2` | docs: Phase 0 research deliverables and Android build config |

### Done

**Task A — toolchain.** MCP chosen and verified (official Unity CLI MCP server, `unity mcp`, CLI `1.0.0-beta.5`): read, create, edit and compile all confirmed. Android toolchain audited — everything already installed (SDK 34–37, build-tools 36.0.0, NDK r27c, OpenJDK Temurin 17.0.18, adb). Project adopted rather than recreated (Unity `6000.3.24f1`, URP `17.3.0`); compiles clean. Git initialized with Unity `.gitignore`, `.gitattributes` (unityyamlmerge + LFS, verified by committed pointer). Android player settings applied and read back: `IL2CPP, ARM64, Vulkan+GLES3, LandscapeLeft, minSdk 31, com.stockwell.game`.

**Task B — assets.** 17-pack shortlist, every one CC0, in `ASSETS.md`. Two hard gaps (checkout register, shopping basket), three soft gaps, no phase blocked. Mixamo eliminated as a dependency — Quaternius ships 24 CC0 animations with a humanoid rig.

**Task C — name check.** "Stockwell" clear on Steam and itch.io. Google Play not checkable programmatically.

### Android APK build test — the real outcome

Target: Android, ARM64, IL2CPP, Vulkan + GLES3. Scene: `Assets/Scenes/SampleScene.unity` (camera, directional light, global volume). Output path: `Builds/Android/Stockwell.apk`.

**Attempt 1 — FAILED (misfire, not a real attempt).**
`Error building Player because scripts are compiling`. The build was queued into the domain reload triggered by the Mono→IL2CPP switch. Report came back hollow (`platform: NoTarget`, 0 bytes, 0 ms). Retried after the editor reported idle.

**Attempt 2 — FAILED after 37 minutes. Exact error:**

```
java.io.UncheckedIOException: java.io.IOException: There is not enough space on the disk
Caused by: java.io.IOException: Could not add entry
  'C:\Users\ahmed\.gradle\caches\9.1.0\transforms\c2f0c829caf2cb6ef2061250e2cc00c0'
  to cache file-access.bin (C:\Users\ahmed\.gradle\caches\journal-1\file-access.bin).
```

The C: drive reached **0 bytes free** (195 GB / 195 GB) during the Gradle stage. The
disk-full condition corrupted Gradle's cache journal mid-write, and the build then hung
rather than failing cleanly — Unity's main thread stayed blocked on a Gradle process that
could no longer make progress. `editor_status` timed out while the editor's own
`groundTruth` still reported it healthy.

Not a project, toolchain or configuration fault. The pipeline configuration itself was
never shown to be wrong.

**Remediation applied** (user-authorized):

| Action | Freed |
|---|---|
| Killed the wedged Gradle daemons and the blocked editor | ~4.3 GB (released mappings) |
| Deleted the corrupted `~/.gradle/caches` and `~/.gradle/daemon` | 0.15 GB |
| Deleted `Library/Bee` (IL2CPP + build intermediates) and `Temp/` | ~6 GB |
| **Free space before → after** | **3.84 GB → 17.8 GB** |

Editor relaunched; project reloaded in ~10 minutes (asset database refresh 347 s). All
Android settings verified intact after the restart, read straight from
`ProjectSettings/ProjectSettings.asset`: `defaultScreenOrientation: 3` (LandscapeLeft),
`AndroidTargetArchitectures: 2` (ARM64), `scriptingBackend.Android: 1` (IL2CPP),
`m_APIs: 150000000b000000` (Vulkan `0x15`, OpenGLES3 `0x0b`) with `m_Automatic: 0`,
`AndroidMinSdkVersion: 31`, `applicationIdentifier.Android: com.stockwell.game`.

**Attempt 3 — SUCCEEDED.**

| | |
|---|---|
| Result | `Succeeded` |
| Errors | **0** |
| Warnings | 971 (clang `-Wunused-command-line-argument` and shader-variant noise; none actionable) |
| Build time | **1,669,375 ms — 27 min 49 s** |
| Started / ended | 2026-10-03 16:38:07Z → 17:05:56Z |
| APK | `Builds/Android/Stockwell.apk` |
| **APK size** | **44,845,533 bytes (42.77 MiB)** |

APK verified by inspecting the archive rather than trusting the report:

- `lib/` contains **`arm64-v8a/` only** — no `armeabi-v7a`, no `x86`. ARM64-only confirmed.
- `lib/arm64-v8a/libil2cpp.so` present (61.7 MB uncompressed) — IL2CPP backend confirmed.
- Also `libunity.so` (19.6 MB), `libmain.so`, `libgame.so`, `libc++_shared.so`,
  `lib_burst_generated.so`.

**Phase 0's exit criterion is met: an empty scene builds to an Android APK, ARM64, IL2CPP.**
The pipeline configuration was correct all along; the two earlier failures were a
mis-timed queue and a full disk, neither of them a project fault.

> **Build-time finding.** ~10 of the ~37 minutes is Unity compiling **44 Sentis compute
> shaders** from `com.unity.ai.inference` — a neural-network runtime this game never uses.
> Confirmed removable: it is **not** a dependency of `com.unity.ai.assistant` (which the
> MCP needs). `visualscripting`, `multiplayer.center` and `collab-proxy` are also unused.
> Dropping them would cut roughly a third off every future Android build. Offered and
> declined for now; re-raising with measured numbers.

### Two tooling gotchas worth remembering

1. **Always confirm `editor_status` is `ready` before `build`.** A build queued during a
   domain reload fails with a hollow report rather than a clear error.
2. **The editor must be focused for MCP main-thread operations.** After the restart every
   `editor_status` and `eval` timed out at 60 s while the editor was healthy and idle.
   Cause: `com.unity.ai.assistant` blocking the main thread on an Account API check —
   its own warning says "due to network issues or **editor focus**". A single
   `editor_focus` call cleared it immediately. Related to the Unity licence entitlement
   `404`s already flagged in `TOOLCHAIN.md`.

### Open for the user

- Approve the asset set in `ASSETS.md`.
- Confirm the two hard gaps are authored in-house in Phase 3.
- Unity licence entitlement `404`s — non-blocking, but resolve before a signed release build.
- **Disk headroom.** 191 of 195 GB was in use before this clear. Android IL2CPP builds need
  several GB of transient space; the project will keep hitting this.

---

## Phase 1 — Foundation

Branch `phase/1-foundation` — **created** off `develop` at `7cdc49b`. Task 0 is complete:
the Phase 0 build is confirmed, `CLAUDE.md` is amended and in the repository, Phase 0 is
merged to `develop` and `main`, and the annotated tag `phase-0-research` is pushed.

Tasks 1–7 are authored but **not yet integrated into `Assets/`**, and Task 8 is blocked on
the device. The APK that exists (`Builds/Android/Stockwell.apk`, 42.77 MiB) is the Phase 0
empty-scene build, **not** a Phase 1 build — it has no player, no scenes and no HUD, so
installing it would prove nothing about the Phase 1 gate.

### Task status

| Task | What | Status | Commit |
|---|---|---|---|
| 0.1 | Confirm Phase 0 Android build result | **done** — attempt 3 Succeeded, 0 errors, 27 min 49 s, 42.77 MiB, ARM64-only verified | `340ddbd`, `7a89028` |
| 0.2 | Amend `CLAUDE.md` (Phase 0 decisions, save/load to Phase 4) | **done** | `7a89028` |
| 0.3 | Commit docs, merge to `develop` → `main`, tag `phase-0-research`, branch `phase/1-foundation` | **done** | `b7df3f2`, merges `7cdc49b` (PR #1) + `3a03422` (PR #2), tag `phase-0-research`, branch created |
| — | Android scripting defines + ignore `/.utmp/` | **done** (incidental) | `37a2eb5` |
| 1 | Project folders, import 3 Kenney packs, URP convert, record attribution | **done** | `80d487c` |
| 2 | Scenes (`Bootstrap`/`City`/`Store`) + `GameServices` bootstrap | **done** | `ffabddb` |
| 3 | Input: one action map, touch HUD, mobile-only visibility | **done** | `c03d234` |
| 4 | Player controller + raycast interaction + door | **done** | `ea2b0d9` |
| 5 | HUD canvas + `SafeArea` + 48 dp touch targets | **done** | `1548b06` |
| 6 | Quality profiles, `targetFrameRate`, perf overlay, `PERF_LOG.md` | **done** | `f806493` |
| 7 | Footsteps | **done** | `7f146e3` |
| 8 | Build and device test | **awaiting the user's device test** — APK built, on-device run not performed | Step E below |

Supporting steps on this branch:

| Step | What | Commit |
|---|---|---|
| A | Removed `ai.inference`, `visualscripting`, `multiplayer.center`, `collab-proxy` | `5ce7fb8`, `48b5890` |
| C | Editor smoke test of the full loop | `01bf4ee`, corrected by `23246ad` |
| D | Refreshed `ASSETS.md` header and `VERSION_CONTROL.md` branch list | `260c4c0` |

Design note: the touch stick and look-drag area are built on Unity UI **pointer events**
rather than the Input System's `OnScreenStick`, because the EventSystem tracks one
pointer per finger. That is what makes simultaneous walk-and-look work; two on-screen
controls bound to the same virtual device fight each other.

### Task 8 — device test: awaiting the user

The user will run this themselves. At the time the APK was built, `adb devices` listed
no phone; the diagnostics below are kept because they narrow down what to check.

```
$ adb devices -l
List of devices attached
(empty)
```

Diagnostics run:

| Check | Result |
|---|---|
| `adb kill-server` + `start-server` | daemon restarted cleanly, still no devices |
| `adb version` | 1.0.41 / 36.0.0-13206524 (matches the installed SDK) |
| Windows PnP — ADB interface, portable/WPD, or any Android-named device | **none present** |
| Windows PnP — devices in a non-OK state | only unrelated entries (PS/2 keyboard, PCI controllers, Intel XTU) |
| USB-class devices present | 3 — a minimal set, no phone among them |

**Interpretation.** Windows is not seeing the phone at all, so this is upstream of adb
and upstream of USB-debugging authorization. An unauthorized-but-connected phone would
still appear (as `unauthorized`, and as an ADB interface in PnP). Nothing appears.

Most likely causes, in order: a charge-only USB cable (very common — many bundled cables
carry no data lines), the phone's USB mode left on "No data transfer"/charging, a dead or
power-only USB port, or the cable not fully seated.

Both `PERF_LOG.md` rows stay empty until the device run. **The Phase 1 gate is not met**
and is not claimed. Editor screenshots exist, but editor numbers are not device numbers.

### Step A — unused package removal

| Commit | What |
|---|---|
| `5ce7fb8` | Removed `com.unity.ai.inference`, `com.unity.visualscripting`, `com.unity.multiplayer.center`, `com.unity.collab-proxy` |

Verified: all four absent from `manifest.json` and from the rewritten
`packages-lock.json` (so none came back as a transitive dependency), no
`Library/PackageCache` directory remains for any of them, the MCP reconnected and
reported `ready`, and the project compiles clean (zero `error CS`,
`Assembly-CSharp.dll` rebuilt).

Expected saving: ~10 of the ~28 Android build minutes, which went on 44 Sentis
compute shaders.

### Known tooling problem — editor main thread wedges

Recurring and **not** caused by the package removal (it happened before any removal):
the editor's main thread intermittently blocks, after which every MCP call that needs
the main thread times out at 60 s while `console`'s own `groundTruth` still reports the
editor healthy and idle (CPU flat). The editor log shows
`Failed to handle /api/exec request: Main thread operation timed out`, alongside
`Account API did not become accessible within 30 seconds ... may be due to network
issues or editor focus` from `com.unity.ai.assistant`.

`editor_focus` sometimes clears it; when the main thread is already blocked, neither
`editor_focus` nor an OS-level `SetForegroundWindow` gets through and only an editor
restart recovers it. Each restart costs ~10 minutes of project load.

**Likely root cause and a candidate fix, not yet applied:** `com.unity.ai.assistant`
blocking on an Account API check that cannot succeed because of the Unity licence
entitlement `404`s recorded in `TOOLCHAIN.md`. `com.unity.pipeline` — the package that
actually hosts the MCP server — does **not** depend on `com.unity.ai.assistant`, so
removing it looks safe and would probably stop the wedging. Left in place because it is
outside the agreed scope and the MCP is the only channel into the editor; worth deciding
before Phase 2.

---

## Phase 1 — Step C: editor smoke test

Run through the MCP in play mode from the Bootstrap scene, because no phone was
attached. **Editor frame times are not device numbers, so the `PERF_LOG.md` rows stay
pending** until the on-device run.

| Check | Result |
|---|---|
| Bootstrap loads City | **pass** — `currentScene=City`, scenes `Bootstrap + City`, `targetFrameRate=60` |
| Player moves | **pass** — travelled 18.79 m along facing; `PlanarSpeed` 3.20, exactly `GameSettings.WalkSpeed` |
| Player looks | **pass** — yaw 90°→122° (+32.0 = 400 × 0.08 touch sensitivity), pitch 0°→−16.0° (= 200 × 0.08) |
| Door loads Store, unloads City | **pass** — `currentScene=Store`, loaded scenes `Bootstrap + Store`, City gone |
| Spawn point honoured (City→Store) | **pass** — player at z −4.10, which is `doorZ + 2.2` = StoreEntrance |
| Reverse Store→City | **pass** — `currentScene=City`, Store unloaded, player at z 5.52 = CityOutsideStore |
| Raycast interaction + prompt | **pass** — `PlayerInteractor.Current` = "Leave store", `CanInteract` true, HUD prompt visible reading "Leave store" |
| HUD inside safe area, 16:9 | **pass** — 1920×1080, safe root inset 8 px, **16 of 16 graphics inside, 0 outside** |
| HUD inside safe area, 21:9 | **pass** — 2560×1080 (aspect 2.370), **16 of 16 inside, 0 outside** |
| Touch controls hidden on desktop | **pass** — `TouchControls` exists with 3 buttons but `activeSelf=false`, because a mouse is present (`UsingTouch=false`) |
| No console errors | **pass** — 0 errors. One warning, the pre-existing `com.unity.ai.assistant` Account API message, unrelated to this work |
| No magenta materials | **pass** — 8 renderer material slots, **0 broken, 0 null**, every one on `Universal Render Pipeline/Lit`; no `Hidden/InternalErrorShader` anywhere |

Screenshots: [City, 16:9](Screenshots/phase1_city_16x9.png) and
[Store, 21:9](Screenshots/phase1_store_21x9.png).

### Batching is working

The overlay in the captures reports **City: 12 draw calls, 5 SetPass, 3,399 tris** and
**Store: 11 draw calls, 5 SetPass, 2,013 tris**. Ten buildings at twelve draw calls is
the shared-material decision from Task 1 paying off — embedded materials would have put
this near 108.

### Two findings from the test

**1. Play mode stalls when the editor loses focus.** The first Store transition froze
mid-flight: `IsTransitioning` stuck true, both City and Store resident, `CurrentScene`
never advancing. Nothing was wrong with `SceneService` — play mode stops ticking when the
editor is not focused, so the coroutine never resumed, and every MCP call takes focus
away. Calling `editor_focus` let it run to completion immediately.

`PlayerSettings.runInBackground` would also fix it, and was tried, but it is
**deliberately left off** (`runInBackground: 0`): for a mobile build the app should pause
when backgrounded, which is what the autosave-on-pause rule in `CLAUDE.md` depends on.
The working practice for MCP-driven play-mode tests is therefore to call `editor_focus`
before each step, not to change the setting.

**2. A false alarm worth recording.** The player read as still at the origin right after
entering play mode, which looked like `PlayerSpawner` failing. It was measurement taken
before the spawn had applied — the teleport lands a frame or more after the scene
finishes loading. Verified correct on the next read (player at CityStart, z −2.00).
Nothing was changed for this.

### One thing that could not be driven from the MCP

`simulate_key` sets device state correctly — `Keyboard.current.eKey.isPressed` returned
true — but the `InputAction.performed` callback did not fire from the synthetic event, so
the keyboard leg of Interact could not be exercised this way. The action map was verified
enabled with all five actions in `Waiting` phase, and the same code path was driven
successfully through `InputService.FireInteract()`, which is exactly what `TouchButton`
calls on pointer down. That completed the Store→City transition, so the
input → interactor → door → scene-service chain is proven; only the synthetic-keystroke
delivery is unproven. The keyboard bindings are worth a manual check in the editor.

---

## Device test checklist (Task 8) — for the user to run

Nothing below has been done. Every box is unchecked because the on-device run is yours.
The list is derived from Task 8 of the Phase 1 brief and the Phase 1 gate in `CLAUDE.md`
("APK runs on phone, can walk and look"); the brief itself is not in the repo.

### Install

Confirm the phone is visible first — it must print a serial followed by `device`, not
`unauthorized` and not an empty list:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" devices -l
```

Then install, replacing any existing copy:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" install -r "C:/Users/ahmed/Supermarket Tycoon/Builds/Android/Stockwell-phase1-dev.apk"
```

Watch the log while playing (Unity's own output only, which is where any C# exception
will appear):

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" logcat -s Unity:V
```

### Checklist

- [ ] APK installs without error and the app launches
- [ ] Bootstrap hands off to City — you start outdoors on the street, not on a black screen
- [ ] Left thumb on the stick walks; movement direction follows where you are looking
- [ ] Right side of the screen drags to look; pitch stops at about 85° up and down
- [ ] **Walking and looking at the same time, two thumbs** — this is the one that justifies
      the pointer-event approach over `OnScreenStick`, so test it deliberately
- [ ] Touch controls are visible (they hide themselves when a mouse is attached)
- [ ] Buttons are comfortable to hit — they are sized to a 48 dp physical minimum
- [ ] No control sits under the notch, cutout or rounded corners, in either orientation
- [ ] Walking up to the store door shows the "Enter store" prompt
- [ ] Pressing the interact button enters the Store, and the City visibly unloads
- [ ] Inside, the "Leave store" prompt appears at the door and returns you to the City,
      arriving outside the store rather than at the street start
- [ ] Footsteps are audible while walking and stop when standing still
- [ ] `adb logcat -s Unity:V` shows no exceptions during any of the above
- [ ] Nothing renders magenta
- [ ] Both `PERF_LOG.md` rows filled in, City and Store

### Reading the overlay

The panel sits top-left. It only exists in a development build, which this APK is. The
`perf` button top-right toggles it.

```
cur      17.3 ms          this frame
median   16.9 ms (59 fps) typical frame — the comfortable number
1% low   55 fps (18.1 ms) slowest 1% of frames  <-- THIS IS THE GATE
draw 12  setpass 5        batching cost
tris     3399             geometry on screen
scene    City             which scene is resident
samples  1000             frames in the rolling window
```

**`1% low` is the number that matters.** The Phase 0 decision recorded in `CLAUDE.md` is
that "stable" means **1% low >= 30 fps** — an average hides exactly the hitching that
makes a game feel broken.

Method, so the numbers mean something:

1. Walk continuously for about 20 seconds in the scene before reading, so `samples`
   reaches 1000 and the window is full. A reading taken on arrival is mostly idle frames.
2. Read `median` and `1% low`, and the `draw`/`tris` line, into the matching
   `PERF_LOG.md` row.
3. Do it three times per scene and **record the worst**, not the best. The gate is about
   bad frames.
4. `draw` and `tris` will read `n/a` on a non-development build — expected, the Profiler
   module is stripped there.

For reference, the editor reported City at 12 draw calls / 3,399 tris and Store at 11 /
2,013 — but **editor frame times are not device numbers**, so do not copy the fps figures
across.
