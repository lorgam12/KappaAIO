namespace KappaAIO.Champions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    using KappaAIO.Core;
    using KappaAIO.Core.Managers;

    using Color = System.Drawing.Color;

    internal class AurelionSol
    {
        public static float Qsize;

        private static MissileClient QMissle;

        public static Spell.Skillshot Q { get; private set; }

        public static Spell.Active W { get; private set; }

        public static Spell.Active W2 { get; private set; }

        public static Spell.Skillshot R { get; private set; }

        private static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        private static Menu Menuini, AutoMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillstealMenu, DrawMenu, ColorMenu;

        public static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 600, SkillShotType.Linear, 0, 1000, 180);
            W = new Spell.Active(SpellSlot.W, 600);
            W2 = new Spell.Active(SpellSlot.W, 350);
            R = new Spell.Skillshot(SpellSlot.R, 1475, SkillShotType.Linear, 250, 1750, 180);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("AurelionSol", "AurelionSol");
            AutoMenu = Menuini.AddSubMenu("Auto Settings");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            ComboMenu.AddGroupLabel("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass Settings");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            KillstealMenu = Menuini.AddSubMenu("KillSteal Settings");
            KillstealMenu.AddGroupLabel("Stealer Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawings");
            ColorMenu = Menuini.AddSubMenu("ColorPicker");
            ColorMenu.AddGroupLabel("Color Picker");

            Menuini.AddGroupLabel("Global Settings");
            Menuini.Add("qrange", new Slider("Q Range [{0}]", 800, 350, 1500));
            Menuini.Add("wmax", new Slider("Max W Range [{0}]", 750, 600, 1500));
            Menuini.Add("wmin", new Slider("Min W Range [{0}]", 400, 200, 599));
            foreach (var spell in SpellList.Where(s => s != W))
            {
                Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
            }

            AutoMenu.AddGroupLabel("Automated Settings");
            AutoMenu.Add("GapQ", new CheckBox("Anti-Gapcloser Q"));
            AutoMenu.Add("GapR", new CheckBox("Anti-Gapcloser R"));
            AutoMenu.Add("IntQ", new CheckBox("Interrupter Q"));
            AutoMenu.Add("IntR", new CheckBox("Interrupter R"));
            AutoMenu.Add("Danger", new ComboBox("Interrupter DangerLevel", 1, "High", "Medium", "Low"));
            AutoMenu.AddGroupLabel("Anti GapCloser Spells");
            foreach (var spell in
                from spell in Gapcloser.GapCloserList
                from enemy in EntityManager.Heroes.Enemies.Where(enemy => spell.ChampName == enemy.ChampionName)
                select spell)
            {
                AutoMenu.Add(spell.SpellName, new CheckBox(spell.ChampName + " " + spell.SpellSlot));
            }

            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");
            DrawMenu.AddSeparator(1);
            foreach (var spell in SpellList)
            {
                ComboMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                if (spell != R)
                {
                    HarassMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    HarassMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    if (spell != W)
                    {
                        LaneClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                        LaneClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                        JungleClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                        JungleClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    }
                }
                if (spell == W)
                {
                    DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Max Range"));
                    ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", Color.Chartreuse));
                }
                if (spell != W)
                {
                    DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                    ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", Color.Chartreuse));
                    KillstealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                    KillstealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
                }
            }

            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Extra Settings");
            ComboMenu.Add("disableAA", new CheckBox("Disable AA When W Active", false));
            ComboMenu.Add("qmode", new ComboBox("Cast Q2 mode", 0, "Anyone", "Only Target"));
            ComboMenu.Add("Rfinisher", new CheckBox("R Finisher"));
            ComboMenu.Add("Raoe", new Slider("R AoE hit", 2, 1, 6));

            HarassMenu.AddSeparator(0);
            HarassMenu.AddGroupLabel("Extra Settings");
            HarassMenu.Add("qmode", new ComboBox("Q2 mode", 0, "Anyone", "Only Target"));

            DrawMenu.Add("w2", new CheckBox("W Min Range"));
            ColorMenu.Add("w2", new ColorPicker("W2 Color", Color.Chartreuse));

            Game.OnTick += Game_OnTick;
            Gapcloser.OnGapcloser += Auto.Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Auto.Interrupter_OnInterruptableSpell;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid)
            {
                return;
            }

            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe
                && miss.SData.Name.Contains("AurelionSolQMissile"))
            {
                QMissle = null;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null)
                {
                    if (Q.IsReady() && AutoMenu.checkbox("GapQ") && rengar.IsValidTarget(Q.Range))
                    {
                        Q.Cast(rengar);
                        return;
                    }
                    if (R.IsReady() && AutoMenu.checkbox("GapR") && rengar.IsValidTarget(500))
                    {
                        R.Cast(rengar);
                    }
                }
            }

            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && miss.SData.Name.Contains("AurelionSolQMissile"))
                {
                    QMissle = miss;
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            Auto.KillSteal();
            updatespells();

            Orbwalker.DisableAttacking = W.Handle.ToggleState == 2 && ComboMenu.checkbox("disableAA") && Common.orbmode(Orbwalker.ActiveModes.Combo);
            if (Common.orbmode(Orbwalker.ActiveModes.Combo))
            {
                Compat.Combo();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.Harass))
            {
                Compat.Harass();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                Clear.LaneClear();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.JungleClear))
            {
                Clear.JungleClear();
            }
        }

        private static void updatespells()
        {
            var f = (QMissle?.StartPosition.Distance(QMissle.Position) + Q.Width) / 16;
            if (f != null)
            {
                Qsize = (float)f;
            }

            Q.Range = (uint)Menuini.slider("qrange");
            W.Range = (uint)Menuini.slider("wmax");
            W2.Range = (uint)Menuini.slider("wmin");
        }

        private static class Auto
        {
            public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
            {
                if (sender == null || !sender.IsEnemy || !sender.IsKillable() || Common.danger(AutoMenu) > e.DangerLevel)
                {
                    return;
                }
                if (AutoMenu.checkbox("IntQ") && Q.IsReady() && sender.IsValidTarget(Q.Range))
                {
                    Q.Cast(sender);
                    return;
                }
                if (AutoMenu.checkbox("IntR") && R.IsReady() && sender.IsValidTarget(500))
                {
                    R.Cast(sender);
                }
            }

            public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
            {
                if (sender == null || !sender.IsEnemy || !sender.IsKillable() || !AutoMenu.checkbox(e.SpellName))
                {
                    return;
                }
                if (AutoMenu.checkbox("GapQ") && Q.IsReady() && sender.IsValidTarget(Q.Range))
                {
                    Q.Cast(sender);
                    return;
                }
                if (AutoMenu.checkbox("GapR") && R.IsReady() && sender.IsValidTarget(500))
                {
                    R.Cast(sender);
                }
            }

            public static void KillSteal()
            {
                foreach (var spell in SpellList.Where(s => s != W))
                {
                    if (spell.GetKStarget() != null && spell.IsReady() && KillstealMenu.checkbox(spell.Slot + "ks"))
                    {
                        spell.Cast(spell.GetKStarget());
                    }
                    if (spell.GetJStarget() != null && spell.IsReady() && KillstealMenu.checkbox(spell.Slot + "js"))
                    {
                        spell.Cast(spell.GetJStarget());
                    }
                }
            }
        }

        private static class Compat
        {
            public static void Combo()
            {
                var Qmode = ComboMenu.combobox("qmode");

                if (QMissle != null && Qmode == 0 && EntityManager.Heroes.Enemies.Any(e => e.IsInRange(QMissle, Qsize) && e.IsKillable()))
                {
                    Q.Cast(Game.CursorPos);
                }

                var target = TargetSelector.GetTarget(1000, DamageType.Magical);
                var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                var Rtarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                var useQ = ComboMenu.checkbox(Q.Slot.ToString()) && target.IsValidTarget(Q.Range) && Q.IsReady();
                var useW = ComboMenu.checkbox(W.Slot.ToString()) && W.IsReady();
                var useR = ComboMenu.checkbox(R.Slot.ToString()) && R.IsReady();
                var Rfinisher = ComboMenu.checkbox("Rfinisher");

                if (useQ && target != null)
                {
                    if (Q.Handle.ToggleState != 2 && !target.IsValidTarget(250))
                    {
                        Q.Cast(target, Q.hitchance(Menuini));
                    }
                }
                if (QMissle != null && Q.Handle.ToggleState == 2 && Qmode == 1 && target.IsInRange(QMissle, Qsize))
                {
                    Q.Cast(Game.CursorPos);
                }

                if (useW)
                {
                    if (Wtarget != null)
                    {
                        if (W.Handle.ToggleState != 2 && Wtarget.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                        if (W.Handle.ToggleState == 2 && Wtarget.IsValidTarget(W2.Range))
                        {
                            W.Cast();
                        }
                    }

                    if (Wtarget == null && W.Handle.ToggleState == 2)
                    {
                        W.Cast();
                    }
                }
                if (useR)
                {
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(R.Range));
                    if (enemies != null)
                    {
                        var aiHeroClients = enemies as AIHeroClient[] ?? enemies.ToArray();
                        foreach (var enemy in aiHeroClients)
                        {
                            var Rectangle = new Geometry.Polygon.Rectangle(
                                Player.Instance.ServerPosition,
                                Player.Instance.ServerPosition.Extend(enemy.ServerPosition, R.Range).To3D(),
                                R.Width);

                            if (aiHeroClients.Count(e => Rectangle.IsInside(e)) >= ComboMenu.slider("Raoe"))
                            {
                                R.Cast(enemy.ServerPosition);
                            }
                        }
                    }

                    if (Rfinisher && Rtarget != null && Rtarget.IsKillable(R.Range)
                        && R.Slot.GetDamage(Rtarget) > Prediction.Health.GetPrediction(Rtarget, R.CastDelay))
                    {
                        R.Cast(Rtarget, R.hitchance(Menuini));
                    }
                }
            }

            public static void Harass()
            {
                var Qmode = HarassMenu.combobox("qmode");

                if (QMissle != null && Qmode == 0 && EntityManager.Heroes.Enemies.Any(e => e.IsInRange(QMissle, Qsize) && e.IsKillable()))
                {
                    Q.Cast(Game.CursorPos);
                }

                var target = TargetSelector.GetTarget(1000, DamageType.Magical);
                var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);

                var useQ = HarassMenu.checkbox(Q.Slot.ToString()) && target.IsValidTarget(Q.Range) && Q.IsReady() && Q.Mana(HarassMenu);
                var useW = HarassMenu.checkbox(W.Slot.ToString()) && W.IsReady() && W.Mana(HarassMenu);

                if (useQ)
                {
                    if (Q.Handle.ToggleState != 2 && !target.IsValidTarget(250))
                    {
                        if (Q.Handle.ToggleState != 2)
                        {
                            Q.Cast(target, Q.hitchance(Menuini));
                        }
                    }
                    if (QMissle != null && Q.Handle.ToggleState == 2 && Qmode == 1 && target.IsInRange(QMissle, Qsize))
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }

                if (useW)
                {
                    if (Wtarget != null)
                    {
                        if (W.Handle.ToggleState != 2 && Wtarget.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                        if (W.Handle.ToggleState == 2 && Wtarget.IsValidTarget(W2.Range))
                        {
                            W.Cast();
                        }
                    }

                    if (Wtarget == null && W.Handle.ToggleState == 2)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static class Clear
        {
            public static void LaneClear()
            {
                var useQ = LaneClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && Q.Mana(LaneClearMenu);
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable(Q.Range));
                if (!useQ || minions == null)
                {
                    return;
                }

                foreach (var minion in minions)
                {
                    if (minion.CountEnemyMinions(180) > 2)
                    {
                        Q.Cast(minion);
                    }
                }
            }

            public static void JungleClear()
            {
                var useQ = JungleClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && Q.Mana(JungleClearMenu);
                var mob =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(m => m.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable(Q.Range));
                if (!useQ || mob == null)
                {
                    return;
                }

                Q.Cast(mob);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            // Spells Drawings
            foreach (var spell in SpellList)
            {
                var color = ColorMenu.Color(spell.Slot.ToString());
                spell.SpellRange(color, DrawMenu.checkbox(spell.Slot.ToString()));
            }
            W2.SpellRange(ColorMenu.Color("w2"), DrawMenu.checkbox("w2"));

            // Damage
            DrawingsManager.DrawTotalDamage(DamageType.Magical, DrawMenu.checkbox("damage"));
        }
    }
}