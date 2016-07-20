using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using KappaAIO.Core.CommonStuff;

namespace KappaAIO.Core.Managers
{
    internal static class HitChanceManager
    {
        public static HitChance hitchance(this Spell.SpellBase spell, Menu m)
        {
            switch (m.combobox(spell.Slot + "hit"))
            {
                case 0:
                    {
                        return HitChance.High;
                    }

                case 1:
                    {
                        return HitChance.Medium;
                    }

                case 2:
                    {
                        return HitChance.Low;
                    }
            }

            return HitChance.Unknown;
        }

        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, HitChance hitChance)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChance >= hitChance || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static void Cast(this Spell.SpellBase spell, Obj_AI_Base target, HitChance hitChance)
        {
            var thisspell = spell as Spell.Skillshot;
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = thisspell.GetPrediction(target);
                if (pred.HitChance >= hitChance || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static void Cast(this Spell.Chargeable spell, Obj_AI_Base target, HitChance hitChance)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.MaximumRange))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChance >= hitChance || target.IsCC())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, HitChance hitChance, bool value = true)
        {
            if (target != null && spell.IsReady() && target.IsKillable(spell.Range))
            {
                var pred = spell.GetPrediction(target);
                if (pred.HitChance >= hitChance || value)
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        public static void Cast(this Spell.Skillshot spell, Obj_AI_Base target, bool value = true)
        {
            if (target != null && value && spell.IsReady() && target.IsKillable(spell.Range))
            {
                spell.Cast(spell.GetPrediction(target).CastPosition);
            }
        }

        public static void Cast(this Spell.Chargeable spell, Obj_AI_Base target, bool value = true)
        {
            if (target != null && value && spell.IsReady() && target.IsKillable(spell.MaximumRange))
            {
                if (value)
                {
                    spell.Cast(spell.GetPrediction(target).CastPosition);
                }
            }
        }
    }
}
