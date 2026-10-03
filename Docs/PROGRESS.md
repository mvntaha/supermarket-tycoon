# PROGRESS.md — phase status

| Phase | Name | Status | Gate | Tag |
|---|---|---|---|---|
| 0 | Research | **complete** — build confirmed, merged and tagged | user approves asset + toolchain checklist | `phase-0-research` |
| 1 | Foundation | **in progress, blocked** | APK runs on phone, can walk and look | — |
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

Branch `phase/1-foundation` — **not yet created.** Task 0.3 gates it, and Task 0.1 (build
confirmation) gates Task 0.3, so no Phase 1 work has been committed.

### Task status

| Task | What | Status | Commit |
|---|---|---|---|
| 0.1 | Confirm Phase 0 Android build result | **done** — attempt 3 Succeeded, 0 errors, 27 min 49 s, 42.77 MiB, ARM64-only verified | `340ddbd`, `24859ce` |
| 0.2 | Amend `CLAUDE.md` (Phase 0 decisions, save/load to Phase 4) | **done** | `24859ce` |
| 0.3 | Commit docs, merge to `develop` → `main`, tag `phase-0-research`, branch `phase/1-foundation` | **done** | `24859ce` + tag `phase-0-research` |
| 1 | Project folders, import 3 Kenney packs, URP convert, record attribution | **not done** — packs downloaded and verified, not imported | — |
| 2 | Scenes (`Bootstrap`/`City`/`Store`) + `GameServices` bootstrap | **not done** — scripts authored, not integrated | — |
| 3 | Input: one action map, touch HUD, mobile-only visibility | **not done** — action asset and scripts authored, not integrated | — |
| 4 | Player controller + raycast interaction + door | **not done** — scripts authored, not integrated | — |
| 5 | HUD canvas + `SafeArea` + 48 dp touch targets | **not done** — scripts authored, not integrated | — |
| 6 | Quality profiles, `targetFrameRate`, perf overlay, `PERF_LOG.md` | **partially done** — `PERF_LOG.md` created with headers; overlay authored, not integrated | — |
| 7 | Footsteps | **not done** — script authored, not integrated | — |
| 8 | Build and device test | **blocked** — see below | — |

**No commit hashes exist for Phase 1 yet.** Nothing has been committed because Task 0.3
has not run, and the brief requires work to happen on `phase/1-foundation`.

### Work prepared but not applied

21 C# scripts authored (`Core`, `Input`, `World`, `Player`, `UI`, `Diagnostics`, `Audio`)
and a rewritten `InputSystem_Actions.inputactions` with a single `Gameplay` map
(Move, Look, Interact, PickUpDrop, Place; WASD + arrows, mouse delta, E/Q/F). JSON
validated. All three Kenney packs downloaded (3.67 MB) and their contents verified;
attribution recorded in `ASSETS.md`. None of it is in `Assets/` yet.

Design note: the touch stick and look-drag area are built on Unity UI **pointer events**
rather than the Input System's `OnScreenStick`, because the EventSystem tracks one
pointer per finger. That is what makes simultaneous walk-and-look work; two on-screen
controls bound to the same virtual device fight each other.

### Task 8 — device test: BLOCKED, not skipped

**`adb devices` does not list the phone.** Per the brief, stopping rather than skipping.

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

Both `PERF_LOG.md` rows and the City/Store screenshots stay empty until the device is
reachable. **The Phase 1 gate cannot be met without them.**
