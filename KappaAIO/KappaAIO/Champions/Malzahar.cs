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

    using Core;
    using Core.Managers;

    internal class Malzahar
    {
        private static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        private static Spell.Skillshot Q { get; set; }

        private static Spell.Skillshot W { get; set; }

        private static Spell.Targeted E { get; set; }

        private static Spell.Targeted R { get; set; }

        private static Menu menuIni, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, MiscMenu, DrawMenu, ColorMenu;

        private static bool IsCastingR;

        public static void Execute()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Circular, 250, 500, 90);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular, 250, int.MaxValue, 80);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Targeted(SpellSlot.R, 700);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            menuIni = MainMenu.AddMenu("Malzahar", "Malzahar");
            ComboMenu = menuIni.AddSubMenu("Combo Settings");
            HarassMenu = menuIni.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass");
            LaneClearMenu = menuIni.AddSubMenu("LaneClear Settings");
            LaneClearMenu.AddGroupLabel("LaneClear");
            JungleClearMenu = menuIni.AddSubMenu("JungleClear Settings");
            JungleClearMenu.AddGroupLabel("JungleClear");
            KillStealMenu = menuIni.AddSubMenu("KillSteal Settings");
            KillStealMenu.AddGroupLabel("Stealer");
            MiscMenu = menuIni.AddSubMenu("Misc Settings");
            DrawMenu = menuIni.AddSubMenu("Drawings Settings");
            DrawMenu.AddGroupLabel("Drawings");
            ColorMenu = menuIni.AddSubMenu("ColorPicker");
            ColorMenu.AddGroupLabel("ColorPicker");

            foreach (var spell in SpellList.Where(s => s == Q))
            {
                menuIni.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
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
                CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                ComboMenu.Add("DontUlt" + enemy.BaseSkinName, cb);
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
                CheckBox cb = new CheckBox(enemy.BaseSkinName) { CurrentValue = false };
                KillStealMenu.Add("DontUlt" + enemy.BaseSkinName, cb);
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

            Common.ShowNotification("KappaMalzahar - Loaded", 5000);

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Player.OnIssueOrder += Player_OnIssueOrder;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
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
                if (Eready && E.Slot.GetDamage(target) >= args.RemainingHealth)
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

            if (MiscMenu.checkbox("Qgap") && (e.End.IsInRange(Player.Instance, Q.Range) || sender.IsValidTarget(Q.Range)))
            {
                Q.Cast(sender, Q.hitchance(menuIni));
            }

            if (MiscMenu.checkbox("blockR") && Player.Instance.IsUnderEnemyturret())
            {
                return;
            }

            if (MiscMenu.checkbox("Rgap") && (e.End.IsInRange(Player.Instance, R.Range) || sender.IsValidTarget(R.Range)))
            {
                R.Cast(sender);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && MiscMenu.checkbox("Rgap") && rengar.IsValidTarget(R.Range))
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
                if (MiscMenu.checkbox("Qint") && sender.IsValidTarget(Q.Range))
                {
                    Q.Cast(sender, Q.hitchance(menuIni));
                }

                if (MiscMenu.checkbox("blockR") && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                if (MiscMenu.checkbox("Rint") && sender.IsValidTarget(R.Range))
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

        private static void Game_OnUpdate(EventArgs args)
        {
            IsCastingR = Player.Instance.Buffs.FirstOrDefault(b => b.Name.ToLower().Contains("malzaharrsound")) != null;
            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;

            if (IsCastingR)
            {
                return;
            }

            if (Common.orbmode(Orbwalker.ActiveModes.Combo))
            {
                ComboLogic();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.Harass))
            {
                HarassLogic();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClearLogic();
            }
            if (Common.orbmode(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClearLogic();
            }
            Rlogic();
            KillStealLogic();
        }

        private static void Rlogic()
        {
            if (MiscMenu.checkbox("RTurret") && R.IsReady())
            {
                if (MiscMenu.checkbox("blockR") && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                var targets =
                    EntityManager.Heroes.Enemies.Where(
                        e => e.IsUnderTurret() && !e.IsUnderHisturret() && !e.IsUnderEnemyturret() && e.IsValidTarget(R.Range));
                if (targets != null)
                {
                    foreach (var target in targets.Where(target => target != null))
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void ComboLogic()
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

            var Qready = ComboMenu.checkbox("Q") && Q.IsReady() && target.IsValidTarget(Q.Range);

            var Wready = ComboMenu.checkbox("W") && W.IsReady() && target.IsValidTarget(W.Range);

            var Eready = ComboMenu.checkbox("E") && E.IsReady() && target.IsValidTarget(E.Range);

            var Rfinready = ComboMenu.checkbox("RFinisher") && R.IsReady() && target.IsValidTarget(R.Range);

            var Rcomready = ComboMenu.checkbox("RCombo") && R.IsReady() && target.IsValidTarget(R.Range);

            var RTurret = ComboMenu.checkbox("RTurret") && R.IsReady() && target.IsValidTarget(R.Range) && target.IsUnderTurret()
                          && !target.IsUnderHisturret() && !target.IsUnderEnemyturret();

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready)
            {
                Q.Cast(target, Q.hitchance(menuIni));
            }

            if (Eready)
            {
                E.Cast(target);
            }

            if (!ComboMenu.checkbox("DontUlt" + target.BaseSkinName))
            {
                if (MiscMenu.checkbox("blockR") && Player.Instance.IsUnderEnemyturret())
                {
                    return;
                }

                if (Rcomready && target.TotalDamage(DamageType.Magical) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (Rfinready && R.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                {
                    R.Cast(target);
                }

                if (RTurret)
                {
                    R.Cast(target);
                }
            }
        }

        private static void HarassLogic()
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

            var Qready = HarassMenu.checkbox("Q") && Q.Mana(HarassMenu) && Q.IsReady() && target.IsValidTarget(Q.Range);

            var Wready = HarassMenu.checkbox("W") && W.Mana(HarassMenu) && W.IsReady();

            var Eready = HarassMenu.checkbox("E") && E.Mana(HarassMenu) && E.IsReady() && target.IsValidTarget(E.Range);

            if (Wready)
            {
                W.Cast(target);
            }

            if (Qready)
            {
                Q.Cast(target, Q.hitchance(menuIni));
            }

            if (Eready)
            {
                E.Cast(target);
            }
        }

        private static void LaneClearLogic()
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
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(W.Range));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(
                            minions.Cast<Obj_AI_Base>().ToArray(),
                            W.Range,
                            W.Radius + 100,
                            W.CastDelay,
                            W.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        W.Cast(location.CastPosition);
                    }
                }
            }

            if (Qready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(Q.Range + 50));

                if (minions != null)
                {
                    var location =
                        Prediction.Position.PredictCircularMissileAoe(
                            minions.Cast<Obj_AI_Base>().ToArray(),
                            Q.Range,
                            Q.Radius + 50,
                            Q.CastDelay,
                            Q.Speed).OrderByDescending(r => r.GetCollisionObjects<Obj_AI_Minion>().Length).FirstOrDefault();

                    if (location != null && location.CollisionObjects.Length >= 2)
                    {
                        Q.Cast(location.CastPosition);
                    }
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable() && m.IsValidTarget(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.Slot.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void JungleClearLogic()
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
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(e => e.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable() && m.IsValidTarget(W.Range));
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }

            if (Qready)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(e => e.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable() && m.IsValidTarget(Q.Range));
                if (minion != null)
                {
                    Q.Cast(minion);
                }
            }

            if (Eready)
            {
                var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.IsKillable() && m.IsValidTarget(E.Range));

                if (minions != null)
                {
                    foreach (var minion in minions.Where(minion => E.Slot.GetDamage(minion) >= minion?.TotalShield()))
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            if (IsCastingR)
            {
                return;
            }
            foreach (var spell in SpellList.Where(s => s.IsReady()))
            {
                if (KillStealMenu.checkbox(spell.Slot + "ks") && spell.GetKStarget() != null)
                {
                    if (spell == R && !KillStealMenu.checkbox("DontUlt" + spell.GetKStarget().BaseSkinName))
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

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                var color = ColorMenu.Color(spell.Slot.ToString());
                spell.SpellRange(color, DrawMenu.checkbox(spell.Slot.ToString()));
            }

            DrawingsManager.DrawTotalDamage(DamageType.Magical, DrawMenu.checkbox("damage"));
        }
    }
}