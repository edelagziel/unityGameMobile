# Battle Forge skill trees

## Try the feature

Open the primary project, `unityGameMobile`, with Unity 6000.6.7f1, choose a portrait Game view and press Play. Bootstrap loads Home. Win one battle with the existing stand-in Win button for 150 Cash and return to the menu. Open the Skill Tree tab. Select Army Drills and buy level 1 for 100 Cash. With a fresh balance the screen and HUD now show 50 Cash. Stop and start Play to see the saved level. Use the tree title button to cycle through Global Training, Swordsman and Archer. Locked nodes can be selected to read their requirements.

Global Training opens on a neutral Core between its two root skills, Army Drills and Trade Routes. Drag the map in either direction and turn the mouse wheel over a node to zoom. On touchscreens, drag with one finger or pinch with two. **Center** frames the full hierarchy around Core. Zoom in over a branch to inspect its labels. The tree title switches maps.

## Runtime ownership

The existing scene tab hosts a runtime-built uGUI graph under the existing Safe Area. The view raises selection and purchase events. The plain C# controller coordinates navigation, wallet notifications and rendering. The screen model owns the selected context and skill. The Bootstrap-scoped skill service validates the catalog, quotes the next level, checks prerequisites, requests payment and exposes cumulative effects.

`SkillTreeView` builds the screen controls once. `SkillGraphRenderer` creates nodes indexed by skill ID and draws one edge per distinct prerequisite skill, including prerequisites on later levels. All edges and nodes share the same centered content transform. Switching definitions replaces that content. Switching owners on the same definition refreshes its offers in place. Selecting a node moves the highlight and updates the details from the current offers. Cash and purchases refresh offers, and each node updates its label and color only when its displayed level or availability changes.

`MapPanZoomController` extends the masked two-axis ScrollRect. It keeps the map point under the wheel cursor or moving pinch midpoint anchored until a map boundary is reached. Maximum zoom is 2×. The lower limit is 0.45× or the scale required to fit the entire map, whichever is smaller. When a scaled axis fits inside the viewport, that axis stays centered. The details and purchase controls sit outside the mask.

A context combines a stable tree ID and owner ID. Each receipt also includes the skill ID. IDs allow ASCII letters, digits, underscores, dots and hyphens. Slashes separate the three components. Conservation of these IDs across releases preserves purchases. Renaming the title is safe. Removing a definition hides its effects. Reducing the number of authored levels caps the effective level. Reintroducing the same ID restores the applicable receipts.

The tree picker uses the owner `player` for `global` and `type.<tree-id>` for the other examples. Inventory integration can call `SkillTreeController.Open(new SkillContext("swordsman", "card.2837"))` to show one instance, or pass `type.swordsman` to share upgrades across a type. Inventory owns issuance and authorization of these IDs. Gameplay can query `ISkillService.GrantedEffects(context)` for each source skill, level and effect.

## Radial layout and connection colors

Each ordered pair of prerequisite and dependent skills produces one arrow. Since later upgrades also contribute edges, an outer ring can contain a skill whose first level is already available. The edge retains its base threshold and its thresholds for individual target levels. A base gate and a level gate on the same source use their greater threshold for that level. For the selected owner's next target level, amber marks a missing requirement, green marks a met requirement and gray marks an edge that applies to a different level. For a mastered target, each arrow compares the current source level with the highest threshold that pair required across all levels.

Core appears at the origin of every map. A solid neutral spoke connects it to every skill without incoming visual prerequisites. Each other skill has one solid arrow from its primary parent. All solid links remain visible during selection and purchases. Selecting a skill reveals its additional incoming prerequisite arrows as thinner dashed lines. Core is a presentation marker with no purchase ID or gameplay requirements.

The layout chooses a deepest incoming parent, breaking ties by category and stable IDs. When reciprocal level requirements form a cycle between skill nodes, it selects one entry into the group, then walks real directed requirements to attach the other members. A source cycle with no outside prerequisite receives one Core spoke to its stable entry. The remaining real dependencies are secondary links.

Roots occupy separate angular sectors. Children share their parent's sector according to branch size. Every primary edge advances exactly one ring, and all rings use the same pitch, enlarged when needed for node and caption clearance. Lines end on the circle rims. A line that would cross another node or Core detours through the spaces between rings. Nodes and strokes share map-local coordinates. The initial framing includes the outermost nodes and routes. Zooming in clips the map at the viewport.

Node faces use slate for locked skills, green for purchasable skills, brown for an unpurchased skill short of Cash, blue for an acquired skill with levels remaining, and gold for mastery. A pale outline marks selection. The level count and details panel give the exact progression state.

## Authoring

