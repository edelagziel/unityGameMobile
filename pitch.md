# Pitch: Battle Forge

**Team:** Eden Lagziel, Alan Zirkiev
**Platform:** Android, portrait, one thumb · Unity 6
**References:** *Stick War: Legacy* (one lane, two bases), *Minecraft* (3×3 crafting), *Clash Royale* (4-card loadout), *Yu-Gi-Oh!* (Monster, Spell and Trap cards), *Battle Bag: War Zone*

**Our twist:** cheap, weak minions hold the line, but real power comes from crafting. In the middle of a real-time battle you lay materials on a 3×3 table and turn them into hero, spell and trap cards; a rare Soul makes the crafted card stronger.

## Core loop
Before battle you pick 4 cards in the Inventory. Each battle starts with those 4 cards in your 4 card slots, ready to play.

- **Gold** grows over time and with every kill. Spend it on minions, materials, or **Upg income** to earn gold faster.
- **Minions:** a sliding row of 3 weak units bought with gold: **Swordsman** (melee), **Shield** (tank) and **Healer** (every 2 seconds heals all allies nearby a little; does not attack). Each extra summon of the same minion costs more; prices reset every battle.
- **Materials:** a sliding row of 4: **wood, stone, iron** and the rare **Soul**. Each button has its own color, name, price and count, and turns grey at 0. **Tap** buys one; **swipe sideways** scrolls the row; **drag up** puts a material on the table; drag it back or off the table to cancel, and it returns to your stock. Enemies sometimes drop wood, stone or iron, and rarely a Soul; a Soul can also be bought, but costs more.
- **Craft:** lay materials on the 3×3 table in a recipe's shape. The shape can sit anywhere and can be mirrored left-right, but not turned or flipped upside down. When it matches, **Spawn** shows the card's picture; tap it and the card goes into a free slot.
- **Soul:** one Soul on any free cell makes the crafted card 5–10% stronger (HP and damage for heroes, effect power for spells and traps). The matcher removes the Soul first, checks the shape, then adds the bonus.
- **Cards:** one tap, no aiming. A **Monster** card sends heroes from your base (for example Archers ×3). A **Spell** works at once on the whole field. A **Trap** is armed with one tap and fires on a battlefield event, for example when an enemy reaches the middle of the lane. A played card frees its slot for a crafted one.
- **Later:** **Recent recipes** (one tap fills the table) and the in-battle **Recipe book**.
- **Fight:** everyone walks and fights on their own along one lane. The enemy sends 4 waves, and the 5th brings a **boss**. **Destroy the enemy base to win**, even before the boss if you rush; lose if your base falls.

## Meta
- **Earn:** **Cash** after every battle, based on kills and on clearing the stage; a loss still pays a little. Cleared stages can be replayed.
- **Spend:** buy locked cards in the **Inventory** (a card's recipe unlocks with it) and permanent upgrades in the **Skill tree**: Base (HP), Minions (speed, HP), Heroes (damage, attack speed, HP).
- **Chests:** open with an animation and give a random locked card, or Cash once all cards are owned.
- **Unlock:** the game starts with 3 cards: one Monster, one Spell, one Trap. Story stages open one by one.

## Why come back?
1. **Daily rewards and quests:** a free reward every day and small quests (for example "craft 5 cards" or "win 2 battles").
2. **Collection and upgrades:** the next card or skill-tree upgrade is almost affordable, and every new card changes how you fight.

## Who and when
Casual players aged 16–35 who like simple strategy and card games. One battle takes 2–3 minutes, so it fits a bus ride or a queue; several stages in a row make a longer session at home. All controls sit in the lower part of the screen, within reach of one thumb; the battlefield on top is only watched.

## Goals
- Stage cleared (enemy base destroyed)
- Boss beaten
- All Story stages cleared
- New card bought or won from a chest
- Skill-tree upgrade bought
- Daily quest completed
- First card crafted with a Soul
- All cards collected

## Feel
- **Crafting:** materials snap into the table with a click; a matching recipe lights up Spawn with the card's picture, and the new card flies into its slot. A Soul adds a glow to the card.
- **Minions:** tapping a minion sends it out at once, and its next price ticks up; the Healer's aura pulses every 2 seconds.
- **Combat:** hit sounds, damage numbers, and coins popping out of every kill.
- **Spells and traps:** a flash across the field and a short screen shake; an armed trap shows a mark on the lane until it fires.
- **Boss:** a warning banner, the music changes, and the screen shakes on every hit.
- **Results:** "You win" or "You lose", and cash counts up into the counter.

## Scope check
- [x] Hybrid-casual: simple core + a meta layer
- [x] Portrait, one thumb
- [x] 1–3 minute sessions (one battle)
- [x] Small: about 3 levels (more for a team of 2–3)

**Full plan:** 6 Story stages on one battlefield · 4 heroes (knight, archer, wizard, viking) · 3 minions · 15 cards: 5 Monster, 5 Spell, 5 Trap · 4 materials · 2 enemy types (club, ranged orb) + 1 boss.

**First playable:** 3 stages · battle with the 4-card loadout · 3 minions · 4 materials with Soul · 6–9 cards (2–3 of each type) · Inventory and Recipes screens · Results.
**After that:** Skill tree, Shop, chests, daily rewards and quests, ads, Recent recipes, in-battle Recipe book. More stages and cards are added as data once the loop is fun.

**Screens:** Lobby (Play, daily rewards, quests, settings) · Inventory (buy cards, pick 4) · Recipes (view the recipes of owned cards) · Skill tree · Shop · Battle · Results. Stage 1 doubles as a tutorial with a few arrow hints.

**Team of 2:** stages, cards, recipes, units and enemy waves are data. Spells and traps share one set of effects (damage, freeze, slow); a trap only waits for its trigger. Our extra system is **real-time crafting**: shape matching on the 3×3 table (any position, left-right mirror) with the Soul bonus. First we test it in a greybox: the table, the material row and soldiers walking.

**Ads:** later in the course, through the publisher SDK. Rewarded: "double your cash" on Results, and in the Shop 100 Cash (3 a day) or a chest (1 a day). Interstitial: between battles.

**Out of scope:** multiplayer and Survival mode, a real-money shop (ideas for after the course), complex enemy AI, card merging, a second meta currency, cosmetic skins.
