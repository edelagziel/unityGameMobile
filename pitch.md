# Pitch: Squad Pack

**Team:** Eden Lagziel, Alan Zirkiev
**Reference:** *Battle Bag: War Zone* (Voodoo, 2025). Same core loop and military theme, with our own name and sprites.

## Core loop
Between waves the player spends coins on **Summon**: 3 random items (soldiers and weapons) appear under the bag. With one thumb he drags them into the bag grid:
- **Shapes:** a soldier takes 1×2 cells (standing), a weapon takes 2×1 (lying). The bag is small, so every cell counts. Items he doesn't want go to the trash bin.
- **Connect:** weapons placed next to a soldier give him damage. The bag shows the squad's total damage and HP.
- **Merge:** drop an item on an identical one and it becomes the next tier (max 4). The soldier's look changes with the tier: recruit → helmet → helmet + vest → full gear.

Then he taps **Battle**: the soldiers leave the base on the left and fight enemies coming from the right on their own. Kills give coins for the next summon. Survive all 6 waves to win the level; if the base HP reaches 0, he loses.

## Meta
- **Earn:** gems for every level cleared and from reward chests at waves 3 and 6. Every weapon also counts its kills.
- **Spend:** on the **Upgrades** screen, a weapon with enough kills can be leveled up with gems (more damage). On the **Skill tree** screen, gems buy talents such as faster fire, more base HP or more soldier HP.
- **Unlock:** new weapon types join the summon pool as you progress (shotgun at level 2, grenade at level 4), and the next level opens.

## Why come back?
1. **Daily reward with a streak:** free gems every day, bigger on day 3 and day 7.
2. **Almost there:** a weapon that needs only a few more kills to level up, or a talent that is almost affordable.

## Who and when
Casual players who like simple strategy, aged 16–35. They play on the bus, in a queue or on a short break, holding the phone in one hand. One level takes 1–3 minutes.

## Goals
- Wave cleared, reward chest opened
- Level cleared
- Weapon leveled up
- Talent bought on the skill tree
- New weapon type unlocked
- Boss beaten at the end of level 5

## Feel
- **Placing:** items snap into the cells with a click and a light haptic; a weapon connecting to a soldier pops a "+" and the damage counter rolls up.
- **Merge:** a pop with a scale bounce, particles, and the helmet or vest appearing on the soldier.
- **Combat:** damage numbers flying up, short screen shake on grenades and boss hits, a different shot sound for each weapon.
- **Win:** gems burst from the screen and fly into the counter.

## Scope check
- [x] Hybrid-casual: simple core + a meta layer
- [x] Portrait, one thumb
- [x] 1–3 minute sessions
- [x] Small: about 3 levels (more for a team of 2–3)

**Screens:** Battle (level select, bag and fight) · Upgrades (weapons) · Skill tree.

**Team of 2:** 5 levels in one world, with a boss at the end of level 5. Levels are data (waves, enemies, bag shape), so adding more later is cheap. On top of the basic game we add:
- **Skill tree** as a second meta system.
- **Bag obstacles:** *Empty cells* are holes in the bag. Every level has its own bag shape, so the player has to plan how to fit 1×2 soldiers and 2×1 weapons. *Chains* lock a random soldier or weapon at the start of a level: it can't be moved or merged, but it still works when items are connected next to it.

**Out of scope:** energy, shop and paid packs, ads, base building, multiple worlds.
