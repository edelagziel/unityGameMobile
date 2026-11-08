# Pitch: Battle Forge

**Team:** Eden Lagziel, Alan Zirkiev
**Platform:** Android, portrait, one thumb · Unity 6
**References:** *Stick War: Legacy* (one lane, two bases), *Minecraft* (3×3 crafting), *Yu-Gi-Oh!* (Monster, Spell and Trap cards), *Battle Bag: War Zone*
**Our twist:** your army comes only from crafting. In the middle of a real-time battle you buy materials, lay them on a 3×3 table and turn them into cards. A recipe depends on both the materials and their shape.

## Core loop
Each battle starts with 4 empty card slots and a few materials, enough for one or two crafts.
- **Gold** grows over time and with every kill. Tap **Upg income** to earn gold faster.
- **Materials:** 6 types, each with a meaning: wood (bows), stone (heavy troops), iron (swords and armor), leather (light troops), crystal (spells), gunpowder (traps). Each material is one button that shows how many you have and its price: **tap** buys one, **hold and drag** puts one on the table. The button turns grey when you have none; the price turns red when you can't afford it.
- **Craft:** lay materials on the 3×3 table in a recipe's shape. The shape can sit anywhere on the table and can be mirrored left-right, but not turned or flipped upside down. Tap a placed material to take it back. When the table matches a recipe, the **Spawn** button shows the card's picture; tap it and the card goes into a free slot.
- **Recent:** the last 3 recipes you crafted. One tap fills the table with their materials; a recipe you don't have the materials for is greyed out.
- **Recipe book:** a button opens your recipes in a see-through window over the table; the fight goes on behind it.
- **Cards:** one tap, no aiming. A **Monster** card sends a squad of one soldier type (for example Archers ×3). A **Spell** works at once on the whole field. A **Trap** is armed on the lane between your base and the middle, and fires when the first enemy steps on it. A played card disappears and frees its slot.
- **Fight:** troops walk and fight on their own along one lane. The enemy base sends 4 waves, and the 5th brings a **boss**, a soldier with much more HP. **Destroy the enemy base to win**, even before the boss if you rush; lose if your base falls.

## Meta
- **Earn:** **cash** after every battle, based on kills and on clearing the stage, so a loss still pays a little. Each new stage pays more; cleared stages can be replayed.
- **Spend:** buy locked recipes in the **Recipe book** (menu tab).
- **Unlock:** every recipe you own can be crafted in every battle; nothing to pick before a fight. The game starts with 3 recipes open: one Monster, one Spell, one Trap. Story stages open one by one.

## Why come back?
1. **Recipe collection:** the next recipe is almost affordable, and every new one changes how you fight.
2. **Daily reward:** free cash once a day.

## Who and when
Casual players aged 16–35 who like simple strategy and card games. One battle takes 2–3 minutes, so it fits a bus ride or a queue; several stages in a row make a longer session at home. All controls sit in the lower part of the screen, within reach of one thumb; the battlefield on top is only watched.

## Goals
- Stage cleared (enemy base destroyed)
- Boss beaten
- All 6 Story stages cleared
- New recipe bought
- Every owned recipe crafted at least once
- All 15 recipes collected

## Feel
- **Crafting:** materials snap into the table with a click; a matching recipe lights up Spawn with the card's picture, and the new card flies into its slot.
- **Combat:** troops march out with a short horn, hit sounds and damage numbers, coins pop out of every kill.
- **Spells:** a flash across the whole field and a short screen shake.
- **Traps:** an "armed" mark on the lane, then a burst when an enemy steps on it.
- **Boss:** a warning banner, the music changes, and the screen shakes on every hit.
- **Results:** cash counts up into the counter. Music in the menu and in battle.

## Scope check
- [x] Hybrid-casual: simple core + a meta layer
- [x] Portrait, one thumb
- [x] 1–3 minute sessions (one battle)
- [x] Small: about 3 levels (more for a team of 2–3)

**Content:** 6 Story stages on one battlefield · 5 soldier types · 15 recipes: 5 Monster, 5 Spell, 5 Trap · 6 materials · 3 enemy types + 1 boss.

**Screens:** Menu with two tabs (Battle: current stage and Play; Recipes: recipe book and buying) · Battle · Results. Stage 1 doubles as a tutorial with 3–4 arrow hints.

**Team of 2:** stages, materials, recipes, cards and enemy waves are data, so adding more is cheap. Spells and traps share one set of effects (damage, freeze, slow); a trap only waits for its trigger. On top of the basic game (lane battle, cards, cash and unlocks) we add **real-time crafting** as our extra system: the 3×3 table with material and shape matching (any position, left-right mirror), material buttons (tap to buy, drag to place), the in-battle recipe book and recent recipes that fill the table in one tap.

**Out of scope:** multiplayer and Survival mode, a real-money shop (ideas for after the course), ads, gems or a second meta currency, picking cards before battle, rare materials and material drops, card upgrades, daily quests, complex enemy AI, card merging.
