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
    using EloBuddy.SDK.Rendering;

    using KappaAIO.Core;
    using KappaAIO.Core.Managers;

    using SharpDX;

    using Color = System.Drawing.Color;

    internal class Kindred
    {
        private static Spell.Skillshot Q;

        private static Spell.Active W;

        private static Spell.Targeted E;

        private static Spell.Active R;

        private static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        private static Menu MenuIni, AutoMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, DrawMenu, ColorMenu;

        public static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Linear, 250, int.MaxValue, -1);
            W = new Spell.Active(SpellSlot.W, 900);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Active(SpellSlot.R, 500);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            MenuIni = MainMenu.AddMenu("Kindred", "Kindred");
            AutoMenu = MenuIni.AddSubMenu("Auto");
            ComboMenu = MenuIni.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo");
            HarassMenu = MenuIni.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass");
            LaneClearMenu = MenuIni.AddSubMenu("LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear");
            JungleClearMenu = MenuIni.AddSubMenu("JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear");
            KillStealMenu = MenuIni.AddSubMenu("Stealer");
            DrawMenu = MenuIni.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawings");
            ColorMenu = MenuIni.AddSubMenu("ColorPicker");
            ColorMenu.AddGroupLabel("ColorPicker");

            MenuIni.Add("focusE", new CheckBox("Focus Target With E Mark"));
            MenuIni.Add("focusP", new CheckBox("Focus Target Passive Mark", false));
            MenuIni.Add("wr", new Slider("Reduce W Range by [800 - {0}]", 250, 0, 500));

            AutoMenu.AddGroupLabel("Auto Settings");
            AutoMenu.Add("Gap", new CheckBox("Anti GapCloser - Q"));
            AutoMenu.AddSeparator(0);
            AutoMenu.AddGroupLabel("AutoR Settings");
            AutoMenu.Add("R", new CheckBox("Use R"));
            AutoMenu.Add("Rhp", new Slider("Use R If MY HP under [{0}%]", 35));
            AutoMenu.Add("Rally", new Slider("Use R If ALLY HP under [{0}%]", 25));
            AutoMenu.AddSeparator(0);
            AutoMenu.AddGroupLabel("Anti GapCloser - Spells");
            foreach (var gapspell in
                EntityManager.Heroes.Enemies.SelectMany(enemy => Gapcloser.GapCloserList.Where(e => e.ChampName == enemy.ChampionName)))
            {
                AutoMenu.Add(gapspell.SpellName, new CheckBox(gapspell.SpellName + " - " + gapspell.SpellSlot));
            }

            ComboMenu.Add("Qmode", new ComboBox("Q Mode", 0, "Auto", "Kite", "Chase", "To Mouse"));
            HarassMenu.Add("Qmode", new ComboBox("Q Mode", 0, "Auto", "Kite", "Chase", "To Mouse"));
            foreach (var spell in SpellList)
            {
                if (spell != R)
                {
                    ComboMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    HarassMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    HarassMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    LaneClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    LaneClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    JungleClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    JungleClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                }
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", Color.Chartreuse));
            }

            ComboMenu.AddGroupLabel("Extra Settings");
            ComboMenu.Add("QW", new CheckBox("Use Smart W Q"));

            KillStealMenu.Add(Q.Slot + "ks", new CheckBox("Q Killsteal"));
            KillStealMenu.Add(Q.Slot + "js", new CheckBox("Q JungleSteal"));

            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");

            Common.ShowNotification("KappaKindred - Loaded", 5000);

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnUnkillableMinion += Clear.Orbwalker_OnUnkillableMinion;
            Orbwalker.OnPostAttack += Compat.Orbwalker_OnPostAttack;
            Obj_AI_Base.OnBasicAttack += Auto.Obj_AI_Base_OnBasicAttack;
            Obj_AI_Base.OnProcessSpellCast += Auto.Obj_AI_Base_OnProcessSpellCast;
            Gapcloser.OnGapcloser += Auto.Gapcloser_OnGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var color = ColorMenu.Color(spell.Slot.ToString());
                spell.SpellRange(color, DrawMenu.checkbox(spell.Slot.ToString()));
            }

            DrawingsManager.DrawTotalDamage(DamageType.Physical, DrawMenu.checkbox("damage"));
            /*
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                Logics.Qlogic(ComboMenu, target, true);
            }
            */
        }

        private static void Game_OnTick(EventArgs args)
        {
            W.Range = (uint)(900 - MenuIni.slider("wr"));

            Auto.KillSteal();

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

        private static class Auto
        {
            public static void KillSteal()
            {
                if (KillStealMenu.checkbox(Q.Slot + "ks") && Q.GetKStarget() != null)
                {
                    Q.Cast(Q.GetKStarget());
                }

                if (KillStealMenu.checkbox(Q.Slot + "js") && Q.GetJStarget() != null)
                {
                    Q.Cast(Q.GetJStarget());
                }
            }

            public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
            {
                if (!sender.IsEnemy || !AutoMenu.checkbox("Gap") || !Q.IsReady() || sender == null || e == null || e.End == Vector3.Zero
                    || !e.End.IsInRange(Player.Instance, 500) || !AutoMenu.checkbox(e.SpellName))
                {
                    return;
                }

                Q.Cast(Player.Instance.ServerPosition.Extend(sender.ServerPosition, -400).To3D());
            }

            public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe && args.Slot == SpellSlot.Q)
                {
                    Orbwalker.ResetAutoAttack();
                }

                if (!(args.Target is AIHeroClient) || !sender.IsEnemy || !AutoMenu.checkbox("R") || !R.IsReady() && Player.Instance.IsDead)
                {
                    return;
                }

                var caster = sender;
                var enemy = sender as AIHeroClient;
                var target = (AIHeroClient)args.Target;
                Common.ally = EntityManager.Heroes.Allies.FirstOrDefault(a => a.IsInRange(args.End, 100) && a.IsKillable(R.Range) && !a.IsMe);
                var hitally = Common.ally != null && args.End != Vector3.Zero && args.End.Distance(Common.ally) < 100;
                var hitme = args.End != Vector3.Zero && args.End.Distance(Player.Instance) < 100;
                var allyhp = AutoMenu.slider("Rally");
                var mehp = AutoMenu.slider("Rhp");

                if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || enemy == null || caster == null)
                {
                    return;
                }

                if (((target.IsAlly && !target.IsMe) || hitally) && Common.ally != null && Common.ally.IsValidTarget(R.Range))
                {
                    var spelldamageally = enemy.GetSpellDamage(Common.ally, args.Slot);
                    var damagepercentally = (spelldamageally / Common.ally.TotalShieldHealth()) * 100;
                    var deathally = damagepercentally >= Common.ally.HealthPercent || spelldamageally >= Common.ally.TotalShieldHealth()
                                    || caster.GetAutoAttackDamage(Common.ally, true) >= Common.ally.TotalShieldHealth()
                                    || enemy.GetAutoAttackDamage(Common.ally, true) >= Common.ally.TotalShieldHealth();

                    if (Common.ally.IsInRange(Player.Instance, R.Range) && !Common.ally.IsMe)
                    {
                        if (allyhp > Common.ally.HealthPercent || deathally)
                        {
                            R.Cast();
                        }
                    }
                }

                if (target.IsMe || hitme)
                {
                    var spelldamageme = enemy.GetSpellDamage(Player.Instance, args.Slot);
                    var damagepercentme = (spelldamageme / Player.Instance.TotalShieldHealth()) * 100;
                    var deathme = damagepercentme >= Player.Instance.HealthPercent || spelldamageme >= Player.Instance.TotalShieldHealth()
                                  || caster.GetAutoAttackDamage(Player.Instance, true) >= Player.Instance.TotalShieldHealth()
                                  || enemy.GetAutoAttackDamage(Player.Instance, true) >= Player.Instance.TotalShieldHealth();

                    if (mehp > Player.Instance.HealthPercent || deathme)
                    {
                        R.Cast();
                    }
                }
            }

            public static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (!(args.Target is AIHeroClient) || !AutoMenu.checkbox("R") || !R.IsReady() && Player.Instance.IsDead)
                {
                    return;
                }

                var caster = sender;
                var target = (AIHeroClient)args.Target;
                var allyhp = AutoMenu.slider("Rally");
                var mehp = AutoMenu.slider("Rhp");

                if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || target == null || caster == null || !target.IsAlly
                    || !target.IsKillable())
                {
                    return;
                }

                var aaprecent = (caster.GetAutoAttackDamage(target, true) / target.TotalShieldHealth()) * 100;
                var death = caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() || aaprecent >= target.HealthPercent;

                if (target.IsAlly && !target.IsMe && target.IsValidTarget(R.Range))
                {
                    if (allyhp > target.HealthPercent || death)
                    {
                        R.Cast();
                    }
                }

                if (target.IsMe)
                {
                    if (mehp > target.HealthPercent || death)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static class Compat
        {
            private static AIHeroClient target()
            {
                var Etarget =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        enemy =>
                        enemy.IsValidTarget(Player.Instance.GetAutoAttackRange()) && enemy.Buffs.Any(buff => buff.Name == "KindredERefresher"));

                var Ptarget =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        enemy =>
                        enemy.IsValidTarget(Player.Instance.GetAutoAttackRange()) && enemy.Buffs.Any(buff => buff.Name == "KindredHitTracker"));

                if (Etarget != null && MenuIni.checkbox("focusE"))
                {
                    return Etarget;
                }
                if (Ptarget != null && Etarget == null && MenuIni.checkbox("focusP"))
                {
                    return Ptarget;
                }
                return TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            }

            public static void Combo()
            {
                var target = Compat.target();

                if (target == null || !target.IsKillable())
                {
                    return;
                }

                var useQW = ComboMenu.checkbox(Q.Slot.ToString()) && ComboMenu.checkbox(W.Slot.ToString()) && ComboMenu.checkbox("QW")
                            && target.IsValidTarget(Q.Range) && !target.IsValidTarget(Player.Instance.AttackRange);
                var useQ = ComboMenu.checkbox(Q.Slot.ToString()) && target.IsValidTarget(Q.Range)
                           && !target.IsValidTarget(Player.Instance.GetAutoAttackRange()) && Q.IsReady();
                var useW = ComboMenu.checkbox(W.Slot.ToString()) && target.IsValidTarget(W.Range) && W.IsReady();
                var useE = ComboMenu.checkbox(E.Slot.ToString()) && target.IsValidTarget(E.Range) && E.IsReady();

                if (useE)
                {
                    E.Cast(target);
                }
                if (useQW)
                {
                    if (W.Handle.ToggleState != 2 && W.IsReady())
                    {
                        W.Cast();
                    }
                    else
                    {
                        Logics.Qlogic(ComboMenu, target);
                    }
                }
                else
                {
                    if (useW && W.Handle.ToggleState != 2)
                    {
                        W.Cast();
                    }

                    if (useQ)
                    {
                        Logics.Qlogic(ComboMenu, target);
                    }
                }
            }

            public static void Harass()
            {
                var target = Compat.target();

                if (target == null || !target.IsKillable())
                {
                    return;
                }

                var useQ = HarassMenu.checkbox(Q.Slot.ToString()) && target.IsValidTarget(Q.Range)
                           && !target.IsValidTarget(Player.Instance.GetAutoAttackRange()) && Q.IsReady() && Q.Mana(HarassMenu);
                var useW = HarassMenu.checkbox(W.Slot.ToString()) && target.IsValidTarget(W.Range) && W.IsReady() && W.Mana(HarassMenu);
                var useE = HarassMenu.checkbox(E.Slot.ToString()) && target.IsValidTarget(E.Range) && E.IsReady() && E.Mana(HarassMenu);

                if (useE)
                {
                    E.Cast(target);
                }

                if (useW && W.Handle.ToggleState != 2)
                {
                    W.Cast();
                }

                if (useQ)
                {
                    Logics.Qlogic(HarassMenu, target);
                }
            }

            public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
            {
                var orbtarget = target as Obj_AI_Base;
                if (orbtarget == null)
                {
                    return;
                }

                if (Common.orbmode(Orbwalker.ActiveModes.Combo))
                {
                    var useQ = ComboMenu.checkbox(Q.Slot.ToString()) && Q.IsReady();
                    if (!useQ)
                    {
                        return;
                    }
                    Logics.Qlogic(ComboMenu, orbtarget);
                }

                if (Common.orbmode(Orbwalker.ActiveModes.Harass))
                {
                    var useQ = HarassMenu.checkbox(Q.Slot.ToString()) && Q.IsReady();
                    if (!useQ)
                    {
                        return;
                    }
                    Logics.Qlogic(HarassMenu, orbtarget);
                }
            }
        }

        private static class Clear
        {
            public static void LaneClear()
            {
                var Etarget =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        m => m.IsValidTarget(Player.Instance.GetAutoAttackRange()) && m.Buffs.Any(buff => buff.Name == "KindredERefresher"));

                if (Etarget != null)
                {
                    Orbwalker.ForcedTarget = Etarget;
                }
                if (Orbwalker.IsAutoAttacking)
                {
                    return;
                }
                var useQ = LaneClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && Q.Mana(LaneClearMenu);
                var useW = LaneClearMenu.checkbox(W.Slot.ToString()) && W.IsReady() && W.Mana(LaneClearMenu);
                var useE = LaneClearMenu.checkbox(E.Slot.ToString()) && E.IsReady() && E.Mana(LaneClearMenu);

                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable());

                foreach (var minion in minions)
                {
                    var aakill = Player.Instance.GetAutoAttackDamage(minion)
                                 > Prediction.Health.GetPrediction(minion, (int)Player.Instance.AttackDelay);

                    if (useQ && minion.CountEnemyMinions(400) >= 2)
                    {
                        Q.Cast(minion);
                    }

                    if (useE && !aakill && E.Slot.GetDamage(minion) > Prediction.Health.GetPrediction(minion, 1000))
                    {
                        E.Cast(minion);
                    }
                    if (useW && Player.Instance.CountEnemyMinions(W.Range) > 2 && W.Handle.ToggleState != 2)
                    {
                        W.Cast();
                    }
                }
            }

            public static void JungleClear()
            {
                var Etarget =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .FirstOrDefault(
                            m => m.IsValidTarget(Player.Instance.GetAutoAttackRange()) && m.Buffs.Any(buff => buff.Name == "KindredERefresher"));

                if (Etarget != null)
                {
                    Orbwalker.ForcedTarget = Etarget;
                }
                if (Orbwalker.IsAutoAttacking)
                {
                    return;
                }
                var useQ = JungleClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && Q.Mana(JungleClearMenu);
                var useW = JungleClearMenu.checkbox(W.Slot.ToString()) && W.IsReady() && W.Mana(JungleClearMenu);
                var useE = JungleClearMenu.checkbox(E.Slot.ToString()) && E.IsReady() && E.Mana(JungleClearMenu);

                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsKillable()).OrderByDescending(m => m.MaxHealth);

                foreach (var minion in minions)
                {
                    if (useQ)
                    {
                        Q.Cast(minion);
                    }

                    if (useE)
                    {
                        E.Cast(minion);
                    }

                    if (useW && minion.IsValidTarget(W.Range) && W.Handle.ToggleState != 2)
                    {
                        W.Cast();
                    }
                }
            }

            public static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
            {
                if (!Common.orbmode(Orbwalker.ActiveModes.LaneClear) || !Common.orbmode(Orbwalker.ActiveModes.LastHit) || target == null)
                {
                    return;
                }

                var useQ = LaneClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && target.IsKillable(Q.Range)
                           && Q.Slot.GetDamage(target) > Prediction.Health.GetPrediction(target, Q.CastDelay);
                if (useQ)
                {
                    Q.Cast(target);
                }
            }
        }

        public static class Logics
        {
            public static void Qlogic(Menu m, Obj_AI_Base target, bool Draw = false)
            {
                var pos = new Vector3();
                var danger = target.CountEnemeis(600) > 3;

                switch (m.combobox("Qmode"))
                {
                    case 0:
                        {
                            if (danger)
                            {
                                var ally =
                                    EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(750))
                                        .FirstOrDefault(a => a.IsValidTarget(1000) && !a.IsMe);
                                if (ally != null)
                                {
                                    pos = ally.ServerPosition;
                                }
                            }
                            if (target.IsValidTarget(Player.Instance.GetAutoAttackRange() - 100))
                            {
                                pos = Player.Instance.ServerPosition.Extend(target.ServerPosition, -400).To3D();
                            }
                            if (!target.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                            {
                                pos = Q.GetPrediction(target).CastPosition;
                            }
                        }
                        break;
                    case 1:
                        {
                            if (target.IsValidTarget(Player.Instance.GetAutoAttackRange() - 100))
                            {
                                pos = Player.Instance.ServerPosition.Extend(target.ServerPosition, -400).To3D();
                            }
                        }
                        break;
                    case 2:
                        {
                            if (!target.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                            {
                                pos = Q.GetPrediction(target).CastPosition;
                            }
                        }
                        break;
                    case 3:
                        {
                            pos = Game.CursorPos;
                        }
                        break;
                }
                if (!Draw)
                {
                    Q.Cast(pos);
                }
                if (Draw)
                {
                    Circle.Draw(SharpDX.Color.White, 100, pos);
                }
            }
        }
    }
}