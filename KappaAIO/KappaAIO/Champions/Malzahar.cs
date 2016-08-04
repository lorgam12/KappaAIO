using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;

namespace KappaAIO.Champions
{
    internal class Malzahar : Base
    {
        private static Spell.Skillshot Q { get; }

        private static Spell.Skillshot W { get; }

        private static Spell.Targeted E { get; }

        private static Spell.Targeted R { get; }

        private static bool IsCastingR;

        static Malzahar()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 250, 500, 90);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 250, int.MaxValue, 80);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 700);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("Malzahar", "Malzahar");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            LaneClearMenu.AddGroupLabel("LaneClear");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            JungleClearMenu.AddGroupLabel("JungleClear");
            KillStealMenu = Menuini.AddSubMenu("KillSteal Settings");
            KillStealMenu.AddGroupLabel("Stealer");
            MiscMenu = Menuini.AddSubMenu("Misc Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            DrawMenu.AddGroupLabel("Drawings");
            ColorMenu = Menuini.AddSubMenu("ColorPicker");
            ColorMenu.AddGroupLabel("ColorPicker");

            foreach (var spell in SpellList.Where(s => s == Q))
            {
                Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
            }

            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("RCombo", new CheckBox("Use R Combo"));
            ComboMenu.Add("RFinisher", new CheckBox("Use R Finisher"));
            ComboMenu.Add("RTurret", new CheckBox("Use R if enemy Under Ally Turret"));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Don't Use Ult On:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                var cb = new CheckBox(enemy.BaseSkinName + " (" + enemy.Name + ")") { CurrentValue = false };
                ComboMenu.Add("DontUlt" + enemy.ID(), cb);
            }

            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");

            foreach (var spell in SpellList)
            {
                if (spell != R)
                {
                    HarassMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    HarassMenu.Add(spell.Slot + "mana", new Slider("Use" + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    LaneClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    LaneClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    JungleClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    JungleClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    KillStealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
                }

                KillStealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }

            KillStealMenu.AddGroupLabel("Don't Use Ult On:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                var cb = new CheckBox(enemy.BaseSkinName + " (" + enemy.Name + ")") { CurrentValue = false };
                KillStealMenu.Add("DontUlt" + enemy.ID(), cb);
            }

            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.Add("RSave", new CheckBox("Block All Commands When Casting R"));
            MiscMenu.Add("Qgap", new CheckBox("Q on GapCloser"));
            MiscMenu.Add("Rgap", new CheckBox("R on GapCloser"));
            MiscMenu.Add("Qint", new CheckBox("Q interrupt DangerSpells"));
            MiscMenu.Add("Rint", new CheckBox("R interrupt DangerSpells"));
            MiscMenu.Add("RTurret", new CheckBox("R Enemy Under Ally Tower"));
            MiscMenu.Add("blockR", new CheckBox("Block R under Enemy Turret", false));
            MiscMenu.Add("danger", new ComboBox("Spells DangerLevel to interrupt", 2, "High", "Medium", "Low"));

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Player.OnIssueOrder += Player_OnIssueOrder;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null)
            {
                return;
            }

            if (Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                var Eready = LaneClearMenu.checkbox("E") && E.IsReady();
                if (Eready && E.GetDamage(target) >= args.RemainingHealth)
                {
                    E.Cast(target);
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || IsCastingR)
            {
                return;
            }

            if (MiscMenu.checkbox("Qgap") && (e.End.IsInRange(user, Q.Range) || sender.IsKillable(Q.Range)))
            {
                Q.Cast(sender, Q.hitchance(Menuini));
            }

            if (MiscMenu.checkbox("blockR") && user.IsUnderEnemyturret())
            {
                return;
            }

            if (MiscMenu.checkbox("Rgap") && (e.End.IsInRange(user, R.Range) || sender.IsKillable(R.Range)))
            {
                R.Cast(sender);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && MiscMenu.checkbox("Rgap") && rengar.IsKillable(R.Range))
                {
                    R.Cast(rengar);
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || IsCastingR)
            {
                return;
            }

            if (Common.danger(MiscMenu) >= e.DangerLevel)
            {
                if (MiscMenu.checkbox("Qint") && sender.IsKillable(Q.Range))
                {
                    Q.Cast(sender, Q.hitchance(Menuini));
                }

                if (MiscMenu.checkbox("blockR") && user.IsUnderEnemyturret())
                {
                    return;
                }

                if (MiscMenu.checkbox("Rint") && sender.IsKillable(R.Range))
                {
                    R.Cast(sender);
                }
            }
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (MiscMenu.checkbox("Rsave") && IsCastingR)
            {
                args.Process = false;
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe)
            {
                return;
            }

            if (MiscMenu.checkbox("Rsave") && IsCastingR)
            {
                args.Process = false;
            }
        }

        public override void Active()
        {
            IsCastingR = user.Buffs.FirstOrDefault(b => b.Name.ToLower().Contains("malzaharrsound")) != null;
            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;

            if (IsCastingR)
            {
                return;
            }

            Rlogic();
        }

        public override void Draw()
        {
        }

        private static void Rlogic()
        {
            if (MiscMenu.checkbox("RTurret") && R.IsReady())
            {
                if (MiscMenu.checkbox("blockR") && user.IsUnderEnemyturret())
                {
                    return;
                }

                var targets = EntityManager.Heroes.Enemies.Where(e => e.IsUnderTurret() && !e.IsUnderHisturret() && !e.IsUnderEnemyturret() && e.IsKillable(R.Range));
                if (targets != null)
                {
                    foreach (var target in targets.Where(target => target != null))
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        public override void Combo()
        {
            if (IsCastingR)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range + 50, DamageType.Mixed);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var Qready = ComboMenu.checkbox("Q") && Q.IsReady() && target.IsKillable(Q.Range);

            var Wready = ComboMenu.checkbox("W") && W.IsReady() && target.IsKillable(W.Range);

            var Eready = ComboMenu.checkbox("E") && E.IsReady() && target.IsKillable(E.Range);

            var Rfinready = ComboMenu.checkbox("RFinisher") && R.IsReady() && target.IsKillable(R.Range);

            var Rcomready = ComboMenu.checkbox("RCombo") && R.IsReady() && target.IsKillable(R.Range);

            var RTurret = ComboMenu.checkbox("RTurret") && R.IsReady() && target.IsKillable(R.Range) && target.IsUnderTurret() && !target.IsUnderHisturret() && !target.IsUnderEnemyturret();

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready)
            {
                Q.Cast(target, Q.hitchance(Menuini));
            }

            if (Eready)
            {
                E.Cast(target);
            }

            if (!ComboMenu.checkbox("DontUlt" + target.ID()))
            {
                if (MiscMenu.checkbox("blockR") && user.IsUnderEnemyturret())
                {
                    return;
                }

                if (Rcomready && target.TotalDamage(SpellList) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (Rfinready && R.GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (RTurret)
                {
                    R.Cast(target);
                }
            }
        }

        public override void Harass()
        {
            if (IsCastingR)
            {
                return;
            }

            var target = TargetSelector.GetTarget(Q.Range + 50, DamageType.Mixed);

            if (target == null || !target.IsKillable())
            {
                return;
            }

            var Qready = HarassMenu.checkbox("Q") && Q.Mana(HarassMenu) && Q.IsReady() && target.IsKillable(Q.Range);

            var Wready = HarassMenu.checkbox("W") && W.Mana(HarassMenu) && W.IsReady();

            var Eready = HarassMenu.checkbox("E") && E.Mana(HarassMenu) && E.IsReady() && target.IsKillable(E.Range);

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready)
            {
                Q.Cast(target, Q.hitchance(Menuini));
            }

            if (Eready)
            {
                E.Cast(target);
            }
        }

        public override void LaneClear()
        {
            if (IsCastingR)
            {
                return;
            }

            var Qready = LaneClearMenu.checkbox("Q") && Q.Mana(LaneClearMenu) && Q.IsReady();
            var Wready = LaneClearMenu.checkbox("W") && W.Mana(LaneClearMenu) && W.IsReady();
            var Eready = LaneClearMenu.checkbox("E") && E.Mana(LaneClearMenu) && E.IsReady();

            if (Wready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsKillable(W.Range));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), W.Range, W.Radius + 100, W.CastDelay, W.Speed)
                            .OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length)
                            .FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        W.Cast(location.CastPosition);
                    }
                }
            }

            if (Qready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsKillable(Q.Range + 50));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(minions.Cast<Obj_AI_Base>().ToArray(), Q.Range, Q.Radius + 50, Q.CastDelay, Q.Speed)
                            .OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length)
                            .FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        Q.Cast(location.CastPosition);
                    }
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsKillable(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        public override void JungleClear()
        {
            if (IsCastingR)
            {
                return;
            }

            var Qready = JungleClearMenu.checkbox("Q") && Q.Mana(JungleClearMenu) && Q.IsReady();
            var Wready = JungleClearMenu.checkbox("W") && W.Mana(JungleClearMenu) && W.IsReady();
            var Eready = JungleClearMenu.checkbox("E") && E.Mana(JungleClearMenu) && E.IsReady();

            if (Wready)
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(e => e.MaxHealth).FirstOrDefault(m => m.IsKillable() && m.IsKillable(W.Range));
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }

            if (Qready)
            {
                var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(e => e.MaxHealth).FirstOrDefault(m => m.IsKillable() && m.IsKillable(Q.Range));
                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsKillable() && m.IsKillable(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        public override void KillSteal()
        {
            if (IsCastingR)
            {
                return;
            }

            foreach (var spell in SpellList.Where(s => s.IsReady()))
            {
                if (KillStealMenu.checkbox(spell.Slot + "ks") && spell.GetKStarget() != null)
                {
                    if (spell == R && !KillStealMenu.checkbox("DontUlt" + spell.GetKStarget().ID()))
                    {
                        spell.Cast(spell.GetKStarget());
                    }

                    spell.Cast(spell.GetKStarget());
                }

                if (spell != R)
                {
                    if (KillStealMenu.checkbox(spell.Slot + "js") && spell.GetJStarget() != null)
                    {
                        spell.Cast(spell.GetJStarget());
                    }
                }
            }
        }
    }
}
