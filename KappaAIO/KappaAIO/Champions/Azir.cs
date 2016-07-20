using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;
using SharpDX;

namespace KappaAIO.Champions
{
    internal class Azir : Base
    {
        private static Spell.Skillshot Q { get; }

        private static Spell.Skillshot W { get; }

        private static Spell.Skillshot E { get; }

        private static Spell.Skillshot R { get; }

        public static Vector3 Rpos(Obj_AI_Base target)
        {
            Common.ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(R.Range)).FirstOrDefault(a => a.IsKillable() && a.IsValidTarget(1250) && !a.IsMe);
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(s => s.IsKillable(1250));
            if (Common.tower != null)
            {
                return user.ServerPosition.Extend(Common.tower.ServerPosition, R.Range).To3D();
            }

            return Common.ally != null ? user.ServerPosition.Extend(Common.ally.ServerPosition, R.Range).To3D() : R.GetPrediction(target).CastPosition;
        }

        internal static bool Ehit(Obj_AI_Base target)
        {
            return
                Orbwalker.AzirSoldiers.Select(soldier => new Geometry.Polygon.Rectangle(ObjectManager.Player.ServerPosition, soldier.ServerPosition, target.BoundingRadius + 35))
                    .Any(rectangle => rectangle.IsInside(target));
        }

