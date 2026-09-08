using UnityEngine;
using Verse;

namespace OMWPersonaDevouringPawn
{
    public class OMWPersonaDevouringPawnMod : Mod
    {
        private readonly PersonaDevouringSettings settings;
        private readonly ModContentPack content;
        public static PersonaDevouringSettings Settings { get; private set; }

        public OMWPersonaDevouringPawnMod(ModContentPack content) : base(content)
        {
            this.content = content;
            settings = GetSettings<PersonaDevouringSettings>();
            Settings = settings;
            Log.Message($"[{content.Name}] loaded.");
        }

        public override string SettingsCategory()
        {
            return "OMW_SettingsCategory".Translate().ToString();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect, content);
        }
    }
}
