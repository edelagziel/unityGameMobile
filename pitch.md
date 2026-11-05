# Pitch: Squad Pack

**Team:** Eden Lagziel, Alan Zirkiev
**Reference:** *Battle Bag: War Zone* (Voodoo, 2025). Same core loop and military theme, with our own name and sprites.
**Our twist:** battle cards you can merge, so the player stays active during the fight.

## Core loop
**Build:** before each wave the player spends coins on **Summon**: 3 random items appear under the bag, and he drags them into the bag grid with one thumb.
- **Shapes:** a soldier takes 1×2 cells; weapons take 1×1, 2×1 or 2×2. Bigger weapons are stronger but need more space. Items he doesn't want go to the trash bin.
- **Connect:** weapons touching a soldier give him damage. The bag shows the squad's total damage and HP.
- **Merge:** drop an item on an identical one and it becomes the next tier (max 4). The soldier's look changes with the tier: recruit → helmet → helmet + vest → full gear.

**Fight:** he taps **Battle**. The bag shrinks and locks, and the soldiers leave the base on the left and fight enemies coming from the right on their own. Kills give coins. A level has 5 waves, and **the 5th is always a boss**: a slow tank with a lot of HP. Beat it to win; if the base HP reaches 0, the try is lost.

**Battle cards:** a card bar under the bag fills in about 20 seconds, faster with every kill, and gives a random card. Cards come in 5 rarities (common 60%, uncommon 25%, rare 10%, epic 4%, legendary 1%). The hand holds 3 cards; while it is full, the bar stops. A tap on a card hits the whole field. Dropping a card on another of the same rarity merges them into a random card of the next rarity. The player decides: use a card now, or save it for the boss.

**Tries:** a level takes 3–5 tries. After a loss the level restarts from wave 1, but the bag and coins stay, so the next try starts stronger; cards reset. After a win the next level starts from zero: an empty bag with its own shape and 0 coins.

## Meta
- **Earn:** coins from kills are spent inside the level. **Gems** come only from beating the boss, plus the daily reward.
- **Spend:** gems level up weapons on the **Upgrades** screen and buy talents on the **Skill tree** screen (faster fire, more base HP, more soldier HP).
- **Unlock:** the player starts with knife and pistol; the shotgun joins the summon pool at level 2 and the grenade at level 4. Clearing a level opens the next one.

## Why come back?
1. **Daily reward with a streak:** free gems every day, bigger on day 3 and day 7.
2. **One more try:** after a loss the bag and coins are kept, so the next try is stronger.

## Who and when
Casual players who like simple strategy, aged 16–35, playing one-handed on the bus, in a queue or on a short break. A lost try usually ends around wave 3–4 (about 1.5 minutes); a winning try is 4 waves of about 25 seconds plus a boss of about 45 seconds (about 2.5 minutes). A level with 3–5 tries takes about 5–9 minutes.

## Goals
- Wave survived
- Boss beaten, level cleared
- Rare, epic or legendary card made by merging
- Weapon leveled up
- Talent bought
- New weapon unlocked
- All 5 levels cleared

## Feel
- **Placing:** items snap into the cells with a click and a light haptic; connecting a weapon pops a "+" and the damage counter rolls up.
- **Merge:** a pop with a scale bounce, particles, and new gear appearing on the soldier.
- **Cards:** every rarity has its own color and glow; a card merge flashes and flips into the new card; big cards shake the screen.
- **Boss:** a warning banner, a big HP bar and a heavy shake on every hit; gems fly into the counter after the kill.

## Scope check
- [x] Hybrid-casual: simple core + a meta layer
- [x] Portrait, one thumb
- [x] 1–3 minute sessions (one try)
- [x] Small: about 3 levels (more for a team of 2–3)

**Content:** 1 soldier type (4 tiers) · 4 weapons: knife 1×1, grenade 1×1, pistol 2×1, shotgun 2×2 · 3 enemy types (infantry, runner, heavy) · 1 boss (tank, stronger on every level) · 8 cards in 5 rarities, built from 4 effect types (damage, freeze, repair base, squad boost) with bigger numbers at higher rarity · 5 levels in one world.

**Screens:** Battle (level select; build phase with a big bag; fight phase with a small bag and the card hand at the bottom, under the thumb) · Upgrades (weapons) · Skill tree.

**Team of 2:** levels and cards are data (waves, enemies, boss HP, bag shape, card numbers), so adding more later is cheap. On top of the basic game (core loop, weapon upgrades, 5 levels) we add:
- **Battle cards with rarities and card merge:** our main extra system.
- **Skill tree:** 6 talents, a second way to spend gems.
- **Bag shapes:** empty cells are holes in the bag. Every level has its own bag shape, so the player has to plan how to fit items of different shapes.

**Out of scope:** energy, shop and paid packs, ads, base building, multiple worlds, rotating items, cards kept between tries, replaying cleared levels.
