namespace KappaAIO.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Notifications;

    using Managers;

    using SharpDX;

    public static class Common
    {
        public static int lastNotification = 0;

        public static AIHeroClient ally;

        public static Obj_AI_Turret tower;

        public static DangerLevel danger(Menu m)
        {
            switch (m.combobox("Danger"))
            {
                case 0:
                    {
                        return DangerLevel.High;
                    }
                case 1:
                    {
                        return DangerLevel.Medium;
                    }
                case 2:
                    {
                        return DangerLevel.Low;
                    }
            }
            return DangerLevel.Low;
        }

        public static int CountEnemeis(this Obj_AI_Base target, float range)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.ServerPosition) && e.IsKillable());
        }

        public static int CountEnemeis(this GameObject target, float range)
        {
            return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.Position) && e.IsKillable());
        }

        public static int CountAllies(this Obj_AI_Base target, float range)
        {
            return EntityManager.Heroes.Allies.Count(e => e.IsValidTarget(range, false, target.ServerPosition) && e.IsKillable());
        }

        public static int CountEnemyMinions(this Obj_AI_Base target, float range)
        {
            return EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => m.IsKillable() && m.IsInRange(target, range));
        }

        public static int CountAllyMinions(this Obj_AI_Base target, float range)
        {
            return EntityManager.MinionsAndMonsters.AlliedMinions.Count(m => m.IsKillable() && m.IsInRange(target, range));
        }

        public static int combobox(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        public static int slider(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        public static bool checkbox(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        public static bool keybind(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        public static System.Drawing.Color Color(this Menu m, string id)
        {
            return m[id].Cast<ColorPicker>().CurrentValue;
        }

        public static bool Mana(this Spell.SpellBase spell, Menu m)
        {
            return ObjectManager.Player.ManaPercent > m.slider(spell.Slot + "mana");
        }

        public static float Mana()
        {
            var mana = 0f;
            var q = Player.GetSpell(SpellSlot.Q);
            var w = Player.GetSpell(SpellSlot.W);
            var e = Player.GetSpell(SpellSlot.E);
            var r = Player.GetSpell(SpellSlot.R);
            if (q.IsReady)
            {
                // Q mana
                mana += q.SData.Mana;
            }

            if (w.IsReady)
            {
                // W mana
                mana += w.SData.Mana;
            }

            if (e.IsReady)
            {
                // E mana
                mana += e.SData.Mana;
            }

            if (r.IsReady)
            {
                // R mana
                mana += r.SData.Mana;
            }

            return mana;
        }

        public static float Mana(this Spell.SpellBase spell)
        {
            return spell.Handle.SData.Mana;
        }

        public static string ID(this Obj_AI_Base target)
        {
            return target.BaseSkinName + target.NetworkId;
        }

        public static bool IsKillable(this Obj_AI_Base target)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
        }

        public static bool IsKillable(this Obj_AI_Base target, float range)
        {
            return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("ChronoShift")
                   && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil")
                   && !target.IsDead && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability)
                   && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget(range);
        }

        public static bool IsCC(this Obj_AI_Base target)
        {
            return target.Spellbook.IsChanneling || !target.CanMove || target.HasBuffOfType(BuffType.Charm)
                   || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Snare)
                   || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
        }

        public static Vector2 PredPos(this Obj_AI_Base target, int time)
        {
            return Prediction.Position.PredictUnitPosition(target, time);
        }

        public static float PredHP(this Obj_AI_Base target, int time)
        {
            return Prediction.Health.GetPrediction(target, time);
        }

        public static bool brandpassive(this Obj_AI_Base target)
        {
            return target.HasBuff("BrandAblaze");
        }

        public static int countpassive(this Obj_AI_Base target)
        {
            return target.GetBuffCount("BrandAblaze");
        }

        public static bool orbmode(Orbwalker.ActiveModes mode)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(mode);
        }

        public static void ShowNotification(string message, int duration = -1, string message2 = "New Notification")
        {
            Notifications.Show(new SimpleNotification(message, message2), duration);
        }

        public static void Log(string message, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0)
        {
            Console.WriteLine("{0} ({1}): {2}", Path.GetFileName(file), line, message);
        }

        public static class LastCastedSpell
        {
            public static float Time;

            public static SpellSlot Spell;

            public static string Name;
        }
    }
}