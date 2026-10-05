# PERF_LOG.md — frame times per phase

One row per phase per scene, measured on a real device. Required by the mobile
performance rules in `CLAUDE.md` ("test on a real phone from Phase 1 and log frame
times each phase").

**"Stable" means 1% low >= 30 fps**, not average — a 30 fps average with hitching
feels broken, and a store full of pooled products is exactly the workload that hitches.
Measured with the in-game `PerfOverlay` (development builds only), which reports the
median and the 99th-percentile frame time over a rolling 1000-frame window.

`Application.targetFrameRate` is **60** during Phases 1–5 so there is headroom to
measure against. Dropping it to 30 is a Phase 6 decision.

## How to read a row

- **median ms** — typical frame time. Lower is better.
- **1% low fps** — frame rate of the slowest 1% of frames. **This is the gate number.**
- **draw calls / tris** — from `ProfilerRecorder`; `n/a` on a release build where the
  Profiler module is stripped.
- **APK size** — the installed artifact, for tracking bloat phase over phase.

## Log

| Phase | Date | Device | Android | Scene | Median ms | 1% low fps | Draw calls | Tris | APK size |
|---|---|---|---|---|---|---|---|---|---|
| 1 | _pending_ | _pending_ | _pending_ | City | | | | | 36.78 MiB |
| 1 | _pending_ | _pending_ | _pending_ | Store | | | | | 36.78 MiB |

> **Both rows are deliberately empty.** They are filled from the on-device run in Task 8,
> which the user runs themselves. The APK exists
> (`Builds/Android/Stockwell-phase1-dev.apk`, 38,561,425 B / 36.78 MiB, development
> build) but no phone was reachable, so there are no device numbers yet.
>
> The editor smoke test measured City at 12 draw calls / 3,399 tris and Store at 11 /
> 2,013, recorded in `PROGRESS.md`. **Those are not device numbers and must not be copied
> into this table** — an editor on a desktop GPU tells you nothing about a phone's frame
> time. Draw calls and triangle counts do carry over; frame times do not.
>
> See the "Device test checklist" in `PROGRESS.md` for how to take the readings.
## Notes

- Readings are taken after ~20 s of walking in the scene, so the rolling window is full.
- Reset the overlay's window (`PerfOverlay.ResetSamples`) when changing scene, or the
  previous scene's frames skew the percentile.
- Record the worst of three runs, not the best. The gate is about the bad frames.
