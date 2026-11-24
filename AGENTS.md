# AGENTS.md — context for AI coding tools

AI tools read this file before working on the repo. Keep it short, true and up to date.
Claude Code reads it through `CLAUDE.md` (one line: `@AGENTS.md`).

## The game
Battle Forge is a hybrid-casual card battler for Android, portrait, one thumb. Before a battle the player picks 4 cards. In a 2–3 minute real-time lane battle, cheap minions hold the line while the player crafts hero, spell and trap cards on a 3×3 table from wood, stone, iron and a rare Soul, then destroys the enemy base. Cash earned after battles buys new cards (Inventory) and permanent upgrades (Skill tree); daily rewards and quests bring the player back. Full pitch: `pitch.md`.

## Tech
- Unity 6000.6.7f1, C#, Built-in Render Pipeline, uGUI + TextMeshPro.
- Target: Android, portrait, minimum API 29, package `com.edenalan.battleforge`.
- VContainer 1.19.0 for dependency injection (services and screens).
- Unity Test Framework, EditMode tests.

## Architecture
- **Scenes** (Scene List order): `Bootstrap` (0) → `Home` (1). There is no separate Level scene: every screen, including the battle (`LevelView`), lives in Home.
- **Bootstrap** runs once per launch. The `BootstrapLifetimeScope` GameObject persists across scene loads and registers every service. `BootstrapFlow` calls `InitializeAsync` on each one in registration order, then loads Home. In the editor, `Assets/Editor/PlayFromBootstrap.cs` makes every Play start in Bootstrap.
- **Services** (`Assets/Scripts/Services`), started in this order:
  1. `ISaveService` → `PlayerPrefsSaveService`: generic `Save<T>` / `Load<T>`, each value as JSON in PlayerPrefs, written immediately. The only class that touches PlayerPrefs.
  2. `IConfigService` → `ScriptableObjectConfigService` receives the root asset from Bootstrap. Gameplay injects `IConfigService` and reads typed domains such as `config.Economy` (`IEconomyConfig`).
  3. `IWalletService` → `WalletService`: the player's Cash (`Add`, `TrySpend`, `CashChanged`), loaded from and saved through `ISaveService`.
  Every service implements `IInitializableService`.
- **How scenes get services**: a scope's parent is the Bootstrap scope, so its controllers receive services by constructor injection. `HomeLifetimeScope` gets that parent from `SceneLifetimeScope` (code); `ScreenNavigationLifetimeScope` from its Parent field in the Inspector (`BootstrapLifetimeScope`), so the navigation code doesn't depend on Game.
- **Economy numbers** (rewards, prices, growth) live only in `EconomyConfig`: `BattleRewards` (`RewardFor(BattleOutcome)`) and `PriceCurve` (plain data structs from the Inspector) (`PriceAt(level)` = round(base × growth^level), the same formula as the economy sheet; `UpgradeCost(level)` for the Skill tree upgrade). The Lesson 4 slides write `Services.Config.Economy`; here a controller takes `IConfigService config` in its constructor and reads `config.Economy`.
- **Home** has two scopes:
  - `ScreenNavigationLifetimeScope`: all screens. `IScreenNavigator` shows one screen at a time. The five tabs (Shop, Inventory, Battle/MainMenu, Skill tree, Recipes) are `TabbedScreenView`s and switch with `ResetTo`; Settings and Level open with `Push` and close with `Pop`.
  - `HomeLifetimeScope` owns Home-specific dependencies. It registers the HUD's `CashView` and `CashController`, which receives `IWalletService` from Bootstrap.
  - Keep Screen Navigation independent of the Home container. Its existing scope owns `LevelModel`, `LevelController`, and `BattleRewardGranter`. Introduce a `BattleLifetimeScope` only when a dependency needs to be created and disposed for each battle.
