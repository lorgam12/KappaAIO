﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Constants;
using SharpDX;

namespace KappaAIO.KappaEvade
{
    static class SpellsDetector
    {
        public delegate void TargetedSpellDetected(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args, Database.TargetedSpells.TSpell spell);

        public static event TargetedSpellDetected OnTargetedSpellDetected;

        public delegate void SkillShotDetected(
            Obj_AI_Base sender,
            GameObjectProcessSpellCastEventArgs args,
            Database.SkillShotSpells.SSpell spell,
            Vector3 Start,
            Vector3 End,
            float Range,
            float Width,
            MissileClient missile);

        public static event SkillShotDetected OnSkillShotDetected;

        public static void Init()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;

            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null || missile == null || !missile.IsValid || missile.IsAutoAttack() || caster.IsEnemy)
                return;

            Chat.Print("OnCreate Detected " + missile.SData.Name + " " + missile.SData.MissileSpeed + " " + missile.SData.CastRange);
            if (Database.SkillShotSpells.SkillShotsSpellsList.Any(s => s.hero == caster.Hero && missile.SData.Name.ToLower() == s.MissileName.ToLower()))
            {
                Chat.Print("OnCreate Added " + missile.SData.Name);
                var spell = Database.SkillShotSpells.SkillShotsSpellsList.FirstOrDefault(s => s.hero == caster.Hero && missile.SData.Name.ToLower() == s.MissileName.ToLower());
                OnSkillShotDetected?.Invoke(caster, null, spell, missile.StartPosition, missile.EndPosition, spell.Range, spell.Width, missile);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var hero = args.Target as AIHeroClient;
            if (caster == null || caster.IsEnemy)
                return;

            if (Database.SkillShotSpells.SkillShotsSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot && args.SData.Name.ToLower() == s.SpellName.ToLower()))
            {
                Chat.Print("OnProcessSpellCast Detected " + args.SData.Name);
                var spell = Database.SkillShotSpells.SkillShotsSpellsList.FirstOrDefault(s => s.hero == caster.Hero && s.slot == args.Slot && args.SData.Name.ToLower() == s.SpellName.ToLower());
                OnSkillShotDetected?.Invoke(sender, args, spell, args.Start, args.End, spell.Range, spell.Width, null);
            }

            if (hero == null)
                return;
            if (Database.TargetedSpells.TargetedSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot))
            {
                var spell = Database.TargetedSpells.TargetedSpellsList.FirstOrDefault(s => s.hero == caster.Hero && s.slot == args.Slot);
                OnTargetedSpellDetected?.Invoke(sender, hero, args, spell);
            }
        }
    }
}