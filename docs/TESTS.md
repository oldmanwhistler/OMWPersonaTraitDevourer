# Persona Devouring Pawn test checklist

These checklists target the installed RimWorld 1.6 content. For each item, create or generate a persona weapon containing the trait, give a colonist the Persona Devourer trait, devour the weapon, and verify the resulting `Devoured: <trait label>` hediff and gameplay effect. Leave the checkbox unchecked until the behavior has been tested in-game.

## Royalty

### Passive/stat and hediff traits

- [ ] `PsychicSensitivityUpMajor` — psychic hypersensitizer
- [ ] `PsychicSensitivityUpMinor` — psychic sensitizer
- [ ] `PsychicSensitivityDownMinor` — psychic quiet (negative; verify ignored)
- [ ] `PsychicSensitivityDownMajor` — psychic fog (negative; verify ignored)
- [ ] `NoPain` — painless
- [ ] `SpeedBoost` — fast mover
- [ ] `HungerMaker` — hunger pangs (negative; verify ignored)
- [ ] `NeuralHeatRecoveryGain` — neural cooling
- [ ] `PsyfocusMeditationBonus` — psy-meditative

### Thought and kill traits

- [ ] `ThoughtKind` — kind thoughts
- [ ] `ThoughtCalm` — calm thoughts
- [ ] `ThoughtMuttering` — mad muttering (negative; verify ignored)
- [ ] `ThoughtWailing` — mad wailing (negative; verify ignored)
- [ ] `OnKill_PsyfocusGain` — kill-focused
- [ ] `OnKill_ThoughtGood` — kill-happy
- [ ] `OnKill_ThoughtBad` — kill sorrow (negative; verify ignored)
- [ ] `NeedKill` — kill thirst (negative/unsupported bond behavior; verify ignored)
- [ ] `NeverBond` — freewielder (neutral; verify ignored)
- [ ] `Jealous` — jealous (negative; verify ignored)

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

### On-kill traits

- [ ] `MPT_OnKill_NeedFilledFood` — kill-satiating
- [ ] `MPT_OnKill_NeedFilledComfort` — kill-comforting
- [ ] `MPT_OnKill_NeedFilledJoy` — kill-joyful
- [ ] `MPT_OnKill_NeedFilledBeauty` — beauty-in-death
- [ ] `MPT_OnKill_NeedFilledRest` — kill-relaxation
- [ ] `MPT_OnKill_NeedFilledInOutdoors` — kill-niche
- [ ] `MPT_OnKill_NeedFilledChemical` — kill-euphoric
- [ ] `MPT_OnKill_Invis` — lightweaving
- [ ] `MPT_OnKill_TransformToGold` — midas-touch
- [ ] `MPT_OnKill_TransformToPlasteel` — fullmetal
- [ ] `MPT_OnKill_TransformToWood` — logger
- [ ] `MPT_OnKill_TransformToBioferrite` — ferrous (Anomaly-gated)
- [ ] `MPT_OnKill_TransformToMeat_Twisted` — twisting (Anomaly-gated/negative; verify ignored)

### Held/stat traits

- [ ] `MPT_Held_MeleeHitChanceUp` — conclusive
- [ ] `MPT_Held_MeleeHitChanceDown` — squeamish (negative; verify ignored)
- [ ] `MPT_Held_ArmorRatingUp` — non-newtonian
- [ ] `MPT_Held_ArmorRatingDown` — flaw-maker (negative; verify ignored)
- [ ] `MPT_Held_PawnBeautyUp` — enchanting
- [ ] `MPT_Held_PawnBeautyDown` — repellent (negative; verify ignored)
- [ ] `MPT_Held_MeleeDodgeChanceUp` — edgedancer
- [ ] `MPT_Held_MeleeDodgeChanceDown` — death-marked (negative; verify ignored)
- [ ] `MPT_Held_ComfyTempUp` — thermoregulating
- [ ] `MPT_Held_ComfyTempDown` — thermally diffusing (negative; verify ignored)
- [ ] `MPT_Held_ToxicSensitivityUp` — baneful (negative; verify ignored)
- [ ] `MPT_Held_ToxicSensitivityDown` — non-reactive
- [ ] `MPT_Held_PsychicEntropyMaxUp` — entropic heavyweight
- [ ] `MPT_Held_PsychicEntropyMaxDown` — entropic lightweight (negative; verify ignored)
- [ ] `MPT_Held_MechBandwidthUp` — multithreaded (Biotech-gated)
- [ ] `MPT_Held_MechBandwidthDown` — faint (negative; verify ignored)
- [ ] `MPT_Held_StaggerDurationDown` — reduced stagger duration
- [ ] `MPT_Held_StaggerDurationUp` — increased stagger duration (negative; verify ignored)
- [ ] `MPT_Held_MeleeDamageFactorUp` — increased melee damage
- [ ] `MPT_Held_MeleeDamageFactorDown` — reduced melee damage (negative; verify ignored)
- [ ] `MPT_Held_Vomit` — vomiting (negative; verify ignored)

