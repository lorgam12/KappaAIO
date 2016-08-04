using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappaAIO.Core.CommonStuff
{
    public static class DamageLib
    {
        public static readonly List<Data> Database = new List<Data>();

        public struct Data
        {
            public SpellSlot slot;

            public DamageType DamageType;

            public float[] Floats;

            public float AD;

            public float AP;

            public float[] MaxHP;
        }

        private static int SmiteDamage(Obj_AI_Base target)
        {
            var lvl = Player.Instance.Level;
            if (target is AIHeroClient)
            {
                return 20 + (8 * lvl);
            }

            if (lvl <= 4)
            {
                return 370 + (20 * lvl);
            }

            if (lvl <= 9)
            {
                return 450 + (30 * (lvl - 4));
            }

            if (lvl <= 14)
            {
                return 600 + (40 * (lvl - 9));
            }

            return 600 + (50 * (lvl - 14));
        }

        public static void DamageDatabase()
        {
            Database.Clear();
            switch (Player.Instance.Hero)
            {
                // AurelionSol
                case Champion.AurelionSol:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 70, 110, 150, 190, 230 },
                            AP = 0.75f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(new Data {
                        slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Floats = new float[] { 15, 30, 45, 60, 75 },
                        AP = 1f,
                        AD = 0,
                        MaxHP = new float[] { 0, 0, 0, 0, 0 }
                    });
                    Database.Add(new Data {
                        slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Floats = new float[] { 150, 250, 350 },
                        AP = 0.7f,
                        AD = 0,
                        MaxHP = new float[] { 0, 0, 0 }
                    });
                    break;

                // Azir
                case Champion.Azir:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 65, 85, 105, 125, 145 },
                            AP = 0.5f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 60, 90, 120, 150, 180 },
                            AP = 0.4f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 150, 225, 300 },
                            AP = 0.6f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0 }
                        });
                    break;

                // Brand
                case Champion.Brand:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 80, 110, 140, 170, 200 },
                            AP = 0.55f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.W,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 75, 120, 165, 210, 255 },
                            AP = 0.6f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 80, 110, 140, 170, 200 },
                            AP = 0.35f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 150, 300, 500 },
                            AP = 0.5f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0 }
                        });
                    break;

                // Kindred
                case Champion.Kindred:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 55, 75, 95, 115, 135 },
                            AP = 0,
                            AD = 0.2f,
                            MaxHP = new[] { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.W,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 25, 30, 35, 40, 45 },
                            AP = 0,
                            AD = 0.4f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 40, 75, 110, 145, 180 },
                            AP = 0,
                            AD = 0.2f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    break;

                // Malzahar
                case Champion.Malzahar:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 70, 110, 150, 190, 230 },
                            AP = 0.7f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.W,
                            DamageType = DamageType.Mixed,
                            Floats = new float[] { 30 + 10, 33 + 15, 35 + 20, 37 + 25, 40 + 35 },
                            AP = 0.1f,
                            AD = 0.4f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 80, 115, 150, 185, 220 },
                            AP = 0.65f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 0, 0, 0, 0, 0 },
                            AP = 0.07f,
                            AD = 0,
                            MaxHP = new[] { 0.25f, 0.35f, 0.45f }
                        });
                    break;

                // LeeSin
                case Champion.LeeSin:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 50, 80, 110, 140, 170 },
                            AP = 0,
                            AD = 0.9f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 60, 95, 130, 165, 200 },
                            AP = 0,
                            AD = 1f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 175, 375, 575 },
                            AP = 0,
                            AD = 2f,
                            MaxHP = new float[] { 0, 0, 0 }
                        });
                    break;

                // Xerath
                case Champion.Xerath:
                    Database.Add(
                        new Data {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 80, 120, 160, 200, 240 },
                            AP = 0.75f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.W,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 60, 90, 120, 150, 180 },
                            AP = 0.6f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 80, 110, 140, 170, 200 },
                            AP = 0.45f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 200, 230, 260 },
                            AP = 0.43f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0 }
                        });
                    break;

                // Xerath
                case Champion.Yasuo:
                    Database.Add(
                        new Data
                        {
                            slot = SpellSlot.Q,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 20, 40, 60, 80, 100 },
                            AP = 0,
                            AD = 1f,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data
                        {
                            slot = SpellSlot.E,
                            DamageType = DamageType.Magical,
                            Floats = new float[] { 70, 90, 110, 130, 150 },
                            AP = 0.6f,
                            AD = 0,
                            MaxHP = new float[] { 0, 0, 0, 0, 0 }
                        });
                    Database.Add(
                        new Data
                        {
                            slot = SpellSlot.R,
                            DamageType = DamageType.Physical,
                            Floats = new float[] { 200, 300, 400 },
                            AP = 0,
                            AD = 1.5f,
                            MaxHP = new float[] { 0, 0, 0 }
                        });
                    break;
            }
        }

        public static float GetDamage(this Spell.SpellBase Spell, Obj_AI_Base target)
        {
            if (Spell.Name.ToLower().Contains("smite"))
            {
                return Player.Instance.CalculateDamageOnUnit(target, DamageType.True, SmiteDamage(target));
            }

            if (!Database.Exists(s => s.slot == Spell.Slot && Player.GetSpell(s.slot).IsReady))
            {
                return Player.Instance.GetSpellDamage(target, Spell.Slot);
            }

            var spell = Database.FirstOrDefault(s => s.slot == Spell.Slot);
            var dmg = 0f;
            var AP = Player.Instance.FlatMagicDamageMod;
            var AD = Player.Instance.FlatPhysicalDamageMod;
            var sLevel = Spell.Level - 1;
            var ready = Spell.IsLearned && Spell.IsReady();

            if (ready)
            {
                dmg = spell.Floats[sLevel] + (target.MaxHealth * spell.MaxHP[sLevel]) + (spell.AD * AD) + (spell.AP * AP);
            }

            if (Player.Instance.Hero == Champion.LeeSin && Spell.Slot == SpellSlot.Q && target.Buffs.Any(b => b.Name.ToLower().Contains("qone")))
            {
                var missinghealth = target.MaxHealth - target.Health;
                dmg += 0.08f * missinghealth;
            }

            return ready ? Player.Instance.CalculateDamageOnUnit(target, spell.DamageType, dmg - 15) : 0;
        }

        public static float TotalDamage(this Obj_AI_Base target, List<Spell.SpellBase> list)
        {
            return list.Where(spell => spell != null).Sum(spell => spell.GetDamage(target));
        }
    }
}
