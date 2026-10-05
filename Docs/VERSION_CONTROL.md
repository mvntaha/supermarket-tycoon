# VERSION_CONTROL.md — branching and release model

Repository: <https://github.com/mvntaha/supermarket-tycoon> (public)

The branching model mirrors the phase gates in `CLAUDE.md`, so the git history reads as the step-by-step build of the game.

## Branches

| Branch | Role | Who merges into it |
|---|---|---|
| `main` | **Gated.** Only ever holds approved phase gates. Every commit on `main` is a state the user signed off on, and each is tagged. | `develop`, via PR, only after a phase gate is approved |
| `develop` | Integration. Completed tasks land here between gates. Should always compile. | `phase/*` branches, via PR |
| `phase/<n>-<name>` | All actual work for one phase. One branch per phase. | — (work happens here) |

Current branches:

```
main                ← phase gates only, tagged. At tag phase-0-research.
develop             ← integration. Phase 0 merged.
phase/0-research    ← Phase 0 work. Merged via PR #1; kept for history.
phase/1-foundation  ← Phase 1 work (current branch)
```

| Branch | Merged | Tag |
|---|---|---|
| `phase/0-research` → `develop` | PR #1, merge `7cdc49b` | — |
| `develop` → `main` | PR #2, merge `3a03422` | `phase-0-research` |
| `phase/1-foundation` → `develop` | open | — |

## The loop, per phase

1. Branch `phase/<n>-<name>` off `develop`.
2. Work. **Commit at the end of every task** (`CLAUDE.md` working rule), each commit scoped to one task.
3. Open a PR `phase/<n>-...` → `develop` when the phase's work is done.
4. Present the exit gate to the user. **Do not merge before approval** — the gate is the user's decision, and the open PR is what makes that decision reviewable.
5. On approval: merge to `develop`, then PR `develop` → `main`, merge, and tag.
6. Branch the next phase off `develop`.

This puts a reviewable diff in front of the user at every gate rather than a fait accompli, which is the whole point of the gates.

## Tags

One annotated tag per approved gate, on `main`:

| Tag | Phase | Gate condition (from `CLAUDE.md`) |
|---|---|---|
| `phase-0-research` | 0 | user approves the asset + toolchain checklist |
| `phase-1-foundation` | 1 | APK runs on phone, can walk and look |
| `phase-2-store-core` | 2 | order, stock, price items |
| `phase-3-customers` | 3 | full buy-and-pay loop works |
| `phase-4-economy` | 4 | real "one more day" loop |
| `phase-5-world-menu` | 5 | start-to-finish flow works |
| `phase-6-polish` | 6 | stable on-device build |

```bash
git tag -a phase-0-research -m "Phase 0 gate: asset plan and toolchain approved"
git push origin phase-0-research
```

## Commit convention

`type: short imperative summary`, then a body explaining *why* where it is not obvious.

Types in use: `chore`, `feat`, `fix`, `docs`, `perf`, `refactor`, `art` (asset imports), `build`.

Commits are co-authored by Claude where Claude wrote them.

## Rules that matter for a Unity repo

- **Never commit `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `*.csproj`, `*.sln`.** All in `.gitignore`.
- **Never commit a keystore.** `*.keystore`, `*.jks` and `keystore.properties` are gitignored. Signing material for a Play release stays off the repo entirely — it is public.
- **Binary art and audio go through Git LFS** (`.gitattributes`). Verified working. Third-party packs land under `Assets/ThirdParty/<Source>/` and are LFS-tracked automatically by extension.
- **Unity YAML assets use `merge=unityyamlmerge`.** Scenes and prefabs must never be auto-merged by git's line-based merge — it silently corrupts them. If two branches touch one scene, one side rebases.
- **`.meta` files are committed.** Dropping them breaks every reference in the project.
- **LFS quota watch.** GitHub free tier gives 1 GB storage and 1 GB/month bandwidth. The asset plan estimates ~80–150 MB, which fits, but every clone spends bandwidth.

## Build output

APKs and AABs are gitignored. Builds are local artefacts, reproducible from source; they do not belong in a public repo and would burn LFS quota.
