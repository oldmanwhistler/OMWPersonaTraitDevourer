# [OMW] Persona Trait Devourer

## Description

### English

Pawns with the "Persona Devourer" trait can consume persona weapons and permanently acquire their buffs.

Inspired by on the webtoon [Artifact Devouring Player](https://www.webtoons.com/en/action/artifact-devouring-player/ep-1-player/viewer?title_no=8200&episode_no=2).

### Simplified Chinese

拥有“人格吞噬者”特质的殖民者可以吞噬人格武器，并永久获得其增益效果。

### French

Les colonisateurs dotés du trait « Dévoreur d'élément de personnalité » peuvent consommer des armes liées et obtenir leurs bonus de manière permanente.

### Japanese

「ペルソナ喰らい」の特性を持つポーンは、ペルソナ武器を喰らうことで、そのバフを永続的に獲得できます。

### Korean

'페르소나 포식자' 특성을 가진 림은 페르소나 무기를 소비하여 해당 버프를 영구적으로 획득할 수 있습니다.

### Russian

Пешки с чертой «Пожиратель персоны» могут поглощать оружие с синергией и навсегда получать его баффы.

## How it works

1. Have a pawn with **Persona Devourer** trait or gene.
2. Right click on a persona weapon on the ground and choose **Devour** from the float menu.
3. The pawn spends two in-game seconds devouring it, and destroys the weapon.
4. Each supported beneficial persona ability becomes a permanent buff with its own saved hediff named `Devoured: <persona trait>`.

Devouring is irreversible. Duplicate abilities are skipped, negative or neutral abilities are ignored, and unsupported positive abilities prevent the entire weapon from being consumed. Validation is atomic: a failed devour leaves the weapon and pawn unchanged.

## Mod Support

### Mod Requirements

- Royalty DLC

### Mod optional compatibility

- Biotech DLC - will add a gene to give the trait
- [More Persona Traits](https://steamcommunity.com/sharedfiles/filedetails/?id=2863308112)

These mods are not required. Their persona weapon traits are detected when active.

### Mods tested with

- [Vanilla Persona Weapons Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2826922787)
- [Medieval Persona Weapons](https://steamcommunity.com/sharedfiles/filedetails/?id=2869057049)

## Language support

Machine translated. Send corrections.

- English
- French
- Korean
- Simplified Chinese
- Japanese
- Russian

## Performance

- Patches Pawn Kill and Inventory Right-Click

## Modifying and Testing

### Trait rules and overrides

Edit `Defs/PersonaDevouringTraitRules.xml` using `WeaponTraitDef.defName` values. Leave a comment if there's a bug or if there are other mods I should support.

### Settings and diagnostics

Open the mod settings menu and select **Dump loaded WeaponTraitDefs**. This writes `docs/WeaponTraitDefs.xml`, containing every loaded persona trait with:

- Source mod
- Devourable, ignored, suppressed, or unsupported status
- Unsupported classification reason
- Traits it overrides
- Traits that override it

### Debug actions

With developer mode enabled:

- **Actions → Apply all devourable persona traits** applies every currently supported and rule-allowed trait to the selected pawn in random order.
- **Actions → Generate supported persona weapon test set** creates test persona weapons covering active weapon types and supported traits.

The first action does not require the pawn to have the Persona Devourer trait, making it useful for testing hediff effects directly.

## AI

[AI Disclosure](docs/AI.md)

## GitHub


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details.