### Bonded hediff traits

- [ ] `MPT_Bonded_ImmunePenoxycyline` — penoxycylined
- [ ] `MPT_Bonded_ImmuneParasite` — parasn't (currently commented in installed XML)
- [ ] `MPT_Bonded_ImmuneMechanite` — mechan't
- [ ] `MPT_Bonded_PainOffsetUpMinor` — inflaming (negative; verify ignored)
- [ ] `MPT_Bonded_PainOffsetUpMajor` — agonizing (negative; verify ignored)
- [ ] `MPT_Bonded_PainOffsetDownMinor` — relieving
- [ ] `MPT_Bonded_PainOffsetDownMajor` — mollifying
- [ ] `MPT_Bonded_Vomit` — disgorging (negative; verify ignored)
- [ ] `MPT_Bonded_BleedFactorUp` — hemophile (negative; verify ignored)
- [ ] `MPT_Bonded_BleedFactorDown` — clotting
- [ ] `MPT_Bonded_NaturalHealingFactorUp` — regenerative
- [ ] `MPT_Bonded_NaturalHealingFactorDown` — slow-healing (negative; verify ignored)

### Bonded thought traits

- [ ] `MPT_Bonded_ThoughtWeatherDown` — stormdepressed (negative; verify ignored)
- [ ] `MPT_Bonded_ThoughtWeatherUp` — meteorological
- [ ] `MPT_Bonded_ThoughtFemaleHate` — misogynist (negative; verify ignored)
- [ ] `MPT_Bonded_ThoughtFemaleLove` — enatic
- [ ] `MPT_Bonded_ThoughtMaleHate` — misandrist (negative; verify ignored)
- [ ] `MPT_Bonded_ThoughtMaleLove` — agnatic
- [ ] `MPT_Bonded_ThoughtNoAnimalHate` — zoophile (negative; verify ignored)
- [ ] `MPT_Bonded_ThoughtAnimalHate` — zoophobia (negative; verify ignored)

### Bonded stat-offset traits

- [ ] `MPT_Bonded_ResearchUp` — insightful
- [ ] `MPT_Bonded_ResearchDown` — rambling (negative; verify ignored)
- [ ] `MPT_Bonded_GlobalWorkspeedUp` — driven
- [ ] `MPT_Bonded_GlobalWorkspeedDown` — slacking (negative; verify ignored)
- [ ] `MPT_Bonded_ShootingAccuracyPawnUp` — spotter
- [ ] `MPT_Bonded_ShootingAccuracyPawnDown` — trooper-aimed (negative; verify ignored)
- [ ] `MPT_Bonded_TradePriceImprovementUp` — poker faced
- [ ] `MPT_Bonded_TradePriceImprovementDown` — histrionic (negative; verify ignored)
- [ ] `MPT_Bonded_SurgerySuccessChanceFactorUp` — exact
- [ ] `MPT_Bonded_SurgerySuccessChanceFactorDown` — careless (negative; verify ignored)
- [ ] `MPT_Bonded_RestRateMultiplierUp` — improved rest rate
- [ ] `MPT_Bonded_RestRateMultiplierDown` — worsened rest rate (negative; verify ignored)
- [ ] `MPT_Bonded_ImmunityGainSpeedUp` — immune response
- [ ] `MPT_Bonded_ImmunityGainSpeedDown` — impaired immunity (negative; verify ignored)
- [ ] `MPT_Bonded_MedicalTendQualityOffsetUp` — medical precision
- [ ] `MPT_Bonded_MedicalTendQualityOffsetDown` — medical error (negative; verify ignored)
- [ ] `MPT_Bonded_GlobalLearningFactorUp` — quick study
- [ ] `MPT_Bonded_GlobalLearningFactorDown` — slow study (negative; verify ignored)
- [ ] `MPT_Bonded_MentalBreakThresholdDown` — unstable (negative; verify ignored)
- [ ] `MPT_Bonded_MentalBreakThresholdUp` — stable
- [ ] `MPT_Bonded_CookSpeedUp` — culinary drive
- [ ] `MPT_Bonded_CookSpeedDown` — culinary drag (negative; verify ignored)
- [ ] `MPT_Bonded_MiningSpeedUp` — mining drive
- [ ] `MPT_Bonded_MiningSpeedDown` — mining drag (negative; verify ignored)
- [ ] `MPT_Bonded_PlantSpeedUp` — green thumb
- [ ] `MPT_Bonded_PlantSpeedDown` — plant neglect (negative; verify ignored)
- [ ] `MPT_Bonded_ConstructionUp` — builder
- [ ] `MPT_Bonded_ConstructionDown` — shaky builder (negative; verify ignored)
- [ ] `MPT_Bonded_AimingDelayFactorDown` — quick aim
- [ ] `MPT_Bonded_AimingDelayFactorUp` — slow aim (negative; verify ignored)
- [ ] `MPT_Bonded_GeneralLaborSpeedUp` — labor drive
- [ ] `MPT_Bonded_GeneralLaborSpeedDown` — labor drag (negative; verify ignored)
- [ ] `MPT_Bonded_SocialFightChanceFactorDown` — pacifying
- [ ] `MPT_Bonded_SocialFightChanceFactorUp` — quarrelsome (negative; verify ignored)
- [ ] `MPT_Bonded_PainShockThresholdUp` — pain tolerance
- [ ] `MPT_Bonded_PainShockThresholdDown` — fragile (negative; verify ignored)
- [ ] `MPT_Bonded_HuntingStealthUp` — hunter's hush
- [ ] `MPT_Bonded_HuntingStealthDown` — noisy hunter (negative; verify ignored)
- [ ] `MPT_Bonded_RanchingUp` — animal handler
- [ ] `MPT_Bonded_RanchingDown` — poor handler (negative; verify ignored)
- [ ] `MPT_Bonded_WardenUp` — warden's aid
- [ ] `MPT_Bonded_WardenDown` — warden's burden (negative; verify ignored)
- [ ] `MPT_Bonded_LifespanDown` — shortened life (negative; verify ignored)
- [ ] `MPT_Bonded_LifespanUp` — longevity
- [ ] `MPT_Bonded_CleaningSpeedDown` — messy (negative; verify ignored)
- [ ] `MPT_Bonded_CleaningSpeedUp` — cleanliness
- [ ] `MPT_Bonded_PsychicEntropyMaxDown` — entropic lightweight (negative; verify ignored)
- [ ] `MPT_Bonded_PsychicEntropyMaxUp` — entropic heavyweight
- [ ] `MPT_Bonded_FertilityUp` — fertile
- [ ] `MPT_Bonded_FertilityDown` — infertile (negative; verify ignored)
- [ ] `MPT_Bonded_MechStatsUp` — mech enhancement (Biotech-gated)
- [ ] `MPT_Bonded_MechStatsDown` — mech impairment (negative; verify ignored)
- [ ] `MPT_Bonded_CertaintyLossFactorUp` — ideological resilience
- [ ] `MPT_Bonded_CertaintyLossFactorDown` — ideological doubt (negative; verify ignored)
- [ ] `MPT_Bonded_AnomalyStudyUp` — anomaly insight (Anomaly-gated)
- [ ] `MPT_Bonded_AnomalyStudyDown` — anomaly confusion (negative; verify ignored)