        internal static bool Ehit(Obj_AI_Base target, Vector3 pos)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, E.CastDelay);
            return new Geometry.Polygon.Rectangle(ObjectManager.Player.ServerPosition, pos, target.BoundingRadius + 35).IsInside(targetpos);
        }

        static Azir()
        {
            try
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1000, 65) { AllowedCollisionCount = int.MaxValue };
                W = new Spell.Skillshot(SpellSlot.W, 525, SkillShotType.Circular);
                E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 1200, 80) { AllowedCollisionCount = int.MaxValue };
                R = new Spell.Skillshot(SpellSlot.R, 350, SkillShotType.Linear, 500, 1000, 220) { AllowedCollisionCount = int.MaxValue };

                if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerFlash")) != null)
                {
                    Flash = new Spell.Skillshot(user.GetSpellSlotFromName("SummonerFlash"), 450, SkillShotType.Circular);
                }

                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);

                Menuini = MainMenu.AddMenu("KappAzir", "KappAzir");
                AutoMenu = Menuini.AddSubMenu("Auto Settings");
                JumperMenu = Menuini.AddSubMenu("Jumper Settings");
                ComboMenu = Menuini.AddSubMenu("Combo Settings");
                HarassMenu = Menuini.AddSubMenu("Harass Settings");
                LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
                JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
                KillStealMenu = Menuini.AddSubMenu("KillSteal Settings");
                DrawMenu = Menuini.AddSubMenu("Drawings Settings");
                ColorMenu = Menuini.AddSubMenu("ColorPicker");

                foreach (var spell in SpellList.Where(s => s != E))
                {
                    Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
                    Menuini.AddSeparator(0);
                }

                AutoMenu.AddGroupLabel("Settings");
                AutoMenu.Add("gap", new CheckBox("Anti-GapCloser"));
                AutoMenu.Add("int", new CheckBox("Interrupter"));
                AutoMenu.Add("Danger", new ComboBox("Interrupter DangerLevel", 1, "High", "Medium", "Low"));
                AutoMenu.AddGroupLabel("Turret Settings");
                AutoMenu.Add("tower", new CheckBox("Create Turrets"));
                AutoMenu.Add("Tenemy", new Slider("Create Turret If [{0}] Enemies Near", 3, 1, 6));
                if (EntityManager.Heroes.Enemies.Any(e => e.Hero == Champion.Rengar))
                {
                    AutoMenu.Add("rengar", new CheckBox("Anti-Rengar Leap"));
                }

                JumperMenu.Add("jump", new KeyBind("WEQ Flee Key", false, KeyBind.BindTypes.HoldActive, 'A'));
                JumperMenu.Add("normal", new KeyBind("Normal Insec Key", false, KeyBind.BindTypes.HoldActive, 'S'));
                JumperMenu.Add("new", new KeyBind("New Insec Key", false, KeyBind.BindTypes.HoldActive, 'Z'));
                JumperMenu.Add("flash", new CheckBox("Use Flash for Possible AoE"));
                JumperMenu.Add("delay", new Slider("Delay EQ", 200, 0, 500));
                JumperMenu.Add("range", new Slider("Check for soldiers Range", 800, 0, 1000));

                ComboMenu.AddGroupLabel("Combo Settings");
                ComboMenu.AddGroupLabel("Q Settings");
                ComboMenu.Add("Q", new CheckBox("Use Q"));
                ComboMenu.Add("WQ", new CheckBox("Use W > Q"));
                ComboMenu.Add("Qaoe", new CheckBox("Use Q Aoe", false));
                ComboMenu.Add("QS", new Slider("Soldiers To Use Q", 1, 1, 3));
                ComboMenu.AddSeparator(0);
                ComboMenu.AddGroupLabel("W Settings");
                ComboMenu.Add("W", new CheckBox("Use W"));
                ComboMenu.Add("Wsave", new CheckBox("Save 1 W Stack", false));
                ComboMenu.Add("WS", new Slider("Soldier Limit To Create", 3, 1, 3));
                ComboMenu.AddSeparator(0);
                ComboMenu.AddGroupLabel("E Settings");
                ComboMenu.Add("E", new CheckBox("Use E"));
                ComboMenu.Add("Ekill", new CheckBox("E Killable Enemy Only"));
                ComboMenu.Add("Edive", new CheckBox("E Dive Turrets", false));
                ComboMenu.Add("EHP", new Slider("Only E if my HP is more than [{0}%]", 50));
                ComboMenu.Add("Esafe", new Slider("Dont E Into [{0}] Enemies", 3, 1, 6));
                ComboMenu.AddSeparator(0);
                ComboMenu.AddGroupLabel("R Settings");
                ComboMenu.Add("R", new CheckBox("Use R"));
                ComboMenu.Add("Rkill", new CheckBox("R Finisher"));
                ComboMenu.Add("insec", new CheckBox("Try to insec in Combo"));
                ComboMenu.Add("Raoe", new Slider("R AoE Hit [{0}] Enemies", 3, 1, 6));
                ComboMenu.Add("Rsave", new CheckBox("R Save Self"));
                ComboMenu.Add("RHP", new Slider("Push Enemy If my health is less than [{0}%]", 35));

                HarassMenu.AddGroupLabel("Harass Settings");
                HarassMenu.Add("toggle", new KeyBind("Auto Harass Key", false, KeyBind.BindTypes.PressToggle, 'H'));
                HarassMenu.AddSeparator(0);
                HarassMenu.AddGroupLabel("Q Settings");
                HarassMenu.Add("Q", new CheckBox("Use Q"));
                HarassMenu.Add("WQ", new CheckBox("Use W > Q"));
                HarassMenu.Add("QS", new Slider("Soldiers To Use Q", 1, 1, 3));
                HarassMenu.Add(Q.Slot + "mana", new Slider("Stop using Q if Mana < [{0}%]", 65));
                HarassMenu.AddSeparator(0);
                HarassMenu.AddGroupLabel("W Settings");
                HarassMenu.Add("W", new CheckBox("Use W"));
                HarassMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
                HarassMenu.Add("WS", new Slider("Soldier Limit To Create", 3, 1, 3));
                HarassMenu.Add(W.Slot + "mana", new Slider("Stop using W if Mana < [{0}%]", 65));
                HarassMenu.AddSeparator(0);
                HarassMenu.AddGroupLabel("E Settings");
                HarassMenu.Add("E", new CheckBox("Use E"));
                HarassMenu.Add("Edive", new CheckBox("E Dive Turrets", false));
                HarassMenu.Add("EHP", new Slider("Only E if my HP is more than [{0}%]", 50));
                HarassMenu.Add("Esafe", new Slider("Dont E Into [{0}] Enemies", 3, 1, 6));
                HarassMenu.Add(E.Slot + "mana", new Slider("Stop using E if Mana < [{0}%]", 65));

                LaneClearMenu.AddGroupLabel("LaneClear Settings");
                LaneClearMenu.Add("Q", new CheckBox("Use Q"));
                LaneClearMenu.Add(Q.Slot + "mana", new Slider("Stop using Q if Mana < [{0}%]", 65));
                LaneClearMenu.Add("W", new CheckBox("Use W"));
                LaneClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
                LaneClearMenu.Add(W.Slot + "mana", new Slider("Stop using W if Mana < [{0}%]", 65));

                JungleClearMenu.AddGroupLabel("JungleClear Settings");
                JungleClearMenu.Add("Q", new CheckBox("Use Q"));
                JungleClearMenu.Add(Q.Slot + "mana", new Slider("Stop using Q if Mana < [{0}%]", 65));
                JungleClearMenu.Add("W", new CheckBox("Use W"));
                JungleClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
                JungleClearMenu.Add(W.Slot + "mana", new Slider("Stop using W if Mana < [{0}%]", 65));

                KillStealMenu.AddGroupLabel("Stealer Settings");
                foreach (var spell in SpellList.Where(s => s != W && s != E))
                {
                    KillStealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                    KillStealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
                }

                DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
                DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");
                DrawMenu.AddSeparator(1);
                foreach (var spell in SpellList)
                {
                    DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                    ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
                }

                DrawMenu.Add("insec", new CheckBox("Draw Insec Helpers"));

                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                GameObject.OnCreate += GameObject_OnCreate;
                Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            }
            catch (Exception e)
            {
                Common.Logger.Error(e.ToString());
            }
        }

        public override void Active()
        {
            updatespells();

            if (NewInsec)
            {
                var rpos = user.ServerPosition.Extend(insectpos(), R.Range).To3D();

                var qtime = Game.Time - insecqtime;
                if ((qtime > 0.1f && qtime < 0.1) || TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsKillable(R.Range - 75))
                {
                    R.Cast(rpos);
                }
            }

            if (HarassMenu.keybind("toggle"))
            {
                this.Harass();
            }

            if (JumperMenu.keybind("jump"))
            {
                Jump(Game.CursorPos);
            }

            if (JumperMenu.keybind("normal") && TargetSelector.SelectedTarget != null)
            {
                Normal(TargetSelector.SelectedTarget);
            }

            if (JumperMenu.keybind("new"))
            {
                New();
            }

            if (AutoMenu.checkbox("tower"))
            {
                var azirtower = ObjectManager.Get<GameObject>().FirstOrDefault(o => o != null && o.Name.ToLower().Contains("towerclicker") && user.Distance(o) < 500);
                if (azirtower != null && azirtower.CountEnemeis(800) >= AutoMenu.slider("Tenemy"))
                {
                    Player.UseObject(azirtower);
                }
            }

            NormalInsec = JumperMenu.keybind("normal");
            NewInsec = JumperMenu.keybind("new");
        }

        public override void Draw()
        {
            // Insec Helper
            var target = TargetSelector.SelectedTarget;
            var colors = System.Drawing.Color.White;

            if (DrawMenu.checkbox("insec") && (NormalInsec || NewInsec))
            {
                float x;
                float y;
                if (target == null)
                {
                    x = Game.CursorPos.WorldToScreen().X;
                    y = Game.CursorPos.WorldToScreen().Y - 15;
                    Drawing.DrawText(x, y, colors, "SELECT A TARGET", 5);
                }
                else
                {
                    x = target.ServerPosition.WorldToScreen().X;
                    y = target.ServerPosition.WorldToScreen().Y;
                    Drawing.DrawText(x, y, colors, "SELECTED TARGET", 5);
                    Circle.Draw(Color.Red, target.BoundingRadius, target.ServerPosition);
                    if (NewInsec && !Orbwalker.AzirSoldiers.Any(s => s.IsInRange(target, 420) && s.IsAlly))
                    {
                        x = Game.CursorPos.WorldToScreen().X;
                        y = Game.CursorPos.WorldToScreen().Y - 15;
                        Drawing.DrawText(x, y, colors, "CREATE A SOLDIER NEAR THE TARGET FIRST", 5);
                    }
                }
                if (target != null)
                {
                    var insecpos = insectpos(target);
                    if (insecpos == Vector3.Zero)
                    {
                        x = Game.CursorPos.WorldToScreen().X;
                        y = Game.CursorPos.WorldToScreen().Y - 15;
                        Drawing.DrawText(x, y, colors, "Cant Detect Insec Position", 5);
                    }
                    else
                    {
                        x = insecpos.WorldToScreen().X;
                        y = insecpos.WorldToScreen().Y;
                        Drawing.DrawText(x, y, colors, "Insec Position", 5);
                    }

                    if (target != null && insecpos != Vector3.Zero)
                    {
                        var pos = target.ServerPosition.Extend(insecpos, -200).To3D();
                        var rpos = user.ServerPosition.Extend(insecpos, R.Range).To3D();
                        Circle.Draw(Color.White, 100, rpos);
                        Circle.Draw(Color.White, 100, pos);
                        Circle.Draw(Color.White, 200, insecpos);
                        Line.DrawLine(colors, pos, rpos);
                    }
                }
            }
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (target == null || args.Target == null || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) < 1)
            {
                return;
            }

            var orbtarget = args.Target as Obj_AI_Base;
            foreach (var sold in Orbwalker.ValidAzirSoldiers)
            {
                if (sold != null)
                {
                    var sold1 = sold;
                    var minion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsInRange(sold1, sold1.GetAutoAttackRange()) && m.IsKillable());
                    if (minion != null && minion != orbtarget)
                    {
                        var killable = user.GetAutoAttackDamage(orbtarget, true) >= Prediction.Health.GetPrediction(orbtarget, (int)user.AttackDelay);
                        if (!killable)
                        {
                            args.Process = false;
                            Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        }
                    }
                }
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W)
            {
                //Orbwalker.ResetAutoAttack();
            }

            Common.LastCastedSpell.Spell = args.Slot;
            Common.LastCastedSpell.Time = Game.Time;
        }

        public static void updatespells()
        {
            R.Width = 107 * (R.Level - 1) < 200 ? 220 : (107 * (R.Level - 1)) + 5;

            var count = Orbwalker.AzirSoldiers.Count(s => s.IsAlly);
            Q.Width = count != 0 ? 65 * count : 65;
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsKillable(300) || e.End == Vector3.Zero || !e.End.IsInRange(user, 300) || !kCore.GapMenu.checkbox(e.SpellName + sender.ID())
                || !AutoMenu.checkbox("Gap") || !R.IsReady())
            {
                return;
            }

            R.Cast(sender);
        }

        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsKillable(R.Range) || e.DangerLevel < Common.danger(AutoMenu) || !AutoMenu.checkbox("int") || !R.IsReady())
            {
                return;
            }

            R.Cast(sender);
        }

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Rengar_LeapSound.troy" && sender != null)
            {
                var rengar = EntityManager.Heroes.Enemies.FirstOrDefault(e => e.Hero == Champion.Rengar);
                if (rengar != null && R.IsReady() && AutoMenu.checkbox("gap") && AutoMenu.checkbox("rengar") && rengar.IsKillable(R.Range))
                {
                    R.Cast(rengar);
                }
            }
        }

        public override void Combo()
        {
            var menu = ComboMenu;
            var QS = Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) >= menu.slider("QS");
            var Qc = QS && menu.checkbox("Q") && Q.IsReady();
            var Qaoe = menu.checkbox("Qaoe");
            var Wc = menu.checkbox("W") && W.IsReady();
            var Ec = menu.checkbox("E") && E.IsReady();
            var Rc = menu.checkbox("R") && R.IsReady();
            var Wsave = menu.checkbox("Wsave") && W.Handle.Ammo < 2;
            var Wlimit = menu.slider("WS") >= Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly);
            var target = TargetSelector.GetTarget(Q.Range + 25, DamageType.Magical);

            if (target == null || !target.IsKillable(Q.Range))
            {
                return;
            }

            if (Wc && Wlimit)
            {
                if (Wsave)
                {
                    return;
                }

                if (target.IsKillable(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }

                if (menu.checkbox("Q") && !target.IsKillable(W.Range) && Q.IsReady() && user.Mana > Q.Mana() + W.Mana() && target.IsKillable(Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = user.Position.Extend(target.Position, W.Range);
                    W.Cast(p.To3D());
                }
            }

            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) == 0)
            {
                return;
            }

            if (Qc)
            {
                var predQ = Q.GetPrediction(target);
                Q.Cast(target, Q.hitchance(Menuini), target.IsCC());

                if (Ec && user.Mana > Q.Mana() + E.Mana() && Ehit(target, predQ.CastPosition) && predQ.HitChance >= HitChance.Medium)
                {
                    if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= user.HealthPercent) || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
                    {
                        return;
                    }

                    if (E.Cast(predQ.CastPosition))
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }

                if (Qaoe)
                {
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(Q.Range) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(enemies.Cast<Obj_AI_Base>().ToArray(), Q.Range, (int)Orbwalker.AzirSoldierAutoAttackRange, Q.CastDelay, Q.Speed);
                    var castpos = pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length).FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > 1)
                    {
                        Q.Cast(castpos.CastPosition);
                    }
                }
            }

            if (Ec && Ehit(target))
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= user.HealthPercent)
                    || (menu.checkbox("Edive") && target.IsUnderEnemyturret() && target.IsUnderHisturret()))
                {
                    return;
                }

                var time = user.Distance(target) / E.Speed;
                var killable = target.TotalDamage(SpellList) >= Prediction.Health.GetPrediction(target, (int)time);
                if (menu.checkbox("Ekill") && killable && user.Mana >= Common.Mana())
                {
                    E.Cast(target.ServerPosition);
                }
                else
                {
                    E.Cast(target.ServerPosition);
                }
            }

            if (Rc)
            {
                var Raoe = user.CountEnemeis(R.Range) >= menu.slider("Raoe") || user.CountEnemeis(R.Width) >= menu.slider("Raoe");

                if (target.IsKillable(R.Range - 25))
                {
                    if ((menu.checkbox("Rkill") && R.GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay)) || (menu.checkbox("Rsave") && menu.slider("RHP") >= user.HealthPercent)
                        || Raoe)
                    {
                        R.Cast(Rpos(target));
                    }
                }
            }

            if (menu.checkbox("insec") && target.IsKillable() && !target.IsKillable(R.Range))
            {
                if (target.CountEnemeis(750) >= menu.slider("Esafe") || menu.slider("EHP") >= user.HealthPercent)
                {
                    return;
                }

                var time = R.CastDelay + Q.CastDelay + E.CastDelay;
                var kill = target.TotalDamage(SpellList) + 100 > Prediction.Health.GetPrediction(target, time);
                if (kill)
                {
                    Normal(target);
                }
            }
        }

        public override void Harass()
        {
            var menu = HarassMenu;
            var Qmana = Q.Mana(menu);
            var Wmana = W.Mana(menu);
            var Emana = E.Mana(menu);
            var QS = Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) >= menu.slider("QS");
            var Qc = QS && Qmana && menu.checkbox("Q") && Q.IsReady();
            var Wc = Wmana && menu.checkbox("W") && W.IsReady();
            var Ec = Emana && menu.checkbox("E") && E.IsReady();
            var Wsave = menu.checkbox("Wsave") && W.Handle.Ammo > 1;
            var Wlimit = menu.slider("WS") >= Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly);
            var target = TargetSelector.GetTarget(Q.Range + 25, DamageType.Magical);

            if (target == null || !target.IsKillable(Q.Range))
            {
                return;
            }

            if (Wc && Wsave && Wlimit)
            {
                if (target.IsKillable(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }

                if (menu.checkbox("Q") && !target.IsKillable(W.Range) && Q.IsReady() && user.Mana > Q.Mana() + W.Mana() && target.IsKillable(Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = user.Position.Extend(target.Position, W.Range);
                    W.Cast(p.To3D());
                }
            }

            if (Orbwalker.AzirSoldiers.Count(s => s.IsAlly) == 0)
            {
                return;
            }

            if (Qc)
            {
                if (target.IsCC())
                {
                    Q.Cast(target);
                }

                Q.Cast(target, Q.hitchance(Menuini));
            }

            if (Ec && Ehit(target))
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= user.HealthPercent) || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
                {
                    return;
                }

                E.Cast(target.ServerPosition);
            }
        }

        public static bool NormalInsec;

        public static bool NewInsec;

        public static float insecqtime;

        public static void Normal(Obj_AI_Base target, bool Combo = false)
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (target == null || insectpos(target) == null || Common.Mana() > user.Mana || !R.IsReady())
            {
                return;
            }

            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var rpos = user.ServerPosition.Extend(insectpos(target), R.Range).To3D();

            if (target.IsKillable(R.Range))
            {
                if (JumperMenu.checkbox("flash") && Flash != null)
                {
                    var flashrange = Flash.Range + 250;
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsKillable(flashrange) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(enemies.Cast<Obj_AI_Base>().ToArray(), flashrange, R.Width + 25, R.CastDelay, R.Speed);
                    var castpos = pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length).FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > user.CountEnemeis(R.Range))
                    {
                        Flash.Cast(castpos.CastPosition);
                    }
                }

                R.Cast(rpos);
            }
            else
            {
                if (Q.IsInRange(target))
                {
                    Jump(insecpos);
                }
            }

            Orbwalker.OrbwalkTo(insecpos);
        }

        public static void New()
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            var target = TargetSelector.SelectedTarget;
            if (target == null || insectpos(target) == null || !R.IsReady() || user.Mana < Common.Mana() || !target.IsKillable() || NormalInsec)
            {
                return;
            }

            var delay = JumperMenu.slider("delay");
            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var qpos = user.ServerPosition.Extend(insectpos(target), Q.Range - 200).To3D();
            var soldier = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly && s.IsInRange(target, 200));
            var ready = E.IsReady() && Q.IsReady() && user.Mana > Q.Mana() + E.Mana();

            if (ready && soldier != null)
            {
                EloBuddy.SDK.Core.DelayAction(
                    () =>
                        {
                            if (E.Cast(target.ServerPosition))
                            {
                                EloBuddy.SDK.Core.DelayAction(() => Q.Cast(qpos), delay);
                                insecqtime = Game.Time;
                            }
                        },
                    100);
            }

            Orbwalker.OrbwalkTo(insecpos);
        }

        public static Vector3 insectpos(Obj_AI_Base target)
        {
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(t => !t.IsDead && t.IsInRange(target, 1250));
            Common.ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(R.Range)).FirstOrDefault(a => !a.IsMe && a.IsValidTarget() && a.IsInRange(target, 1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : user.ServerPosition;
        }

        public static Vector3 insectpos()
        {
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(t => t.IsKillable(1250));
            Common.ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.CountEnemeis(R.Range)).FirstOrDefault(a => !a.IsMe && a.IsValidTarget(1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : user.ServerPosition;
        }

        public static void Jump(Vector3 pos)
        {
            var delay = JumperMenu.slider("delay");
            var range = JumperMenu.slider("range");
            var qpos = user.ServerPosition.Extend(pos, Q.Range - 100).To3D();
            var wpos = user.ServerPosition.Extend(pos, W.Range).To3D();
            var epos = Orbwalker.AzirSoldiers.OrderBy(s => s.Distance(pos)).FirstOrDefault(s => s.IsAlly);
            var ready = E.IsReady() && Q.IsReady() && user.Mana > Q.Mana() + E.Mana() + W.Mana();

            if (ready && Orbwalker.AzirSoldiers.Count(s => s.IsAlly && s.IsInRange(user, range)) < 1)
            {
                if (Common.LastCastedSpell.Spell == SpellSlot.E)
                {
                    return;
                }

                W.Cast(wpos);
            }
            if (ready && epos != null)
            {
                EloBuddy.SDK.Core.DelayAction(
                    () =>
                    {
                        if (E.Cast(epos.ServerPosition))
                        {
                            EloBuddy.SDK.Core.DelayAction(() => Q.Cast(qpos), delay);
                        }
                    },
                    250);
            }

            if (Common.LastCastedSpell.Spell == SpellSlot.E)
            {
                var timer = Game.Time - Common.LastCastedSpell.Time;
                if (timer - delay < 0.1f && Q.IsReady())
                {
                    Q.Cast(qpos);
                }
            }
        }

        public override void JungleClear()
        {
            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());

            foreach (var mob in mobs)
            {
                if (mob != null)
                {
                    var menu = JungleClearMenu;
                    var Qc = mob.IsKillable(Q.Range) && Q.IsReady() && menu.checkbox("Q") && Q.Mana(menu);
                    var Wc = mob.IsKillable(W.Range) && W.IsReady() && menu.checkbox("W") && W.Mana(menu);
                    var wsave = menu.checkbox("Wsave") && W.Handle.Ammo < 2;

                    if (Wc)
                    {
                        if (wsave)
                        {
                            return;
                        }

                        W.Cast(mob.ServerPosition);
                    }

                    if (Qc && Orbwalker.AzirSoldiers.Count(s => s.IsAlly) > 0)
                    {
                        Q.Cast(mob);
                    }
                }
            }
        }

        public override void KillSteal()
        {
            foreach (var spell in SpellList.Where(s => s != W && s != E))
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

        public override void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());
            if (minions != null)
            {
                var menu = LaneClearMenu;
                var Qc = Q.IsReady() && menu.checkbox("Q") && Q.Mana(menu);
                var Wc = W.IsReady() && menu.checkbox("W") && W.Mana(menu);
                var wsave = menu.checkbox("Wsave") && W.Handle.Ammo < 2;

                var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

                var bestQ = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(objAiMinions.ToArray(), Orbwalker.AzirSoldierAutoAttackRange, (int)Q.Range);

                var bestW = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(objAiMinions.ToArray(), Orbwalker.AzirSoldierAutoAttackRange, (int)W.Range);

                if (Wc && bestW.HitNumber > 0 && bestW.CastPosition != null)
                {
                    if (wsave)
                    {
                        return;
                    }

                    W.Cast(bestW.CastPosition);
                }

                if (Qc && bestQ.HitNumber > 0 && bestQ.CastPosition != null)
                {
                    Q.Cast(bestQ.CastPosition);
                }
            }
        }
    }
}