- **End of a battle**: `LevelView.Finished(BattleOutcome)` (the stand-in Win/Lose buttons for now) → `LevelController` → `BattleRewardGranter` (the only place a battle pays Cash: reward from `IConfigService`, into `IWalletService`) → `BattleResultPanel`, a half-transparent panel inside LevelView over the dimmed battle → Back to menu (`ResetTo<MainMenuView>`). A battle pays once.
- **Screens are MVC**: `XView` (MonoBehaviour: UI references and events only), `XController` (plain C#, `IStartable` + `IDisposable`, the logic), `XModel` (screen state).
- **Assemblies**:
  - `Services` owns shared service contracts, implementations, configuration models and battle domain data in `Assets/Scripts/Services`. It references no other project assembly.
  - `Game.asmdef` at `Assets/Scripts` owns Bootstrap, the Bootstrap and Home scopes, and the Cash HUD. The HUD uses a GUID-based `.asmref` to select Game inside Features.
  - `Game.ScreenNavigation.asmdef` at `Assets/Scripts/Features` owns navigation, Home screens and Battle. A GUID-based `.asmref` keeps `Composition/ScreenNavigation` in this assembly. Both presentation assemblies reference Services, VContainer, uGUI and TextMeshPro independently.
  - Tests (Editor only): `Services.Tests` (`Assets/Tests/Services`), `Game.ScreenNavigation.Tests` (`Assets/Tests/EditMode`).

## Game configuration

- Bootstrap holds `Assets/Config/GameConfig.asset`. Its Economy field references the existing `EconomyConfig.asset` beside it. Each domain owns its values and validation. Missing required references prevent startup. Invalid local economy numbers produce a startup error and the shipped-asset test catches them before release.
- To add a domain, define its data under `Services/Config/Models/<Domain>`, create its asset in `Assets/Config`, and assign it to a typed field in `Models/GameConfig.cs`. Expose its gameplay-facing read API through `IConfigService`. Validate it during initialization and test the shipped reference and values. Bootstrap still takes one asset.
- A future remote implementation can start with the local defaults, fetch overrides during `InitializeAsync`, validate the merged candidate, and publish it through the same domain properties. On an unavailable source or invalid override, keep the validated defaults. Apply overrides in memory so the authored assets retain their values.
- Game configuration holds designer-authored rules shared across players. Player settings hold individual preferences such as audio volume. Persistent player state records progress such as Cash. Player-owned settings and progress belong to their respective features and use the save service for persistence.

## Folder ownership

Paths below are relative to `Assets/Scripts`.

- `Composition` holds all LifetimeScopes, including the scene scope base. Keep registration and parent wiring here. Launch sequencing belongs in `Bootstrap/BootstrapFlow.cs`.
- In `Services/<Service>`, put contracts in `Interfaces`. Use `Local` for PlayerPrefs and ScriptableObject adapters and add `Remote` only for an implemented remote source. The wallet uses the save contract, so its logic lives directly in `Services/Wallet`. Shared startup contracts live in `Services/Interfaces`.
- Economy definitions live in `Services/Config/Models/Economy`. Their authored ScriptableObject asset lives in `Assets/Config`. Shared battle vocabulary such as `BattleOutcome` lives in `Services/Domain/Battle` so both configuration and presentation can consume it.
- Colocate each screen's MVC files under its feature. `Features/Home` owns the menu screens, shared screen navigation and Cash HUD. `Features/Battle` owns Level, its result panel and reward flow. Folder placement describes responsibility. Scope registrations determine instance lifetime.
- Move Unity assets and folders with their existing `.meta` files. Preserve GUIDs, serialized type identities and each script's assembly when moving across an assembly boundary.

## Rules for AI
- Screens and gameplay code get services through their interfaces by constructor injection. Never call PlayerPrefs outside `PlayerPrefsSaveService`, and never add a static service locator.
- A new service: interface + class in `Services`, implement `IInitializableService`, add one `Register` line in `BootstrapLifetimeScope` in init order (after the services it uses).
- Services must never reference Game. If a service needs game data, pass it in.
- No economy number in a script: add a field to `EconomyConfig` (or reuse `PriceCurve`), set it in the asset, read it through `IConfigService`.
- A new screen: `XView`, `XController`, `XModel` in its owning feature folder (for example, `Features/Home/Shop`), registered in `ScreenNavigationLifetimeScope` and added to its Screens list.
- Don't change Player Settings, the package name, API levels or the Scene List unless asked.
- Don't edit `.unity` scenes or prefabs as text. Write the code, then list the Editor steps for a person.
- VContainer replaces a new file named `*LifetimeScope.cs` with an empty template when Unity imports it. After creating one, check its content.
- Don't invent APIs (Unity, VContainer). If unsure, say so and point to the docs.
- Keep changes small; say what you changed and why. Run the EditMode tests before you say you're done.
- Never commit secrets (keystores, passwords, API keys), `unityGameMobile.slnx`, or the TextMesh Pro font assets that change on Play.

## Conventions
- Namespaces start with `Game.`: `Game.Bootstrap`, `Game.Hud`, `Game.ScreenNavigation`, `Game.Services.Save`, `Game.Services.Wallet`.
- One class per file; the file name is the class name; interfaces start with `I`.
- Views: `[SerializeField] private` references, log an error in `Awake` if one is missing, remove listeners in `OnDestroy`.
- Controllers: subscribe in `Start`, unsubscribe in `Dispose`.
- Save keys are lowercase and dotted, and each key belongs to one service (`wallet.cash`).
- Git: work on `feature/...` branches from `develop`, merge into `develop` through a pull request; `main` gets a pull request from `develop` at each milestone, tagged `milestone-N`.
