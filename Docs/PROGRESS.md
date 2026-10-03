# PROGRESS.md — phase status

| Phase | Name | Status | Gate | Tag |
|---|---|---|---|---|
| 0 | Research | **awaiting user approval** | user approves asset + toolchain checklist | `phase-0-research` (not yet cut) |
| 1 | Foundation | not started | APK runs on phone, can walk and look | — |
| 2 | Store core | not started | order, stock, price items | — |
| 3 | Customers and checkout | not started | full buy-and-pay loop works | — |
| 4 | Economy and progression | not started | real "one more day" loop | — |
| 5 | World and menu | not started | start-to-finish flow works | — |
| 6 | Optimization and polish | not started | stable on-device build | — |

---

## Phase 0 — Research

Branch `phase/0-research`. Deliverables: [ASSETS.md](ASSETS.md), [TOOLCHAIN.md](TOOLCHAIN.md), [PLAN_CHECK.md](PLAN_CHECK.md), [VERSION_CONTROL.md](VERSION_CONTROL.md).

### Done

**Task A — toolchain**

- MCP chosen and verified: **official Unity CLI MCP server** (`unity mcp`, Unity CLI `1.0.0-beta.5`). Read, create, edit and compile all confirmed working against the live editor. Rationale and rejected alternatives in `TOOLCHAIN.md`.
- Android toolchain audited — **everything was already installed**, nothing to add: Android Build Support on `6000.3.24f1`, SDK platforms 34–37, build-tools 36.0.0, NDK r27c (`27.2.12479018`), OpenJDK Temurin `17.0.18+8`, adb.
- Project adopted rather than recreated: Unity `6000.3.24f1`, URP `17.3.0`, mobile + PC render pipeline assets already present from the URP template. **Compiles clean, zero errors.**
- Git initialized with a Unity `.gitignore`, `.gitattributes` (unityyamlmerge + LFS), remote set to `mvntaha/supermarket-tycoon`, branch model in `VERSION_CONTROL.md`. LFS verified by committed pointer.
- Android player settings applied and read back: `backend=IL2CPP arch=ARM64 apis=Vulkan,OpenGLES3 orient=LandscapeLeft minSdk=AndroidApiLevel31 id=com.stockwell.game`.

**Task B — asset research**

- Full shortlist in `ASSETS.md`: 17 recommended packs across all six categories, **every one CC0**, with a pick and a backup each, licence table, URP status, poly/mobile cost notes and style-mismatch risks.
- Two hard gaps (checkout register, shopping basket) and three soft gaps identified, all with resolutions; **no phase is blocked**.
- The brief's one external dependency, Mixamo, was eliminated — Quaternius ships 24 CC0 animations with a humanoid rig.

**Task C — name check**

- **"Stockwell" is clear** on Steam and itch.io. No fallback name needed. Google Play could not be checked programmatically; manual check advised before any store listing.

### Build test

Android APK build of the near-empty template scene (`SampleScene`: camera, directional light, global volume), ARM64 / IL2CPP.

| | |
|---|---|
| Target | Android, ARM64, IL2CPP, Vulkan + GLES3 |
| Scene | `Assets/Scenes/SampleScene.unity` (build index 0) |
| Output | `Builds/Android/Stockwell.apk` |
| Result | _recorded below_ |

First attempt failed with `Error building Player because scripts are compiling` — the Mono→IL2CPP switch had triggered a domain reload and the build was queued into it. Retried after the editor reported idle. **Worth remembering for later phases: always confirm `editor_status` is `ready` before queueing a build**, especially right after changing a player setting that forces a domain reload.

> **Result:** see the note appended at the end of this file once the build settled.

### Frame-time log

Not applicable in Phase 0 (no gameplay, no device run). Per `PLAN_CHECK.md` B2, Phase 1 starts `Docs/PERF_LOG.md` with one row per phase and defines "stable 30 fps" as **1% low ≥ 30 fps**, not average.

### Decisions carried into Phase 1

From `PLAN_CHECK.md`, four things are cheaper to decide now than to refactor later:

1. **A1** — scanning interaction: one tap per item inside the existing raycast interact system.
2. **A2** — service locator shape: one `GameServices` bootstrap, settled while there is nothing to migrate.
3. **B3** — store/city scene boundary: two scenes, additive load at the door, city unloaded while inside.
4. **B4** — anchor touch UI to safe-area insets from the first HUD, not as Phase 6 polish.

### Open for the user

- Approve the asset set in `ASSETS.md` (or cut/substitute packs).
- Confirm the two hard gaps are authored in-house in Phase 3 rather than searched for further.
- Note the Unity licence entitlement errors in the editor log — non-blocking now, worth resolving before a signed release build.
