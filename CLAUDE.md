# CLAUDE.md — Stockwell (working title)

Mobile-first, first-person, low-poly 3D supermarket tycoon in Unity 6. Personal/portfolio project. Keep it simple, playable and finished rather than complex.

## Game summary
Player starts at a main menu, then spawns in a small city block with ambient NPCs and decorative shops. One shop is the player's own supermarket, which is the only enterable store. Player starts with some cash, orders stock, carries boxes to shelves, sets prices. NPC customers browse, queue and pay. Player scans items at the checkout. Money is spent on stock, shelves, store expansion and upgrades. The store upgrades in place (no separate buildings).

## Core loop
Order stock (phone/computer in store) -> delivery boxes arrive -> stock shelves and set prices -> customers shop, queue, pay -> earn money -> spend on products, shelves, space, checkout, staff.

## Locked decisions
- Engine: Unity 6, URP, mobile quality profile.
- Platform: Android first (tested on an Android 12 phone). PC build uses the same input actions.
- Perspective: first-person, landscape.
- Assets: free only, low-poly, URP-compatible. Track license per asset in `Docs/ASSETS.md`. CC-BY assets are credited in an in-game Credits screen.
- Single player, English only, Android only (no iOS).
- Store: one internal supermarket built from modular grid tiles; upgrades activate new tiles/slots.
- Art: clean, bright low-poly. UI: rounded panels, touch targets >= 48dp, minimal text.

## Mobile performance rules (non-negotiable)
- Target 30 fps stable on a mid-range Android phone.
- ARM64 only, IL2CPP, Vulkan with OpenGL ES 3 fallback.
- Baked lighting; no or minimal realtime shadows.
- Shared material atlases to keep draw calls low.
- Products are pooled and instanced. Never spawn per-product heavy objects.
- Max ~8-10 active NPCs in store, fewer in city. Simple NavMesh agents.
- Test on a real phone from Phase 1 and log frame times each phase.

## Architecture
- Data-driven: ScriptableObjects for products (name, price, size, category, prefab, unlock level) and upgrades.
- Systems: Economy, Inventory/Orders, Shelf/Slot, Customer AI, Checkout, Time/Day, Save, UI, Input.
- Customer AI: state machine (Enter -> Pick items -> Queue -> Pay -> Leave) on NavMesh.
- Interaction: one raycast interact/pick up/place system used for everything.
- Input: Unity Input System, one action map. Touch: left joystick move, right-side drag look, buttons for Interact, Pick up/Drop, Place. PC maps keyboard/mouse to the same actions.
- Saving: JSON in persistent data path, autosave on pause/background.
- Pricing: each product has a market price; player price affects purchase chance.
- Day cycle with store hours and an end-of-day summary (revenue, expenses, profit).

## Folder conventions
`Assets/_Project/{Scripts,Prefabs,Data,Scenes,UI,Art,Audio}`, third-party under `Assets/ThirdParty/<Source>/`. Namespaces per system. One responsibility per script. No singletons beyond a small service locator or bootstrap.

## Decisions from Phase 0 review
- Scanning is one tap per item inside the raycast interact system.
- One `GameServices` bootstrap registers service interfaces; no other singletons.
- Progression raises customer spawn rate and basket size; the active NPC cap is never raised.
- Price-to-purchase-chance uses a sigmoid around market price; parameters live on the product ScriptableObject.
- Save file carries a `version` int from the first write; unknown version resets with a notice.
- "Stable" means 1% low frame rate >= 30 fps, logged per phase in `Docs/PERF_LOG.md`.
- City and Store are separate scenes; Store loads additively and City unloads while inside.
- Touch UI anchors to safe-area insets.
- Grid tiles are prefabs with explicit shelf-slot anchors.
- Checkout logic must not depend on the player (a cashier NPC will operate it).

## Phases (each has an exit gate; do not start the next until the user approves)
0. Research: asset shortlist (source, license, poly count, URP status), Unity MCP + Android toolchain verified. Gate: user approves checklist.
1. Foundation: project, git, mobile build pipeline, player controller with touch + PC. Gate: APK runs on phone, can walk and look.
2. Store core: product data, shelves, ordering, delivery boxes, pricing. Gate: order, stock, price items.
3. Customers and checkout: NPC state machine, queue, payment. Gate: full buy-and-pay loop works.
4. Economy and progression: day cycle, summary, upgrades, unlocks, in-place expansion, save/load. Gate: real "one more day" loop.
5. World and menu: main menu, city block, ambient NPCs, audio, credits screen. Gate: start-to-finish flow works.
6. Optimization and polish: profiling, bugs, UI pass, final build. Gate: stable on-device build.

## Out of scope for v1
Theft, trash cleanup, staff hiring (beyond a simple cashier upgrade), random events, multiplayer, iOS, open-world city.

## Working rules
- Git from day one with a Unity `.gitignore`; commit at the end of every task; tag each phase gate.
- Use the Unity MCP to inspect and edit the project; verify compile state after every script change.
- Prefer execution over option menus: decide, state the decision, proceed. Ask only when a choice changes scope or cost.
- Keep reports short: what changed, what to test, what is next.
- Update `Docs/ASSETS.md` and `Docs/PROGRESS.md` whenever assets or phase status change.
