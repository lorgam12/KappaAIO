using System;
using System.Drawing;
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

namespace KappaAIO.Champions
{
    internal class AurelionSol : Base
    {
        public static float Qsize;

        private static MissileClient QMissle;

        private static Spell.Skillshot Q { get; }

        private static Spell.Active W { get; }

        private static Spell.Active W2 { get; }

        private static Spell.Skillshot R { get; }

        static AurelionSol()
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
            KillStealMenu = Menuini.AddSubMenu("KillSteal Settings");
            KillStealMenu.AddGroupLabel("Stealer Settings");
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
                    KillStealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                    KillStealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
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

            Gapcloser.OnGapcloser += Auto.Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Auto.Interrupter_OnInterruptableSpell;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid)
            {
                return;
            }

            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe && miss.SData.Name.Contains("AurelionSolQMissile"))
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
                    if (Q.IsReady() && AutoMenu.checkbox("GapQ") && rengar.IsKillable(Q.Range))
                    {
                        Q.Cast(rengar);
                        return;
                    }

                    if (R.IsReady() && AutoMenu.checkbox("GapR") && rengar.IsKillable(500))
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

        public override void Active()
        {
            Orbwalker.DisableAttacking = W.Handle.ToggleState == 2 && ComboMenu.checkbox("disableAA") && Common.orbmode(Orbwalker.ActiveModes.Combo);

            var f = (QMissle?.StartPosition.Distance(QMissle.Position) + Q.Width) / 16;
            if (f != null)
            {
                Qsize = (float)f;
            }

            Q.Range = (uint)Menuini.slider("qrange");
            W.Range = (uint)Menuini.slider("wmax");
            W2.Range = (uint)Menuini.slider("wmin");
        }

        public override void Combo()
        {
            var Qmode = ComboMenu.combobox("qmode");

            if (QMissle != null && Qmode == 0 && EntityManager.Heroes.Enemies.Any(e => e.IsInRange(QMissle, Qsize) && e.IsKillable()))
            {
                Q.Cast(Game.CursorPos);
            }

            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            var Rtarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            var useQ = ComboMenu.checkbox(Q.Slot.ToString()) && target.IsKillable(Q.Range) && Q.IsReady();
            var useW = ComboMenu.checkbox(W.Slot.ToString()) && W.IsReady();
            var useR = ComboMenu.checkbox(R.Slot.ToString()) && R.IsReady();
            var Rfinisher = ComboMenu.checkbox("Rfinisher");

            if (useQ && target != null)
            {
                if (Q.Handle.ToggleState != 2 && !target.IsKillable(250))
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
                    if (W.Handle.ToggleState != 2 && Wtarget.IsKillable(W.Range) && !Wtarget.IsKillable(W2.Range))
                    {
                        W.Cast();
                    }

                    if (W.Handle.ToggleState == 2 && Wtarget.IsKillable(W2.Range))
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
                        var predpos = Prediction.Position.PredictUnitPosition(enemy, R.CastDelay);
                        var Rectangle = new Geometry.Polygon.Rectangle(user.ServerPosition, user.ServerPosition.Extend(predpos, R.Range).To3D(), R.Width);

                        if (aiHeroClients.Count(e => Rectangle.IsInside(Prediction.Position.PredictUnitPosition(e, R.CastDelay))) >= ComboMenu.slider("Raoe"))
                        {
                            R.Cast(predpos.To3D());
                        }
                    }
                }

                if (Rfinisher && Rtarget != null && Rtarget.IsKillable(R.Range) && R.GetDamage(Rtarget) > Prediction.Health.GetPrediction(Rtarget, R.CastDelay))
                {
                    R.Cast(Rtarget, R.hitchance(Menuini));
                }
            }
        }

        public override void Harass()
        {
            var Qmode = HarassMenu.combobox("qmode");

            if (QMissle != null && Qmode == 0 && EntityManager.Heroes.Enemies.Any(e => e.IsInRange(QMissle, Qsize) && e.IsKillable()))
            {
                Q.Cast(Game.CursorPos);
            }

            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            var Wtarget = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            var useQ = HarassMenu.checkbox(Q.Slot.ToString()) && target.IsKillable(Q.Range) && Q.IsReady() && Q.Mana(HarassMenu);
            var useW = HarassMenu.checkbox(W.Slot.ToString()) && W.IsReady() && W.Mana(HarassMenu);

            if (useQ)
            {
                if (Q.Handle.ToggleState != 2 && !target.IsKillable(250))
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
                    if (W.Handle.ToggleState != 2 && Wtarget.IsKillable(W.Range))
                    {
                        W.Cast();
                    }

                    if (W.Handle.ToggleState == 2 && Wtarget.IsKillable(W2.Range))
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

        public override void LaneClear()
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

        public override void JungleClear()
        {
            var useQ = JungleClearMenu.checkbox(Q.Slot.ToString()) && Q.IsReady() && Q.Mana(JungleClearMenu);
            var mob = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).FirstOrDefault(m => m.IsKillable(Q.Range));
            if (!useQ || mob == null)
            {
                return;
            }

            Q.Cast(mob);
        }

        public override void KillSteal()
        {
            foreach (var spell in SpellList.Where(s => s != W))
            {
                if (spell.GetKStarget() != null && spell.IsReady() && KillStealMenu.checkbox(spell.Slot + "ks"))
                {
                    spell.Cast(spell.GetKStarget());
                }

                if (spell.GetJStarget() != null && spell.IsReady() && KillStealMenu.checkbox(spell.Slot + "js"))
                {
                    spell.Cast(spell.GetJStarget());
                }
            }
        }

        public override void Draw()
        {
            W2.SpellRange(ColorMenu.Color("W2"), DrawMenu.checkbox("w2"));
        }

        private static class Auto
        {
            public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
            {
                if (sender == null || !sender.IsEnemy || !sender.IsKillable() || Common.danger(AutoMenu) > e.DangerLevel)
                {
                    return;
                }

                if (AutoMenu.checkbox("IntQ") && Q.IsReady() && sender.IsKillable(Q.Range))
                {
                    Q.Cast(sender);
                    return;
                }

                if (AutoMenu.checkbox("IntR") && R.IsReady() && sender.IsKillable(500))
                {
                    R.Cast(sender);
                }
            }

            public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
            {
                if (sender == null || !sender.IsEnemy || !sender.IsKillable() || !kCore.GapMenu.checkbox(e.SpellName + sender.ID()))
                {
                    return;
                }

                if (AutoMenu.checkbox("GapQ") && Q.IsReady() && sender.IsKillable(Q.Range))
                {
                    Q.Cast(sender);
                    return;
                }

                if (AutoMenu.checkbox("GapR") && R.IsReady() && sender.IsKillable(500))
                {
                    R.Cast(sender);
                }
            }
        }
    }
}
