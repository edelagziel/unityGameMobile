Battle Forge — Decisions Log

Record real design and technical decisions, with the newest entries first.

2026-11-24 — Organize scripts by feature and preserve assembly ownership

Group LifetimeScopes under `Composition` so developers can find dependency registrations in one place. Keep each screen's MVC files together. The Level screen, result panel and reward grant belong in `Features/Battle`, even though the screen runs in Home and its dependencies live in the navigation scope.

Keep shared services and configuration in the `Services` assembly. Both battle presentation and economy configuration consume `BattleOutcome`, so its source lives in `Services/Domain/Battle`.

Two GUID-based assembly references preserve the existing dependency graph across folder boundaries. The Cash HUD under Home compiles into `Game`, and the navigation scope under Composition compiles into `Game.ScreenNavigation`. The refactor preserved namespaces, Unity asset GUIDs and DI lifetimes. [Implementation in 76fabee](https://github.com/edelagziel/unityGameMobile/commit/76fabee)

2026-11-24 — Keep Screen Navigation independent and give Home its own scope

Options: Combine navigation and HUD registrations in one Home container / Keep Screen Navigation independent and broaden the existing HUD scope to cover Home-specific dependencies.

Chose: Keep `BootstrapLifetimeScope` as the parent of two sibling scopes in Home. `ScreenNavigationLifetimeScope` keeps its existing screen registrations and serialized references. `HomeLifetimeScope` replaces `HudLifetimeScope` and registers `CashView` and `CashController`. The HUD receives the global wallet from Bootstrap.

Why: Screen Navigation owns the screens and their history. The HUD needs the wallet and can operate independently of navigation. Level already runs as a screen in Home, with `LevelModel`, `LevelController`, and `BattleRewardGranter` registered in Screen Navigation. Introduce a `BattleLifetimeScope` only when a concrete dependency needs to be created and disposed for each battle.

AI: Codex verified the two containers against the existing registrations and checked the shared wallet and Level navigation in Play Mode.

Who: Eden

2026-11-24 — Economy: a win pays 150, a loss 30, upgrades cost 100 × 1.5 per level

Options: Upgrade price growth 1.3 / 1.5 / 2.0 (base cost 100, max level 10, 180 Cash per session)

Chose: A win pays 150 Cash and a loss 30. The Skill tree upgrade costs 100 × 1.5^level (level = upgrades already bought), up to level 10. The numbers live in `Assets/Config/EconomyConfig.asset` and in our economy sheet, linked in the README.

Why: A session is 2 battles, and the sheet counts it as 1 win and 1 loss (180 Cash) on purpose: if the economy works at a 50% win rate, a stronger player only gets there faster. The first upgrade costs less than one win, so a new player buys it in the first session (0.6 sessions). With 1.5, upgrades 2–4 come every 0.8–1.9 sessions and the last one after about 63 sessions: quick wins first, then a long goal and a reason to come back. With 1.3 all 10 upgrades are bought in about 24 sessions and Cash soon has nothing left to buy; with 2.0 upgrade 4 alone takes 4.4 sessions, a wall, and the sheet's second check fails. A loss pays a fifth of a win, so a bad session still moves the meta (as in the pitch) while winning stays worth five times more.

AI: Claude computed the curve for growth 1.3, 1.5 and 2.0 and proposed counting a session as one win and one loss. We filled in the sheet ourselves (both checks Yes) and checked the game: Win pays +150 from the config.

Who: Alan (economy sheet and numbers); Eden (pending confirmation)

2026-11-21 — Package name com.edenalan.battleforge and minimum API 29 (decided 2026-11-13)

Options: Unity default com.DefaultCompany.unityGameMobile / com.edenalan.battleforge; minimum API 23 (Unity default) / 26 / 29

Chose: com.edenalan.battleforge, minimum API level 29 (Android 10), IL2CPP + ARM64, portrait.

Why: The package name is the app's permanent ID on the phone and on Google Play and cannot change after publishing, so it names our team and our game instead of Unity's default. API 29 is the course target and still covers most Android phones in use. Google Play requires 64-bit builds, so IL2CPP + ARM64.

AI: Claude explained the reverse-domain format and that the package name can't change after publishing; we chose the name.

Who: Alan (set the Player Settings), Eden

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
