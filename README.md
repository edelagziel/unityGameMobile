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
- **Services** sit behind interfaces in their own assembly: 1 · `ISaveService` (PlayerPrefs, JSON), 2 · `IConfigService` (the `EconomyConfig` asset: every economy number), 3 · `IWalletService` (the player's Cash, saved through Save). Screens get them by constructor injection (VContainer) and never touch PlayerPrefs.
- **Home** holds every screen, including the battle: tabs switch with the screen navigator, Settings and Level open on top and close with Back.
- **Assemblies**: `Game` → `Services`; `Services` references nothing of ours. Details in `AGENTS.md`.

## Milestone 4
- **Economy sheet:** [Battle Forge — Economy](https://docs.google.com/spreadsheets/d/1F4X58x4EBTdsWn25kLN42NWPBHyPkzjbTYCo3uSB7S0/edit?usp=sharing). A session is 2 battles: 1 win (150 Cash) + 1 loss (30 Cash) = 180 Cash. The Skill tree upgrade costs 100 × 1.5^level. Both checks say Yes; the last upgrade comes after about 63 sessions. Why 1.5: see `DECISIONS.md`.
- **The numbers in the game:** `Assets/Config/EconomyConfig.asset`, read through `IConfigService` (the slides' `Services.Config.Economy`, here by constructor injection). No economy number lives in a script.
- **Loop:** Battle → Win / Lose → the result over the dimmed battle (+150 / +30) → Back to menu, with the Cash added and saved.

## Milestone 2
- Phone recording (current build, recorded 21.11), Home → Level → Home: 


https://github.com/user-attachments/assets/0fc4849c-0829-4a94-8acf-947168882d1f


- Device Simulator, phone and iPad:

<img src="docs/phone_view.png" width="280"> <img src="docs/ipad_view.png" width="280">

## Repo map
| File | What it is |
| --- | --- |
| `pitch.md` | One-page pitch of the game |
| `DECISIONS.md` | Every real decision the team made, and why |
| `Assets/Config/EconomyConfig.asset` | Every economy number: battle rewards, upgrade price curve |
| `AGENTS.md` | Context for AI coding tools: architecture, rules, conventions |
| `CLAUDE.md` | Points Claude Code to `AGENTS.md` |
| `docs/architecture.webp` | The architecture diagram above |
| `docs/simulator-*.png` | Device Simulator screenshots (Milestone 2) |
| `docs/phone_view.png`, `docs/ipad_view.png` | Device Simulator screenshots (Milestone 2) |
