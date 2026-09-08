# Persona Devouring Pawn test checklist

The current focused test pass uses only Core, Royalty, and this mod. Compatibility traits are documented below but are not included in the active test cycle until the Royalty-only path is verified.

These checklists target the installed RimWorld 1.6 content. For supported items, create or generate a persona weapon containing the trait, give a colonist the Persona Devourer trait, devour the weapon, and verify the resulting `Devoured: <trait label>` hediff and gameplay effect. For unsupported or ignored items, verify the weapon is preserved and the trait is not transferred. Leave the checkbox unchecked until the behavior has been tested in-game.

Passive traits that only add stat offsets are intentionally excluded from adaptor testing because those stages are already working correctly. The active checklist is ordered as OnKill effects, OnHit effects, Bonded effects, then unsupported or ignored traits.

## Royalty

Royalty stat-offset traits are intentionally omitted from this adaptor checklist; their passive stages are already working correctly. The remaining entries are ordered by active adaptor category.

### OnKill effects

- [ ] `OnKill_PsyfocusGain` — kill-focused
- [ ] `OnKill_ThoughtGood` — kill-happy

### OnHit effects

- None in the tested Royalty set.

### Bonded effects

- [ ] `NoPain` — painless
- [ ] `ThoughtKind` — kind thoughts
- [ ] `ThoughtCalm` — calm thoughts

### Unsupported or ignored

- [ ] `OnKill_ThoughtBad` — negative; verify ignored
- [ ] `NeedKill` — negative; verify ignored
- [ ] `NeverBond` — neutral; verify ignored
- [ ] `Jealous` — negative; verify ignored
- [ ] `HungerMaker` — negative; verify ignored
- [ ] `ThoughtMuttering` — negative; verify ignored
- [ ] `ThoughtWailing` — negative; verify ignored
- [ ] `PsychicSensitivityDownMinor` — negative; verify ignored
- [ ] `PsychicSensitivityDownMajor` — negative; verify ignored

`PsychicSensitivityUpMajor`, `PsychicSensitivityUpMinor`, `SpeedBoost`, `NeuralHeatRecoveryGain`, and `PsyfocusMeditationBonus` are passive stat-offset checks and are intentionally not repeated here.

## Vanilla Persona Weapons Expanded (2826922787)

This mod adds a persona weapon type but no additional `WeaponTraitDef`s in its installed 1.6 XML.

### Weapon type coverage

- [ ] `VPWE_MeleeWeapon_PsyfocusStaffBladelink` — persona eltex staff

### Integration checks

- [ ] Generated/devoured weapon retains the mod's persona weapon behavior before consumption.
- [ ] Devour works on the weapon when spawned on the map.
- [ ] Devour works on the weapon in the Devourer's inventory.
- [ ] Devour works on the weapon while equipped, through the Gear tab.

## More Persona Traits (2863308112)

### OnKill effects

- [ ] `MPT_OnKill_NeedFilledFood` — kill-satiating
- [ ] `MPT_OnKill_NeedFilledComfort` — kill-comforting
- [ ] `MPT_OnKill_NeedFilledJoy` — kill-joyful
- [ ] `MPT_OnKill_NeedFilledBeauty` — beauty-in-death
- [ ] `MPT_OnKill_NeedFilledRest` — kill-relaxation
- [ ] `MPT_OnKill_NeedFilledInOutdoors` — kill-niche
- [ ] `MPT_OnKill_NeedFilledChemical` — kill-euphoric
- [ ] `MPT_OnKill_Invis` — lightweaving

### OnHit effects

- [ ] `MPT_OnHit_Plague` — plague bringer
- [ ] `MPT_OnHit_Flu` — flu carrier
- [ ] `MPT_OnHit_Pain` — pain-dealer
- [ ] `MPT_OnHit_Stun` — stunning
- [ ] `MPT_OnHit_SpawnFoam` — foamy
- [ ] `MPT_OnHit_SpawnFuel` — combustible
- [ ] `MPT_OnHit_HealWielder` — life-tapping
- [ ] `MPT_OnHit_FoodSelfUp` — food restoration
- [ ] `MPT_OnHit_RestSelfUp` — rest restoration

### Bonded effects

