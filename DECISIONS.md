Battle Forge — Decisions Log

Record real design and technical decisions, with the newest entries first.

2026-11-19 — Services behind interfaces, started by a Bootstrap scope (VContainer)

Options: Static `Services` class (as in Lesson 3) / VContainer root scope with constructor injection / Singleton MonoBehaviours

Chose: Every service is an interface in its own Services assembly (`ISaveService`, `IWalletService`) with one implementation registered in `BootstrapLifetimeScope`. Bootstrap initializes the services in registration order, then loads Home. Scene scopes take the Bootstrap scope as their parent, and controllers get the services in their constructors.

Why: Our screens already use VContainer, so services arrive the same way as everything else: one way to get dependencies and no global state. A controller's constructor shows exactly what it needs, and tests can pass a fake (`WalletServiceTests` uses an in-memory save). Save goes through `ISaveService`, so moving from PlayerPrefs to a cloud save later is one new class and one line in Bootstrap, with no change to any screen. The generic `Save<T>` / `Load<T>` stores any serializable data (Cash, settings, progress) with the same methods.

AI: Claude proposed the VContainer root scope and the generic save API instead of the lesson's static `Services` class. We checked the VContainer APIs against its source, and the EditMode tests and Play mode confirmed it works.

Who: Alan (implemented); Eden (screen architecture with VContainer)

2026-11-19 — One game scene: Level becomes a screen in Home

Options: Separate Home and Level scenes with a scene flow / Level as a screen inside Home

Chose: Keep one game scene. `LevelView` is a screen in Home: Battle pushes it and Back pops it, through the same screen navigator as the menu tabs. The Level scene and `ScreenFlow` were removed; the Scene List is Bootstrap, Home.

Why: The navigator already handles every other screen, so the battle uses the same system instead of a second one. A battle will be reset by its own logic rather than by reloading a scene, and nothing has to be passed between scenes.

AI: Claude first built Home and Level as two scenes, following the lesson slides. Eden pointed out that `ScreenFlow` needed a new method for every new scene and that a reset should come from logic; we merged Eden's per-screen MVC refactor and moved Level into Home.

Who: Eden (proposed); Alan (agreed, wired the scene)

2026-11-09 — Add Basic Units to reduce crafting pressure

Options: Crafting-only army / Reusable Basic Units / Pause combat during crafting

Chose: Add two reusable Basic Unit buttons. Basic Units are regular soldiers that can attack and defend without crafting. Each summon costs Gold, and the price increases with repeated summons of the same unit type. Prices reset each battle.

Why: Crafting during real-time combat may be stressful on a portrait phone. Basic Units give players a quick way to respond to enemies while preserving crafting for more advanced troops and tactical cards. Increasing costs discourage relying only on Basic Units.

AI: Suggested testing Basic Units as a way to reduce crafting pressure, initially recommending simple defensive units. We refined the idea so they can also attack and destroy the enemy base, with increasing Gold costs to maintain balance.

Who: Eden (proposed); Alan (pending confirmation)

2026-11-09 — Expand Meta progression beyond recipe purchases

Options: Recipes only / Recipes and unit upgrades / Additional currencies and progression systems

Chose: Allow players to spend earned Cash on both crafting recipes and permanent soldier-level upgrades. Each soldier type has its own level, with small HP and attack-damage increases. Cosmetic skins are reserved for future development.

Why: The instructor pointed out that Cash would become useless after purchasing every recipe. Unit upgrades provide another reason to earn Cash and improve progression without introducing a second currency.

AI: Suggested additional uses for Cash, including permanent base and income upgrades. We chose individual soldier leveling instead to make progression more directly connected to combat.

Who: Eden (proposed); Alan (pending confirmation)

2026-11-09 — Add reward chests and advertising integration

Options: Battle rewards only / Reward chests / Reward chests with optional rewarded ads

Chose: Plan free reward chests for gameplay achievements and optional ad chests opened by watching rewarded ads. Chests initially reward Cash or other existing progression rewards. Skins may be added as future rewards.

Why: Chests add variety to progression and provide a natural way to integrate the rewarded advertisements required later in the course. Ads should remain optional for chest rewards.

AI: Suggested separating free chests, rewarded-ad chests and special boss chests. It also recommended postponing chest implementation until the core gameplay works.

Who: Eden (proposed); Alan (pending confirmation)

2026-11-09 — Build a smaller first playable version

Options: Build the complete game immediately / Build a smaller playable version first / Reduce the final game vision permanently

Chose: Keep the planned full game while developing a smaller first playable version: 3 stages, 4 materials and 6–9 recipes. Keep the 3×3 crafting algorithm with position-independent matching and horizontal mirroring from the beginning. Defer Recent Recipes and the in-battle Recipe Book.

Why: The instructor approved the concept but warned that balancing all the planned content could take too much time. A smaller version lets us verify the core gameplay before expanding to more stages and recipes.

AI: Recommended separating the Greybox prototype, first playable version and full game vision, while prioritizing one-thumb usability testing.

Who: Instructor (recommended); Eden and Alan (implementation decision pending confirmation)

2026-11-09 — Prioritize one-thumb crafting usability testing

Options: Continue combat normally / Slow combat during crafting / Pause combat during crafting

Chose: Start by testing crafting while combat continues normally. Build an early Greybox with a 3×3 table, three material buttons and moving soldiers. Consider slowing or pausing combat only if playtesting shows that the interaction is uncomfortable.

Why: Real-time crafting is central to the concept, but dragging materials while fighting could be difficult on a portrait mobile screen. Testing before polishing the interface reduces development risk.

AI: Supported testing the real-time interaction first rather than changing the combat speed without evidence.

Who: Instructor (recommended test); Eden (preferred real-time combat); Alan (pending confirmation)

2026-11-09 — Keep event-based traps and rare material drops

Options: Position-triggered traps / Event-based traps; Shop-only materials / Rare materials from enemies

Chose: Traps activate in response to specific battlefield events rather than only when enemies step on a fixed location. Common materials can be purchased, while certain rare materials may drop from defeated enemies and cannot be bought in the shop. The exact number of material types remains undecided.

Why: Event-based traps create more tactical possibilities. Rare drops give players reasons to adapt crafting choices based on what happens during combat, rather than relying exclusively on Gold purchases.

AI: Recommended sharing effect logic between Spells and Traps to reduce implementation complexity, while keeping the event triggers configurable. It also advised postponing more complicated drop behavior if necessary for the first version.

Who: Eden (proposed); Alan (pending confirmation)
