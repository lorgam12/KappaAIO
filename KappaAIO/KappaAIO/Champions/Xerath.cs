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
    internal class Xerath : Base
    {
        private static bool hasbought;

        private static Item Scryb { get; }

        private static Spell.Chargeable Q { get; }

        private static Spell.Skillshot W { get; }

        private static Spell.Skillshot E { get; }

        private static Spell.Skillshot R { get; }

        private static bool AttacksEnabled
        {
            get
            {
                if (IsCastingR)
                {
                    return false;
                }

                if (Q.IsCharging)
                {
                    return false;
                }

                if (Common.orbmode(Orbwalker.ActiveModes.Combo))
                {
                    return IsPassiveUp || (!Q.IsReady() && !W.IsReady() && !E.IsReady());
                }

                return true;
            }
        }

        public static bool IsPassiveUp
        {
            get
            {
                return Player.HasBuff("XerathAscended2OnHit");
            }
        }

        public static bool IsCastingR
        {
            get
            {
                return Player.HasBuff("XerathLocusOfPower2") || (Common.LastCastedSpell.Name == "XerathLocusOfPower2" && EloBuddy.SDK.Core.GameTickCount - Common.LastCastedSpell.Time < 500);
            }
        }

        public static class RCharge
        {
            public static int CastT;

            public static int Index;

            public static Vector3 Position;

            public static bool TapKeyPressed;
        }

        static Xerath()
        {
            Scryb = new Item((int)ItemId.Farsight_Alteration, 3500f);
            Q = new Spell.Chargeable(SpellSlot.Q, 750, 1500, 1500, 500, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue };
            W = new Spell.Skillshot(SpellSlot.W, 1100, SkillShotType.Circular, 250, int.MaxValue, 100) { AllowedCollisionCount = int.MaxValue };
            E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear, 250, 1600, 70) { AllowedCollisionCount = 0 };
            R = new Spell.Skillshot(SpellSlot.R, 3200, SkillShotType.Circular, 500, int.MaxValue, 120) { AllowedCollisionCount = int.MaxValue };

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("Xerath", "Xerath");
            RMenu = Menuini.AddSubMenu("R Settings");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            ComboMenu.AddGroupLabel("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            HarassMenu.AddGroupLabel("Harass Settings");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            JungleClearMenu.AddGroupLabel("JungleClear Settings");
            KillStealMenu = Menuini.AddSubMenu("Stealer");
            KillStealMenu.AddGroupLabel("Stealer Settings");
            MiscMenu = Menuini.AddSubMenu("Misc Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("Color Picker");

            foreach (var spell in SpellList)
            {
                Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
                Menuini.AddSeparator(0);
            }

            RMenu.AddGroupLabel("R Settings");
            RMenu.Add("R", new CheckBox("Use R"));
            RMenu.Add("scrybR", new CheckBox("Use Scrybing Orb while Ulting"));
            RMenu.Add("Rmode", new ComboBox("R Mode", 0, "Auto", "Custom Delays", "On Tap"));
            RMenu.Add("Rtap", new KeyBind("R Tap Key", false, KeyBind.BindTypes.HoldActive, 'S'));
            RMenu.AddGroupLabel("R Custom Delays");
            for (var i = 1; i <= 5; i++)
            {
                RMenu.Add("delay" + i, new Slider("Delay " + i, 0, 0, 1500));
            }

            RMenu.Add("Rblock", new CheckBox("Block Commands While Casting R"));
            RMenu.Add("Rnear", new CheckBox("Focus Targets Near Mouse Only"));
            RMenu.Add("Mradius", new Slider("Mouse Radius", 750, 300, 1500));

            HarassMenu.Add("toggle", new KeyBind("Auto Harass", false, KeyBind.BindTypes.PressToggle, 'H'));

            foreach (var spell in SpellList.Where(s => s != R))
            {
                ComboMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));

                HarassMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                HarassMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% > [{0}%]"));
                HarassMenu.AddSeparator(0);

                LaneClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                LaneClearMenu.Add(spell.Slot + "mode", new ComboBox(spell.Slot + " Mode", 0, "LaneClear", "LastHit", "Both"));
                LaneClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% > [{0}%]"));
                LaneClearMenu.AddSeparator(0);

                JungleClearMenu.Add(spell.Slot.ToString(), new CheckBox("Use " + spell.Slot));
                JungleClearMenu.Add(spell.Slot + "mana", new Slider("Use " + spell.Slot + " if Mana% > [{0}%]"));
                JungleClearMenu.AddSeparator(0);

                KillStealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                KillStealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
                KillStealMenu.AddSeparator(0);
            }

            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("gap", new CheckBox("E Anti-GapCloser"));
            MiscMenu.Add("int", new CheckBox("E Interrupter"));
            MiscMenu.Add("Danger", new ComboBox("Interrupter Danger Level", 1, "High", "Medium", "Low"));
            MiscMenu.Add("flee", new KeyBind("Escape with E", false, KeyBind.BindTypes.HoldActive, 'A'));
            MiscMenu.Add("Notifications", new CheckBox("Use Notifications"));
            MiscMenu.Add("autoECC", new CheckBox("Auto E On CC enemy"));
            MiscMenu.Add("scrybebuy", new CheckBox("Auto Scrybing Orb Buy"));
            MiscMenu.Add("scrybebuylevel", new Slider("Buy Orb at level [{0}]", 9, 1, 18));

            foreach (var spell in SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
            }

            DrawMenu.Add("Rmini", new CheckBox("Draw R Range (MiniMap)", false));
            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");

            foreach (var spell in SpellList)
            {
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", Color.Chartreuse));
            }

            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Player.OnIssueOrder += Player_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && MiscMenu.checkbox("gap") && rengar.IsKillable(E.Range))
                {
                    E.Cast(rengar);
                }
            }
        }

        private static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (IsCastingR && RMenu.checkbox("Rblock"))
            {
                args.Process = false;
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            args.Process = AttacksEnabled;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || e == null || !E.IsReady() || !kCore.GapMenu.checkbox(e.SpellName + sender.ID()) || e.End == Vector3.Zero || !MiscMenu.checkbox("gap"))
            {
                return;
            }

            if (e.End.IsInRange(user, 650) || sender.IsKillable(650))
            {
                E.Cast(sender);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                Common.LastCastedSpell.Name = args.SData.Name;

                Common.LastCastedSpell.Time = args.Time;

                switch (args.SData.Name)
                {
                    case "XerathLocusOfPower2":
                        RCharge.CastT = 0;
                        RCharge.Index = 0;
                        RCharge.Position = new Vector3();
                        RCharge.TapKeyPressed = false;
                        break;
                    case "XerathLocusPulse":
                        RCharge.CastT = EloBuddy.SDK.Core.GameTickCount;
                        RCharge.Index++;
                        RCharge.Position = args.End;
                        RCharge.TapKeyPressed = false;
                        break;
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || e == null || !E.IsReady() || e.DangerLevel < Common.danger(MiscMenu) || !sender.IsKillable(500) || !MiscMenu.checkbox("int"))
            {
                return;
            }

            E.Cast(sender);
        }

        public override void Combo()
        {
            UseSpells(ComboMenu.checkbox("Q"), ComboMenu.checkbox("W"), ComboMenu.checkbox("E"));
        }

        public override void Harass()
        {
            UseSpells(HarassMenu.checkbox("Q") && Q.Mana(HarassMenu), HarassMenu.checkbox("W") && W.Mana(HarassMenu), HarassMenu.checkbox("E") && E.Mana(HarassMenu));
        }

        private static void UseSpells(bool useQ, bool useW, bool useE)
        {
            var Target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);

            if (Target != null && useE && E.IsReady())
            {
                if (user.Distance(Target) < E.Range * 0.4f)
                {
                    E.Cast(Target, E.hitchance(Menuini));
                }
                else if (!useW || !W.IsReady())
                {
                    E.Cast(Target, E.hitchance(Menuini));
                }
            }

            if (Target != null && useW && W.IsReady())
            {
                W.Cast(Target, W.hitchance(Menuini));
            }

            if (useQ && Q.IsReady() && Target != null)
            {
                if (Q.IsCharging && Target.IsKillable(Q.Range - 100))
                {
                    Q.Cast(Target, Q.hitchance(Menuini));
                }
                else if (!useW || !W.IsReady() || user.Distance(Target) > W.Range)
                {
                    if(Q.hitchance(Menuini) >= HitChance.Low)
                    Q.StartCharging();
                }
            }
        }

        private static AIHeroClient GetTargetNearMouse(float distance)
        {
            AIHeroClient bestTarget = null;
            var bestRatio = 0f;
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsKillable() && (Game.CursorPos.Distance(target.ServerPosition) < distance && user.Distance(target) < R.Range))
            {
                return TargetSelector.SelectedTarget;
            }

            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                if (hero == null || !hero.IsKillable(R.Range) || Game.CursorPos.Distance(hero.ServerPosition) > distance)
                {
                    continue;
                }

                var damage = user.CalculateDamageOnUnit(hero, DamageType.Magical, 100);
                var ratio = damage / (1 + hero.Health) * TargetSelector.GetPriority(hero);

                if (ratio > bestRatio)
                {
                    bestRatio = ratio;
                    bestTarget = hero;
                }
            }

            return bestTarget;
        }

        private static void WhileCastingR()
        {
            if (!RMenu.checkbox("R"))
            {
                return;
            }

            var rMode = RMenu.combobox("Rmode");

            var rTarget = RMenu.checkbox("Rnear") ? GetTargetNearMouse(RMenu.slider("Mradius")) : TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (rTarget != null)
            {
                if (rTarget.TotalShieldHealth() - R.GetDamage(rTarget) < 0)
                {
                    if (EloBuddy.SDK.Core.GameTickCount - RCharge.CastT <= 0)
                    {
                        return;
                    }
                }

                if (RCharge.Index != 0 && rTarget.Distance(RCharge.Position) > 1000)
                {
                    if (EloBuddy.SDK.Core.GameTickCount - RCharge.CastT <= Math.Min(2500, rTarget.Distance(RCharge.Position) - 1000))
                    {
                        return;
                    }
                }

                scrybeorbuse();
                switch (rMode)
                {
                    case 0:
                        R.Cast(rTarget, R.hitchance(Menuini));
                        break;

                    case 1:
                        var delay = RMenu.slider("delay" + (RCharge.Index + 1));
                        if (EloBuddy.SDK.Core.GameTickCount - RCharge.CastT > delay)
                        {
                            R.Cast(rTarget, R.hitchance(Menuini));
                        }

                        break;

                    case 2:
                        if (RCharge.TapKeyPressed)
                        {
                            R.Cast(rTarget);
                        }

                        break;
                }
            }
        }

        public override void KillSteal()
        {
            foreach (var spell in SpellList.Where(s => s != R))
            {
                if (KillStealMenu.checkbox(spell.Slot + "ks"))
                {
                    if (spell.IsReady() && spell.GetKStarget() != null)
                    {
                        if (spell == Q)
                        {
                            if (!Q.IsCharging)
                            {
                                Q.StartCharging();
                                return;
                            }

                            if (Q.IsCharging && Q.IsInRange(Q.GetKStarget()))
                            {
                                Q.Cast(Q.GetKStarget(), Q.hitchance(Menuini));
                            }
                        }
                        else
                        {
                            spell.Cast(spell.GetKStarget());
                        }
                    }
                }

                if (KillStealMenu.checkbox(spell.Slot + "js"))
                {
                    if (spell.IsReady() && spell.GetJStarget() != null)
                    {
                        if (spell == Q)
                        {
                            if (!Q.IsCharging)
                            {
                                Q.StartCharging();
                                return;
                            }

                            if (Q.IsCharging && Q.IsInRange(Q.GetJStarget()))
                            {
                                Q.Cast(Q.GetJStarget(), Q.hitchance(Menuini));
                            }
                        }
                        else
                        {
                            spell.Cast(spell.GetJStarget());
                        }
                    }
                }
            }
        }

        public override void LaneClear()
        {
            var useQ = LaneClearMenu.checkbox("Q") && Q.IsReady() && Q.Mana(LaneClearMenu);

            var useW = LaneClearMenu.checkbox("W") && W.IsReady() && W.Mana(LaneClearMenu);

            var useE = LaneClearMenu.checkbox("E") && E.IsReady() && E.Mana(LaneClearMenu);

            var allMinionsQ = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsKillable(Q.MaximumRange));

            var allMinionsW = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsKillable(W.Range));

            var objAiMinionsQ = allMinionsQ as Obj_AI_Minion[] ?? allMinionsQ.ToArray();

            if (useQ && allMinionsQ != null)
            {
                var Qpos = EntityManager.MinionsAndMonsters.GetLineFarmLocation(objAiMinionsQ.ToArray(), Q.Width, (int)Q.MaximumRange);

                var useQi = LaneClearMenu.combobox(Q.Slot + "mode");

                if (useQi == 0 || useQi == 2)
                {
                    if (Q.IsCharging)
                    {
                        var locQ = Qpos.CastPosition;
                        if (Qpos.HitNumber >= 1)
                        {
                            Q.Cast(locQ);
                        }
                    }
                    else if (Qpos.HitNumber > 0)
                    {
                        Q.StartCharging();
                    }
                }

                if (useQi == 1 || useQi == 2)
                {
                    var minion = objAiMinionsQ.FirstOrDefault(m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay));
                    if (Q.IsCharging && minion != null)
                    {
                        Q.Cast(minion);
                    }
                    else if (minion != null)
                    {
                        Q.StartCharging();
                    }
                }
            }

            if (useW && allMinionsW != null)
            {
                var objAiMinions = allMinionsW as Obj_AI_Minion[] ?? allMinionsW.ToArray();
                var Wpos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(objAiMinions.ToArray(), W.Width, (int)W.Range, W.CastDelay, W.Speed);
                var useWi = LaneClearMenu.combobox(W.Slot + "mode");

                if (useWi == 0 || useWi == 2)
                {
                    var locW = Wpos.CastPosition;
                    if (Wpos.HitNumber >= 1)
                    {
                        W.Cast(locW);
                    }
                }

                if (useWi == 1 || useWi == 2)
                {
                    var minion = objAiMinions.FirstOrDefault(m => W.GetDamage(m) >= Prediction.Health.GetPrediction(m, W.CastDelay));
                    if (minion != null)
                    {
                        W.Cast(minion);
                    }
                }
            }

            if (useE)
            {
                var useEi = LaneClearMenu.combobox(E.Slot + "mode");
                foreach (var minion in EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsKillable(E.Range)))
                {
                    if (minion != null && (useEi == 0 || useEi == 2))
                    {
                        E.Cast(minion);
                    }

                    if (minion != null && (useEi == 1 || useEi == 2))
                    {
                        if (E.GetDamage(minion) >= Prediction.Health.GetPrediction(minion, E.CastDelay))
                        {
                            E.Cast(minion);
                        }
                    }
                }
            }
        }

        public override void JungleClear()
        {
            var useQ = JungleClearMenu.checkbox("Q") && Q.IsReady() && Q.Mana(JungleClearMenu);

            var useW = JungleClearMenu.checkbox("W") && W.IsReady() && W.Mana(JungleClearMenu);

            var useE = JungleClearMenu.checkbox("E") && E.IsReady() && E.Mana(JungleClearMenu);

            var allMinionsQ = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m != null && m.IsKillable(Q.MaximumRange));
            var allMinionsW = EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m != null && m.IsKillable(W.Range));
            var objAiMinionsQ = allMinionsQ as Obj_AI_Minion[] ?? allMinionsQ.ToArray();
            var objAiMinionsW = allMinionsW as Obj_AI_Minion[] ?? allMinionsW.ToArray();

            if (useQ && allMinionsQ != null)
            {
                var Qpos = EntityManager.MinionsAndMonsters.GetLineFarmLocation(objAiMinionsQ.ToArray(), Q.Width, (int)Q.MaximumRange);

                if (Q.IsCharging)
                {
                    var locQ = Qpos.CastPosition;
                    if (Qpos.HitNumber >= 1)
                    {
                        Q.Cast(locQ);
                    }
                }
                else if (Qpos.HitNumber > 0)
                {
                    Q.StartCharging();
                }

                var minion = objAiMinionsQ.FirstOrDefault(m => Q.GetDamage(m) >= Prediction.Health.GetPrediction(m, Q.CastDelay));
                if (Q.IsCharging && minion != null)
                {
                    Q.Cast(minion);
                }
                else if (minion != null)
                {
                    Q.StartCharging();
                }
            }

            if (useW && allMinionsW != null)
            {
                var Wpos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(objAiMinionsW.ToArray(), W.Width, (int)W.Range, W.CastDelay, W.Speed);

                var locW = Wpos.CastPosition;
                if (Wpos.HitNumber >= 1)
                {
                    W.Cast(locW);
                }

                var minion = objAiMinionsW.FirstOrDefault(m => W.GetDamage(m) >= Prediction.Health.GetPrediction(m, W.CastDelay));
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }

            if (useE)
            {
                foreach (var minion in EntityManager.MinionsAndMonsters.GetJungleMonsters().Where(m => m != null && m.IsKillable(E.Range)))
                {
                    if (E.GetDamage(minion) >= Prediction.Health.GetPrediction(minion, E.CastDelay))
                    {
                        E.Cast(minion);
                    }

                    E.Cast(minion);
                }
            }
        }

        public override void Active()
        {
            if (user.IsDead)
            {
                return;
            }

            if (MiscMenu.keybind("flee"))
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (target != null)
                {
                    E.Cast(target);
                }

                Orbwalker.OrbwalkTo(Game.CursorPos);
            }

            if (MiscMenu.checkbox("autoECC"))
            {
                var ecc = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.IsKillable(E.Range) && e.IsCC());
                if (ecc != null)
                {
                    E.Cast(ecc);
                }
            }

            ScrybingOrb();

            R.Range = (uint)(1925 + R.Level * 1200);

            if (HarassMenu.keybind("toggle"))
            {
                this.Harass();
            }

            Orbwalker.DisableAttacking = IsCastingR;
            Orbwalker.DisableMovement = IsCastingR;
            RCharge.TapKeyPressed = RMenu.keybind("Rtap");

            if (IsCastingR)
            {
                WhileCastingR();
                return;
            }

            if (R.IsReady() && MiscMenu.checkbox("Notifications") && Environment.TickCount - Common.lastNotification > 5000)
            {
                foreach (var enemy in
                    EntityManager.Heroes.Enemies.Where(h => h != null && h.IsKillable() && R.GetDamage(h) * 3 > h.Health))
                {
                    Common.ShowNotification(enemy.ChampionName + ": is killable R!!!", 4000);
                    Common.lastNotification = Environment.TickCount;
                }
            }
        }

        public static void scrybeorbuse()
        {
            if (!RMenu.checkbox("scrybR"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Scryb.IsOwned(user)
                && (target.IsDashing() || target.Distance(R.GetPrediction(target).CastPosition) > 150 || NavMesh.IsWallOfGrass(Prediction.Position.PredictUnitPosition(target, 150).To3D(), 50)))
            {
                Scryb.Cast(R.GetPrediction(target).CastPosition);
            }
        }

        public static void ScrybingOrb()
        {
            var level = MiscMenu.slider("scrybebuylevel");
            var buy = MiscMenu.checkbox("scrybebuy");

            if (!buy)
            {
                return;
            }

            if (hasbought)
            {
                return;
            }

            if (!Scryb.IsOwned(user) && user.IsInShopRange() && user.Level >= level)
            {
                Scryb.Buy();
                hasbought = true;
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (user.IsDead)
            {
                return;
            }

            /*
            var Rcirclemap = DrawMenu.checkbox("Rmini");

            if (Rcirclemap && R.IsReady())
            {
                DrawingsManager.DrawCricleMinimap(Color.White, R.Range, user.ServerPosition, 2, 20);
            }
            */
        }

        public override void Draw()
        {
            if (user.IsDead)
            {
                return;
            }

            if (IsCastingR)
            {
                if (RMenu.checkbox("Rnear"))
                {
                    Circle.Draw(SharpDX.Color.Red, RMenu.slider("Mradius"), Game.CursorPos);
                }
            }

            if (MiscMenu.checkbox("Notifications") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t != null && t.IsKillable())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage * 5 > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, Color.Red, (int)(t.Health / rDamage) + " x Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        DrawingsManager.drawLine(t.Position, user.Position, 10, Color.Yellow);
                    }
                }
            }
        }
    }
}
