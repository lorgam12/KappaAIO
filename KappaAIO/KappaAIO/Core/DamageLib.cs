namespace KappaAIO.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    public static class DamageLib
    {
        public static readonly List<Data> Database = new List<Data>();

        public struct Data
        {
            public SpellSlot slot;

            public DamageType DamageType;

            public float[] Floats;

            public float Float;
        }

        public static void DamageDatabase()
        {
            var AD = Player.Instance.TotalAttackDamage;
            var AP = Player.Instance.TotalMagicalDamage;
            var level = Player.Instance.Level;
            Database.Clear();
            switch (Player.Instance.Hero)
            {
                // AurelionSol
                case Champion.AurelionSol:
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.Q, DamageType = DamageType.Magical, Floats = new float[] { 70, 110, 150, 190, 230 }, Float = 0.75f 
                            });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.W, DamageType = DamageType.Magical, Floats = new float[] { 15, 30, 45, 60, 75 },
                                Float = 1f
                            });
                    Database.Add(
                        new Data { slot = SpellSlot.R, DamageType = DamageType.Magical, Floats = new float[] { 150, 250, 350 }, Float = 0.7f });
                    break;

                // Azir
                case Champion.Azir:
                    Database.Add(
                        new Data { slot = SpellSlot.Q, DamageType = DamageType.Magical, Floats = new float[] { 65, 85, 105, 125, 145 }, Float = 0.5f });
                    Database.Add(new Data { slot = SpellSlot.W, DamageType = DamageType.Magical, Float = 50 + (10 * Player.Instance.Level) + 0.4f });
                    Database.Add(
                        new Data { slot = SpellSlot.E, DamageType = DamageType.Magical, Floats = new float[] { 60, 90, 120, 150, 180 }, Float = 0.4f });
                    Database.Add(
                        new Data { slot = SpellSlot.R, DamageType = DamageType.Magical, Floats = new float[] { 150, 225, 300 }, Float = 0.6f });
                    break;

                // Brand
                case Champion.Brand:
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.Q, DamageType = DamageType.Magical, Floats = new float[] { 80, 110, 140, 170, 200 }, Float = 0.55f 
                            });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.W, DamageType = DamageType.Magical, Floats = new float[] { 75, 120, 165, 210, 255 }, Float = 0.6f 
                            });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.E, DamageType = DamageType.Magical, Floats = new float[] { 80, 110, 140, 170, 200 }, Float = 0.35f 
                            });
                    Database.Add(
                        new Data { slot = SpellSlot.R, DamageType = DamageType.Magical, Floats = new float[] { 150, 300, 500 }, Float = 0.5f });
                    break;

                // Kindred
                case Champion.Kindred:
                    Database.Add(
                        new Data { slot = SpellSlot.Q, DamageType = DamageType.Physical, Floats = new float[] { 55, 75, 95, 115, 135 }, Float = 0.2f });
                    Database.Add(
                        new Data { slot = SpellSlot.W, DamageType = DamageType.Physical, Floats = new float[] { 25, 30, 35, 40, 45 }, Float = 0.4f });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.E, DamageType = DamageType.Physical, Floats = new float[] { 40, 75, 110, 145, 180 }, Float = 0.2f 
                            });
                    break;

                // Malzahar
                case Champion.Malzahar:
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.Q, DamageType = DamageType.Magical, Floats = new float[] { 70, 110, 150, 190, 230 }, Float = 0.7f 
                            });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.W, DamageType = DamageType.Mixed, Floats = new float[] { 30 + 10, 33 + 15, 35 + 20, 37 + 25, 40 + 35 },
                                Float = (0.4f * AD) + (0.1f * AP)
                            });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.E, DamageType = DamageType.Magical, Floats = new float[] { 80, 115, 150, 185, 220 }, Float = 0.65f 
                            });
                    break;

                // Xerath
                case Champion.Xerath:
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.Q, DamageType = DamageType.Magical, Floats = new float[] { 80, 120, 160, 200, 240 }, Float = 0.75f 
                            });
                    Database.Add(
                        new Data { slot = SpellSlot.W, DamageType = DamageType.Magical, Floats = new float[] { 60, 90, 120, 150, 180 }, Float = 0.6f });
                    Database.Add(
                        new Data
                            {
                                slot = SpellSlot.E, DamageType = DamageType.Magical, Floats = new float[] { 80, 110, 140, 170, 200 }, Float = 0.45f 
                            });
                    Database.Add(
                        new Data { slot = SpellSlot.R, DamageType = DamageType.Magical, Floats = new float[] { 200, 230, 260 }, Float = 0.43f });
                    break;
            }
        }

        public static float GetDamage(this Spell.SpellBase Spell, Obj_AI_Base target)
        {
            var spell = Database.FirstOrDefault(s => s.slot == Spell.Slot);
            var dmg = 0f;
            var AP = Player.Instance.TotalMagicalDamage;
            var AD = Player.Instance.TotalAttackDamage;
            var sLevel = Spell.Level - 1;
            var ready = Spell.IsLearned && Spell.IsReady();
            var dmg2 = 0f;

            if (ready)
            {
                if (Player.Instance.Hero == Champion.Malzahar && Spell.Slot == SpellSlot.R)
                {
                    return new[] { target.MaxHealth * 0.25f, target.MaxHealth * 0.35f, target.MaxHealth * 0.45f }[sLevel] + (0.07f * (AP / 100));
                }

                if (Player.Instance.Hero == Champion.Kindred && Spell.Slot == SpellSlot.E && ready)
                {
                    dmg2 += 0.05f * target.MaxHealth;
                }

                if (spell.DamageType == DamageType.Magical)
                {
                    dmg2 += spell.Float * AP;
                }

                if (spell.DamageType == DamageType.Physical)
                {
                    dmg2 += spell.Float * AD;
                }

                dmg += spell.Floats[sLevel] + dmg2;
            }
            return ready ? Player.Instance.CalculateDamageOnUnit(target, spell.DamageType, dmg - 15) : 0;
        }

        public static float TotalDamage(this Obj_AI_Base target, List<Spell.SpellBase> list)
        {
            return list.Where(spell => spell != null).Sum(spell => spell.GetDamage(target));
        }
    }
}