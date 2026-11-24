# Battle Forge

**Team:** Eden Lagziel · Alan Zirkiev
**Course:** Mobile Game Development, Shenkar

## The game in one sentence
Take 4 cards into a real-time lane battle, craft more on a 3×3 table (a rare Soul makes them stronger) and destroy the enemy base; come back tomorrow for daily rewards, quests and new cards to unlock.

## How to open and build
1. Open the project in Unity 6000.6.7f1 (the version approved by the course).
2. File → Build Profiles → Android → Switch Platform.
3. Build and run on an Android phone. In the editor, Play always starts from the Bootstrap scene.

## Architecture
![Scenes, services and init order](docs/architecture.webp)

- **Bootstrap** (scene 0, no UI) starts the services in order, keeps them alive with DontDestroyOnLoad, then opens **Home** (scene 1).
- **Services** initialize in order. `ISaveService` persists values as JSON in PlayerPrefs. `IConfigService` reads local defaults from the root asset assigned to Bootstrap. `IWalletService` loads the player's Cash through the save service. Screens receive services by constructor injection.
- **Home** holds every screen, including the battle: tabs switch with the screen navigator, Settings and Level open on top and close with Back.
- **Home scopes.** Home has two children of the persistent Bootstrap scope. `HomeLifetimeScope` wires the Cash HUD to the wallet. `ScreenNavigationLifetimeScope` owns screen navigation, the Level screen and its battle reward flow.
- **Runtime assemblies.** `Game` contains Bootstrap and the Cash HUD. `Game.ScreenNavigation` contains the navigator and screens. Both depend on `Services`, which has no dependency on either assembly. [Folder and assembly conventions](AGENTS.md#folder-ownership)

## Milestone 4
- **Economy sheet:** [Battle Forge — Economy](https://docs.google.com/spreadsheets/d/1F4X58x4EBTdsWn25kLN42NWPBHyPkzjbTYCo3uSB7S0/edit?usp=sharing). A session is 2 battles: 1 win (150 Cash) + 1 loss (30 Cash) = 180 Cash. The Skill tree upgrade costs 100 × 1.5^level. Both checks say Yes; the last upgrade comes after about 63 sessions. Why 1.5: see `DECISIONS.md`.
- **In the game**, Bootstrap references `GameConfig.asset`. Its Economy field points to `EconomyConfig.asset`, where designers edit the rewards and price curve. The battle reward code reads `config.Economy.BattleRewards.RewardFor(outcome)`. [Configuration conventions](AGENTS.md#game-configuration)
- **Loop:** Battle → Win / Lose → the result over the dimmed battle (+150 / +30) → Back to menu, with the Cash added and saved.

## Milestone 2
- Phone recording (current build, recorded 21.11), Home → Level → Home: 


https://github.com/user-attachments/assets/0fc4849c-0829-4a94-8acf-947168882d1f


- Device Simulator, phone and iPad:

<img src="docs/phone_view.png" width="280"> <img src="docs/ipad_view.png" width="280">

## Repo map

| Path | What it contains |
| --- | --- |
| [Composition](Assets/Scripts/Composition) | VContainer scopes, dependency registrations and parent wiring |
| [Bootstrap](Assets/Scripts/Bootstrap) | Service startup sequence and transition to Home |
| [Services](Assets/Scripts/Services) | Save, config and wallet contracts and implementations, economy models and the shared battle outcome |
| [Home features](Assets/Scripts/Features/Home) | Menu screens, screen navigation and Cash HUD |
| [Battle feature](Assets/Scripts/Features/Battle) | Level MVC, result panel and reward grant |
| [Tests](Assets/Tests) | EditMode coverage for services, screen navigation and the battle reward flow |
| `pitch.md` | One-page pitch of the game |
| `DECISIONS.md` | Every real decision the team made, and why |
| `CREDITS.md` | Where every piece of art, the font and the libraries come from |
| [Config assets](Assets/Config) | Root game configuration and the economy defaults for battle rewards and upgrade prices |
| `AGENTS.md` | Context for AI coding tools: architecture, rules, conventions |
| `CLAUDE.md` | Points Claude Code to `AGENTS.md` |
| `docs/architecture.webp` | The architecture diagram above |
| `docs/phone_view.png`, `docs/ipad_view.png` | Device Simulator screenshots (Milestone 2) |
