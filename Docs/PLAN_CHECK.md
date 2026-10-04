# PLAN_CHECK.md — review of CLAUDE.md

Phase 0 review, 2026-10-04. Gaps, risks and contradictions found in the plan while researching it, plus a coverage summary.

`CLAUDE.md` is a genuinely good brief: scope is tight, the out-of-scope list is honest, and the performance rules are stated as non-negotiable rather than aspirational. Most of what follows is detail it does not yet pin down, not disagreement with it.

---

## A. Things that conflict or need a decision

### A1. The checkout scanner is a core verb with no asset and no design

`CLAUDE.md` says "Player scans items at the checkout" and lists scanning in the core loop, but:

- **There is no free low-poly register or scanner prop.** This is one of the two hard asset gaps. Plan: author it in-house in Phase 3 (see `ASSETS.md`).
- **The scanning interaction is undefined.** Everything else routes through "one raycast interact/pick up/place system", but scanning is a repeated, timed, per-item action — plausibly the most-performed action in the game. Is it one tap per item? A drag across the scanner? A hold? This decides whether checkout is satisfying or tedious, and it affects the Phase 3 gate ("full buy-and-pay loop works").

**Recommendation:** decide at the start of Phase 3 and keep it inside the existing raycast interact system — one tap per item, scanner flashes, beep, price accumulates. No new input mechanism.

### A2. "No singletons beyond a small service locator or bootstrap" is underspecified

Nine systems are named (Economy, Inventory/Orders, Shelf/Slot, Customer AI, Checkout, Time/Day, Save, UI, Input) and they all need to reach each other. Without deciding the locator's shape now, this becomes nine ad-hoc `FindObjectOfType` calls by Phase 4.

**Recommendation:** settle it in Phase 1, while there is almost nothing to migrate — one `GameServices` bootstrap registering interfaces, resolved once in `Awake`. Cheap now, expensive at Phase 4.

### A3. Day cycle and store hours vs. "max ~8-10 active NPCs"

A day cycle with store hours implies busy and quiet periods, and progression implies more customers as the store grows. But the NPC cap is fixed at 8–10. If the store's growth is expressed as *more customers*, the cap is hit early and progression stops feeling like growth.

**Recommendation:** express progression as **throughput, not crowd size** — customers arrive more often and spend more, while concurrency stays capped. Spawn-rate and basket-size curves, not a higher cap. Worth stating explicitly in Phase 4 so the cap is never quietly raised to fake progress.

### A4. Pricing model needs a stated curve, not just a rule

"Each product has a market price; player price affects purchase chance" is the right mechanic but has no shape. A linear ramp makes pricing boring; a cliff makes it punishing and opaque.

**Recommendation:** Phase 2 picks an explicit curve (a sigmoid around market price is the usual answer) and puts its parameters on the product ScriptableObject so it is tunable without code. Needed before customers exist in Phase 3.

### A5. The Credits screen is now optional, which changes a Phase 5 assumption

`CLAUDE.md` says CC-BY assets get credited in an in-game Credits screen. **As planned, every recommended asset is CC0, so no attribution is legally required.** The Credits screen stops being a dependency and becomes a courtesy.

**Recommendation:** still build it in Phase 5 — crediting Kenney, Quaternius and pensamientoazul is good practice and free — but it no longer gates anything. If any CC-BY asset is added later, it becomes mandatory again. The attribution table at the end of `ASSETS.md` exists to generate it from.

---

## B. Gaps in the plan

### B1. No save-data migration story

Saving is JSON in the persistent data path with autosave on pause/background. But the save shape will change in every phase from 2 to 5, and the phone will be holding real saves from Phase 2 onward.

**Recommendation:** put a `version` int in the save file from the very first write in Phase 2, and fail closed (reset with a notice) on an unknown version rather than deserializing garbage. One field now avoids an unpleasant afternoon later.

### B2. "Log frame times each phase" has no defined method

A non-negotiable rule says to test on a real phone from Phase 1 and log frame times each phase, but nothing says where, or what counts as a pass beyond "30 fps stable".

**Recommendation:** add `Docs/PERF_LOG.md` with one row per phase: device, scene, median and 1%-low frame time, draw calls, tris, APK size. The MCP's `get_performance_stats` and `capture_game_view` make this nearly free. Define "stable" as **1% low ≥ 30 fps**, not average — average 30 fps with hitching feels broken, and a supermarket full of pooled products is exactly the shape of workload that hitches.

### B3. Nothing defines where the store ends and the city begins

The player spawns in a city block and enters one store. That boundary is the single biggest streaming/perf decision in the game, and it is unspecified. One scene or two? Is the city unloaded while inside?

**Recommendation:** decide in Phase 1, before the player controller locks assumptions in — most likely two scenes with an additive load at the door, so the city is fully unloaded while the player is inside. This protects the NPC cap and the draw-call budget, and the city is explicitly decorative, so unloading it costs nothing.

### B4. No input-remapping or aspect-ratio plan for a landscape phone game

