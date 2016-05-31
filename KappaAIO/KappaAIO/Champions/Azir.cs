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

    using Core;
    using Core.Managers;

    using SharpDX;

    internal static class Azir
    {
        private static Menu Menuini, Auto, JumperMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillstealMenu, DrawMenu, ColorMenu;

        private static Spell.Skillshot Q;

        private static Spell.Skillshot W;

        private static Spell.Skillshot E;

        private static Spell.Skillshot R;

        private static Spell.Skillshot Flash;

        private static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        internal static bool Ehit(this Obj_AI_Base target)
        {
            return
                Orbwalker.AzirSoldiers.Select(
                    soldier => new Geometry.Polygon.Rectangle(ObjectManager.Player.ServerPosition, soldier.ServerPosition, target.BoundingRadius + 35))
                    .Any(rectangle => rectangle.IsInside(target));
        }

        internal static bool Ehit(this Obj_AI_Base target, Vector3 pos)
        {
            var targetpos = Prediction.Position.PredictUnitPosition(target, E.CastDelay);
            return new Geometry.Polygon.Rectangle(ObjectManager.Player.ServerPosition, pos, target.BoundingRadius + 35).IsInside(targetpos);
        }

        public static void Execute()
        {
            try
            {
                Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear, 250, 1000, 65) { AllowedCollisionCount = int.MaxValue };
                W = new Spell.Skillshot(SpellSlot.W, 525, SkillShotType.Circular);
                E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 1200, 80) { AllowedCollisionCount = int.MaxValue };
                R = new Spell.Skillshot(SpellSlot.R, 350, SkillShotType.Linear, 500, 1000, 220) { AllowedCollisionCount = int.MaxValue };

                if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerFlash")) != null)
                {
                    Flash = new Spell.Skillshot(Player.Instance.GetSpellSlotFromName("SummonerFlash"), 450, SkillShotType.Circular);
                }

                SpellList.Add(Q);
                SpellList.Add(W);
                SpellList.Add(E);
                SpellList.Add(R);

                Menuini = MainMenu.AddMenu("KappAzir", "KappAzir");
                Auto = Menuini.AddSubMenu("Auto Settings");
                JumperMenu = Menuini.AddSubMenu("Jumper Settings");
                ComboMenu = Menuini.AddSubMenu("Combo Settings");
                HarassMenu = Menuini.AddSubMenu("Harass Settings");
                LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
                JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
                KillstealMenu = Menuini.AddSubMenu("KillSteal Settings");
                DrawMenu = Menuini.AddSubMenu("Drawings Settings");
                ColorMenu = Menuini.AddSubMenu("ColorPicker");

                foreach (var spell in SpellList.Where(s => s != E))
                {
                    Menuini.Add(spell.Slot + "hit", new ComboBox(spell.Slot + " HitChance", 0, "High", "Medium", "Low"));
                    Menuini.AddSeparator(0);
                }

                Auto.AddGroupLabel("Settings");
                Auto.Add("gap", new CheckBox("Anti-GapCloser"));
                Auto.Add("int", new CheckBox("Interrupter"));
                Auto.Add("Danger", new ComboBox("Interrupter DangerLevel", 1, "High", "Medium", "Low"));
                Auto.AddGroupLabel("Turret Settings");
                Auto.Add("tower", new CheckBox("Create Turrets"));
                Auto.Add("Tenemy", new Slider("Create Turret If [{0}] Enemies Near", 3, 1, 6));
                Auto.AddGroupLabel("Anti GapCloser Spells");
                foreach (var spell in
                    from spell in Gapcloser.GapCloserList
                    from enemy in EntityManager.Heroes.Enemies.Where(enemy => spell.ChampName == enemy.ChampionName)
                    select spell)
                {
                    Auto.Add(spell.SpellName, new CheckBox(spell.ChampName + " " + spell.SpellSlot));
                }

                if (EntityManager.Heroes.Enemies.Any(e => e.Hero == Champion.Rengar))
                {
                    Auto.Add("rengar", new CheckBox("Rengar Leap"));
                }

                JumperMenu.Add("jump", new KeyBind("WEQ Flee Key", false, KeyBind.BindTypes.HoldActive, 'A'));
                JumperMenu.Add("normal", new KeyBind("Normal Insec Key", false, KeyBind.BindTypes.HoldActive, 'S'));
                JumperMenu.Add("new", new KeyBind("New Insec Key", false, KeyBind.BindTypes.HoldActive, 'Z'));
                JumperMenu.Add("flash", new CheckBox("Use Flash for Possible AoE"));
                JumperMenu.Add("delay", new Slider("Delay EQ", 200, 0, 500));
                JumperMenu.Add("range", new Slider("Check for soldiers Range", 800, 0, 1000));

                ComboMenu.AddGroupLabel("Combo Settings");
                ComboMenu.Add("key", new KeyBind("Combo Key", false, KeyBind.BindTypes.HoldActive, 32));
                ComboMenu.AddSeparator(0);
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
                HarassMenu.Add("key", new KeyBind("Harass Key", false, KeyBind.BindTypes.HoldActive, 'C'));
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
                LaneClearMenu.Add("key", new KeyBind("LaneClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
                LaneClearMenu.Add("Q", new CheckBox("Use Q"));
                LaneClearMenu.Add(Q.Slot + "mana", new Slider("Stop using Q if Mana < [{0}%]", 65));
                LaneClearMenu.Add("W", new CheckBox("Use W"));
                LaneClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
                LaneClearMenu.Add(W.Slot + "mana", new Slider("Stop using W if Mana < [{0}%]", 65));

                JungleClearMenu.AddGroupLabel("JungleClear Settings");
                JungleClearMenu.Add("key", new KeyBind("JungleClear Key", false, KeyBind.BindTypes.HoldActive, 'V'));
                JungleClearMenu.Add("Q", new CheckBox("Use Q"));
                JungleClearMenu.Add(Q.Slot + "mana", new Slider("Stop using Q if Mana < [{0}%]", 65));
                JungleClearMenu.Add("W", new CheckBox("Use W"));
                JungleClearMenu.Add("Wsave", new CheckBox("Save 1 W Stack"));
                JungleClearMenu.Add(W.Slot + "mana", new Slider("Stop using W if Mana < [{0}%]", 65));

                KillstealMenu.AddGroupLabel("Stealer Settings");
                foreach (var spell in SpellList.Where(s => s != W && s != E))
                {
                    KillstealMenu.Add(spell.Slot + "ks", new CheckBox("KillSteal " + spell.Slot));
                    KillstealMenu.Add(spell.Slot + "js", new CheckBox("JungleSteal " + spell.Slot));
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

                Common.ShowNotification("KappaAzir - Loaded", 5000);

                Game.OnTick += Game_OnTick;
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Drawing.OnDraw += Drawing_OnDraw;
                GameObject.OnCreate += GameObject_OnCreate;
                Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            }
            catch (Exception e)
            {
                Common.Log(e.ToString());
            }
        }

        public static void Game_OnTick(EventArgs args)
        {
            updatespells();

            if (NewInsec)
            {
                var rpos = Player.Instance.ServerPosition.Extend(insectpos(), R.Range).To3D();

                var qtime = Game.Time - insecqtime;
                if ((qtime > 0.1f && qtime < 0.1) || TargetSelector.SelectedTarget.IsValidTarget(R.Range - 75))
                {
                    R.Cast(rpos);
                }
            }

            if (ComboMenu.keybind("key"))
            {
                Combo();
            }

            if (HarassMenu.keybind("key") || HarassMenu.keybind("toggle"))
            {
                Harass();
            }
            if (LaneClearMenu.keybind("key"))
            {
                LaneClear();
            }
            if (JungleClearMenu.keybind("key"))
            {
                JungleClear();
            }
            if (JumperMenu.keybind("jump"))
            {
                Jump(Game.CursorPos);
            }
            if (JumperMenu.keybind("normal"))
            {
                Normal(TargetSelector.SelectedTarget);
            }

            if (JumperMenu.keybind("new"))
            {
                New();
            }

            if (Auto.checkbox("tower"))
            {
                var azirtower =
                    ObjectManager.Get<GameObject>()
                        .FirstOrDefault(o => o != null && o.Name.ToLower().Contains("towerclicker") && Player.Instance.Distance(o) < 500);
                if (azirtower != null && azirtower.CountEnemeis(800) >= Auto.slider("Tenemy"))
                {
                    Player.UseObject(azirtower);
                }
            }

            NormalInsec = JumperMenu.keybind("normal");
            NewInsec = JumperMenu.keybind("new");
        }

        public static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (target == null || args.Target == null || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)
                || Orbwalker.ValidAzirSoldiers.Count(s => s.IsAlly) < 1)
            {
                return;
            }
            var orbtarget = args.Target as Obj_AI_Base;
            foreach (var sold in Orbwalker.ValidAzirSoldiers)
            {
                if (sold != null)
                {
                    var sold1 = sold;
                    var minion =
                        EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                            m => m.IsInRange(sold1, sold1.GetAutoAttackRange()) && m.IsKillable());
                    if (minion != null && minion != orbtarget)
                    {
                        var killable = Player.Instance.GetAutoAttackDamage(orbtarget, true)
                                       >= Prediction.Health.GetPrediction(orbtarget, (int)Player.Instance.AttackDelay);
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
                Orbwalker.ResetAutoAttack();
            }

            Common.LastCastedSpell.Spell = args.Slot;
            Common.LastCastedSpell.Time = Game.Time;
        }

        public static void updatespells()
        {
            R.Width = 107 * (R.Level - 1) < 200 ? 220 : (107 * (R.Level - 1)) + 5;

            int count = Orbwalker.AzirSoldiers.Count(s => s.IsAlly);
            Q.Width = count != 0 ? 65 * count : 65;
        }

        public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsValidTarget(300) || e.End == Vector3.Zero
                || !e.End.IsInRange(Player.Instance, 300) || !Auto.checkbox(e.SpellName) || !Auto.checkbox("Gap") || !R.IsReady())
            {
                return;
            }

            R.Cast(sender);
        }

        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy || sender == null || e == null || !sender.IsValidTarget(R.Range) || e.DangerLevel == Common.danger(Auto)
                || !Auto.checkbox("int") || !R.IsReady())
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
                if (rengar != null && R.IsReady() && Auto.checkbox("gap") && Auto.checkbox("rengar") && rengar.IsValidTarget(R.Range))
                {
                    R.Cast(rengar);
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            // Insec Helper
            var target = TargetSelector.SelectedTarget;
            var colors = System.Drawing.Color.White;

            if (DrawMenu.checkbox("insec") && (NormalInsec || NewInsec))
            {
                var insecpos = insectpos(target);
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
                    var rpos = Player.Instance.ServerPosition.Extend(insecpos, R.Range).To3D();
                    Circle.Draw(Color.White, 100, rpos);
                    Circle.Draw(Color.White, 100, pos);
                    Circle.Draw(Color.White, 200, insecpos);
                    Line.DrawLine(colors, pos, rpos);
                }
            }

            // Spells Drawings
            foreach (var spell in SpellList)
            {
                var color = ColorMenu.Color(spell.Slot.ToString());
                spell.SpellRange(color, DrawMenu.checkbox(spell.Slot.ToString()));
            }

            // Damage
            DrawingsManager.DrawTotalDamage(DamageType.Magical, DrawMenu.checkbox("damage"));
        }

        public static void Combo()
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

                if (target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }
                if (menu.checkbox("Q") && !target.IsValidTarget(W.Range) && Q.IsReady() && Player.Instance.Mana > Q.Mana() + W.Mana()
                    && target.IsValidTarget(Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = Player.Instance.Position.Extend(target.Position, W.Range);
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

                if (Ec && Player.Instance.Mana > Q.Mana() + E.Mana() && target.Ehit(predQ.CastPosition) && predQ.HitChance >= HitChance.Medium)
                {
                    if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                        || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
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
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(Q.Range) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(
                        enemies.Cast<Obj_AI_Base>().ToArray(),
                        Q.Range,
                        (int)Orbwalker.AzirSoldierAutoAttackRange,
                        Q.CastDelay,
                        Q.Speed);
                    var castpos =
                        pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length)
                            .FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > 1)
                    {
                        Q.Cast(castpos.CastPosition);
                    }
                }
            }

            if (Ec && target.Ehit())
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                    || (menu.checkbox("Edive") && target.IsUnderEnemyturret() && target.IsUnderHisturret()))
                {
                    return;
                }

                var time = Player.Instance.Distance(target) / E.Speed;
                var killable = target.TotalDamage(DamageType.Magical) >= Prediction.Health.GetPrediction(target, (int)time);
                if (menu.checkbox("Ekill") && killable && Player.Instance.Mana >= Common.Mana())
                {
                    E.Cast(target);
                }
                else
                {
                    E.Cast(target);
                }
            }
            if (Rc)
            {
                var Raoe = Player.Instance.CountEnemeis(R.Range) >= menu.slider("Raoe")
                           || Player.Instance.CountEnemeis(R.Width) >= menu.slider("Raoe");

                if (target.IsValidTarget(R.Range - 25))
                {
                    if ((menu.checkbox("Rkill") && R.Slot.GetDamage(target) >= Prediction.Health.GetPrediction(target, R.CastDelay))
                        || (menu.checkbox("Rsave") && menu.slider("RHP") >= Player.Instance.HealthPercent) || (Raoe))
                    {
                        R.Cast(target.Rpos());
                    }
                }
            }

            if (menu.checkbox("insec") && target.IsKillable() && !target.IsValidTarget(R.Range))
            {
                if (target.CountEnemeis(750) >= menu.slider("Esafe") || menu.slider("EHP") >= Player.Instance.HealthPercent)
                {
                    return;
                }

                var time = R.CastDelay + Q.CastDelay + E.CastDelay;
                var kill = target.TotalDamage(DamageType.Magical) + 100 > Prediction.Health.GetPrediction(target, time);
                if (kill)
                {
                    Normal(target);
                }
            }
        }

        public static Vector3 Rpos(this Obj_AI_Base target)
        {
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(R.Range))
                    .FirstOrDefault(a => a.IsKillable() && a.IsValidTarget(1250) && !a.IsMe);
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(s => s.IsValidTarget(1250));
            if (Common.tower != null)
            {
                return Player.Instance.ServerPosition.Extend(Common.tower.ServerPosition, R.Range).To3D();
            }
            return Common.ally != null
                       ? Player.Instance.ServerPosition.Extend(Common.ally.ServerPosition, R.Range).To3D()
                       : R.GetPrediction(target).CastPosition;
        }

        public static void Harass()
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
                if (target.IsValidTarget(W.Range))
                {
                    var pred = W.GetPrediction(target);
                    W.Cast(pred.CastPosition);
                }
                if (menu.checkbox("Q") && !target.IsValidTarget(W.Range) && Q.IsReady() && Player.Instance.Mana > Q.Mana() + W.Mana()
                    && target.IsValidTarget(Q.Range - 25) && menu.checkbox("WQ"))
                {
                    var p = Player.Instance.Position.Extend(target.Position, W.Range);
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

            if (Ec && target.Ehit())
            {
                if ((target.CountEnemeis(750) >= menu.slider("Esafe")) || (menu.slider("EHP") >= Player.Instance.HealthPercent)
                    || (!menu.checkbox("Edive") && target.IsUnderHisturret()))
                {
                    return;
                }

                E.Cast(target);
            }
        }

        public static bool NormalInsec;

        public static bool NewInsec;

        public static float insecqtime;

        public static void Normal(Obj_AI_Base target, bool Combo = false)
        {
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (target == null || insectpos(target) == null || Common.Mana() > Player.Instance.Mana || !R.IsReady())
            {
                return;
            }

            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var rpos = Player.Instance.ServerPosition.Extend(insectpos(target), R.Range).To3D();

            if (target.IsValidTarget(R.Range))
            {
                if (JumperMenu.checkbox("flash") && Flash != null)
                {
                    var flashrange = Flash.Range + 250;
                    var enemies = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(flashrange) && e.IsKillable());
                    var pred = Prediction.Position.PredictCircularMissileAoe(
                        enemies.Cast<Obj_AI_Base>().ToArray(),
                        flashrange,
                        R.Width + 25,
                        R.CastDelay,
                        R.Speed);
                    var castpos =
                        pred.OrderByDescending(p => p.GetCollisionObjects<AIHeroClient>().Length)
                            .FirstOrDefault(p => p.CollisionObjects.Contains(target));
                    if (castpos?.GetCollisionObjects<AIHeroClient>().Length > Player.Instance.CountEnemeis(R.Range))
                    {
                        Flash.Cast(castpos.CastPosition);
                    }
                }
                R.Cast(rpos);
            }
            else
            {
                if (Q.IsInRange(insecpos))
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
            if (target == null || insectpos(target) == null || !R.IsReady() || Player.Instance.Mana < Common.Mana() || !target.IsValidTarget()
                || NormalInsec)
            {
                return;
            }

            var delay = JumperMenu.slider("delay");
            var insecpos = target.ServerPosition.Extend(insectpos(target), -200).To3D();
            var qpos = Player.Instance.ServerPosition.Extend(insectpos(target), Q.Range - 200).To3D();
            var soldier = Orbwalker.AzirSoldiers.FirstOrDefault(s => s.IsAlly && s.IsInRange(target, 200));
            var ready = E.IsReady() && Q.IsReady() && Player.Instance.Mana > Q.Mana() + E.Mana();

            if (ready && soldier != null)
            {
                Core.DelayAction(
                    () =>
                        {
                            if (E.Cast(target))
                            {
                                Core.DelayAction(() => Q.Cast(qpos), delay);
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
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(R.Range))
                    .FirstOrDefault(a => !a.IsMe && a.IsKillable() && a.IsInRange(target, 1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : Player.Instance.ServerPosition;
        }

        public static Vector3 insectpos()
        {
            Common.tower = EntityManager.Turrets.Allies.FirstOrDefault(t => t.IsValidTarget(1250));
            Common.ally =
                EntityManager.Heroes.Allies.OrderByDescending(a => a.CountEnemeis(R.Range)).FirstOrDefault(a => !a.IsMe && a.IsValidTarget(1000));
            if (Common.tower != null)
            {
                return Common.tower.ServerPosition;
            }

            return Common.ally != null ? Common.ally.ServerPosition : Player.Instance.ServerPosition;
        }

        public static void Jump(Vector3 pos)
        {
            var delay = JumperMenu.slider("delay");
            var range = JumperMenu.slider("range");
            var qpos = Player.Instance.ServerPosition.Extend(pos, Q.Range - 100).To3D();
            var wpos = Player.Instance.ServerPosition.Extend(pos, W.Range).To3D();
            var epos = Player.Instance.ServerPosition.Extend(pos, E.Range).To3D();
            var ready = E.IsReady() && Q.IsReady() && Player.Instance.Mana > Q.Mana() + E.Mana() + W.Mana();

            if (ready && Orbwalker.AzirSoldiers.Count(s => s.IsAlly && s.IsInRange(Player.Instance, range)) < 1)
            {
                if (Common.LastCastedSpell.Spell == SpellSlot.E)
                {
                    return;
                }
                W.Cast(wpos);
            }
            else
            {
                if (ready)
                {
                    Core.DelayAction(
                        () =>
                            {
                                if (E.Cast(epos))
                                {
                                    Core.DelayAction(() => Q.Cast(qpos), delay);
                                }
                            },
                        100);
                }
            }

            if (Common.LastCastedSpell.Spell == SpellSlot.E)
            {
                var timer = (Game.Time - Common.LastCastedSpell.Time) * 100;
                if (timer - delay < 0.1f && Q.IsReady())
                {
                    Q.Cast(qpos);
                }
            }
        }

        public static void JungleClear()
        {
            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());

            foreach (var mob in mobs)
            {
                if (mob != null)
                {
                    var menu = JungleClearMenu;
                    var Qc = mob.IsValidTarget(Q.Range) && Q.IsReady() && menu.checkbox("Q") && Q.Mana(menu);
                    var Wc = mob.IsValidTarget(W.Range) && W.IsReady() && menu.checkbox("W") && W.Mana(menu);
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

        public static void Killsteal()
        {
            foreach (var spell in SpellList.Where(s => s != W && s != E))
            {
                if (KillstealMenu.checkbox(spell.Slot + "ks") && spell.GetKStarget() != null)
                {
                    spell.Cast(spell.GetKStarget());
                }
                if (KillstealMenu.checkbox(spell.Slot + "js") && spell.GetJStarget() != null)
                {
                    spell.Cast(spell.GetJStarget());
                }
            }
        }

        public static void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderByDescending(m => m.MaxHealth).Where(m => m.IsKillable());
            if (minions != null)
            {
                var menu = LaneClearMenu;
                var Qc = Q.IsReady() && menu.checkbox("Q") && Q.Mana(menu);
                var Wc = W.IsReady() && menu.checkbox("W") && W.Mana(menu);
                var wsave = menu.checkbox("Wsave") && W.Handle.Ammo < 2;

                var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();

                var bestQ = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    objAiMinions.ToArray(),
                    Orbwalker.AzirSoldierAutoAttackRange,
                    (int)Q.Range);

                var bestW = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                    objAiMinions.ToArray(),
                    Orbwalker.AzirSoldierAutoAttackRange,
                    (int)W.Range);

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