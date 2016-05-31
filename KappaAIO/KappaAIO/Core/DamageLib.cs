namespace KappaAIO.Core
{
    using EloBuddy;
    using EloBuddy.SDK;

    public static class DamageLib
    {
        public static float GetDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            var AD = Player.Instance.TotalAttackDamage;
            var AP = Player.Instance.TotalMagicalDamage;
            var damageType = DamageType.Mixed;
            var spell = Player.GetSpell(slot);
            var sLevel = spell.Level - 1;
            var dmg = 0f;
            var ready = spell.IsLearned && spell.IsReady;

            switch (Player.Instance.Hero)
            {
                // AurelioSol
                case Champion.AurelionSol:
                    {
                        damageType = DamageType.Magical;
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 70, 110, 150, 190, 230 }[sLevel] + 0.75f * AP;
                                    }
                                }
                                break;
                            case SpellSlot.W:
                                {
                                    if (ready)
                                    {
                                        dmg += (8 * Player.Instance.Level) + new float[] { 15, 30, 45, 60, 75 }[sLevel]
                                               + (0.255f + (0.015f * Player.Instance.Level)) * AP;
                                    }
                                }
                                break;
                            case SpellSlot.R:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 150, 250, 350 }[sLevel] + 0.7f * AP;
                                    }
                                }
                                break;
                        }
                        break;
                    }

                // Azir
                case Champion.Azir:
                    {
                        damageType = DamageType.Magical;
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                if (ready)
                                {
                                    dmg += new float[] { 65, 85, 105, 125, 145 }[sLevel] + 0.5f * AP;
                                }

                                break;
                            case SpellSlot.W:
                                if (ready)
                                {
                                    dmg += 50 + (10 * Player.Instance.Level) + 0.4f * AP;
                                }

                                break;
                            case SpellSlot.E:
                                if (ready)
                                {
                                    dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.4f * AP;
                                }

                                break;
                            case SpellSlot.R:
                                if (ready)
                                {
                                    dmg += new float[] { 150, 225, 300 }[sLevel] + 0.6f * AP;
                                }

                                break;
                        }

                        break;
                    }

                // Brand
                case Champion.Brand:
                    {
                        damageType = DamageType.Magical;
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                if (ready)
                                {
                                    dmg += new float[] { 80, 110, 140, 170, 200 }[sLevel] + 0.55f * AP;
                                }

                                break;
                            case SpellSlot.W:
                                if (ready)
                                {
                                    if (target.brandpassive())
                                    {
                                        dmg += new float[] { 93, 150, 205, 260, 320 }[sLevel] + 0.75f * AP;
                                    }
                                    else
                                    {
                                        dmg += new float[] { 75, 120, 165, 210, 255 }[sLevel] + 0.6f * AP;
                                    }
                                }

                                break;
                            case SpellSlot.E:
                                if (ready)
                                {
                                    dmg += new float[] { 80, 110, 140, 170, 200 }[sLevel] + 0.35f * AP;
                                }

                                break;
                            case SpellSlot.R:
                                if (ready)
                                {
                                    dmg += new float[] { 150, 300, 500 }[sLevel] + 0.5f * AP;
                                }

                                break;
                        }

                        break;
                    }
                // Kindred
                case Champion.Kindred:
                    {
                        damageType = DamageType.Physical;
                        dmg += Player.Instance.GetAutoAttackDamage(target, true);
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.2f * AD;
                                    }
                                }

                                break;
                            case SpellSlot.W:
                                {
                                    if (ready)
                                    {
                                        dmg += (new float[] { 25, 30, 35, 40, 45 }[sLevel] + 0.4f * AD);
                                    }
                                }

                                break;
                            case SpellSlot.E:
                                {
                                    if (ready)
                                    {
                                        dmg += (new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.2f * AD) + target.MaxHealth * 0.05f;
                                    }
                                }
                                break;
                        }
                    }
                    break;

                // Malzahar
                case Champion.Malzahar:
                    {
                        damageType = DamageType.Magical;
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 70, 110, 150, 190, 230 }[sLevel] + 0.70f * AP;
                                    }
                                }

                                break;
                            case SpellSlot.W:
                                {
                                    if (ready)
                                    {
                                        dmg += (new float[] { 30, 33, 35, 37, 40 }[sLevel] + 0.40f * AD)
                                               + (new float[] { 10, 15, 20, 25, 30 }[sLevel] + 0.10f * AP);
                                    }
                                }

                                break;
                            case SpellSlot.E:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 80, 115, 150, 185, 220 }[sLevel] + 0.65f * AP;
                                    }
                                }

                                break;
                            case SpellSlot.R:
                                {
                                    if (ready)
                                    {
                                        dmg += new[] { target.MaxHealth * 0.25f, target.MaxHealth * 0.35f, target.MaxHealth * 0.45f }[sLevel]
                                               + (0.07f * (AP / 100));
                                    }
                                }

                                break;
                        }

                        break;
                    }

                // Xerath
                case Champion.Xerath:
                    {
                        damageType = DamageType.Magical;
                        switch (slot)
                        {
                            case SpellSlot.Q:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 80, 120, 160, 200, 240 }[sLevel] + 0.75f * AP;
                                    }
                                }
                                break;
                            case SpellSlot.W:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 60, 90, 120, 150, 180 }[sLevel] + 0.6f * AP;
                                    }
                                }
                                break;
                            case SpellSlot.E:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 80, 110, 140, 170, 200 }[sLevel] + 0.45f * AP;
                                    }
                                }
                                break;
                            case SpellSlot.R:
                                {
                                    if (ready)
                                    {
                                        dmg += new float[] { 200, 230, 260 }[sLevel] + 0.43f * AP;
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }

            return target != null ? Player.Instance.CalculateDamageOnUnit(target, damageType, dmg - 15) : dmg;
        }

        public static float TotalDamage(this Obj_AI_Base target, DamageType damageType)
        {
            var damage = SpellSlot.Q.GetDamage(target) + SpellSlot.W.GetDamage(target) + SpellSlot.E.GetDamage(target) + SpellSlot.R.GetDamage(target);
            return target != null ? Player.Instance.CalculateDamageOnUnit(target, damageType, damage) : 0;
        }
    }
}