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

    using SharpDX;

    internal class Brand
    {
        private static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        private static Menu MenuIni, Auto, Combo, Harass, LaneClear, JungleClear, KillSteal, DrawMenu, ColorMenu;

        private static Spell.Skillshot Q;

        private static Spell.Skillshot W;

        private static Spell.Targeted E;

        private static Spell.Targeted R;

        public static void Execute()
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

                MenuIni = MainMenu.AddMenu("Brand", "Brand");
                Auto = MenuIni.AddSubMenu("Auto");
                Combo = MenuIni.AddSubMenu("Combo");
                Harass = MenuIni.AddSubMenu("Harass");
                Harass.AddGroupLabel("Harass");
                LaneClear = MenuIni.AddSubMenu("LaneClear");
                LaneClear.AddGroupLabel("LaneClear");
                JungleClear = MenuIni.AddSubMenu("JungleClear");
                JungleClear.AddGroupLabel("JungleClear");
                KillSteal = MenuIni.AddSubMenu("Stealer");
                DrawMenu = MenuIni.AddSubMenu("Drawings");
                ColorMenu = MenuIni.AddSubMenu("Colors");

                foreach (var spell in SpellList.Where(s => s != E && s != R))
                {
                    MenuIni.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
                    MenuIni.AddSeparator(0);
                }

                Auto.AddGroupLabel("Auto Settings");
                Auto.Add("AutoR", new Slider("Auto R AoE hit [{0}] Targets or more", 2, 1, 6));
                Auto.Add("Gap", new CheckBox("Anti GapCloser"));
                Auto.Add("Int", new CheckBox("Auto Interrupter"));
                Auto.Add("Danger", new ComboBox("Interrupter Danger Level", 1, "High", "Medium", "Low"));
                Auto.AddSeparator(0);
                Auto.AddGroupLabel("Auto Hit Passive");
                Auto.Add("AutoQ", new CheckBox("Auto Q Dotnate Passive"));
                Auto.Add("AutoW", new CheckBox("Auto W Dotnate Passive", false));
                Auto.Add("AutoE", new CheckBox("Auto E Dotnate Passive"));
                Auto.AddSeparator(0);
                Auto.AddGroupLabel("Anti GapCloser - Spells");
                foreach (var gapspell in
                    EntityManager.Heroes.Enemies.SelectMany(enemy => Gapcloser.GapCloserList.Where(e => e.ChampName == enemy.ChampionName)))
                {
                    Auto.Add(gapspell.SpellName, new CheckBox(gapspell.SpellName + " - " + gapspell.SpellSlot));
                }

                Combo.AddGroupLabel("Combo Settings");
                Combo.Add("Q", new CheckBox("Use Q"));
                Combo.AddLabel("Extra Q Settings");
                Combo.Add("Qp", new CheckBox("Q Only for stun"));
                Combo.Add(Q.Slot + "mana", new Slider("Use Q if Mana% is more than [{0}%]", 10));
                Combo.AddSeparator(1);

                Combo.Add("W", new CheckBox("Use W"));
                Combo.AddLabel("Extra W Settings");
                Combo.Add("Wp", new CheckBox("W Only if target has brand passive", false));
                Combo.Add(W.Slot + "mana", new Slider("Use W if Mana% is more than [{0}%]", 5));
                Combo.AddSeparator(1);

                Combo.Add("E", new CheckBox("Use E"));
                Combo.AddLabel("Extra E Settings");
                Combo.Add("Ep", new CheckBox("E Only if target has brand passive", false));
                Combo.Add(E.Slot + "mana", new Slider("Use E if Mana% is more than [{0}%]", 15));
                Combo.AddSeparator(1);

                Combo.Add("RFinisher", new CheckBox("Use R Finisher"));
                Combo.Add("RAoe", new CheckBox("Use R Aoe"));
                Combo.Add("Rhit", new Slider("R AoE hit [{0}] Targets or more", 2, 1, 6));
                Combo.Add(R.Slot + "mana", new Slider("Use R if Mana% is more than [{0}%]"));

                foreach (var spell in SpellList.Where(s => s.Slot != SpellSlot.R))
                {
                    Harass.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    Harass.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    Harass.AddSeparator(1);
                    LaneClear.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    LaneClear.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    LaneClear.AddSeparator(1);
                    JungleClear.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                    JungleClear.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% is more than [{0}%]", 65));
                    JungleClear.AddSeparator(1);
                }

                KillSteal.AddGroupLabel("KillSteal");
                foreach (var spell in SpellList)
                {
                    KillSteal.Add(spell.Slot + "ks", new CheckBox("Use " + spell.Slot));
                }
                KillSteal.AddSeparator(0);
                KillSteal.AddGroupLabel("JungleSteal");
                foreach (var spell in SpellList)
                {
                    KillSteal.Add(spell.Slot + "js", new CheckBox("Use " + spell.Slot));
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

                Common.ShowNotification("KappaBrand - Loaded", 5000);

                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += Drawing_OnDraw;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            }
            catch (Exception e)
            {
                Common.Log(e.ToString());
            }
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (target == null || !Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                return;
            }

            var Eready = LaneClear.checkbox("E") && E.IsReady() && target.IsValidTarget(E.Range) && E.Mana(LaneClear);

            if (Eready && E.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
            {
                E.Cast(target);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
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

            Automated();
            KillStealLogic();
        }

        public static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var Qready = Combo.checkbox("Q") && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(Combo);

            var Wready = Combo.checkbox("W") && W.IsReady() && target.IsValidTarget(W.Range) && W.Mana(Combo);

            var Eready = Combo.checkbox("E") && E.IsReady() && target.IsValidTarget(E.Range) && E.Mana(Combo);

            var RFinisher = Combo.checkbox("RFinisher") && R.IsReady() && target.IsValidTarget(R.Range) && R.Mana(Combo);

            var RAoe = Combo.checkbox("RAoe") && R.IsReady() && target.IsValidTarget(R.Range) && R.Mana(Combo);

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

        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            var Qready = Harass.checkbox("Q") && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(Harass);

            var Wready = Harass.checkbox("W") && W.IsReady() && target.IsValidTarget(W.Range) && W.Mana(Harass);

            var Eready = Harass.checkbox("E") && E.IsReady() && target.IsValidTarget(E.Range) && E.Mana(Harass);

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

        public static void LaneClearLogic()
        {
            var target = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = LaneClear["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(LaneClear);

            var Wready = LaneClear["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && target.IsValidTarget(W.Range) && W.Mana(LaneClear);

            var Eready = LaneClear["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) && E.Mana(LaneClear);

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

        public static void JungleClearLogic()
        {
            var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().FirstOrDefault(m => m.IsValidTarget(Q.Range + 50));

            if (target == null)
            {
                return;
            }

            var Qready = JungleClear.checkbox("Q") && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Mana(JungleClear);
            var Wready = JungleClear.checkbox("W") && W.IsReady() && target.IsValidTarget(W.Range) && W.Mana(JungleClear);
            var Eready = JungleClear.checkbox("E") && E.IsReady() && target.IsValidTarget(E.Range) && E.Mana(JungleClear);

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

        public static void KillStealLogic()
        {
            foreach (var spell in SpellList)
            {
                if (KillSteal.checkbox(spell.Slot + "ks") && spell.GetKStarget() != null)
                {
                    spell.Cast(spell.GetKStarget());
                }

                if (KillSteal.checkbox(spell.Slot + "js") && spell.GetJStarget() != null)
                {
                    spell.Cast(spell.GetJStarget());
                }
            }
        }

        public static void Automated()
        {
            var targets = EntityManager.Heroes.Enemies.Where(e => e.countpassive() >= 2 && e.IsValidTarget() && e.IsKillable());

            if (targets != null)
            {
                var aiHeroClients = targets as AIHeroClient[] ?? targets.ToArray();
                if (Auto.checkbox("AutoQ"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsValidTarget(Q.Range));
                    if (target != null)
                    {
                        Q.Cast(target, Q.hitchance(MenuIni));
                    }
                }
                if (Auto.checkbox("AutoW"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsValidTarget(W.Range));
                    if (target != null)
                    {
                        W.Cast(target, W.hitchance(MenuIni));
                    }
                }
                if (Auto.checkbox("AutoE"))
                {
                    var target = aiHeroClients.FirstOrDefault(e => e.IsValidTarget(E.Range));
                    if (target != null)
                    {
                        E.Cast(target);
                    }
                }
            }

            var hits = Auto.slider("AutoR");
            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(R.Range) && e.IsKillable());
            var aoetarget = enemies.FirstOrDefault(e => Player.Instance.CountEnemeis(400) >= hits);

            if (aoetarget != null)
            {
                R.Cast(aoetarget);
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
                if (Combo.checkbox("Qp"))
                {
                    if (target.brandpassive())
                    {
                        Q.Cast(target, Q.hitchance(MenuIni));
                    }
                }
                else
                {
                    Q.Cast(target, Q.hitchance(MenuIni));
                }
            }

            if (Harassmode)
            {
                Q.Cast(target, Q.hitchance(MenuIni));
            }

            if (LaneClearmode)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                        m => Q.Slot.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay) && Q.GetPrediction(m).HitChance >= HitChance.High);
                if (minion != null)
                {
                    if (Player.Instance.GetAutoAttackDamage(minion, true) >= Prediction.Health.GetPrediction(minion, (int)Orbwalker.AttackDelay))
                    {
                        return;
                    }
                    Q.Cast(minion);
                }
            }

            if (JungleClearmode)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(m => m.MaxHealth)
                        .FirstOrDefault(m => Q.GetPrediction(m).HitChance >= HitChance.High);

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

            var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
            var pred = Prediction.Position.PredictCircularMissileAoe(
                enemies.Cast<Obj_AI_Base>().ToArray(),
                W.Range,
                W.Width + 50,
                W.CastDelay,
                W.Speed);
            var castpos =
                pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length).FirstOrDefault(p => p.CollisionObjects.Contains(target));
            if (Combomode)
            {
                if (Combo.checkbox("Wp"))
                {
                    if (castpos != null && castpos.CollisionObjects.Length > 1)
                    {
                        W.Cast(castpos.CastPosition);
                    }
                    if (target.brandpassive())
                    {
                        W.Cast(target, W.hitchance(MenuIni));
                    }
                }
                else
                {
                    W.Cast(target, W.hitchance(MenuIni));
                }
            }

            if (Harassmode)
            {
                if (castpos != null && castpos.CollisionObjects.Length > 1)
                {
                    W.Cast(castpos.CastPosition);
                }
                W.Cast(target, W.hitchance(MenuIni));
            }

            if (LaneClearmode)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    minions.ToArray(),
                    W.Width + 75,
                    (int)W.Range + 50,
                    W.CastDelay,
                    W.Speed);

                var farmpos = loc.CastPosition;

                if (farmpos != null && loc.HitNumber >= 2)
                {
                    W.Cast(farmpos);
                }
            }

            if (JungleClearmode)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(m => m.MaxHealth)
                        .Where(e => e.IsValidTarget(W.Range) && e.IsKillable());
                var loc = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    minions.ToArray(),
                    W.Width + 75,
                    (int)W.Range + 50,
                    W.CastDelay,
                    W.Speed);
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
                if (Combo.checkbox("Ep") && target.brandpassive())
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
                var minionpassive = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.brandpassive() && m.IsValidTarget(E.Range));
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
                var minionpassive = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m.brandpassive() && m.IsValidTarget(E.Range));
                if (minionpassive != null)
                {
                    foreach (var minion in minionpassive)
                    {
                        var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(e => e.IsValidTarget(E.Range) && e.IsKillable());
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
            var hits = Combo.slider("Rhit");

            if (Combomode)
            {
                if (Combo.checkbox("RAoe"))
                {
                    var AoeHit = target.CountEnemeis(400) >= hits;
                    var bestaoe =
                        EntityManager.Heroes.Enemies.OrderByDescending(e => e.CountEnemeis(400))
                            .FirstOrDefault(e => e.IsValidTarget(R.Range) && e.IsKillable() && e.CountEnemeis(400) >= hits);
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

                if (Combo.checkbox("RFinisher"))
                {
                    var pred = R.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay);
                    var health = R.Slot.GetDamage(target) >= target.TotalShieldHealth();

                    if (Q.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, Q.CastDelay))
                    {
                        return;
                    }
                    if (W.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, W.CastDelay))
                    {
                        return;
                    }
                    if (E.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, E.CastDelay))
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
            if (!sender.IsEnemy || !Auto.checkbox("Int") || sender == null || e == null)
            {
                return;
            }

            if (e.DangerLevel >= Common.danger(Auto) && sender.IsValidTarget(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender, Q.hitchance(MenuIni));
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
                                Q.Cast(sender, Q.hitchance(MenuIni));
                            }
                        }
                    }
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || !Auto.checkbox("Gap") || sender == null || e == null || e.End == Vector3.Zero
                || !e.End.IsInRange(Player.Instance, Q.Range))
            {
                return;
            }

            if (Auto.checkbox(e.SpellName) && sender.IsValidTarget(Q.Range))
            {
                if (sender.brandpassive())
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(sender, Q.hitchance(MenuIni));
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
                                Q.Cast(sender, Q.hitchance(MenuIni));
                            }
                        }
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