### On-hit traits

- [ ] `MPT_OnHit_AddedFlame_Minor`
- [ ] `MPT_OnHit_AddedFlame`
- [ ] `MPT_OnHit_AddedFlame_Major`
- [ ] `MPT_OnHit_AddedBlunt_Minor`
- [ ] `MPT_OnHit_AddedBlunt`
- [ ] `MPT_OnHit_AddedBlunt_Major`
- [ ] `MPT_OnHit_AddedCut_Minor`
- [ ] `MPT_OnHit_AddedCut`
- [ ] `MPT_OnHit_AddedCut_Major`
- [ ] `MPT_OnHit_AddedEMP`
- [ ] `MPT_OnHit_AddedBulletToxic_Minor`
- [ ] `MPT_OnHit_AddedBulletToxic`
- [ ] `MPT_OnHit_AddedBulletToxic_Major`
- [ ] `MPT_OnHit_Plague`
- [ ] `MPT_OnHit_PlagueSelf`
- [ ] `MPT_OnHit_Flu`
- [ ] `MPT_OnHit_FluSelf`
- [ ] `MPT_OnHit_Pain`
- [ ] `MPT_OnHit_PainSelf`
- [ ] `MPT_OnHit_DrainBlood`
- [ ] `MPT_OnHit_BloodLossSelf`
- [ ] `MPT_OnHit_BloodLoss`
- [ ] `MPT_OnHit_SpawnAsh`
- [ ] `MPT_OnHit_SpawnSlime`
- [ ] `MPT_OnHit_SpawnBlood`
- [ ] `MPT_OnHit_SpawnFoam`
- [ ] `MPT_OnHit_SpawnFuel`
- [ ] `MPT_OnHit_SpawnFuelSelf`
- [ ] `MPT_OnHit_Stun`
- [ ] `MPT_OnHit_ChangeWeather`
- [ ] `MPT_OnHit_ChangeWeatherToPall` (Anomaly-gated)
- [ ] `MPT_OnHit_HealWielder`
- [ ] `MPT_OnHit_TeleportTarget`
- [ ] `MPT_OnHit_TeleportSelf`
- [ ] `MPT_OnHit_FoodSelfUp`
- [ ] `MPT_OnHit_FoodSelfDown` (negative; verify ignored)
- [ ] `MPT_OnHit_RestSelfUp`
- [ ] `MPT_OnHit_RestSelfDown` (negative; verify ignored)
- [ ] `MPT_OnHit_ThoughtSelfUp`
- [ ] `MPT_OnHit_ThoughtSelfDown` (negative; verify ignored)

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
