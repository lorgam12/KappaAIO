﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaAIO.Core;
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;
using SharpDX;

namespace KappaAIO.Champions
{
    internal class Brand : Base
    {
        private static readonly Spell.Skillshot Q;

        private static readonly Spell.Skillshot W;

        private static readonly Spell.Targeted E;

        private static readonly Spell.Targeted R;

        static Brand()
        {
            try
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1600, 120);
                W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 650, -1, 200);
                E = new Spell.Targeted(SpellSlot.E, 630);
                R = new Spell.Targeted(SpellSlot.R, 750);

                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);

                Menuini = MainMenu.AddMenu("Brand", "Brand");
                AutoMenu = Menuini.AddSubMenu("Auto");
                ComboMenu = Menuini.AddSubMenu("Combo");
                HarassMenu = Menuini.AddSubMenu("Harass");
                HarassMenu.AddGroupLabel("Harass");
                LaneClearMenu = Menuini.AddSubMenu("LaneClear");
                LaneClearMenu.AddGroupLabel("LaneClear");
                JungleClearMenu = Menuini.AddSubMenu("JungleClear");
                JungleClearMenu.AddGroupLabel("JungleClear");
                KillStealMenu = Menuini.AddSubMenu("Stealer");
                DrawMenu = Menuini.AddSubMenu("Drawings");
                ColorMenu = Menuini.AddSubMenu("Colors");

                foreach (var spell in SpellList.Where(s => s != E && s != R))
                {
                    Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
                    Menuini.AddSeparator(0);
                }

                AutoMenu.AddGroupLabel("Auto Settings");
                AutoMenu.Add("AutoR", new Slider("Auto R AoE hit [{0}] Targets or more", 2, 1, 6));
                AutoMenu.Add("Gap", new CheckBox("Anti GapCloser"));
                AutoMenu.Add("Int", new CheckBox("Auto Interrupter"));
                AutoMenu.Add("Danger", new ComboBox("Interrupter Danger Level", 1, "High", "Medium", "Low"));
                AutoMenu.AddSeparator(0);
                AutoMenu.AddGroupLabel("Auto Hit Passive");
                AutoMenu.Add("AutoQ", new CheckBox("Auto Q Dotnate Passive"));
                AutoMenu.Add("AutoW", new CheckBox("Auto W Dotnate Passive", false));
                AutoMenu.Add("AutoE", new CheckBox("Auto E Dotnate Passive"));

                ComboMenu.AddGroupLabel("Combo Settings");
                ComboMenu.Add("Q", new CheckBox("Use Q"));
                ComboMenu.AddLabel("Extra Q Settings");
                ComboMenu.Add("Qp", new CheckBox("Q Only for stun"));
                ComboMenu.Add(Q.Slot + "mana", new Slider("Use Q if Mana% is more than [{0}%]", 10));
                ComboMenu.AddSeparator(1);

                ComboMenu.Add("W", new CheckBox("Use W"));
                ComboMenu.AddLabel("Extra W Settings");
                ComboMenu.Add("Wp", new CheckBox("W Only if target has brand passive", false));
                ComboMenu.Add(W.Slot + "mana", new Slider("Use W if Mana% is more than [{0}%]", 5));
                ComboMenu.AddSeparator(1);

                ComboMenu.Add("E", new CheckBox("Use E"));
                ComboMenu.AddLabel("Extra E Settings");
                ComboMenu.Add("Ep", new CheckBox("E Only if target has brand passive", false));
                ComboMenu.Add(E.Slot + "mana", new Slider("Use E if Mana% is more than [{0}%]", 15));
                ComboMenu.AddSeparator(1);

                ComboMenu.Add("RFinisher", new CheckBox("Use R Finisher"));
                ComboMenu.Add("RAoe", new CheckBox("Use R Aoe"));
                ComboMenu.Add("Rhit", new Slider("R AoE hit [{0}] Targets or more", 2, 1, 6));
                ComboMenu.Add(R.Slot + "mana", new Slider("Use R if Mana% is more than [{0}%]"));

                foreach (var spell in SpellList.Where(s => s.Slot != SpellSlot.R))
                {
                    HarassMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    HarassMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    HarassMenu.AddSeparator(1);
                    LaneClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    LaneClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    LaneClearMenu.AddSeparator(1);
                    JungleClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    JungleClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    JungleClearMenu.AddSeparator(1);
                }

                KillStealMenu.AddGroupLabel("KillSteal");
                foreach (var spell in SpellList)
                {
                    KillStealMenu.Add(spell.Slot + "ks", new CheckBox("Use " + spell.Slot));
                }

                KillStealMenu.AddSeparator(0);
                KillStealMenu.AddGroupLabel("JungleSteal");
                foreach (var spell in SpellList)
                {
                    KillStealMenu.Add(spell.Slot + "js", new CheckBox("Use " + spell.Slot));
                }

                DrawMenu.AddGroupLabel("Drawings");
                DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
                DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");
                DrawMenu.AddSeparator(1);
                foreach (var spell in SpellList)
                {
                    DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                    ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
                }

                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            }
            catch (Exception e)
            {
                Common.Log(e.ToString());
            }
        }

        public override void Active()
        {
            var targets = EntityManager.Heroes.Enemies.Where(e => e.countpassive() >= 2 && e.IsKillable() && e.IsKillable());

            if (targets != null)
            {
                var aiHeroClients = targets as AIHeroClient[] ?? targets.ToArray();
                if (AutoMenu.checkbox("AutoQ"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsKillable(Q.Range));
                    if (target != null)
                    {
                        Q.Cast(target, Q.hitchance(Menuini));
                    }
                }

                if (AutoMenu.checkbox("AutoW"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsKillable(W.Range));
                    if (target != null)
                    {
                        W.Cast(target, W.hitchance(Menuini));
                    }
                }

                if (AutoMenu.checkbox("AutoE"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsKillable(E.Range));
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
            }

            var hits = AutoMenu.slider("AutoR");
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(R.Range) && e.IsKillable());
            var aoetarget = enemies.FirstOrDefault(e => user.CountEnemeis(400) >= hits);

            if (aoetarget != null)
            {
                R.Cast(aoetarget);
            }
        }

        public override void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var Qready = ComboMenu.checkbox("Q") && Q.IsReady() && target.IsKillable(Q.Range) && Q.Mana(ComboMenu);

            var Wready = ComboMenu.checkbox("W") && W.IsReady() && target.IsKillable(W.Range) && W.Mana(ComboMenu);

            var Eready = ComboMenu.checkbox("E") && E.IsReady() && target.IsKillable(E.Range) && E.Mana(ComboMenu);

            var RFinisher = ComboMenu.checkbox("RFinisher") && R.IsReady() && target.IsKillable(R.Range) && R.Mana(ComboMenu);

            var RAoe = ComboMenu.checkbox("RAoe") && R.IsReady() && target.IsKillable(R.Range) && R.Mana(ComboMenu);

            if (Qready)
            {
                Qlogic(target);
            }

            if (Wready)
            {
                Wlogic(target);
            }

            if (Eready)
            {
                Elogic(target);
            }

            if (RFinisher)
            {
                Rlogic(target);
            }

            if (RAoe)
            {
                Rlogic(target);
            }
        }

        public override void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var Qready = HarassMenu.checkbox("Q") && Q.IsReady() && target.IsKillable(Q.Range) && Q.Mana(HarassMenu);

            var Wready = HarassMenu.checkbox("W") && W.IsReady() && target.IsKillable(W.Range) && W.Mana(HarassMenu);

            var Eready = HarassMenu.checkbox("E") && E.IsReady() && target.IsKillable(E.Range) && E.Mana(HarassMenu);

            if (Qready)
            {
                Qlogic(target);
            }

            if (Wready)
            {
                Wlogic(target);
            }

            if (Eready)
            {
                Elogic(target);
            }
        }

        public override void LaneClear()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsKillable(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsKillable(Q.Range) && Q.Mana(LaneClearMenu);

            var Wready = LaneClearMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsKillable(W.Range) && W.Mana(LaneClearMenu);

            var Eready = LaneClearMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsKillable(E.Range) && E.Mana(LaneClearMenu);

            if (Qready)
            {
                Qlogic(target);
            }

            if (Wready)
            {
                Wlogic(target);
            }

            if (Eready)
            {
                Elogic(target);
            }
        }

        public override void JungleClear()
        {
            var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().FirstOrDefault(m => m.IsKillable(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = JungleClearMenu.checkbox("Q") && Q.IsReady() && target.IsKillable(Q.Range) && Q.Mana(JungleClearMenu);
            var Wready = JungleClearMenu.checkbox("W") && W.IsReady() && target.IsKillable(W.Range) && W.Mana(JungleClearMenu);
            var Eready = JungleClearMenu.checkbox("E") && E.IsReady() && target.IsKillable(E.Range) && E.Mana(JungleClearMenu);

            if (Qready)
            {
                Qlogic(target);
            }

            if (Wready)
            {
                Wlogic(target);
            }

            if (Eready)
            {
                Elogic(target);
            }
        }

        public override void KillSteal()
        {
            foreach (var spell in SpellList)
            {
                if (KillStealMenu.checkbox(spell.Slot + "ks") && spell.GetKStarget() != null)
                {
                    spell.Cast(spell.GetKStarget());
                }

                if (KillStealMenu.checkbox(spell.Slot + "js") && spell.GetJStarget() != null)
                {
                    spell.Cast(spell.GetJStarget());
                }
            }
        }

        public override void Draw()
        {
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || !Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                return;
            }

            var Eready = LaneClearMenu.checkbox("E") && E.IsReady() && target.IsKillable(E.Range) && E.Mana(LaneClearMenu);

            if (Eready && E.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
            {
                E.Cast(target);
            }
        }

        public static void Qlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            if (Combomode)
            {
                if (ComboMenu.checkbox("Qp"))
                {
                    if (target.brandpassive())
                    {
                        Q.Cast(target, Q.hitchance(Menuini));
                    }
                }
                else
                {
                    Q.Cast(target, Q.hitchance(Menuini));
                }
            }

            if (Harassmode)
            {
                Q.Cast(target, Q.hitchance(Menuini));
            }

            if (LaneClearmode)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay) && Q.GetPrediction(m).HitChance >= HitChance.High);
                if (minion != null)
                {
                    if (user.GetAutoAttackDamage(minion, true) >= Prediction.Health.GetPrediction(minion, (int)Orbwalker.AttackDelay))
                    {
                        return;
                    }

                    Q.Cast(minion);
                }
            }

            if (JungleClearmode)
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).FirstOrDefault(m => Q.GetPrediction(m).HitChance >= HitChance.High);

                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }
        }

        public static void Wlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(W.Range) && e.IsKillable());
            var pred = Prediction.Position.PredictCircularMissileAoe(enemies.Cast<Obj_AI_Base>().ToArray(), W.Range, W.Width + 50, W.CastDelay, W.Speed);
            var castpos = pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length).FirstOrDefault(p => p.CollisionObjects.Contains(target));
            if (Combomode)
            {
                if (ComboMenu.checkbox("Wp"))
                {
                    if (castpos != null && castpos.CollisionObjects.Length > 1)
                    {
                        W.Cast(castpos.CastPosition);
                    }

                    if (target.brandpassive())
                    {
                        W.Cast(target, W.hitchance(Menuini));
                    }
                }
                else
                {
                    W.Cast(target, W.hitchance(Menuini));
                }
            }

            if (Harassmode)
            {
                if (castpos != null && castpos.CollisionObjects.Length > 1)
                {
                    W.Cast(castpos.CastPosition);
                }

                W.Cast(target, W.hitchance(Menuini));
            }

            if (LaneClearmode)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(e => e.IsKillable(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions.ToArray(), W.Width + 75, (int)W.Range + 50, W.CastDelay, W.Speed);

                var farmpos = loc.CastPosition;

                if (farmpos != null && loc.HitNumber >= 2)
                {
                    W.Cast(farmpos);
                }
            }

            if (JungleClearmode)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).Where(e => e.IsKillable(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions.ToArray(), W.Width + 75, (int)W.Range + 50, W.CastDelay, W.Speed);
                var farmpos = loc.CastPosition;

                if (farmpos != null)
                {
                    W.Cast(farmpos);
                }
            }
        }

        public static void Elogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var Harassmode = Common.orbmode(Orbwalker.ActiveModes.Harass);
            var LaneClearmode = Common.orbmode(Orbwalker.ActiveModes.LaneClear);
            var JungleClearmode = Common.orbmode(Orbwalker.ActiveModes.JungleClear);

            if (Combomode)
            {
                if (ComboMenu.checkbox("Ep") && target.brandpassive())
                {
                    E.Cast(target);
                }
                else
                {
                    E.Cast(target);
                }
            }

            if (Harassmode)
            {
                E.Cast(target);
            }

            if (LaneClearmode)
            {
                var minionpassive = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.brandpassive() && m.IsKillable(E.Range));
                if (minionpassive != null)
                {
                    foreach (var minion in minionpassive)
                    {
                        var count = minion.CountEnemyMinions(300);
                        if (count >= 2)
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }

            if (JungleClearmode)
            {
                var minionpassive = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.brandpassive() && m.IsKillable(E.Range));
                if (minionpassive != null)
                {
                    foreach (var minion in minionpassive)
                    {
                        var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(e => e.IsKillable(E.Range) && e.IsKillable());
                        var count = minions.Count(m => m.IsInRange(minion, 300));
                        if (count >= 2)
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }
        }

        public static void Rlogic(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            var Combomode = Common.orbmode(Orbwalker.ActiveModes.Combo);
            var hits = ComboMenu.slider("Rhit");

            if (Combomode)
            {
                if (ComboMenu.checkbox("RAoe"))
                {
                    var AoeHit = target.CountEnemeis(400) >= hits;
                    var bestaoe = EntityManager.Heroes.Enemies.OrderByDescending(e => e.CountEnemeis(400)).FirstOrDefault(e => e.IsKillable(R.Range) && e.IsKillable() && e.CountEnemeis(400) >= hits);
                    if (AoeHit)
                    {
                        R.Cast(target);
                    }
                    else
                    {
                        if (bestaoe != null)
                        {
                            R.Cast(bestaoe);
                        }
                    }
                }

                if (ComboMenu.checkbox("RFinisher"))
                {
                    var pred = R.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay);
                    var health = R.GetDamage(target) >= target.TotalShieldHealth();

                    if (Q.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay))
                    {
                        return;
                    }

                    if (W.GetDamage(target) >= Prediction.Health.GetPrediction(target, W.CastDelay))
                    {
                        return;
                    }

                    if (E.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
                    {
                        return;
                    }

                    if (pred || health)
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || !AutoMenu.checkbox("Int") || sender == null || e == null)
            {
                return;
            }

            if (e.DangerLevel >= Common.danger(AutoMenu) && sender.IsKillable(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender, Q.hitchance(Menuini));
                    }
                }
                else
                {
                    if (E.IsReady() && Q.IsReady())
                    {
                        if (E.Cast(sender))
                        {
                            if (sender.brandpassive())
                            {
                                Q.Cast(sender, Q.hitchance(Menuini));
                            }
                        }
                    }
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || !AutoMenu.checkbox("Gap") || sender == null || e == null || e.End == Vector3.Zero || !e.End.IsInRange(user, Q.Range))
            {
                return;
            }

            if (kCore.GapMenu.checkbox(e.SpellName + sender.ID()) && sender.IsKillable(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender, Q.hitchance(Menuini));
                    }
                }
                else
                {
                    if (E.IsReady() && Q.IsReady())
                    {
                        if (E.Cast(sender))
                        {
                            if (sender.brandpassive())
                            {
                                Q.Cast(sender, Q.hitchance(Menuini));
                            }
                        }
                    }
                }
            }
        }
    }
}
