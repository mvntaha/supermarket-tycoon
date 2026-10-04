# TOOLCHAIN.md — Phase 0 verification

Verified 2026-10-04 on Windows 10 Pro 19045.

## 1. Unity MCP choice

**Chosen: the official Unity CLI MCP server (`unity mcp`).**

| | |
|---|---|
| Binary | `C:\Users\ahmed\AppData\Local\Unity\bin\unity` |
| Version | Unity CLI `1.0.0-beta.5` |
| Claude Code entry | `unity-editor-mcp` → `unity mcp` (stdio) |
| Transport | Unity relay `1.0.12-build.99`, editor client on port 9001 |
| Requires | Unity 6000.0+ and the `com.unity.ai.assistant` package (present: `2.20.0-pre.1`) |

### Why this one

- **First-party and current.** Unity shipped an official Claude Code plugin in September 2026 that bundles the Unity CLI, Unity's engineering skills and this MCP server. It is maintained by Unity's own engineering team, so it tracks Unity 6 API changes directly instead of chasing them.
- **The main community alternative is being retired.** `ivanmurzak/unity-mcp` (~13.8k stars, v10.2.0) is the most popular third-party server, but it is slated for deprecation now that the Unity CLI ships MCP functionality built in — the CLI route is the documented recommendation. Starting a multi-phase project on a server with a known sunset is the wrong trade.
- **Coverage matches the roadmap.** It exposes ~170 tools including the things later phases actually need: `bake_navmesh` and `bake_navmesh_surfaces` (Phase 3 customer AI), `bake_lighting` (baked lighting is a non-negotiable), `get_performance_stats` and `capture_game_view` (Phase 6 profiling), `build` / `build_status`, `set_player_settings`, `simulate_pointer` / `simulate_key` (touch input testing), and `run_tests`.
- **No extra install was needed.** It was already configured and reachable, so there is no second toolchain to keep in sync.

Rejected: `CoderGamester/mcp-unity` and `MauricePutinas/unity-mcp-claude-code` — both are community servers with narrower tool coverage and no NavMesh/lighting/profiling bake support, and neither has a reason to be preferred over the first-party option.

### Verification performed

| Check | Call | Result |
|---|---|---|
| Editor reachable | `editor_status` | `ready`, Unity `6000.3.24f1`, project path correct |
| Read hierarchy | `get_scene_hierarchy` | SampleScene: Main Camera, Directional Light, Global Volume (with `UniversalAdditionalCameraData`, confirming URP) |
| Create folder | `create_folder _Project/Scripts` | created, GUID returned |
| Create script | `create_script McpToolchainProbe` | `Assets/_Project/Scripts/McpToolchainProbe.cs` created |
| Edit script | rewrote the file body on disk | accepted |
| Compile | `recompile` | `completed`, `compilationFailed: false`, zero errors |
| Read settings | `get_player_settings` | returned product / backend / API level |
| Change target | `switch_build_target Android` | accepted, full reimport ran |

Read, create, edit and compile are all confirmed working end to end.

## 2. Unity install and Android components

