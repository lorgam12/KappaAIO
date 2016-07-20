using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaAIO.Core;
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaAIO.Champions
{
    internal class Kindred : Base
    {
        private static readonly Spell.Skillshot Q;

        private static readonly Spell.Active W;

        private static readonly Spell.Targeted E;

        private static readonly Spell.Active R;

        static Kindred()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Linear, 250, int.MaxValue, -1);
            W = new Spell.Active(SpellSlot.W, 900);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Active(SpellSlot.R, 500);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("Kindred", "Kindred");
            AutoMenu = Menuini.AddSubMenu("Auto");
            ComboMenu = Menuini.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo");
            HarassMenu = Menuini.AddSubMenu("Harass");
            HarassMenu.AddGroupLabel("Harass");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear");
            JungleClearMenu.AddGroupLabel("JungleClear");
            KillStealMenu = Menuini.AddSubMenu("Stealer");
            DrawMenu = Menuini.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawings");
            ColorMenu = Menuini.AddSubMenu("ColorPicker");
            ColorMenu.AddGroupLabel("ColorPicker");

            Menuini.Add("focusE", new CheckBox("Focus Target With E Mark"));
            Menuini.Add("focusP", new CheckBox("Focus Target Passive Mark", false));
            Menuini.Add("wr", new Slider("Reduce W Range by [800 - {0}]", 250, 0, 500));

            AutoMenu.AddGroupLabel("Auto Settings");
            AutoMenu.Add("Gap", new CheckBox("Anti GapCloser - Q"));
            AutoMenu.AddSeparator(0);
            AutoMenu.AddGroupLabel("AutoR Settings");
            AutoMenu.Add("R", new CheckBox("Use R"));
            AutoMenu.Add("Rhp", new Slider("Use R If MY HP under [{0}%]", 35));
            AutoMenu.Add("Rally", new Slider("Use R If ALLY HP under [{0}%]", 25));

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

            Orbwalker.OnUnkillableMinion += Clear.Orbwalker_OnUnkillableMinion;
            Orbwalker.OnPostAttack += Compat.Orbwalker_OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += Auto.Obj_AI_Base_OnProcessSpellCast;
            Gapcloser.OnGapcloser += Auto.Gapcloser_OnGapcloser;
            OnIncDmg += Kindred_OnIncDmg;
        }

        private static void Kindred_OnIncDmg(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args, float IncDamage)
        {
            if(!AutoMenu.checkbox("R") || !R.IsReady() || !sender.IsEnemy) return;

            var allyhp = AutoMenu.slider("Rally");
            var mehp = AutoMenu.slider("Rhp");

            if (target != null && target.IsKillable(R.Range))
            {
                if (target.IsAlly && !target.IsMe)
                {
                    if (IncDamage >= target.Health || allyhp >= target.Health)
                    {
                        R.Cast();
                    }
                }
                if (target.IsMe)
                {
                    if (IncDamage >= target.Health || mehp >= target.Health)
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static AIHeroClient Target()
        {
            var Etarget = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsKillable(user.GetAutoAttackRange()) && enemy.Buffs.Any(buff => buff.Name == "KindredERefresher"));

            var Ptarget = EntityManager.Heroes.Enemies.FirstOrDefault(enemy => enemy.IsKillable(user.GetAutoAttackRange()) && enemy.Buffs.Any(buff => buff.Name == "KindredHitTracker"));

            if (Etarget != null && Menuini.checkbox("focusE"))
            {
                return Etarget;
            }

            if (Ptarget != null && Etarget == null && Menuini.checkbox("focusP"))
            {
                return Ptarget;
            }

            return TargetSelector.GetTarget(Q.Range, DamageType.Physical);
        }

        public override void Active()
        {
            W.Range = (uint)(900 - Menuini.slider("wr"));
        }

        public override void Combo()
        {
            var target = Target();

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var useQW = ComboMenu.checkbox(Q.Slot.ToString()) && ComboMenu.checkbox(W.Slot.ToString()) && ComboMenu.checkbox("QW") && target.IsKillable(Q.Range) && !target.IsKillable(user.AttackRange);
            var useQ = ComboMenu.checkbox(Q.Slot.ToString()) && target.IsKillable(Q.Range) && !target.IsKillable(user.GetAutoAttackRange()) && Q.IsReady();
            var useW = ComboMenu.checkbox(W.Slot.ToString()) && target.IsKillable(W.Range) && W.IsReady();
            var useE = ComboMenu.checkbox(E.Slot.ToString()) && target.IsKillable(E.Range) && E.IsReady();

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

        public override void Harass()
        {
            var target = Target();

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var useQ = HarassMenu.checkbox(Q.Slot.ToString()) && target.IsKillable(Q.Range) && !target.IsKillable(user.GetAutoAttackRange()) && Q.IsReady() && Q.Mana(HarassMenu);
            var useW = HarassMenu.checkbox(W.Slot.ToString()) && target.IsKillable(W.Range) && W.IsReady() && W.Mana(HarassMenu);
            var useE = HarassMenu.checkbox(E.Slot.ToString()) && target.IsKillable(E.Range) && E.IsReady() && E.Mana(HarassMenu);

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

        public override void LaneClear()
        {
            var Etarget = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsKillable(user.GetAutoAttackRange()) && m.Buffs.Any(buff => buff.Name == "KindredERefresher"));

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
                var aakill = user.GetAutoAttackDamage(minion) > Prediction.Health.GetPrediction(minion, (int)user.AttackDelay);

                if (useQ && minion.CountEnemyMinions(400) >= 2)
                {
                    Q.Cast(minion);
                }

                if (useE && !aakill && E.GetDamage(minion) > Prediction.Health.GetPrediction(minion, 1000))
                {
                    E.Cast(minion);
                }

                if (useW && user.CountEnemyMinions(W.Range) > 2 && W.Handle.ToggleState != 2)
                {
                    W.Cast();
                }
            }
        }

        public override void JungleClear()
        {
            var Etarget = EntityManager.MinionsAndMonsters.GetJungleMonsters().FirstOrDefault(m => m.IsKillable(user.GetAutoAttackRange()) && m.Buffs.Any(buff => buff.Name == "KindredERefresher"));

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

                if (useW && minion.IsKillable(W.Range) && W.Handle.ToggleState != 2)
                {
                    W.Cast();
                }
            }
        }

        public override void KillSteal()
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

        public override void Draw()
        {
            /*
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target != null)
            {
                Logics.Qlogic(ComboMenu, target, true);
            }
            */
        }

        private static class Auto
        {
            public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
            {
                if (!sender.IsEnemy || !AutoMenu.checkbox("Gap") || !Q.IsReady() || sender == null || e == null || e.End == Vector3.Zero || !e.End.IsInRange(user, 500)
                    || !kCore.GapMenu.checkbox(e.SpellName + sender.ID()))
                {
                    return;
                }

                Q.Cast(user.ServerPosition.Extend(sender.ServerPosition, -400).To3D());
            }

            public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe && args.Slot == SpellSlot.Q)
                {
                    Orbwalker.ResetAutoAttack();
                }
            }
        }

        private static class Compat
        {
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
            public static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
            {
                if (!Common.orbmode(Orbwalker.ActiveModes.LaneClear) || !Common.orbmode(Orbwalker.ActiveModes.LastHit) || target == null)
                {
                    return;
                }

                var useQ = LaneClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && target.IsKillable(Q.Range) && Q.GetDamage(target) > Prediction.Health.GetPrediction(target, Q.CastDelay);
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
                                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(750)).FirstOrDefault(a => a.IsKillable(1000) && !a.IsMe);
                                if (ally != null)
                                {
                                    pos = ally.ServerPosition;
                                }
                            }

                            if (target.IsKillable(user.GetAutoAttackRange() - 100))
                            {
                                pos = user.ServerPosition.Extend(target.ServerPosition, -400).To3D();
                            }

                            if (!target.IsKillable(user.GetAutoAttackRange()))
                            {
                                pos = Q.GetPrediction(target).CastPosition;
                            }
                        }

                        break;
                    case 1:
                        {
                            if (target.IsKillable(user.GetAutoAttackRange() - 100))
                            {
                                pos = user.ServerPosition.Extend(target.ServerPosition, -400).To3D();
                            }
                        }

                        break;
                    case 2:
                        {
                            if (!target.IsKillable(user.GetAutoAttackRange()))
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
