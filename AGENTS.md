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
- **Bootstrap** (`Assets/Scripts/Game/Bootstrap`): one GameObject with `BootstrapLifetimeScope` (DontDestroyOnLoad, runs once per launch). It registers every service; `BootstrapFlow` calls `InitializeAsync` on each one in registration order, then loads Home. In the editor, `Assets/Editor/PlayFromBootstrap.cs` makes every Play start in Bootstrap.
- **Services** (`Assets/Scripts/Services`), started in this order:
  1. `ISaveService` → `PlayerPrefsSaveService`: generic `Save<T>` / `Load<T>`, each value as JSON in PlayerPrefs, written immediately. The only class that touches PlayerPrefs.
  2. `IWalletService` → `WalletService`: the player's Cash (`Add`, `TrySpend`, `CashChanged`), loaded from and saved through `ISaveService`.
  Every service implements `IInitializableService`.
- **How scenes get services**: a scene scope that needs services derives from `SceneLifetimeScope`; its parent is the Bootstrap scope, so controllers receive services by constructor injection.
- **Home** has two scopes:
  - `ScreenNavigationLifetimeScope`: all screens. `IScreenNavigator` shows one screen at a time. The five tabs (Shop, Inventory, Battle/MainMenu, Skill tree, Recipes) are `TabbedScreenView`s and switch with `ResetTo`; Settings and Level open with `Push` and close with `Pop`.
  - `HudLifetimeScope` (a `SceneLifetimeScope`): the Cash label, `CashView` + `CashController`.
- **Screens are MVC**: `XView` (MonoBehaviour: UI references and events only), `XController` (plain C#, `IStartable` + `IDisposable`, the logic), `XModel` (screen state).
- **Assemblies**:
  - `Services` (`Assets/Scripts/Services`): references nothing of ours.
  - `Game` (`Assets/Scripts/Game`): Bootstrap and HUD. References Services, VContainer, uGUI, TextMeshPro.
  - `Game.ScreenNavigation` (`Assets/Scripts/ScreenNavigation`): navigator and screens. References VContainer, uGUI.
  - Tests (Editor only): `Services.Tests` (`Assets/Tests/Services`), `Game.ScreenNavigation.Tests` (`Assets/Tests/EditMode`).
  Services never references Game or Game.ScreenNavigation.

## Rules for AI
- Screens and gameplay code get services through their interfaces by constructor injection. Never call PlayerPrefs outside `PlayerPrefsSaveService`, and never add a static service locator.
- A new service: interface + class in `Services`, implement `IInitializableService`, add one `Register` line in `BootstrapLifetimeScope` in init order (after the services it uses).
- Services must never reference Game. If a service needs game data, pass it in.
- A new screen: `XView`, `XController`, `XModel` in `Screens/X`, registered in `ScreenNavigationLifetimeScope` and added to its Screens list.
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