Editors present under `C:\Program Files\Unity\Hub\Editor\`: `6000.0.72f1`, `6000.3.24f1`, `6000.4.0f1`, `6000.5.9f1`, `6000.6.0f1`.

**Project editor: `6000.3.24f1`** (matches `ProjectSettings/ProjectVersion.txt`). Android Build Support is installed on this editor — nothing had to be added.

| Component | Status | Version / detail |
|---|---|---|
| Android Build Support (`AndroidPlayer`) | installed | present on `6000.3.24f1` and `6000.6.0f1` |
| Android SDK | installed | platforms `android-34`, `android-35`, `android-36`, `android-37.0`; build-tools `36.0.0` |
| Android NDK | installed | `27.2.12479018` (r27c) |
| OpenJDK | installed | Temurin `17.0.18+8` |
| adb | installed | `.../AndroidPlayer/SDK/platform-tools/adb.exe` |
| CMake | installed | bundled under SDK |

Nothing was missing. The other editors are partial installs (`6000.4.0f1` WebGL only, `6000.5.9f1` Windows only, `6000.0.72f1` none) and are irrelevant to this project.

> One note from the editor log: repeated `[Licensing::Client] Error: Code 404 ... Found 0 entitlement groups and 0 free entitlements`. The editor runs and compiles regardless, so this is not blocking, but the Unity licence/entitlement state is worth checking before the first signed release build.

## 3. Project, git and compile state

The Unity 6 URP project already existed at `C:\Users\ahmed\Supermarket Tycoon` and was adopted rather than recreated:

- Unity `6000.3.24f1`, URP `17.3.0`, Input System `1.20.0`, AI Navigation `2.0.14`.
- URP mobile template: `Assets/Settings/` already contains `Mobile_RPAsset` / `Mobile_Renderer` and `PC_RPAsset` / `PC_Renderer`, which lines up with the "mobile quality profile, PC build shares input actions" decision.
- `Assets/InputSystem_Actions.inputactions` is present from the template and is the starting point for the single action map.
- Compile state: **clean**, zero errors (`recompile` → `completed`).

Git was initialized in Phase 0 — see [VERSION_CONTROL.md](VERSION_CONTROL.md) for the branching model.

- `.gitignore`: Unity-standard, plus build output (`*.apk`, `*.aab`), IDE files, and **keystores / `keystore.properties`** so signing material can never be committed.
- `.gitattributes`: `eol=lf` throughout, `merge=unityyamlmerge` on Unity YAML (`.unity`, `.prefab`, `.asset`, `.mat`, `.anim`, `.controller`, `.meta`), and **Git LFS** on binary art/audio (`.fbx`, `.obj`, `.blend`, `.glb`, `.png`, `.tga`, `.psd`, `.wav`, `.ogg`, `.ttf`, ...) ready for the third-party packs.
- LFS verified: `Assets/TutorialInfo/Icons/URP.png` committed as a pointer (`oid sha256:1d17a9ff...`, 24069 bytes) and confirmed by `git lfs ls-files`.
- Remote: `https://github.com/mvntaha/supermarket-tycoon` (was empty).

## 4. Android build configuration

Player settings applied for the mobile rules in `CLAUDE.md`:

| Setting | Value | Why |
|---|---|---|
| Scripting backend | IL2CPP | required for ARM64 |
| Target architecture | ARM64 only | mobile rule; smallest APK |
| Graphics API | Vulkan, OpenGL ES 3 fallback | mobile rule |
| Orientation | Landscape | locked decision |
| Min API level | 31 (Android 12) | matches the test device |

The measured build result is recorded in [PROGRESS.md](PROGRESS.md), so this file stays the reference for *how* rather than *when*.

### Installing on the Android 12 phone over USB

1. On the phone: **Settings → About phone → tap Build number 7×** to unlock Developer options.
2. **Settings → Developer options → enable USB debugging**. Leave *Install via USB* on as well.
3. Connect by USB, set the connection mode to **File transfer (MTP)**, then accept the *Allow USB debugging* RSA-fingerprint prompt on the phone (tick "Always allow from this computer").
4. Confirm the device is visible. `adb` lives inside the editor install, so use the full path:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" devices
```

The phone should list as `<serial>  device`. `unauthorized` means step 3 was not accepted; `offline` usually means replugging or `adb kill-server` is needed.

5. Install (or reinstall over an existing build) the APK:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" install -r "Builds/Android/Stockwell.apk"
```

6. Watch the game's own logs while it runs:

```bash
"/c/Program Files/Unity/Hub/Editor/6000.3.24f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb.exe" logcat -s Unity:V
```

From Phase 1 onward, Unity's **Build And Run** does steps 5–6 in one click once the device is authorized. Adding the `platform-tools` folder to `PATH` shortens the commands above; they are written out in full here so the path is unambiguous.