Touch controls are specified (left joystick, right drag, three buttons) but not where they sit across aspect ratios from 16:9 to 21:9, nor behind notches/cutouts. A thumb-stick that lands under a cutout is a real bug on real phones.

**Recommendation:** anchor the touch UI to safe-area insets from the start in Phase 1, not as Phase 6 polish. Retrofitting safe areas after the HUD is built is the expensive order.

### B5. "Upgrades activate new tiles/slots" has no authoring model

In-place expansion via modular grid tiles is a good decision, but nothing says how tiles are authored — prefab variants, a tilemap, a runtime-generated grid? This determines whether Phase 4's expansion is an afternoon or a week.

**Recommendation:** Phase 2 defines the grid tile as a prefab with explicit shelf-slot anchors, so Phase 4's expansion is "enable the next tile group" rather than a new system.

---

## C. Risks

| Risk | Severity | Note |
|---|---|---|
| **itch.io supermarket pack is load-bearing, third-party-hosted, and off-style** | **high** | Only free source for retail shelving/counter/cart/freezers. A one-creator "name your own price" page can vanish. **Archive the 1.7 MB download now.** Mitigation and fallback in `ASSETS.md`. |
| **Food Kit as 200 individual prefabs on shelves** | **high** | The single biggest frame-rate risk in the game. Pooling + GPU instancing is already a rule; it must be honoured from the first shelf in Phase 2, not retrofitted. |
| **Unity licence entitlement errors in the editor log** | medium | Repeated `Licensing::Client ... 404 ... 0 entitlements`. Non-blocking today — the editor compiles and builds — but worth resolving before any signed release build. |
| **Git LFS free quota** | medium | 1 GB storage, 1 GB/month bandwidth. Estimated ~80–150 MB of trimmed assets fits, but every clone spends bandwidth. Watch, don't ignore. |
| **Mixamo access** | low | Already de-risked: Quaternius ships 24 CC0 animations with a humanoid rig, so Mixamo is optional. This was the brief's one external dependency and it is now gone. |
| **Freesound per-sound licences** | low | Freesound mixes CC0, CC-BY and CC-BY-NC. Verify each sound individually; CC-BY-NC is rejected outright. |
| **Style drift from mixing three authors** | low–medium | Kenney + Quaternius read as one direction. The itch.io pack does not, until re-materialled and rescaled. |
| **IL2CPP build times** | low | First Android IL2CPP build is slow. Expect multi-minute iteration; keep most testing in the editor and batch device tests per task. |

---

## D. Scope observations

**The out-of-scope list is well judged** — theft, trash, staff hiring, random events, multiplayer, iOS and an open-world city are exactly the features that would turn this from finishable into abandoned.

Two notes:

1. **"Staff hiring (beyond a simple cashier upgrade)" is doing more work than it looks.** A cashier upgrade means an NPC that operates the checkout — which means the checkout logic has to work without the player, which means it cannot be written as player-only code in Phase 3. Worth knowing in Phase 3, not Phase 4. This is the one out-of-scope carve-out with a real architectural consequence.

2. **Phase 5 is the heaviest phase and is positioned last-but-one.** Main menu, city block, ambient NPCs, audio *and* save/load is a lot, and save/load in particular touches every system built in Phases 2–4. Consider pulling save/load forward into Phase 4 — it is the item most likely to surface design problems in the economy, and the one most painful to discover late.

---

## E. Coverage summary

Which required items still have no good free asset.

### Hard gaps — nothing suitable found, author in-house

| Item | Category | Phase | Effort |
|---|---|---|---|
| Checkout register / barcode scanner | Store environment | 3 | low — box, screen quad, wedge |
| Shopping basket | Store environment | 3 | low — or cut to carts-only for v1 |

### Soft gaps — partially covered, finish in-house

| Item | Category | Phase | Note |
|---|---|---|---|
| Packaged, canned and household goods | Grocery products | 2 | thinnest 2 of 6 product categories; author 10–15 box/can/bottle SKUs on the shared atlas |
| Currency and store icons (price tag, truck, shelf, basket) | UI | 4 | coins covered; author the rest in the UI Pack's vector style |
| Small street props (benches, bins, streetlights) | City | 5 | some inside City Kits; fill individually from Poly Pizza CC0 |

### Fully covered

Walls · floors · doors · shelves · fridges/freezers · counters · delivery boxes · signage · produce · dairy · snacks · drinks · city buildings · roads · sidewalks · decorative vehicles · street trees · NPC variants · idle + walk animations · UI panels · UI icons · scanner beep · cash register · footsteps · UI clicks · box pickup · ambient loops · music stingers

### Verdict

**No phase is blocked by a missing asset.** Both hard gaps are single props that are faster to model than to keep searching for, both land in Phase 3, and neither affects Phase 1 or Phase 2. Every asset in the recommended set is CC0, which removes the attribution burden entirely and makes a public repository straightforward.

The items above that genuinely need a decision before coding starts are **A1** (scanning interaction), **A2** (service locator shape), **B3** (store/city scene boundary) and **B4** (safe-area anchoring) — all four land in Phase 1 or early Phase 2, and all four are cheaper to decide now than to refactor later.