- [ ] `MPT_Bonded_ImmunePenoxycyline` — penoxycylined
- [ ] `MPT_Bonded_ImmuneMechanite` — mechan't
- [ ] `MPT_Bonded_PainOffsetDownMinor` — relieving
- [ ] `MPT_Bonded_PainOffsetDownMajor` — mollifying
- [ ] `MPT_Bonded_BleedFactorDown` — clotting
- [ ] `MPT_Bonded_NaturalHealingFactorUp` — regenerative
- [ ] `MPT_Bonded_ThoughtWeatherUp` — meteorological
- [ ] `MPT_Bonded_ThoughtFemaleLove` — enatic
- [ ] `MPT_Bonded_ThoughtMaleLove` — agnatic

Passive held/bonded stat-offset traits are intentionally omitted because their stages are already working correctly.

### Unsupported or ignored

These traits are not adaptor-test targets because the effect is not safely supported, or the trait is negative/neutral and blacklisted:

- [ ] `MPT_OnKill_TransformToGold` — unsupported resource transformation
- [ ] `MPT_OnKill_TransformToPlasteel` — unsupported resource transformation
- [ ] `MPT_OnHit_AddedFlame_Minor`, `MPT_OnHit_AddedFlame`, `MPT_OnHit_AddedFlame_Major` — unsupported added damage
- [ ] `MPT_OnHit_AddedBlunt_Minor`, `MPT_OnHit_AddedBlunt`, `MPT_OnHit_AddedBlunt_Major` — unsupported added damage
- [ ] `MPT_OnHit_AddedCut_Minor`, `MPT_OnHit_AddedCut`, `MPT_OnHit_AddedCut_Major` — unsupported added damage
- [ ] `MPT_OnHit_AddedEMP` — unsupported added damage
- [ ] `MPT_OnHit_AddedBulletToxic_Minor`, `MPT_OnHit_AddedBulletToxic`, `MPT_OnHit_AddedBulletToxic_Major` — unsupported added damage
- [ ] `MPT_OnHit_ChangeWeather`, `MPT_OnHit_ChangeWeatherToPall` — unsupported weather adaptor
- [ ] `MPT_OnHit_TeleportTarget`, `MPT_OnHit_TeleportSelf` — unsupported teleport adaptor
- [ ] `MPT_OnHit_ThoughtSelfUp` — unsupported thought adaptor
- [ ] `MPT_OnHit_DrainBlood`, `MPT_OnHit_BloodLoss` — partial gene-resource adaptor; verify blocked atomically
- [ ] `MPT_OnHit_PlagueSelf`, `MPT_OnHit_FluSelf`, `MPT_OnHit_PainSelf`, `MPT_OnHit_BloodLossSelf` — negative; verify ignored
- [ ] `MPT_OnHit_SpawnAsh`, `MPT_OnHit_SpawnSlime`, `MPT_OnHit_SpawnBlood`, `MPT_OnHit_SpawnFuelSelf` — negative; verify ignored
- [ ] `MPT_OnHit_FoodSelfDown`, `MPT_OnHit_RestSelfDown`, `MPT_OnHit_ThoughtSelfDown` — negative; verify ignored

## Medieval Persona Weapons (2869057049)

This mod adds persona weapon types but no additional `WeaponTraitDef`s in its installed 1.6 XML.

### Weapon type coverage

- [ ] `MPW_Bladelink_Mace`
- [ ] `MPW_Bladelink_Gladius`
- [ ] `MPW_Bladelink_LongSword`
- [ ] `MPW_Bladelink_Knife`
- [ ] `MPW_Bladelink_Spear`
- [ ] `MPW_Bladelink_Beer`
- [ ] `MPW_Bladelink_Log`
- [ ] `MPW_Bladelink_Club`
- [ ] `MPW_Bladelink_Ikwa`
- [ ] `MPW_Bladelink_BreachAxe`
- [ ] `MPW_Bladelink_Axe`
- [ ] `MPW_Bladelink_Warhammer`

### Integration checks

- [ ] Stuff-based weapon generation supplies valid default stuff.
- [ ] Each generated weapon can be devoured when it has a supported Royalty or More Persona Traits ability.
- [ ] Devour works from the ground, inventory, and equipment rows.
