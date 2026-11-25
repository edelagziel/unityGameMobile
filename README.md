# Battle Forge

**Team:** Eden Lagziel · Alan Zirkiev
**Course:** Mobile Game Development, Shenkar

## Game and platform

Battle Forge is a portrait Android card battler for one-thumb play. The design calls for picking four cards, crafting on a 3×3 table and destroying the enemy base. The Milestone 4 prototype demonstrates the Home → battle result → reward → Home loop. [Game pitch](pitch.md)

## Run the prototype

1. Open the project with Unity **6000.6.7f1** and press **Play**. The editor starts from Bootstrap and loads Home.
2. For a phone build, install Android Build Support with its SDK, NDK and OpenJDK. In **File → Build Profiles**, select **Android** and switch platform.
3. Connect an Android 10 (API 29) or newer ARM64 phone with USB debugging enabled, select the device and choose **Build And Run**.

## Architecture
![Scenes, services and init order](docs/architecture.webp)

- **Bootstrap** (scene 0, no UI) starts the services in order, keeps them alive with DontDestroyOnLoad, then opens **Home** (scene 1).
- **Services** initialize in order. `ISaveService` persists values as JSON in PlayerPrefs. `IConfigService` reads local defaults from the root asset assigned to Bootstrap. `IWalletService` loads the player's Cash through the save service. The skill service initializes after the wallet and uses it to record purchases. Screens receive services by constructor injection.
- **Home** holds every screen, including the battle: tabs switch with the screen navigator, Settings and Level open on top and close with Back.
- **Home scopes.** Home has two children of the persistent Bootstrap scope. `HomeLifetimeScope` wires the Cash HUD to the wallet. `ScreenNavigationLifetimeScope` owns screen navigation, the Level screen and its battle reward flow.
- **Runtime assemblies.** `Game` contains Bootstrap and the Cash HUD. `Game.ScreenNavigation` contains the navigator and screens. Both depend on `Services`, which has no dependency on either assembly. [Folder and assembly conventions](AGENTS.md#folder-ownership)

## Milestone 4

**Playable prototype.** From Home, tap **Battle**, then choose **Win** or **Lose** in the level. These buttons simulate the battle outcome. The result panel displays the payout once. Tap **Back to menu** to see the updated Cash balance on Home. The wallet saves each change through the save service, so Cash carries over when the game restarts.

**Economy configuration.** Bootstrap loads `GameConfig.asset`. The battle reads its rewards through `IConfigService.Economy`: 150 Cash for a win and 30 for a loss. Designers can edit both values and the upgrade curve in [EconomyConfig.asset](Assets/Config/EconomyConfig.asset).

**Skill Tree purchases.** From a fresh balance, win a battle for 150 Cash, open Global Training and buy Army Drills level 1 for 100 Cash. The HUD shows 50 Cash. After restart, the wallet restores the balance and the purchased level. Use the tree title to switch to Swordsman or Archer. Each level uses its authored price or the shared economy curve. [Skill Tree guide](docs/SKILL_TREE.md)

**Economy sheet.** The team models one win and one loss per session, for 180 Cash. Both pacing checks read **Yes**, and the sheet projects 63 sessions to afford all ten upgrades. [Team Economy Sheet](https://docs.google.com/spreadsheets/d/1F4X58x4EBTdsWn25kLN42NWPBHyPkzjbTYCo3uSB7S0/edit?usp=sharing) · [Economy decisions](DECISIONS.md)

## Milestone 2
- Milestone 2 phone recording (21.11), Home → Level → Home


https://github.com/user-attachments/assets/0fc4849c-0829-4a94-8acf-947168882d1f


- Device Simulator, phone and iPad:

<img src="docs/phone_view.png" width="280"> <img src="docs/ipad_view.png" width="280">

## Repo map

| Path | What it contains |
| --- | --- |
| [Composition](Assets/Scripts/Composition) | VContainer scopes, dependency registrations and parent wiring |
| [Bootstrap](Assets/Scripts/Bootstrap) | Service startup sequence and transition to Home |
| [Services](Assets/Scripts/Services) | Save, config and wallet contracts and implementations, economy models and the shared battle outcome |
| [Screen Navigation](Assets/Scripts/ScreenNavigation) | Shared navigation and per-screen MVC folders, including Level and its battle result and reward flow |
| [Cash HUD](Assets/Scripts/Features/Home/CashHud) | Home cash display and controller |
| [Tests](Assets/Tests) | EditMode coverage for services, screen navigation and the battle reward flow |
| `pitch.md` | One-page pitch of the game |
| [DECISIONS.md](DECISIONS.md) | Every real decision the team made, and why |
| [CREDITS.md](CREDITS.md) | Where every piece of art, the font and the libraries come from |
| [Config assets](Assets/Config) | Root game configuration and the economy defaults for battle rewards and upgrade prices |
| `AGENTS.md` | Context for AI coding tools: architecture, rules, conventions |
| `CLAUDE.md` | Points Claude Code to `AGENTS.md` |
| `docs/architecture.webp` | The architecture diagram above |
| `docs/phone_view.png`, `docs/ipad_view.png` | Device Simulator screenshots (Milestone 2) |
