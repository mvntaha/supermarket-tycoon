# ASSETS.md — Stockwell asset plan

Phase 0 shortlist. Researched 2026-10-04. Nothing has been downloaded yet — this is the plan to approve before Phase 1 imports anything.

**Strategy: Kenney is the spine, Quaternius is the second voice, everything else is a patch.** Both are CC0, both are clean bright low-poly, and between them they cover every category except retail-specific fixtures. Keeping the count of authors low is what keeps the game looking like one game.

---

## Headline numbers

| | |
|---|---|
| Recommended packs | 17 (12 Kenney, 4 Quaternius, 1 itch.io) + per-sound Freesound picks |
| Distinct authors | 3 packs-authors + Freesound individuals |
| Licence spread | **CC0 for every recommended pick.** No CC-BY in the primary set. |
| Estimated raw download | **~300–400 MB** (estimate, not measured) |
| Estimated in-repo after trimming | **~80–150 MB** |
| Attribution burden | **None legally required** — see [Credits](#credits-consequence) |
| Categories with no good free asset | 2 hard gaps, 3 soft gaps — see [Coverage](#coverage-and-gaps) |

---

## 1. Store environment

Shelves, fridges, counters, checkout register, baskets/carts, delivery boxes, walls, floors, doors, signage.

| Pack | Source | Licence | Models | Format | URP | Covers |
|---|---|---|---|---|---|---|
| **Modular Buildings 2.1** ★ | [kenney.nl](https://kenney.nl/assets/modular-buildings) | CC0 | ~100 | OBJ/FBX/glTF | convert | walls, floors, doors, windows, roof, grid-modular shell |
| **Furniture Kit** ★ | [kenney.nl](https://kenney.nl/assets/furniture-kit) | CC0 | 140 (116 on Poly Pizza) | OBJ/FBX/glTF | convert | counters, cabinets, shelving, interior dressing |
| **Low Poly Supermarket pack** ★ | [pensamientoazul.itch.io](https://pensamientoazul.itch.io/supermarket-3d-assets) | CC0 (credit appreciated, not required) | 29 | FBX | convert | **shelves ×4, cashier counter, shopping cart, freezers ×2, glass door** |
| **Survival Kit** | [kenney.nl](https://kenney.nl/assets/survival-kit) | CC0 | 43 | OBJ/FBX/glTF | convert | crates and boxes → delivery boxes |
| Ultimate House Interior Pack *(backup)* | [quaternius.com](https://poly.pizza/u/Quaternius/Lists) | CC0 | 82 | FBX/OBJ/glTF/Blend | convert | interior fallback if Furniture Kit is thin |
| City Kit (Commercial) *(shared)* | [kenney.nl](https://kenney.nl/assets/city-kit-commercial) | CC0 | 50+ | OBJ/FBX/glTF | convert | shopfront signage, storefront pieces |

★ = recommended pick. Others are backup/supplementary.

**The one pack that is not Kenney or Quaternius is the one that matters most.** The itch.io supermarket pack is the only free source found that covers *retail* fixtures — supermarket shelving, a cashier counter, a shopping cart and glass-door freezers. Nothing in the Kenney or Quaternius libraries has these. It is therefore load-bearing and also the biggest style and quality risk in the plan. See [Risks](#risks).

**Hard gaps here:** checkout *register/scanner* prop, and shopping *basket*. See [Coverage](#coverage-and-gaps).

## 2. Grocery products

Packaged goods across drinks, snacks, dairy, produce, canned, household.

| Pack | Source | Licence | Models | Format | URP | Covers |
|---|---|---|---|---|---|---|
| **Food Kit 2.0** ★ | [kenney.nl](https://kenney.nl/assets/food-kit) | CC0 | 200 (148 on Poly Pizza) | OBJ/FBX/glTF | convert | produce, prepared food, kitchen items — the bulk of the catalogue |
| **Ultimate Food Pack** ★ | [quaternius.com](https://poly.pizza/u/Quaternius/Lists) | CC0 | 50 | FBX/OBJ/glTF/Blend | convert | second style of food, fills Kenney's thin spots |
| **Supermarket pack products** ★ | [pensamientoazul.itch.io](https://pensamientoazul.itch.io/supermarket-3d-assets) | CC0 | 19 of the 29 | FBX | convert | **milk, eggs, cheese, pasta box, coffee, juice, soap, chocolate, bread, ice cream** |

**Honest warning on this category.** Kenney's Food Kit is a *kitchen and produce* kit — fruit, veg, prepared dishes, utensils. It is excellent and large, but it skews away from the shelf-ready *packaged* goods a supermarket sim actually sells: cereal boxes, tin cans, bottled drinks in multipacks, cleaning products, toiletries. The itch.io pack supplies roughly ten packaged SKUs and Quaternius adds more food, but **"household/cleaning" and "canned" remain the thinnest two of the six product categories** named in `CLAUDE.md`.

This is cheap to fix and should not block Phase 1: packaged goods are boxes, cans and bottles — the simplest possible low-poly primitives. The recommendation is to author ~10–15 of them in-house as a shared-atlas product kit during Phase 2, which also guarantees they share one material. Logged as a soft gap rather than a blocker.

## 3. City

Decorative only — the city block is scenery, the store is the game.

| Pack | Source | Licence | Models | Format | URP | Covers |
|---|---|---|---|---|---|---|
| **City Kit (Roads)** ★ | [kenney.nl](https://kenney.nl/assets/city-kit-roads) | CC0 | 90 | OBJ/FBX/glTF | convert | roads, junctions, crossings, sidewalks |
| **City Kit (Commercial)** ★ | [kenney.nl](https://kenney.nl/assets/city-kit-commercial) | CC0 | 50+ | OBJ/FBX/glTF | convert | decorative shops, storefronts, signage |
| **City Kit (Suburban)** ★ | [kenney.nl](https://kenney.nl/assets/city-kit-suburban) | CC0 | 40 | OBJ/FBX/glTF | convert | houses, colour variations |
| **Car Kit** ★ | [kenney.nl](https://kenney.nl/assets/car-kit) | CC0 | 40+ | OBJ/FBX/glTF | convert | decorative parked/passing vehicles |
| Nature Kit | [kenney.nl](https://kenney.nl/assets/nature-kit) | CC0 | 150+ | OBJ/FBX/glTF | convert | street trees, planters |
| Cars Bundle *(backup)* | [quaternius.com](https://poly.pizza/u/Quaternius/Lists) | CC0 | 7 | FBX/OBJ/glTF/Blend | convert | vehicle fallback |

Three City Kits plus Car Kit plus Nature Kit is a complete, single-author, style-locked city block for free. **Deliberately rejected:** Quaternius *Ultimate Buildings Pack* — it is the textured variant, which means real texture memory for scenery the player only walks past. Kenney's city kits use a tiny shared colormap instead. Wrong cost/benefit for decoration on a phone.

**Soft gap:** small street props — benches, bins, streetlights, bus stops. Some arrive inside the City Kits; anything still missing is a one-model-at-a-time fill from [Poly Pizza](https://poly.pizza) (filter CC0), not another whole pack.

## 4. Characters

| Pack | Source | Licence | Content | Format | URP | Notes |
|---|---|---|---|---|---|---|
| **Ultimate Modular Men Pack** ★ | [quaternius.com](https://quaternius.com/packs/ultimatemodularcharacters.html) | CC0 | 11 characters, **24 animations** | FBX/OBJ/glTF/Blend | convert | 4 swappable parts per character |
| **Ultimate Modular Women Pack** ★ | [quaternius.com](https://quaternius.com/packs/ultimatemodularwomen.html) | CC0 | 10 characters, **24 animations**, **humanoid rig included** | FBX/OBJ/glTF/Blend | convert | 4 swappable parts per character |
| Mixamo *(backup / extra clips)* | [mixamo.com](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html) | Adobe: royalty-free commercial, **may not be resold standalone** | animation clips | FBX | n/a | free with an Adobe ID, no CC subscription needed |
| Blocky Characters *(rejected)* | [kenney.nl](https://kenney.nl/assets/blocky-characters) | CC0 | 20, animated | OBJ/FBX/glTF | convert | **do not mix** — see Risks |

**This is the strongest pick in the plan and it quietly removes a dependency.** The brief asks for NPC variants "plus animations (idle, walk) via Mixamo". The Quaternius modular packs ship **24 animations already**, under CC0, with a humanoid rig — so Mixamo is demoted from required to optional. That matters: Mixamo is free today, but Adobe has been tightening its Creative Cloud coupling since 2024 and some non-subscribers have seen access disrupted, and its licence forbids redistributing clips standalone. Depending on it for the core walk cycle would be the single most fragile link in the asset chain. We no longer have to.

21 base characters × 4 swappable parts comfortably exceeds the ~8–10 simultaneous NPCs the performance rules allow, so NPC variety is solved.

## 5. UI

| Pack | Source | Licence | Content | Notes |
|---|---|---|---|---|
| **UI Pack** ★ | [kenney.nl](https://kenney.nl/assets/ui-pack) | CC0 | 430 assets: PNG (2 sizes) + **SVG vector** + 2 TTF fonts + 6 UI SFX | **rounded panels** — matches the UI spec in `CLAUDE.md` directly |
| **Game Icons** ★ | [kenney.nl](https://kenney.nl/assets/game-icons) | CC0 | 105 | gamepad, prompt, interface icons |
| **Game Icons (Expansion)** ★ | [kenney.nl](https://kenney.nl/assets/game-icons-expansion) | CC0 | — | more interface icons |
| Board Game Icons | [kenney-assets.itch.io](https://kenney-assets.itch.io/board-game-icons) | CC0 | — | coin/money/resource icons |
| UI Pack: RPG Extension | [opengameart.org](https://opengameart.org/content/ui-pack-rpg-extension) | CC0 | 87 | extra panel/frame styles |

The SVG vectors are the detail worth noting: touch targets must be ≥48 dp across unknown phone densities, and vector sources mean panels and buttons can be re-exported at the right density instead of being upscaled into mush.

**Soft gap:** currency and store-specific icons (price tag, delivery truck, shelf, basket). Board Game Icons covers coins; the rest is a handful of flat icons, cheapest authored in-house from the UI Pack's own vector style so they match.

## 6. Audio

| Pack | Source | Licence | Content | Covers |
|---|---|---|---|---|
| **Impact Sounds** ★ | [kenney.nl](https://kenney.nl/assets/impact-sounds) | CC0 | 130, WAV + OGG | **footsteps** (carpet, concrete, grass, snow, wood), box pickup/drop thuds |
| **Interface Sounds** ★ | [kenney.nl](https://kenney.nl/assets/interface-sounds) | CC0 | 100 | **UI clicks**, switches |
| **UI Audio** ★ | [kenney.nl](https://kenney.nl/assets/ui-audio) | CC0 | 50 | buttons, switches, generic clicks |
| **Music Jingles** ★ | [kenney.nl](https://kenney.nl/assets/music-jingles) | CC0 | 85 | end-of-day summary stingers, unlock/purchase flourishes |
| **Abstraction Music Loop Bundle** ★ | via [OpenGameArt](https://opengameart.org/content/cc0-calm-relaxing-music) | CC0 | 200+ loops | **ambient / background music loops** |
| CC0 Calm / Relaxing Music *(backup)* | [opengameart.org](https://opengameart.org/content/cc0-calm-relaxing-music) | CC0 | collection | in-store ambience fallback |
| **Store Scanner Beep** ★ | [freesound.org #144418](https://freesound.org/people/zerolagtime/sounds/144418/) | **CC0** | 1 sound | **scanner beep** — noise already removed, intended as checkout foley |
| scanner beep.wav *(backup)* | [freesound.org #202530](https://freesound.org/people/kalisemorrison/sounds/202530/) | **CC0** | 1 sound | scanner beep alternate |
| Beep of a Cash Register #1 | [bigsoundbank.com](https://bigsoundbank.com/beep-of-a-cash-register-s1417.html) | CC0 | 1 sound | **cash register** |
| Cash register beeping | [freesound.org #555122](https://freesound.org/people/Garuda1982/sounds/555122/) | **verify before use** | 1 sound | register alternate — licence not confirmed CC0 |

Every SFX the brief names is covered: scanner beep, cash register, footsteps, UI clicks, box pickup.

> **Freesound rule: licences are per-sound, not per-site.** Freesound hosts CC0, CC-BY and CC-BY-NC side by side. The two scanner beeps above were confirmed CC0; `#555122` was **not** confirmed and must be checked on its own page before it is used. Any CC-BY sound that gets used lands in the Credits screen; any CC-BY-NC sound is rejected outright under the "free only, commercial-safe" rule.

**Trim this one.** The Abstraction bundle is 200+ loops and the single largest download in the plan. The store needs maybe 3–5 loops plus a menu track. Import only the chosen files; do not drop the bundle into `Assets/`.

---

## Licence table

| Licence | Packs | Attribution | Commercial | Verdict |
|---|---|---|---|---|
| **CC0** | all 12 Kenney packs, all 4 Quaternius packs, itch.io supermarket pack, Abstraction loops, OpenGameArt picks, Freesound #144418 / #202530, BigSoundBank register | **none required** | yes | **the whole primary set** |
| CC-BY | none currently selected | credit in Credits screen | yes | allowed, avoided so far |
| Adobe Mixamo | Mixamo (backup only) | none | yes, but **no standalone resale** of clips | optional; avoid depending on it |
| CC-BY-NC / non-commercial | — | — | **no** | **rejected per `CLAUDE.md`** |
| Unity Asset Store "free" | — | per-asset EULA | varies | **none selected** — see note |

**Note on the Unity Asset Store.** It was checked and nothing from it is recommended. Asset Store free assets carry the Asset Store EULA rather than a Creative Commons licence, which makes per-asset licence tracking in this file harder, and they cannot be redistributed in a public git repository the way CC0 files can. Given this repo is public, CC0 sources are strictly better. `Food & Grocery Items - Low Poly` was the one tempting candidate and is logged as an emergency backup only.

### Credits consequence

`CLAUDE.md` says CC-BY assets are credited in an in-game Credits screen. **As planned, every asset is CC0, so no attribution is legally required and the Credits screen is not a blocker for any phase.** It should still be built in Phase 5 — crediting Kenney, Quaternius and pensamientoazul voluntarily is good practice and costs nothing — but it is now a courtesy, not a dependency. If a CC-BY asset is ever added later, it becomes mandatory again.

---

## URP compatibility — the one thing that applies to everything

**Every 3D pack in this plan reads as "needs material conversion", and that is normal, not a defect.**

These packs ship OBJ/FBX/glTF with simple diffuse textures or vertex colours. Unity imports them against the Built-in `Standard` shader. In a URP project, Built-in shaders render **magenta**. The first import will therefore look broken, and that is expected.

Fix, once per pack, at import:

1. Import under `Assets/ThirdParty/<Source>/`.
2. **Window → Rendering → Render Pipeline Converter → Built-in to URP**, convert materials.
3. Verify the colormap texture is assigned to `Base Map` on the resulting `URP/Lit` material.
4. For anything that does not need specular response, switch to `URP/Simple Lit` or unlit — cheaper on a phone and visually identical for flat low-poly.

No pack was found to be *incompatible* with URP. None uses a custom shader, HDRP-only node graph, or Built-in-only feature that would actually block it. "Convert" in the tables above means step 2, not a porting effort.

## Mobile cost assessment

What is cheap here, and what to watch:

**Kenney 3D kits are close to ideal for this project.** Each kit ships a single small shared colormap texture that every model in the kit samples. One kit therefore collapses to **one material and one texture**, which is exactly the "shared material atlases to keep draw calls low" rule, achieved for free rather than by building atlases by hand. This is the main reason to lean on Kenney over richer-looking alternatives.

Things that will cost real frames or megabytes if left alone:

| Risk | Cost | Mitigation |
|---|---|---|
| Food Kit as 200 separate prefabs on shelves | draw calls explode; this is the single biggest perf risk in the game | pool + GPU-instance products, per the architecture rules. Never one plain GameObject per product on a shelf. |
| itch.io supermarket pack has **no shared atlas** and inconsistent scale | extra materials, extra draw calls, broken proportions | remap all 29 to the shared store atlas and normalise scale at import — treat this as a required Phase 2 task, not optional polish |
| Quaternius *Ultimate Buildings* (textured) | real texture memory for background scenery | **rejected**; use Kenney city kits |
| Abstraction bundle, 200+ loops | largest single download, mostly unused | import only the 3–5 loops actually used |
| Quaternius modular characters, 21 × 4 parts, skinned + 24 clips | skinned mesh cost and animator overhead | cap at ~8–10 active NPCs (already a rule); share one animator controller; consider merging the 4 parts per spawned variant |
| Audio imported as WAV | memory | Kenney ships OGG too — prefer OGG, and set **Load In Background / Compressed In Memory** for music, **Decompress On Load** only for short SFX |

## Risks

**Style mismatch — one real risk, one avoided, one rejected.**

1. **Kenney vs Quaternius (low risk, acceptable).** Both are clean bright flat-shaded low-poly. Kenney runs slightly smoother and more rounded; Quaternius slightly chunkier. Mixed in one scene they read as the same art direction. This is the intended pairing.

2. **The itch.io supermarket pack (the real risk).** A third author, a third style, explicitly documented by its creator as having inconsistent scale: *"they do not have the same size or consistent scale."* It is also the pack we cannot do without, since it holds the only retail shelving, counter, cart and freezers. **Mitigation is mandatory, not optional:** normalise scale on import and re-material every piece onto the store's shared atlas. Re-materialled and rescaled, it should sit acceptably next to Kenney. If after Phase 2 it still looks foreign, the fallback is to rebuild the ~6 fixtures in-house — they are boxes with slots, genuinely low-effort geometry — and keep the pack only as reference.

3. **Kenney Blocky Characters vs Quaternius characters (rejected combination).** Blocky Characters are Minecraft-style rectangular figures; Quaternius characters are smooth stylised humans. These do not coexist — mixing them makes the NPC crowd look like a bug. **Pick one and commit; the recommendation is Quaternius**, for the 24 bundled animations and the humanoid rig. Blocky Characters is listed as a wholesale alternative, never a supplement.

**Other risks**

- **Mixamo access.** Free today with an Adobe ID, but Adobe has been tightening Creative Cloud coupling since 2024. Already de-risked: Quaternius supplies the animations, Mixamo is a bonus.
- **The itch.io pack is single-point-of-failure hosting.** A "name your own price" itch.io page by one creator can disappear. It is 1.7 MB. **Archive the download now**, while Phase 0 is still open, rather than discovering it is gone in Phase 2.
- **Freesound per-sound licences** — see the audio note above.
- **Git LFS quota.** GitHub's free tier is 1 GB storage and 1 GB/month bandwidth. The estimated ~80–150 MB of trimmed assets fits comfortably, but LFS bandwidth is consumed by every clone and CI checkout, so it is worth watching rather than ignoring.

---

## Coverage and gaps

Measured against every item `CLAUDE.md` and the Phase 0 brief name.

### Fully covered

Walls · floors · doors · shelves · fridges/freezers · counters · delivery boxes · signage · produce · dairy · snacks · drinks · city buildings · roads · sidewalks · decorative vehicles · street trees · NPC variants · idle + walk animations · UI panels · UI icons · scanner beep · cash register · footsteps · UI clicks · box pickup · ambient loops · music stingers

### Hard gaps — no good free asset found

| Item | Why it matters | Resolution |
|---|---|---|
| **Checkout register / barcode scanner prop** | the player scans items at checkout; it is a core interaction, in shot constantly | **author in-house in Phase 3.** A register is a box, a screen quad and a wedge scanner — a genuinely small modelling job, and in-house means it matches the store atlas exactly. |
| **Shopping basket** | carts exist (itch.io pack), hand baskets do not; customers carrying baskets reads better than 10 carts in a small store | **author in-house in Phase 3,** or cut to carts-only for v1. Low risk either way. |

Both hard gaps are small single props, both land in Phase 3, and neither blocks Phase 1 or Phase 2.

### Soft gaps — partially covered, cheap to finish

| Item | State | Resolution |
|---|---|---|
| **Packaged / canned / household goods** | thinnest of the six product categories; ~10 SKUs from itch.io, Kenney skews to produce | author 10–15 box/can/bottle SKUs on the shared atlas in Phase 2 |
| **Currency and store icons** | coins covered by Board Game Icons; price tag, truck, shelf, basket icons missing | author from the UI Pack's own vector style in Phase 4 |
| **Small street props** (benches, bins, streetlights) | some inside City Kits, inventory unconfirmed | fill individually from Poly Pizza CC0 after the City Kits are imported |

**Nothing in the plan is blocked on a missing asset.** Both hard gaps are single props that are faster to model than to search for, and the brief's one external dependency — Mixamo — has been eliminated.

---

## Name check

| Title | Steam | itch.io | Verdict |
|---|---|---|---|
| **Stockwell** | no game of this name; only *The Bus — London South* (a bus route DLC covering the Stockwell district of London) | no match | **clear — keep it** |

No clash, so `Shelf Life` and `Corner Cart` were not needed. Worth knowing that *Stockwell* is a real London Underground station and district, which is why transport titles surface in search — but no game carries the name. Google Play could not be searched programmatically; a manual check there is advisable before any store listing, though it has no bearing on development.

---

## Download order for Phase 1

Only what Phase 1 actually needs, so the first on-device build stays small:

1. Kenney **Modular Buildings** — the store shell to walk around in
2. Kenney **UI Pack** — touch controls
3. Kenney **Impact Sounds** — footsteps
4. **Archive the itch.io supermarket pack now** (hosting risk, 1.7 MB)

Everything else arrives at the phase that needs it: products and fixtures in Phase 2, characters in Phase 3, the city and the remaining audio in Phase 5.

## Attribution record

Filled in as packs are imported, so the Credits screen can be generated from this table.

| Pack | Author | Licence | Version | Imported | Download size | Credit required | Credit line |
|---|---|---|---|---|---|---|---|
| Modular Buildings | Kenney | CC0 1.0 | 2.1 (2024-02-08) | Phase 1 | 1,825,490 B (1.74 MB) | **No** | Kenney — www.kenney.nl |
| UI Pack | Kenney | CC0 1.0 | — | Phase 1 | 1,229,750 B (1.17 MB) | **No** | Kenney — www.kenney.nl |
| Impact Sounds | Kenney | CC0 1.0 | — | Phase 1 (subset) | 800,850 B (0.76 MB) | **No** | Kenney — www.kenney.nl |

Total Phase 1 import: **3.67 MB** downloaded. Credit is voluntary for all three — the
bundled `License.txt` states plainly that crediting Kenney "is not a requirement".

### Verified at import

- **Modular Buildings** — 108 models each in FBX, OBJ and GLB, plus `Models/Textures/`
  containing **exactly two PNGs** (`variation-a.png`, `variation-b.png`) shared by every
  model. This confirms the one-material-per-kit assumption the mobile cost assessment
  depends on: the whole building set collapses to two materials.
- **Impact Sounds** — 130 clips, **all already OGG** (no WAV in the pack, so no
  conversion needed). Footsteps ship as 5 variants each for carpet, concrete, grass,
  snow and wood. Imported subset only, per the Phase 1 brief.
- **UI Pack** — PNG at two densities plus SVG vector sources, 2 TTF fonts, 6 UI SFX.

### Impact Sounds — imported subset

Only the files actually used, rather than all 130:

| Files | Use |
|---|---|
| `footstep_concrete_000`–`004` | City scene footsteps |
| `footstep_wood_000`–`004` | Store scene footsteps |
| `impactGeneric_light_000`–`004` | box pickup / thud, for Phase 2 delivery boxes |

15 clips. The pack has no `impactWood`; `impactGeneric_light` is the closest match for
a cardboard box and is the lightest-sounding of the available impact families.
