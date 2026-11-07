# Battle Forge — Game Concept One-Pager
**Team:** Eden Lagziel, Alan Zirkiev  
**Platform:** Mobile (Portrait), Unity  
**Genre:** Hybrid-Casual / Real-Time Strategy / Crafting & Card Battler  
**References:** Battle Bag: War Zone, Stick War: Legacy, Minecraft, Yu-Gi-Oh!

**Our Twist:** Players enter battle with four selected cards, choose two reusable basic units, and craft additional troops, spells, and traps in real time. Victory depends on resource management, crafting, and tactical card timing.

## Core Loop

**Prepare:** Before battle, players select four cards from their collection. Cards may be Monsters, Spells, or Traps in any combination.

**Choose Basic Units:** At the start of each battle, a short selection phase presents three available unit cards drawn from the player's unlocked basic-unit collection. The player chooses two. These become reusable unit buttons with cooldowns during battle.

**Earn:** Gold is generated automatically over time and awarded for defeating enemy troops.

**Buy:** Players spend gold in the Material Shop to purchase crafting ingredients.

**Craft:** Players combine materials on a 3×3 Crafting Table to create additional Monster, Spell, or Trap cards. Crafting takes place while combat continues.

**Switch:** Three tabs control the crafting area: **1 — Crafting Table, 2 — Recipes, 3 — Material Shop.** The Recipes tab shows discovered recipes and the materials needed to craft them.

**Play Cards:** Four selected cards are available at the beginning of battle. Each card is consumed after use and disappears from its slot. Additional cards can be crafted during battle, subject to available materials and card capacity.

**Fight:** Units advance and fight automatically along a horizontal battlefield. The player deploys troops, activates tactical cards, and attempts to destroy the enemy base while defending their own.

## Meta

**Collection:** Players collect Monster, Spell, and Trap cards, unlock basic units, and discover crafting recipes.

**Deck Preparation:** Before each battle, players choose four cards to bring from their collection. The selection is not restricted by card type.

**Progression:** Cleared stages, unlocked cards, discovered recipes, and permanent upgrades are saved.

**Rewards:** Battle rewards support unlocking new cards, recipes, and permanent improvements. A separate meta currency can be used for long-term progression.

**Campaign:** 10 single-player stages with increasing difficulty. Multiplayer is a possible future expansion.

## Why Come Back?

1. **Collect & Discover:** Unlock new cards and recipes, experiment with crafting combinations, and create different battle loadouts.
2. **Improve & Progress:** Master resource management, basic-unit combinations, and tactical timing to defeat increasingly challenging opponents.

## Who and When

Casual strategy and card-game players who enjoy short battles, crafting, and tactical decisions.

Designed for portrait mobile play with touch-friendly controls.

**Target battle duration:** 3–5 minutes.

## Goals

- Destroy the enemy base to win a battle.
- Complete all 10 campaign stages.
- Unlock and collect Monster, Spell, and Trap cards.
- Choose effective four-card loadouts.
- Discover and use crafting recipes.
- Earn gold, purchase materials, and create reinforcements.
- Unlock new basic units and permanent improvements.

## Feel

**Battle:** A readable horizontal battlefield shows two opposing bases and automatically fighting units.

**Basic Units:** Two persistent unit buttons clearly communicate availability and cooldowns.

**Crafting:** A compact 3×3 crafting interface lets players combine materials and immediately see the resulting card.

**Cards:** Four clear card slots display available cards. Cards disappear when played, providing immediate feedback.

**Interface:** Three simple switching tabs provide access to the Crafting Table, Recipe Book, and Material Shop without leaving the battle.

**Visual Style:** Stylized fantasy with reusable sprites, intuitive icons, and lightweight effects. Gameplay and code take priority over complex artwork.

## Scope Check

**Core MVP:**
- 10 configurable single-player stages.
- One reusable horizontal battlefield with two bases.
- Pre-battle selection of four collectible cards.
- Selection of two basic units from three offered candidates.
- Reusable basic-unit buttons with cooldowns.
- Single-use Monster, Spell, and Trap cards.
- Real-time 3×3 Crafting Table.
- Recipe Book and Material Shop tabs.
- Gold income from time and enemy kills.
- Simple rule-based enemy deployment and combat.
- Basic card collection, progression, and save system.

**Out of Scope:** Multiplayer networking, complex enemy AI, large skill trees, advanced card merging, monetization, and extensive visual effects.

**Development Approach:** Reusable gameplay components, data-driven card and recipe definitions, and configurable enemy patterns support multiple stages without requiring separate systems for each level.
