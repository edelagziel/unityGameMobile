# Pitch: Battle Forge

**Team:** Eden Lagziel, Alan Zirkiev  
**Platform:** Mobile (Portrait), Unity  
**Genre:** Hybrid-Casual / Real-Time Strategy / Crafting & Card Battler  
**References:** *Battle Bag: War Zone*, *Stick War: Legacy*, *Minecraft*, *Yu-Gi-Oh!*

**Our Twist:** Craft troops and tactical cards during real-time battles. Every crafted result becomes a playable card, allowing players to combine army-building, economic decisions, and reactive card strategy.

## Core Loop

**Earn:** Gold is generated automatically throughout battle. Defeated enemies may drop crafting materials, while economic units such as miners provide additional income but can be attacked and killed.

**Buy & Upgrade:** Spend gold to purchase crafting materials, improve income generation, strengthen the base, or invest in military production.

**Craft:** Arrange materials on a 3×3 crafting grid to discover recipes. The three most recently crafted recipes become available in Quick Craft for faster production. Combat continues normally while crafting.

**Play Cards:** Crafted troops, squads, spells, and traps become playable cards. Troop cards display the number of units they deploy. Players choose when to use them, without additional mana costs.

**Draw & React:** Players also start with cards from a prebuilt deck and draw a random card approximately every 15–20 seconds. Spells and actions are activated manually, while traps can trigger automatically or through timed reactions to battlefield events.

**Fight:** Units move and fight automatically along a horizontal route between opposing bases. The player wins by destroying the enemy base before losing their own.

## Meta

**Progression:** Unlock new crafting recipes, troop types, and tactical cards by completing stages.

**Collection:** Discovered recipes, unlocked cards, and permanent upgrades are saved between battles.

**Game Modes:** Single Player is the focus of the course version. Multiplayer is a potential future expansion with separate balancing and progression rules.

**Campaign:** 10 single-player stages with increasing difficulty, varied enemy compositions, and new tactical challenges.

## Why Come Back?

1. **Discover & Experiment:** Unlock new recipes and cards, create stronger combinations, and experiment with different deck strategies.
2. **Improve & Progress:** Refine crafting decisions, resource management, and tactical timing to overcome more challenging battles.

## Who and When

Casual and strategy-oriented mobile players, approximately **16–35 years old**, who enjoy crafting, deck-building, and short tactical battles.

Designed for portrait, touch-friendly gameplay during breaks, commutes, or short play sessions.

**Target battle duration:** 1–3 minutes. A stage can be retried if the player loses.

## Goals

- Destroy the enemy base and win a battle.
- Complete each of the 10 campaign stages.
- Discover and unlock crafting recipes.
- Craft troops, squads, spells, and traps.
- Successfully use tactical cards during combat.
- Purchase economic or defensive upgrades.
- Unlock new cards and troop types.
- Experiment with different deck combinations.

## Feel

**Crafting:** Materials snap into the grid with clear feedback. Successful recipes trigger a short reveal animation and produce a playable card.

**Cards:** Troop quantities, spell effects, and trap conditions are clearly displayed. Drawing or crafting a card provides satisfying visual feedback.

**Combat:** Units march and fight in real time, with readable attacks, impacts, and defensive reactions.

**Progression:** New discoveries, unlocked recipes, and stage victories are celebrated through concise visual and audio effects.

**Art Direction:** Colorful painterly fantasy with reusable visual assets, keeping development focused on gameplay and code rather than complex art production.

## Scope Check

**Course Requirements:**
- Hybrid-casual game with simple controls and strategic depth.
- Portrait mobile orientation.
- Target session length of 1–3 minutes.
- Manageable scope for a team of two.

**Content:**
- **10 single-player stages** using reusable level configurations.
- 1 main horizontal battlefield layout.
- 3 basic player troop types.
- 3 enemy types and 1 boss type.
- 4 crafting material types.
- 6 initial crafting recipes.
- 4 tactical card effects.
- Passive gold income and basic economic upgrades.
- Simple rule-based enemy AI.

**Screens:** Main Menu · Level Selection · Battle & Crafting · Collection / Deck · Results · Settings.

**Out of Scope:** Multiplayer networking, paid purchases, advertisements, extensive skill trees, large open worlds, complex enemy AI, and advanced card merging or rarity systems.

**Implementation Approach:** Reusable Unity components and data-driven level configurations allow new stages to vary enemy combinations, deployment timing, available resources, and difficulty without requiring new gameplay systems for every stage.
