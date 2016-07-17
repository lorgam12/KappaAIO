using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using SharpDX;
using static KappaAIO.Core.KappaEvade.Database;

namespace KappaAIO.Core.KappaEvade
{
    static class SpellsDetector
    {
        public delegate void TargetedSpellDetected(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args, TargetedSpells.TSpell spell);

        public static event TargetedSpellDetected OnTargetedSpellDetected;

        public static void Init()
        {
            Common.Logger.Info("KappaEvade Loaded");
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var hero = args.Target as AIHeroClient;
            if (caster == null || !caster.IsEnemy || hero == null) return;

            if (TargetedSpells.TargetedSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot))
            {
                var spell = TargetedSpells.TargetedSpellsList.FirstOrDefault(s => s.hero == caster.Hero && s.slot == args.Slot);
                OnTargetedSpellDetected?.Invoke(sender, hero, args, spell);
            }
        }
    }
}