1. Choose **Assets → Create → Game → Skill Tree**. Assign a unique stable tree ID and title. Add the asset to **Assets/Config/GameConfig.asset → Skill Trees**. Array order sets the picker order.
2. Set **Node Spacing** for minimum clearance between skill footprints and **Ring Spacing** for minimum distance between successive rings, both in Canvas units. Add each skill's ID, title, description, category and optional icon. An assigned sprite appears in the center of the node. Skills without a sprite show a category emblem. The radial layout computes node positions, focus and map size automatically.
3. Add **Levels** in purchase order. Each level has a description, pricing choice, requirements and effects. Base requirements apply to every level. Each requirement names a skill in the same tree and its minimum level. All must be satisfied. Multiple prerequisites can connect across branches.
4. For individually balanced levels set **Fixed Cash** to a positive integer. For economy-wide pricing enable **Use Economy Curve**. The shared policy uses the existing Economy upgrade curve at the number of already-purchased levels and respects its maximum. The shipped sample values are provisional.
5. Expand **Effects**, choose **Add effect**, then select its type. Stat modifiers select a stat and additive amount. Gameplay modifiers use a stable rule ID and additive amount. Content unlocks select Card or Recipe and a stable content ID. Ability unlocks use a stable ability ID. Changing the type replaces the selected effect. The minus button removes it. Every purchased level contributes its own entries. Combat and inventory consumers determine how to apply them.
6. Run EditMode tests after design changes. Bootstrap rejects duplicate IDs, invalid prices/levels, invalid effects, missing references and unreachable level cycles with a diagnostic naming the affected definition.

The five additional global examples are Shield Formation, Vanguard, Market Network, Workshop and Reserve Deck. Each uses a `sample.*` effect target for demonstrating branch progression. The existing Army Drills, Trade Routes, Field Forge and Reserve Cards retain their original requirements, effects and prices.

## Purchase durability

WalletService exposes ordinary Cash operations through IWalletService and receipted purchases through IPurchaseWallet. It verifies the expected next quantity and writes a candidate account containing the new Cash balance and receipt in one call to ISaveService. Only a successful save publishes state and notifies subscribers. A retry of the original target cannot debit the following level. A subscriber failure is logged separately from the purchase outcome.

The on-device record is `wallet.account.v1`. Existing installations load the legacy `wallet.cash` until the first account write. The old key remains available for migration. Older builds will read their last legacy balance if installed over this version. Unknown receipt products are retained. PlayerPrefs has no documented hardware-level transaction or corruption-recovery guarantee. The single-record design avoids a split wallet/progression write. Purchases run on Unity's main thread in one process. Large inventories, cloud sync or concurrent clients would need a storage design for those workloads.

## Review map

Paths are relative to the project. New Unity assets and directories include their corresponding .meta files.

| Directory | New files | Modified files |
| --- | --- | --- |
| Assets/Config | Skills/global.asset, Skills/swordsman.asset, Skills/archer.asset | GameConfig.asset |
| Assets/Scripts/Services/Config/Models/Skills | SkillTreeDefinition.cs, SkillDefinition.cs, SkillLevelDefinition.cs, SkillRequirement.cs, SkillEffectDefinition.cs, NumericModifierEffect.cs, StatModifierEffect.cs, GameplayModifierEffect.cs, ContentUnlockEffect.cs, AbilityUnlockEffect.cs, SkillStat.cs, SkillContentType.cs, SkillEffectListAttribute.cs | |
| Assets/Scripts/Services/Config | | Interfaces/IConfigService.cs, Local/ScriptableObjectConfigService.cs, Models/GameConfig.cs |
| Assets/Scripts/Services/Skills | Interfaces/ISkillService.cs, SkillCatalog.cs, SkillContext.cs, SkillPricing.cs, SkillService.cs, SkillAvailability.cs, SkillOffer.cs | |
| Assets/Scripts/Services/Wallet | Interfaces/IPurchaseWallet.cs | WalletService.cs |
| Assets/Scripts/Services/Save/Local | | PlayerPrefsSaveService.cs |
| Assets/Scripts/ScreenNavigation/Screens/SkillTree | ISkillTreeModel.cs, ISkillTreeView.cs, SkillTreeWidgets.cs, MapPanZoomController.cs, SkillMapLayout.cs, SkillMapRouting.cs, SkillMapEdge.cs, SkillDependency.cs, SkillGraphRenderer.cs, SkillNodeView.cs, SkillCircleGraphic.cs, SkillEmblemGraphic.cs, SkillArrowGraphic.cs, SkillDetailsPanel.cs, SkillPurchaseButton.cs | SkillTreeModel.cs, SkillTreeView.cs, SkillTreeController.cs |
| Assets/Scripts/Composition | | BootstrapLifetimeScope.cs, ScreenNavigation/ScreenNavigationLifetimeScope.cs |
| Assets/Tests/Services | SkillServiceTests.cs | |
| Assets/Tests/EditMode | SkillTreeHomeTests.cs, SkillMapLayoutTests.cs, MapPanZoomTests.cs | Game.ScreenNavigation.Tests.asmdef, ScreenMvpRegistrationTests.cs, TabBarNavigationTests.cs, TestDoubles.cs |
| Assets/Tests/InputSupport | Game.InputTestSupport.asmdef, SkillMapTestInput.cs | |
| Assets/Tests/PlayMode | Game.SkillMap.PlayModeTests.asmdef, RadialSkillMapViewTests.cs | |
| Project documentation | docs/SKILL_TREE.md | AGENTS.md |

SkillServiceTests covers the shipped definitions, pricing modes, purchase rejection and replay, cumulative effects, legacy migration, a real PlayerPrefs round trip on isolated keys, and a failed save. SkillTreeHomeTests enters Play Mode from EditMode, opens the shipped Home tab and exercises node identity, selection, scaled edges, tree and owner switching, and purchases with an isolated wallet. It also sends synthetic mouse and touch input through Home's StandaloneInputModule. MapPanZoomTests exercises clamping and focus at two portrait viewport sizes. Physical Android input, physical storage interruption and combat effect application require separate verification.
