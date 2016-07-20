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
using static KappaAIO.Core.CommonStuff.ItemsDatabase.Items;

namespace KappaAIO.Champions
{
    internal class LeeSin : Base
    {
        private static float lasttick;

        private static Obj_AI_Base Qtarget()
        {
            return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(e => e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe));
        }

        private static GameObject MyAlly;

        private static AIHeroClient MyTarget;

        private static int Passive
        {
            get
            {
                return user.GetBuffCount("BlindMonkFlurry");
            }
        }

        private static bool fpsboost
        {
            get
            {
                return Menuini.checkbox("fpsboost");
            }
        }

        private static Spell.Skillshot Q { get; }

        private static Spell.Active Q2 { get; }

        private static Spell.Targeted W { get; }

        private static Spell.Active E { get; }

        private static Spell.Targeted R { get; }

        static LeeSin()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1800, 75) { AllowedCollisionCount = 0 };
            Q2 = new Spell.Active(SpellSlot.Q, 1300);
            W = new Spell.Targeted(SpellSlot.W, 700);
            E = new Spell.Active(SpellSlot.E, 350);
            R = new Spell.Targeted(SpellSlot.R, 375);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("LeeSin", "LeeSin");
            AutoMenu = Menuini.AddSubMenu("Auto Shield");
            RMenu = Menuini.AddSubMenu("BubbaKush Settings");
            JumperMenu = Menuini.AddSubMenu("Insec Settings");
            ComboMenu = Menuini.AddSubMenu("Combo Settings");
            HarassMenu = Menuini.AddSubMenu("Harass Settings");
            LaneClearMenu = Menuini.AddSubMenu("LaneClear Settings");
            JungleClearMenu = Menuini.AddSubMenu("JungleClear Settings");
            KillStealMenu = Menuini.AddSubMenu("Stealer Settings");
            MiscMenu = Menuini.AddSubMenu("Misc Settings");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("ColorPicker");

            Menuini.Add("debug", new CheckBox("Debug Functions", false));
            Menuini.Add("fpsboost", new CheckBox("FPS Boost (FIX FPS Issues)", false));
            Menuini.AddLabel("Fps Boost: Fixed FPS Issues But can reduce the Performance");
            
            AutoMenu.Add("W", new CheckBox("Use W", false));
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                AutoMenu.AddSeparator(0);
                AutoMenu.Add(ally.ID(), new CheckBox("Shield " + ally.ChampionName + " (" + ally.Name + ")"));
                AutoMenu.Add(ally.ID() + "hp", new Slider("Shield Incoming Damage {0}%", 30));
            }

            RMenu.AddGroupLabel("BubbaKush Mode");
            RMenu.Add("list", new ComboBox("Mode", 0, "Auto", "Close > LowHP", "MaxHP > LowHP"));
            RMenu.Add("bubba", new KeyBind("Bubba Kush", false, KeyBind.BindTypes.HoldActive, 'Z'));
            RMenu.AddGroupLabel("HOW IT WORKS");
            RMenu.AddLabel("It will Jump > ult the target from the mode towards other target");

            JumperMenu.AddGroupLabel("Insec Mode");
            JumperMenu.Add("normal", new KeyBind("normal Insec", false, KeyBind.BindTypes.HoldActive, 'S'));
            JumperMenu.AddGroupLabel("HOW IT WORKS");
            JumperMenu.AddLabel("Enable Insec Drawings from Draw Menu To Know your target");
            JumperMenu.AddLabel("The target with red circle is the INSEC target");
            JumperMenu.AddLabel("The ally with blue circle is the INSEC TO target");
            JumperMenu.AddLabel("You can select them by Left clicking on them");
            JumperMenu.AddLabel("On double click you Unselect both of the ally and the target");

            ComboMenu.AddGroupLabel("Combo Mode");
            ComboMenu.Add("mode", new ComboBox("Combo Mode", 0, "Normal", "Star Combo"));
            var Switch = ComboMenu.Add("switch", new KeyBind("Combo Switch", false, KeyBind.BindTypes.HoldActive, 'G'));
            Switch.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (args.NewValue)
                    {
                        ComboMenu["mode"].Cast<ComboBox>().CurrentValue = ComboMenu.combobox("mode") == 0 ? 1 : 0;
                    }
                };
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q1", new CheckBox("Use Q1"));
            ComboMenu.Add("Q2", new CheckBox("Use Q2"));
            ComboMenu.Add("E1", new CheckBox("Use E1"));
            ComboMenu.Add("E2", new CheckBox("Use E2"));
            ComboMenu.Add("WQ1", new CheckBox("WardJump GapClose for Q 1", false));
            ComboMenu.Add("WQ2", new CheckBox("WardJump GapClose for Q 2 (Bridge Q)"));
            ComboMenu.Add("Wmode", new ComboBox("W Mode", 0, "Auto", "Ward Jump", "Shield Self", "Disable"));
            ComboMenu.Add("Passive", new Slider("Passive Count", 1, 0, 2));
            ComboMenu.Add("Rkill", new CheckBox("Use R Kill"));
            ComboMenu.Add("Raoe", new Slider("R AoE Hit [{0}]", 3, 2, 6));
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Star Combo");
            ComboMenu.Add("bubba", new CheckBox("BubbaKush StarCombo"));
            ComboMenu.Add("Wj", new CheckBox("Use WardJump"));
            ComboMenu.Add("ES", new CheckBox("Use E"));

            HarassMenu.AddGroupLabel("Harass Mode");
            HarassMenu.Add("Q1", new CheckBox("Use Q1"));
            HarassMenu.Add("Q2", new CheckBox("Use Q2"));
            HarassMenu.Add("E1", new CheckBox("Use E1"));
            HarassMenu.Add("E2", new CheckBox("Use E2"));
            HarassMenu.Add("Passive", new Slider("Passive Count", 1, 0, 2));

            LaneClearMenu.AddGroupLabel("LaneClear Mode");
            LaneClearMenu.Add("Q1", new CheckBox("Use Q1"));
            LaneClearMenu.Add("Q2", new CheckBox("Use Q2"));
            LaneClearMenu.Add("E1", new CheckBox("Use E1"));
            LaneClearMenu.Add("E2", new CheckBox("Use E2"));
            LaneClearMenu.Add("Passive", new Slider("Passive Count", 1, 0, 2));

            JungleClearMenu.AddGroupLabel("JungleClear Mode");
            JungleClearMenu.Add("Q1", new CheckBox("Use Q1"));
            JungleClearMenu.Add("Q2", new CheckBox("Use Q2"));
            JungleClearMenu.Add("W1", new CheckBox("Use W1"));
            JungleClearMenu.Add("W2", new CheckBox("Use W2"));
            JungleClearMenu.Add("E1", new CheckBox("Use E1"));
            JungleClearMenu.Add("E2", new CheckBox("Use E2"));
            JungleClearMenu.Add("Passive", new Slider("Passive Count", 0, 0, 2));

            KillStealMenu.AddGroupLabel("Killsteal Settings (Champions)");
            KillStealMenu.Add("Q1", new CheckBox("Use Q1"));
            KillStealMenu.Add("Q2", new CheckBox("Use Q2"));
            KillStealMenu.Add("E1", new CheckBox("Use E1"));
            KillStealMenu.Add("R", new CheckBox("Use R"));
            KillStealMenu.AddGroupLabel("Killsteal Settings (Jungle)");
            KillStealMenu.Add("Q1j", new CheckBox("Use Q1"));
            KillStealMenu.Add("Q2j", new CheckBox("Use Q2"));
            KillStealMenu.Add("E1j", new CheckBox("Use E1"));
            KillStealMenu.Add("E2j", new CheckBox("Use E2"));

            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("wjmax", new CheckBox("WardJump MAX Range Always"));
            MiscMenu.Add("wardjump", new KeyBind("Ward Jump", false, KeyBind.BindTypes.HoldActive, 'A'));
            MiscMenu.Add("smiteq", new CheckBox("Smite Q"));
            MiscMenu.Add("Rint", new CheckBox("R Interrupter"));
            MiscMenu.Add("Danger", new ComboBox("Interrupter DangerLevel", 1, "High", "Medium", "Low"));
            MiscMenu.Add("Rgap", new CheckBox("R On GapCloser"));
            MiscMenu.Add("Rgaphp", new Slider("R GapCloser [{0}%]"));

            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.Add("insec", new CheckBox("Draw Insec Lines"));
            DrawMenu.Add("mode", new CheckBox("Draw Combo Mode"));
            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");
            DrawMenu.AddSeparator(1);
            foreach (var spell in SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }

            GameObject.OnCreate += EventsHandler.GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += EventsHandler.OnProcessSpellCast;
            Messages.OnMessage += EventsHandler.Messages_OnMessage;
            Spellbook.OnCastSpell += EventsHandler.Spellbook_OnCastSpell;
            Gapcloser.OnGapcloser += EventsHandler.Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += EventsHandler.Interrupter_OnInterruptableSpell;
            Dash.OnDash += EventsHandler.Dash_OnDash;
            Chat.OnMessage += Chat_OnMessage;
            Chat.OnInput += Chat_OnInput;
            Chat.OnClientSideMessage += Chat_OnClientSideMessage;
            OnIncDmg += LeeSin_OnIncDmg;
        }

        private static void LeeSin_OnIncDmg(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args, float IncDamage)
        {
            if(!sender.IsEnemy || !target.IsAlly || !AutoMenu.checkbox("W") || !SpellsManager.W1 || !W.IsReady()) return;

            var hp = AutoMenu.slider(target.ID() + "hp");
            var dmgpercent = (IncDamage / target.TotalShieldHealth()) * 100;
            if (target != null && target.IsKillable(W.Range) && (dmgpercent >= hp || hp >= target.HealthPercent))
            {
                SpellsManager.W(target, true);
            }
        }

        private static void Chat_OnClientSideMessage(ChatClientSideMessageEventArgs args)
        {
            if (args.Message.Contains("leesin debug:") && !Menuini.checkbox("debug"))
            {
                args.Process = false;
            }
        }

        private static void Chat_OnInput(ChatInputEventArgs args)
        {
            if (args.Input.Contains("leesin debug:") && !Menuini.checkbox("debug"))
            {
                args.Process = false;
            }
        }

        private static void Chat_OnMessage(AIHeroClient sender, ChatMessageEventArgs args)
        {
            if (args.Message.Contains("leesin debug:") && !Menuini.checkbox("debug"))
            {
                args.Process = false;
            }
        }

        public override void Active()
        {
            if (EloBuddy.SDK.Core.GameTickCount - lasttick < 69 && fpsboost)
            {
                return;
            }

            Q.Range = (uint)(SpellsManager.Q1 ? 1100 : 1300);
            E.Range = (uint)(SpellsManager.E1 ? 350 : 700);

            if (JumperMenu.keybind("normal"))
            {
                Insec.Normal();
            }

            if (BubbaKush.IsActive())
            {
                BubbaKush.Execute();
            }

            if (MiscMenu.keybind("wardjump"))
            {
                if (MiscMenu.checkbox("wjmax"))
                {
                    WardJump.Jump(user.ServerPosition.Extend(Game.CursorPos, 600).To3D());
                }
                else if (W.IsInRange(Game.CursorPos))
                {
                    WardJump.Jump(Game.CursorPos);
                }
            }

            lasttick = EloBuddy.SDK.Core.GameTickCount;
        }

        public override void Combo()
        {
            var mode = ComboMenu.combobox("mode");
            AIHeroClient target;

            if (Qtarget() != null && Qtarget() is AIHeroClient && ComboMenu.checkbox("Q2"))
            {
                target = Qtarget() as AIHeroClient;
            }
            else if (Q.IsReady() && W.IsReady() && (ComboMenu.checkbox("WQ1") || ComboMenu.checkbox("WQ2")) && (ComboMenu.combobox("Wmode").Equals(0) || ComboMenu.combobox("Wmode").Equals(1)))
            {
                target = TargetSelector.GetTarget(Q.Range + 200, DamageType.Physical);
            }
            else if (Q.IsReady() && ComboMenu.checkbox("Q1"))
            {
                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            }
            else if (W.IsReady() && (TargetSelector.GetTarget(W.Range, DamageType.Physical) != null && WardJump.IsReady(TargetSelector.GetTarget(W.Range, DamageType.Physical).ServerPosition, true))
                     && ComboMenu.combobox("Wmode") == 0 || ComboMenu.combobox("Wmode") == 1)
            {
                target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            }
            else if (E.IsReady() && ComboMenu.checkbox("E1"))
            {
                target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            }
            else if (R.IsReady())
            {
                target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            }
            else
            {
                target = TargetSelector.GetTarget(user.GetAutoAttackRange(), DamageType.Physical);
            }

            if (target == null)
            {
                return;
            }

            if (Qtarget() != null && Qtarget() is AIHeroClient)
            {
                var qtarget = Qtarget() as AIHeroClient;
                if (Qtarget().NetworkId.Equals(target.NetworkId) && Q.GetDamage(qtarget) >= qtarget.PredHP(Q.CastDelay))
                {
                    SpellsManager.Q(qtarget, false, true);
                }

                if (ComboMenu.checkbox("WQ2") && qtarget.IsKillable(1500) && !qtarget.IsKillable(1300) && WardJump.IsReady(user.ServerPosition.Extend(qtarget, 600).To3D(), true))
                {
                    Chat.Print("leesin debug: WQ2");
                    WardJump.Jump(user.ServerPosition.Extend(qtarget, 600).To3D(), false, true);
                    Q2.Cast();
                }
            }

            if (R.IsReady())
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsKillable(R.Range)))
                {
                    var rec = new Geometry.Polygon.Rectangle(user.ServerPosition, user.ServerPosition.Extend(enemy.ServerPosition, 700).To3D(), target.BoundingRadius * 2);
                    if (EntityManager.Heroes.Enemies.Count(e => e.IsKillable() && rec.IsInside(e.PredPos(100))) >= ComboMenu.slider("Raoe"))
                    {
                        Chat.Print("leesin debug: Raoe");
                        R.Cast(enemy);
                    }
                }
            }

            if (mode.Equals(0))
            {
                if (!target.IsKillable(Q.Range - 100) && target.IsKillable(Q.Range + 300) && ComboMenu.checkbox("WQ1")
                    && WardJump.IsReady(user.ServerPosition.Extend(target.ServerPosition, 600).To3D()) && Q.IsReady())
                {
                    Chat.Print("leesin debug: W Q");
                    WardJump.Jump(user.ServerPosition.Extend(target.ServerPosition, 600).To3D(), false, true);
                    return;
                }

                if (!target.IsKillable(user.GetAutoAttackRange() + 35) && Qtarget() != null && Qtarget() is AIHeroClient && Qtarget().NetworkId.Equals(target.NetworkId))
                {
                    Chat.Print("leesin debug: Q gap");
                    if (ComboMenu.checkbox("Q2") && Q.IsReady())
                    {
                        Q2.Cast();
                        return;
                    }
                }

                if (Passive <= ComboMenu.slider("Passive") || SpellsManager.lastspelltimer > 2500)
                {
                    if (E.IsReady() && target.IsKillable(E.Range))
                    {
                        Chat.Print("leesin debug: Ecombo");
                        SpellsManager.E(target, ComboMenu.checkbox("E1"), ComboMenu.checkbox("E2"));
                        return;
                    }

                    if (Q.IsReady() && target.IsKillable(Q.Range))
                    {
                        Chat.Print("leesin debug: Q combo");
                        if (Qtarget() != null && Qtarget() is AIHeroClient)
                        {
                            SpellsManager.Q(target, false, ComboMenu.checkbox("Q2"));
                            return;
                        }

                        SpellsManager.Q(target, ComboMenu.checkbox("Q1"));
                        return;
                    }

                    if (W.IsReady())
                    {
                        if (ComboMenu.combobox("Wmode").Equals(0) || ComboMenu.combobox("Wmode").Equals(1))
                        {
                            if (target.IsKillable(W.Range) && WardJump.IsReady(target.PredPos(200).To3D()) && !target.IsKillable(user.GetAutoAttackRange() + 35) && !target.IsKillable(E.Range)
                                && !target.PredPos(200).IsInRange(user, user.GetAutoAttackRange() + 15))
                            {
                                Chat.Print("leesin debug: W WardJump");
                                WardJump.Jump(target.PredPos(200).Extend(user, -200).To3D(), false, true);
                                return;
                            }
                        }

                        if (ComboMenu.combobox("Wmode").Equals(2) || ComboMenu.combobox("Wmode").Equals(0))
                        {
                            if (target.IsKillable(user.GetAutoAttackRange()))
                            {
                                Chat.Print("leesin debug: W shield");
                                SpellsManager.W(user, true, true);
                                return;
                            }
                        }
                    }
                }

                if (R.IsReady())
                {
                    if (R.GetDamage(target) >= target.TotalShieldHealth() && ComboMenu.checkbox("Rkill"))
                    {
                        Chat.Print("leesin debug: Rkill");
                        R.Cast(target);
                        return;
                    }
                }
            }

            if (mode.Equals(1))
            {
                if (EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastR < 1500 && EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastR > 350)
                {
                    Chat.Print("leesin debug: Q2 Star");
                    SpellsManager.Q(target, true, true);
                    return;
                }

                if (R.IsReady() && (WardJump.IsReady(target.ServerPosition) || target.IsKillable(R.Range)))
                {
                    if (!target.IsKillable(R.Range) && (Q.IsReady() || Qtarget() == target))
                    {
                        if (WardJump.IsReady(target.ServerPosition, true) && target.IsKillable(W.Range) && ComboMenu.checkbox("Wj"))
                        {
                            Chat.Print("leesin debug: Wardjump Star");
                            WardJump.Jump(target.ServerPosition, false, true);
                        }
                    }
                    else
                    {
                        if (Qtarget() == null && Q.IsReady() && SpellsManager.Q1 && target.IsKillable(Q.Range))
                        {
                            Chat.Print("leesin debug: Q1 Star");
                            SpellsManager.Q(target, true);
                        }
                        else
                        {
                            if (ComboMenu.checkbox("bubba") && user.CountEnemeis(1000) > 1 && (Flash != null && Flash.IsReady() || WardJump.IsReady(target.ServerPosition, true)))
                            {
                                Chat.Print("leesin debug: Bubba Star");
                                BubbaKush.DoBubba(target);
                            }
                            else
                            {
                                if (target.IsKillable(R.Range) && Qtarget() != null && Qtarget().NetworkId.Equals(target.NetworkId))
                                {
                                    Chat.Print("leesin debug: R Star");
                                    R.Cast(target);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!target.IsKillable(Q.Range - 50) && target.IsKillable(Q.Range + 300) && ComboMenu.checkbox("WQ1")
                        && WardJump.IsReady(user.ServerPosition.Extend(target.ServerPosition, 600).To3D(), true) && Q.IsReady() && ComboMenu.checkbox("Wj"))
                    {
                        Chat.Print("leesin debug: W Q");
                        WardJump.Jump(user.ServerPosition.Extend(target.ServerPosition, 600).To3D(), false, true);
                        return;
                    }

                    if (!target.IsKillable(user.GetAutoAttackRange() + 20) && Qtarget() != null && Qtarget() is AIHeroClient && Qtarget().NetworkId.Equals(target.NetworkId))
                    {
                        if (Q.IsReady() && !SpellsManager.Q1)
                        {
                            Chat.Print("leesin debug: Q gap");
                            Q2.Cast();
                            return;
                        }
                    }

                    if (Passive <= ComboMenu.slider("Passive") || SpellsManager.lastspelltimer > 2500)
                    {
                        if (E.IsReady() && target.IsKillable(E.Range + 15))
                        {
                            SpellsManager.E(target, true, true);
                            return;
                        }

                        if (Q.IsReady())
                        {
                            if (Qtarget() != null && Qtarget() is AIHeroClient)
                            {
                                SpellsManager.Q(target, false, true);
                                return;
                            }

                            SpellsManager.Q(target, true);
                            return;
                        }

                        if (W.IsReady())
                        {
                            if (ComboMenu.combobox("Wmode").Equals(0) || ComboMenu.combobox("Wmode").Equals(1))
                            {
                                if (ComboMenu.checkbox("Wj") && !target.IsKillable(user.GetAutoAttackRange() + 35) && target.IsKillable(W.Range)
                                    && !(Q.IsReady() && Q.GetPrediction(target).HitChance >= HitChance.Low))
                                {
                                    Chat.Print("leesin debug: W WardJump");
                                    WardJump.Jump(target.PredPos(200).Extend(user, -200).To3D(), false, true);
                                    return;
                                }
                            }

                            if (ComboMenu.combobox("Wmode").Equals(2) || ComboMenu.combobox("Wmode").Equals(0))
                            {
                                if (target.IsKillable(user.GetAutoAttackRange()))
                                {
                                    Chat.Print("leesin debug: W shield");
                                    SpellsManager.W(user, true, true);
                                    return;
                                }
                            }
                        }

                        if (R.IsReady() && target.IsKillable(R.Range))
                        {
                            if (R.GetDamage(target) >= target.TotalShieldHealth() && ComboMenu.checkbox("Rkill"))
                            {
                                Chat.Print("leesin debug: Rkill");
                                R.Cast(target);
                            }
                        }
                    }
                }
            }
        }

        public override void Harass()
        {
            AIHeroClient target;
            if (Qtarget() != null && Qtarget() is AIHeroClient && HarassMenu.checkbox("Q2"))
            {
                target = Qtarget() as AIHeroClient;
            }
            else if (Q.IsReady() && HarassMenu.checkbox("Q1") && SpellsManager.Q1)
            {
                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            }
            else if (E.IsReady() && (HarassMenu.checkbox("E1") || HarassMenu.checkbox("E2")))
            {
                target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            }
            else
            {
                target = TargetSelector.GetTarget(user.GetAutoAttackRange(), DamageType.Physical);
            }

            if (target == null)
            {
                return;
            }

            if (Passive <= HarassMenu.slider("Passive") || SpellsManager.lastspelltimer > 2500)
            {
                if (Q.IsReady() && target.IsKillable(Q.Range) && ((HarassMenu.checkbox("Q1") && SpellsManager.Q1) || HarassMenu.checkbox("Q2")))
                {
                    SpellsManager.Q(target, HarassMenu.checkbox("Q1"), HarassMenu.checkbox("Q2"));
                    return;
                }

                if (E.IsReady() && target.IsKillable(E.Range) && ((HarassMenu.checkbox("E1") && SpellsManager.E1) || HarassMenu.checkbox("E2")))
                {
                    SpellsManager.E(target, HarassMenu.checkbox("E1"), HarassMenu.checkbox("E2"));
                }
            }
        }

        public override void LaneClear()
        {
            var eminion = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(m => m.Health).FirstOrDefault(m => m.IsKillable(E.Range));
            var Eminions = user.CountEnemyMinions(E.Range) > 1 && SpellsManager.E1;
            var Qminion = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(m => m.Health).FirstOrDefault(m => m.IsKillable(Q.Range) && Q.GetPrediction(m).HitChance >= HitChance.Low);
            var Qlasthit =
                EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(m => m.MaxHealth)
                    .FirstOrDefault(
                        m =>
                        m.IsKillable(Q.Range) && Q.GetPrediction(m).HitChance >= HitChance.Low
                        && (Q.GetDamage(m) >= m.Health && (SpellsManager.Q1 || (Qtarget() != null && Qtarget().ID().Equals(m.ID())))));

            if (LaneClearMenu.slider("Passive") >= Passive || SpellsManager.lastspelltimer > 2500)
            {
                if (Q.IsReady() && ((LaneClearMenu.checkbox("Q1") && SpellsManager.Q1) || LaneClearMenu.checkbox("Q2")))
                {
                    if (Qlasthit != null)
                    {
                        if (LaneClearMenu.checkbox("Q1") && SpellsManager.Q1)
                        {
                            SpellsManager.Q(Qlasthit, LaneClearMenu.checkbox("Q1"));
                            return;
                        }
                        if (LaneClearMenu.checkbox("Q2") && !SpellsManager.Q1 && Qtarget() != null && Qlasthit.ID().Equals(Qtarget().ID()))
                        {
                            SpellsManager.Q(Qlasthit, false, LaneClearMenu.checkbox("Q2"));
                            return;
                        }
                    }


                    if (Qminion != null)
                    {
                        if (LaneClearMenu.checkbox("Q1") && SpellsManager.Q1)
                        {
                            SpellsManager.Q(Qminion, LaneClearMenu.checkbox("Q1"));
                            return;
                        }
                        if (LaneClearMenu.checkbox("Q2") && !SpellsManager.Q1 && Qtarget() != null && Qtarget().ID().Equals(Qminion.ID()))
                        {
                            SpellsManager.Q(Qminion, false, LaneClearMenu.checkbox("Q2"));
                            return;
                        }
                    }
                }

                if (E.IsReady() && ((LaneClearMenu.checkbox("E1") && SpellsManager.E1) || LaneClearMenu.checkbox("E2")))
                {
                    if (Eminions && eminion != null)
                    {
                        if (SpellsManager.E1)
                        {
                            SpellsManager.E(eminion, LaneClearMenu.checkbox("E1"));
                            return;
                        }

                        SpellsManager.E(eminion, false, LaneClearMenu.checkbox("E2"));
                        return;
                    }
                }
            }
        }

        public override void JungleClear()
        {
            var mob = new Obj_AI_Minion();
            if (Qtarget() != null && Qtarget() is Obj_AI_Minion && JungleClearMenu.checkbox("Q2"))
            {
                mob = Qtarget() as Obj_AI_Minion;
            }
            else if (Q.IsReady() && JungleClearMenu.checkbox("Q1") && SpellsManager.Q1)
            {
                mob = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderBy(m => m.Distance(user)).ThenByDescending(m => m.MaxHealth).FirstOrDefault(m => m.IsKillable(Q.Range));
            }
            else if (E.IsReady() && (JungleClearMenu.checkbox("E1") || JungleClearMenu.checkbox("E2")))
            {
                mob = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderBy(m => m.Distance(user)).ThenByDescending(m => m.MaxHealth).FirstOrDefault(m => m.IsKillable(E.Range));
            }
            else
            {
                mob =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderBy(m => m.Distance(user))
                        .ThenByDescending(m => m.MaxHealth)
                        .FirstOrDefault(m => m.IsKillable(user.GetAutoAttackRange() + 20));
            }

            if (mob == null)
            {
                return;
            }

            if (Passive <= JungleClearMenu.slider("Passive") || SpellsManager.lastspelltimer > 3000)
            {
                if (W.IsReady() && mob.IsKillable(E.Range) && ((JungleClearMenu.checkbox("W1") && SpellsManager.W1) || JungleClearMenu.checkbox("W2")))
                {
                    if (JungleClearMenu.checkbox("W1") && SpellsManager.W1)
                    {
                        SpellsManager.W(user, JungleClearMenu.checkbox("W1"));
                        return;
                    }
                    if (JungleClearMenu.checkbox("W2") && !SpellsManager.W1)
                    {
                        SpellsManager.W(user, false, JungleClearMenu.checkbox("W2"));
                        return;
                    }
                }

                if (E.IsReady() && mob.IsKillable(E.Range) && ((JungleClearMenu.checkbox("E1") && SpellsManager.E1) || JungleClearMenu.checkbox("E2")))
                {
                    if (JungleClearMenu.checkbox("E1") && SpellsManager.E1)
                    {
                        SpellsManager.E(mob, JungleClearMenu.checkbox("E1"));
                        return;
                    }
                    if (JungleClearMenu.checkbox("E2") && !SpellsManager.E1)
                    {
                        SpellsManager.E(mob, false, JungleClearMenu.checkbox("E2"));
                        return;
                    }
                }

                if (Q.IsReady() && mob.IsKillable(Q.Range) && ((JungleClearMenu.checkbox("Q1") && SpellsManager.Q1) || JungleClearMenu.checkbox("Q2")))
                {
                    if (JungleClearMenu.checkbox("Q1") && SpellsManager.Q1)
                    {
                        SpellsManager.Q(mob, JungleClearMenu.checkbox("Q1"));
                        return;
                    }
                    if (JungleClearMenu.checkbox("Q2") && !SpellsManager.Q1)
                    {
                        SpellsManager.Q(mob, false, JungleClearMenu.checkbox("Q2"));
                        return;
                    }
                }
            }
        }

        public override void KillSteal()
        {
            if (Q.GetKStarget() != null
                && ((KillStealMenu.checkbox("Q1") && SpellsManager.Q1) || (KillStealMenu.checkbox("Q2") && !SpellsManager.Q1 && Qtarget() != null && Qtarget().ID().Equals(Q.GetKStarget().ID()))))
            {
                SpellsManager.Q(Q.GetKStarget(), KillStealMenu.checkbox("Q1"), KillStealMenu.checkbox("Q2") && Qtarget() != null && Qtarget().ID().Equals(Q.GetKStarget().ID()));
            }

            if (Q.GetJStarget() != null
                && ((KillStealMenu.checkbox("Q1j") && SpellsManager.Q1) || (KillStealMenu.checkbox("Q2j") && !SpellsManager.Q1 && Qtarget() != null && Qtarget().ID().Equals(Q.GetJStarget().ID()))))
            {
                SpellsManager.Q(Q.GetJStarget(), KillStealMenu.checkbox("Q1j"), KillStealMenu.checkbox("Q2j") && Qtarget() != null && Qtarget().ID().Equals(Q.GetJStarget().ID()));
            }

            if (E.GetKStarget() != null && KillStealMenu.checkbox("E1") && SpellsManager.E1)
            {
                SpellsManager.E(E.GetKStarget(), KillStealMenu.checkbox("E1"));
            }

            if (E.GetJStarget() != null && KillStealMenu.checkbox("E1j") && SpellsManager.E1)
            {
                SpellsManager.E(E.GetJStarget(), KillStealMenu.checkbox("E1j"));
            }

            if (R.GetKStarget() != null && R.IsReady() && KillStealMenu.checkbox("R"))
            {
                R.Cast(R.GetKStarget());
            }
        }

        public override void Draw()
        {
            var X = user.ServerPosition.WorldToScreen().X;
            var Y = user.ServerPosition.WorldToScreen().Y;
            if (DrawMenu.checkbox("mode"))
            {
                Drawing.DrawText(X, Y - 20, System.Drawing.Color.White, "Combo Mode: " + (ComboMenu.combobox("mode") == 0 ? "Normal Combo" : "Star Combo"), 10);
            }

            if (DrawMenu.checkbox("insec"))
            {
                if (Insec.InsecTarget != null && Insec.InsecTarget.IsKillable())
                {
                    Circle.Draw(Color.Red, Insec.Range(), 5, Insec.InsecTarget);
                    if (Insec.InsecTo(Insec.InsecTarget) != null)
                    {
                        Circle.Draw(Color.DarkBlue, 100, Insec.InsecTo(Insec.InsecTarget));
                        Circle.Draw(Color.White, 100, 3, Insec.Pos);
                        if (Insec.Pos.Distance(Insec.InsecTo(Insec.InsecTarget)) < 2500)
                        {
                            DrawingsManager.drawLine(Insec.Pos, Insec.InsecTo(Insec.InsecTarget), 2, System.Drawing.Color.White);
                        }

                        if (Insec.Qtarget(Insec.Pos) != null)
                        {
                            Circle.Draw(Color.Red, Insec.Range(), Insec.Qtarget(Insec.Pos));
                        }
                    }
                }
            }

            if (!Menuini.checkbox("debug"))
            {
                return;
            }

            BubbaKush.DoBubba(null, (1 + 1).Equals(2));

            Drawing.DrawText(X, Y, System.Drawing.Color.White, (EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastpW).ToString(), 5);
        }

        private static class BubbaKush
        {
            public static bool IsActive()
            {
                return RMenu.keybind("bubba");
            }

            public static float Rflash;

            private static Vector3 Pos;

            public static void DoBubba(AIHeroClient target = null, bool draw = false)
            {
                if (target == null)
                {
                    target = new AIHeroClient();
                    switch (RMenu.combobox("list"))
                    {
                        case 0:
                            target = Qtarget() != null
                                         ? EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(user)).FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && Qtarget().IsInRange(e.PredPos(250), 450))
                                         : EntityManager.Heroes.Enemies.OrderBy(e => e.PredPos(250).Distance(user))
                                               .FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && WardJump.IsReady(e.ServerPosition) ? e.IsKillable(W.Range) : e.IsKillable(R.Range));
                            break;
                        case 1:
                            target = Qtarget() != null
                                         ? EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(user)).FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && Qtarget().IsInRange(e.PredPos(250), 450))
                                         : EntityManager.Heroes.Enemies.OrderBy(e => e.PredPos(250).Distance(user))
                                               .FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && WardJump.IsReady(e.ServerPosition) ? e.IsKillable(W.Range) : e.IsKillable(R.Range));
                            break;
                        case 2:
                            target = Qtarget() != null
                                         ? EntityManager.Heroes.Enemies.OrderBy(e => e.MaxHealth).FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && Qtarget().IsInRange(e.PredPos(250), 450))
                                         : EntityManager.Heroes.Enemies.OrderBy(e => e.MaxHealth)
                                               .FirstOrDefault(e => e.PredHP(250) > R.GetDamage(e) && WardJump.IsReady(e.ServerPosition) ? e.IsKillable(W.Range) : e.IsKillable(R.Range));
                            break;
                    }
                }

                if (target == null)
                {
                    if (!draw)
                    {
                        Orbwalker.OrbwalkTo(Game.CursorPos);
                    }

                    return;
                }

                var enemy = EntityManager.Heroes.Enemies.OrderBy(e => e.Health).FirstOrDefault(e => e.IsKillable() && !e.NetworkId.Equals(target.NetworkId) && e.IsInRange(target, 750));

                if (enemy == null)
                {
                    if (!draw)
                    {
                        Orbwalker.OrbwalkTo(Game.CursorPos);
                    }

                    return;
                }

                Pos = target.PredPos(250).Extend(enemy.PredPos(300), -275).To3D();
                var rec = new Geometry.Polygon.Rectangle(target.PredPos(200), target.PredPos(200).Extend(enemy.PredPos(250), 750), target.BoundingRadius * 2);

                if (draw)
                {
                    Circle.Draw(Color.White, 100, Pos);
                    rec.Draw(System.Drawing.Color.White, 2);
                    return;
                }

                if (!R.IsReady())
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                    return;
                }

                if (target.IsKillable(R.Range) || user.IsInRange(Pos, 125))
                {
                    if (Flash != null && Flash.IsReady() && !WardJump.IsReady(Pos))
                    {
                        Chat.Print("leesin debug: RFlash");
                        R.Cast(target);
                        Rflash = EloBuddy.SDK.Core.GameTickCount;
                    }

                    if (user.IsInRange(Pos, 100))
                    {
                        Chat.Print("leesin debug: RInRange");
                        R.Cast(target);
                    }

                    foreach (var ene in EntityManager.Heroes.Enemies.Where(e => e.IsKillable(R.Range)))
                    {
                        if (ene != null)
                        {
                            var rect = new Geometry.Polygon.Rectangle(user.ServerPosition, user.ServerPosition.Extend(ene.PredPos(200), ene.BoundingRadius * 2).To3D(), 200);

                            if (EntityManager.Heroes.Enemies.Count(e => rect.IsInside(e)) > 1)
                            {
                                Chat.Print("leesin debug: Rfix");
                                R.Cast(ene);
                            }
                        }
                    }

                    if (SpellsManager.Wtimer < 1500)
                    {
                        Chat.Print("leesin debug: RpW");
                        R.Cast(target);
                    }
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }

                if (Qtarget() != null && Qtarget().IsInRange(target, 300) && !target.IsKillable(R.Range) && !W.IsInRange(Pos) && (WardJump.IsReady(Pos) || Flash != null && Flash.IsReady())
                    && !user.IsInRange(Pos, 125))
                {
                    Chat.Print("leesin debug: qcast");
                    Q2.Cast();
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }

                if (WardJump.IsReady(Pos) && target.IsKillable(600))
                {
                    if (!(target.IsKillable(R.Range) && Flash != null && Flash.IsReady()))
                    {
                        Chat.Print("leesin debug: WardJump");
                        WardJump.Jump(Pos, true);
                    }
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }

                var RF = user.ServerPosition.Extend(target.ServerPosition, 600).To3D();
                if (WardJump.IsReady(target.ServerPosition) && target.IsInRange(RF, R.Range - 100))
                {
                    if (!target.IsKillable(R.Range) && Flash != null && Flash.IsReady())
                    {
                        Chat.Print("leesin debug: WardJump R Flash");
                        WardJump.Jump(RF, true);
                    }
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }

            public static void Execute()
            {
                DoBubba(TargetSelector.SelectedTarget);
            }

            public static void CastFlash()
            {
                if (Pos != null && Flash != null)
                {
                    if (Flash.IsReady() && !user.IsInRange(Pos, 150) && EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastpW > 1000)
                    {
                        Chat.Print("leesin debug: Flash");
                        Flash.Cast(Pos);
                    }
                }
            }
        }

        private static class Insec
        {
            public static AIHeroClient InsecTarget
            {
                get
                {
                    return MyTarget ?? TargetSelector.SelectedTarget;
                }
            }

            public static Obj_AI_Base Qtarget(Vector3 vector3)
            {
                if (LeeSin.Qtarget() != null && LeeSin.Qtarget().IsInRange(vector3, Range()))
                {
                    return LeeSin.Qtarget();
                }

                var Hero =
                    EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(vector3))
                        .FirstOrDefault(
                            e =>
                            e.IsKillable() && e.Distance(vector3) < Range()
                            && (Q.GetPrediction(e).HitChance >= HitChance.Collision || e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe)) && e.Health > Q.GetDamage(e));

                var minion =
                    EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(e => e.Distance(vector3))
                        .FirstOrDefault(
                            e =>
                            e.IsKillable() && e.Distance(vector3) < Range()
                            && (Q.GetPrediction(e).HitChance >= HitChance.Collision || e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe)) && e.Health > Q.GetDamage(e));

                var mob =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderBy(e => e.Distance(vector3))
                        .FirstOrDefault(
                            e =>
                            e.IsKillable() && e.Distance(vector3) < Range()
                            && (Q.GetPrediction(e).HitChance > HitChance.Collision || e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe)) && e.Health > Q.GetDamage(e));

                if (Hero != null)
                {
                    return Hero;
                }

                return minion ?? mob;
            }

            public static Vector3 Pos
            {
                get
                {
                    return InsecTarget.PredPos(250).Extend(InsecTo(InsecTarget), -285).To3D();
                }
            }

            private static bool WardR
            {
                get
                {
                    return WardJump.IsReady(Pos) && Pos.IsInRange(user, 600);
                }
            }

            private static bool WardFlashR
            {
                get
                {
                    return WardJump.IsReady(Pos, true) && Flash != null && Flash.IsReady() && (Pos.IsInRange(user, 400 + Flash.Range) || Pos.IsInRange(user, 200 + Q.Range + Flash.Range))
                           && !Pos.IsInRange(user, 600);
                }
            }

            public static float Range()
            {
                return WardFlashR ? 400 + Flash.Range : 500;
            }

            public static Vector3 InsecTo(Obj_AI_Base target)
            {
                var oldpos = user.ServerPosition.Extend(ObjectManager.Get<Obj_HQ>().FirstOrDefault(i => i.IsAlly).Position, 200).To3D();
                if (MyAlly == null)
                {
                    var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.CountAllies(1000)).FirstOrDefault(a => !a.IsMe && a.IsValidTarget() && a.IsInRange(target.PredPos(200), 1350));
                    var tower = EntityManager.Turrets.Allies.FirstOrDefault(a => !a.IsDead && a.IsInRange(target.PredPos(200), 1350));

                    if (ally != null)
                    {
                        return ally.ServerPosition;
                    }

                    if (tower != null)
                    {
                        return tower.ServerPosition;
                    }
                }

                return MyAlly?.Position ?? oldpos.Extend(Game.CursorPos, 100).To3D();
            }

            public static void Normal()
            {
                Execute(InsecTarget);
            }

            public enum Steps
            {
                Nothing,

                UseQ,

                UseW,

                UseWF,

                UseR,

                UseF
            }

            public static Steps Step;

            private static void Execute(Obj_AI_Base target)
            {
                if (!R.IsReady() || target == null || !target.IsKillable() || Pos == null || user.Mana < 80)
                {
                    Step = Steps.Nothing;
                    Chat.Print("leesin debug: Return");
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                    return;
                }

                Orbwalker.OrbwalkTo(Game.CursorPos);
                if (user.Distance(Pos) < 200)
                {
                    Step = Steps.UseR;
                }
                else if (Step == Steps.Nothing && WardR)
                {
                    Step = Steps.UseW;
                }
                else if (Step == Steps.Nothing && user.Distance(Pos) > 400
                         && ((Qtarget(Pos) != null && user.Distance(Qtarget(Pos)) < Q.Range) || (LeeSin.Qtarget() != null && LeeSin.Qtarget().Distance(Pos) < 500)) && WardJump.IsReady(Pos)
                         && Q.IsReady())
                {
                    Step = Steps.UseQ;
                }
                else if (Step == Steps.Nothing && WardFlashR && SpellsManager.Wtimer > 1000 && SpellsManager.Qtimer > 500 && SpellsManager.Qtimer < 2000)
                {
                    Step = Steps.UseWF;
                }
                else if (Step == Steps.Nothing && user.Distance(Pos) < Flash.Range && !user.IsInRange(Pos, 200) && Flash != null && Flash.IsReady() && SpellsManager.Wtimer > 1000)
                {
                    Step = Steps.UseF;
                }
                else
                {
                    Step = Steps.Nothing;
                }

                Chat.Print("leesin debug:" + Step);
                switch (Step)
                {
                    case Steps.UseR:
                        R.Cast(target);
                        return;
                    case Steps.UseW:
                        WardJump.Jump(Pos);
                        return;
                    case Steps.UseWF:
                        if (user.IsInRange(Pos.Extend(user, 350).To3D(), 600))
                        {
                            WardJump.Jump(Pos.Extend(user, 350).To3D(), false, true);
                        }
                        return;
                    case Steps.UseF:
                        Flash?.Cast(Pos);
                        break;
                    case Steps.UseQ:
                        SpellsManager.Q(Qtarget(Pos), true, true, true);
                        return;
                }

                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }

        private static class WardJump
        {
            public static bool CanJump
            {
                get
                {
                    return W.IsReady() && SpellsManager.W1 && EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastpW > 1000;
                }
            }

            public static bool IsReady(Vector3 pos, bool combo = false)
            {
                return CanJump && ((UseWard != null && UseWard.IsOwned(user)) || Wtarget(pos, combo) != null);
            }

            private static readonly Item[] Wards =
                {
                    YellowTrinket, RubySightStone, SightStone, EyeOfTheEquinox, EyeOfTheOasis, EyeOfTheWatchers, TrackerKnife, TrackerKnife_Cinderhulk,
                    TrackerKnife_Devourer, TrackerKnife_Runic_Echoes, TrackerKnife_Sated_Devourer, TrackerKnife_Warrior
                };

            private static readonly Item[] VisionWards = { VisionWard };

            public static float lastward;

            private static Item UseWard
            {
                get
                {
                    return Ward ?? VWard;
                }
            }

            private static Item Ward
            {
                get
                {
                    return Wards.FirstOrDefault(w => w.IsReady() && w.IsOwned(user));
                }
            }

            private static Item VWard
            {
                get
                {
                    return VisionWards.FirstOrDefault(w => w.IsReady() && w.IsOwned(user));
                }
            }

            private static bool vWardReady
            {
                get
                {
                    return VisionWards.Any(w => w.IsReady());
                }
            }

            private static bool WardReady
            {
                get
                {
                    return Wards.Any(w => w.IsReady());
                }
            }

            private static bool UseWardReady
            {
                get
                {
                    return WardReady ? WardReady : vWardReady;
                }
            }

            public static void Jump(Vector3 vector3, bool bubba = false, bool combo = false)
            {
                if (W.IsReady() && SpellsManager.W1 && SpellsManager.Wtimer > 1000)
                {
                    if (Wtarget(vector3, combo) != null || EloBuddy.SDK.Core.GameTickCount - lastward < 3000)
                    {
                        SpellsManager.W(bubba ? Wtarget(vector3, false, 100) : Wtarget(vector3, combo), true, false, true);
                    }
                    else
                    {
                        if (SpellsManager.Wtimer > 1000 && UseWard != null)
                        {
                            CastWard(vector3.IsInRange(user, UseWard.Range) ? vector3 : user.ServerPosition.Extend(vector3, UseWard.Range).To3D());
                        }
                    }
                }
            }

            private static void CastWard(Vector3 vector3)
            {
                var wardinrage = ObjectsManager.Wards.Any(ward => ward.IsInRange(vector3, 200));

                var ally = EntityManager.Heroes.Allies.Any(a => a.IsInRange(vector3, 200) && a.IsValidTarget() && !a.IsMe);

                var minion = EntityManager.MinionsAndMonsters.AlliedMinions.Any(m => m.IsInRange(vector3, 200) && m.IsValidTarget());

                if (UseWardReady && UseWard.IsInRange(vector3) && (!user.ServerPosition.Extend(vector3, 75).IsWall() || !vector3.IsWall()) && !wardinrage && !ally && !minion
                    && EloBuddy.SDK.Core.GameTickCount - lastward > 1000)
                {
                    UseWard.Cast(vector3);
                    lastward = EloBuddy.SDK.Core.GameTickCount;
                }
            }

            private static Obj_AI_Base Wtarget(Vector3 vector3, bool combo = false, int range = 150)
            {
                if (combo)
                {
                    var wardinragec = ObjectsManager.Wards.OrderBy(e => e.Distance(vector3)).FirstOrDefault(ward => ward.IsInRange(vector3, 300) && ward.IsValid);

                    var allyc = EntityManager.Heroes.Allies.OrderBy(e => e.Distance(vector3)).FirstOrDefault(a => a.IsInRange(vector3, 300) && a.IsValidTarget() && !a.IsMe);

                    var minionc = EntityManager.MinionsAndMonsters.AlliedMinions.OrderBy(e => e.Distance(vector3)).FirstOrDefault(m => m.IsInRange(vector3, 300) && m.IsValidTarget());
                    if (allyc != null)
                    {
                        return allyc;
                    }

                    return minionc ?? wardinragec;
                }

                var wardinrage = ObjectsManager.Wards.FirstOrDefault(ward => ward.IsInRange(vector3, range));

                var ally = EntityManager.Heroes.Allies.FirstOrDefault(a => a.IsInRange(vector3, range) && a.IsValidTarget() && !a.IsMe);

                var minion = EntityManager.MinionsAndMonsters.AlliedMinions.FirstOrDefault(m => m.IsInRange(vector3, range) && m.IsValidTarget());
                if (ally != null)
                {
                    return ally;
                }

                return minion ?? wardinrage;
            }
        }

        private static class SpellsManager
        {
            public static float lastspell;

            public static float lastspelltimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - lastspell;
                }
            }

            public static float LastQ;

            public static float LastFlash;

            public static float FlashTimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - LastFlash;
                }
            }

            public static float LastpQ;

            public static float Qtimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - LastpQ;
                }
            }

            public static float LastW;

            public static float LastpW;

            public static float Wtimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - LastpW;
                }
            }

            public static float LastE;

            public static float LastR;

            public static float Rtimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - LastR;
                }
            }

            public static float LastpE;

            public static float Etimer
            {
                get
                {
                    return EloBuddy.SDK.Core.GameTickCount - LastpE;
                }
            }

            public static bool Q1
            {
                get
                {
                    return LeeSin.Q.Name.ToLower().Contains("qone");
                }
            }

            public static bool W1
            {
                get
                {
                    return LeeSin.W.Name.ToLower().Contains("wone");
                }
            }

            public static bool E1
            {
                get
                {
                    return LeeSin.E.Name.ToLower().Contains("eone");
                }
            }

            public static void Q(Obj_AI_Base target, bool q1, bool Q2 = false, bool insec = false)
            {
                if (LeeSin.Q.IsReady() && target.IsKillable(LeeSin.Q.Range))
                {
                    if (Q1 && q1 && Qtimer > 1500)
                    {
                        if (MiscMenu.checkbox("smiteq") && Smite != null && Smite.IsReady() && LeeSin.Q.GetPrediction(target).HitChance >= HitChance.High
                            && LeeSin.Q.GetPrediction(target)
                                   .CollisionObjects.Count(
                                       o => o.NetworkId != target.NetworkId && (o.IsMinion || o.IsMonster || o.IsMinion()) && o.IsKillable(Smite.Range - 10) && Smite.GetDamage(o) >= o.Health) == 1
                            && !(Common.orbmode(Orbwalker.ActiveModes.LaneClear) || Common.orbmode(Orbwalker.ActiveModes.JungleClear)))
                        {
                            Smite.Cast(
                                LeeSin.Q.GetPrediction(target)
                                    .CollisionObjects.FirstOrDefault(o => o.NetworkId != target.NetworkId && (o.IsMinion || o.IsMonster) && o.IsKillable(Smite.Range) && Smite.GetDamage(o) >= o.Health));
                            LeeSin.Q.Cast(target);
                            LastQ = EloBuddy.SDK.Core.GameTickCount;
                            return;
                        }

                        LeeSin.Q.Cast(target, HitChance.Low);
                        LastQ = EloBuddy.SDK.Core.GameTickCount;
                        return;
                    }

                    if (!Q1 && Q2)
                    {
                        if (insec)
                        {
                            var qhero = EntityManager.Heroes.Enemies.Where(e => e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe)).Any(e => e.IsInRange(target, Insec.Range() - 50));
                            var qminion =
                                EntityManager.MinionsAndMonsters.EnemyMinions.Where(e => e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe))
                                    .Any(e => e.IsInRange(target, Insec.Range() - 50));
                            var qmob =
                                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                                    .Where(e => e.Buffs.Any(b => b.Name.ToLower().Contains("qone") && b.Caster.IsMe))
                                    .Any(e => e.IsInRange(target, Insec.Range() - 50));
                            if (qhero || qminion || qmob)
                            {
                                LeeSin.Q2.Cast();
                                LastQ = EloBuddy.SDK.Core.GameTickCount;
                            }
                        }
                        else
                        {
                            LeeSin.Q2.Cast();
                            LastQ = EloBuddy.SDK.Core.GameTickCount;
                        }
                    }
                }
            }

            public static void W(Obj_AI_Base target, bool w1, bool W2 = false, bool WardJump = false)
            {
                if (LeeSin.W.IsReady())
                {
                    if (WardJump)
                    {
                        if (W1 && w1 && Wtimer >= Game.Ping && target != null)
                        {
                            Chat.Print("leesin debug: W1");
                            LeeSin.W.Cast(target);
                            LastW = EloBuddy.SDK.Core.GameTickCount;
                        }
                    }
                    if (target.IsValidTarget(LeeSin.W.Range))
                    {
                        if (W1 && w1 && Wtimer >= Game.Ping)
                        {
                            Chat.Print("leesin debug: W1");
                            LeeSin.W.Cast(target);
                            LastW = EloBuddy.SDK.Core.GameTickCount;
                        }

                        if (!W1 && W2)
                        {
                            Chat.Print("leesin debug: W2");
                            LeeSin.W.Cast(target);
                            LastW = EloBuddy.SDK.Core.GameTickCount;
                        }
                    }
                }
            }

            public static void E(Obj_AI_Base target, bool e1 = false, bool E2 = false)
            {
                if (LeeSin.E.IsReady() && target.PredPos(250).IsInRange(user, LeeSin.E.Range))
                {
                    if (E1 && e1 && Etimer > 500)
                    {
                        LeeSin.E.Cast();
                        LastE = EloBuddy.SDK.Core.GameTickCount;
                        return;
                    }

                    if (!E1 && E2)
                    {
                        LeeSin.E.Cast();
                        LastE = EloBuddy.SDK.Core.GameTickCount;
                    }
                }
            }
        }

        private static class EventsHandler
        {
            public static void GameObject_OnCreate(GameObject sender, System.EventArgs args)
            {
                if (sender != null && SpellsManager.W1 && W.IsReady() && sender.Distance(user) <= W.Range && SpellsManager.Wtimer > 1000 && !sender.IsDead && sender.IsAlly && SpellsManager.W1
                    && sender.isWard() && W.IsInRange((Obj_AI_Minion)sender) && (RMenu.keybind("bubba") || JumperMenu.keybind("normal") || MiscMenu.keybind("wardjump")))
                {
                    EloBuddy.SDK.Core.RepeatAction(() => SpellsManager.W((Obj_AI_Base)sender, true), 10, 200);
                    WardJump.lastward = EloBuddy.SDK.Core.GameTickCount;
                }
            }

            public static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
            {
                if (!sender.IsEnemy || !R.IsReady() || sender == null || !sender.IsKillable(R.Range) || !MiscMenu.checkbox("Rgap"))
                {
                    return;
                }

                R.Cast(sender);
            }

            public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
            {
                if (!sender.IsEnemy || sender == null || e == null || !sender.IsKillable(R.Range) || e.DangerLevel < Common.danger(MiscMenu) || !MiscMenu.checkbox("Rint") || !R.IsReady())
                {
                    return;
                }

                R.Cast(sender);
            }

            public static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
            {
                if (!sender.IsEnemy || sender == null || e == null || !sender.IsKillable(R.Range) || e.End == Vector3.Zero || !kCore.GapMenu.checkbox(e.SpellName + sender.ID())
                    || !MiscMenu.checkbox("Rgap") || !R.IsReady() || user.HealthPercent > MiscMenu.slider("Rgaphp"))
                {
                    return;
                }

                R.Cast(sender);
            }

            public static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
            {
                if (sender.Owner.IsMe && args.Slot == SpellSlot.W && EloBuddy.SDK.Core.GameTickCount - SpellsManager.LastW < Game.Ping)
                {
                    if ((JumperMenu.keybind("normal") || RMenu.keybind("bubba") || MiscMenu.keybind("wardjump")) && !SpellsManager.W1)
                    {
                        args.Process = false;
                    }

                    args.Process = false;
                    Chat.Print("leesin debug: wblocked");
                }
            }

            public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    SpellsManager.lastspell = EloBuddy.SDK.Core.GameTickCount;
                    if (args.Slot == SpellSlot.Q)
                    {
                        SpellsManager.LastpQ = EloBuddy.SDK.Core.GameTickCount;
                        if (JumperMenu.keybind("normal"))
                        {
                            if (Insec.Step == Insec.Steps.UseQ && Insec.Pos != null)
                            {
                                EloBuddy.SDK.Core.DelayAction(() => { WardJump.Jump(Insec.Pos); }, 250);
                                Chat.Print("leesin debug: procces WardJump");
                            }
                        }
                    }

                    if (args.Slot == SpellSlot.W)
                    {
                        SpellsManager.LastpW = EloBuddy.SDK.Core.GameTickCount;
                        if (JumperMenu.keybind("normal"))
                        {
                            if (Insec.InsecTarget != null)
                            {
                                if (Insec.Step == Insec.Steps.UseW)
                                {
                                    EloBuddy.SDK.Core.DelayAction(() => { R.Cast(Insec.InsecTarget); }, 100);
                                    Chat.Print("leesin debug: procces R");
                                }
                            }

                            if (Insec.Pos != null)
                            {
                                if (Flash != null && Flash.IsReady() && (Insec.Step == Insec.Steps.UseWF || Insec.Step == Insec.Steps.UseF))
                                {
                                    EloBuddy.SDK.Core.DelayAction(() => { Flash.Cast(Insec.Pos); }, 250);
                                    Chat.Print("leesin debug: procces Flash");
                                }
                            }
                        }
                    }

                    if (args.Slot == SpellSlot.E)
                    {
                        SpellsManager.LastpE = EloBuddy.SDK.Core.GameTickCount;
                    }

                    if (args.Slot == SpellSlot.R)
                    {
                        SpellsManager.LastR = EloBuddy.SDK.Core.GameTickCount;
                        if (Flash != null && EloBuddy.SDK.Core.GameTickCount - BubbaKush.Rflash < 500)
                        {
                            BubbaKush.CastFlash();
                        }

                        if (JumperMenu.keybind("normal") && Insec.Pos != null)
                        {
                            if (!user.IsInRange(Insec.Pos, 200) && Flash != null && Flash.IsReady() && Flash.IsInRange(Insec.Pos))
                            {
                                if (Insec.Step == Insec.Steps.UseR)
                                {
                                    EloBuddy.SDK.Core.DelayAction(() => { Flash.Cast(Insec.Pos); }, 200);
                                    Chat.Print("leesin debug: procces Flash");
                                }
                            }
                        }
                    }

                    if (Flash != null && args.Slot == Flash.Slot)
                    {
                        SpellsManager.LastFlash = EloBuddy.SDK.Core.GameTickCount;
                    }
                }
            }

            public static void Messages_OnMessage(Messages.WindowMessage args)
            {
                var target = EntityManager.Heroes.Enemies.OrderBy(e => e.Distance(Game.CursorPos)).FirstOrDefault(e => e.IsKillable() && e.IsInRange(Game.CursorPos, 100));
                var Ally = EntityManager.Heroes.Allies.OrderBy(e => e.Distance(Game.CursorPos)).FirstOrDefault(e => e.IsValidTarget() && !e.IsDead && e.IsInRange(Game.CursorPos, 200));
                var allyminion =
                    EntityManager.MinionsAndMonsters.AlliedMinions.OrderBy(e => e.Distance(Game.CursorPos)).FirstOrDefault(e => e.IsValidTarget() && !e.IsDead && e.IsInRange(Game.CursorPos, 200));
                var allyturret = EntityManager.Turrets.Allies.OrderBy(e => e.Distance(Game.CursorPos)).FirstOrDefault(e => !e.IsDead && e.IsInRange(Game.CursorPos, 200));
                var allym = allyturret != null && allyminion != null && Ally != null;
                if (args.Message == WindowMessages.LeftButtonDown)
                {
                    if (target != null)
                    {
                        if (MyTarget != null && target.Equals(MyTarget))
                        {
                            MyTarget = null;
                            return;
                        }

                        MyTarget = target;
                    }
                    else
                    {
                        if (Ally != null)
                        {
                            MyAlly = Ally;
                            return;
                        }

                        if (allyminion != null)
                        {
                            MyAlly = allyminion;
                            return;
                        }

                        if (allyturret != null)
                        {
                            MyAlly = allyturret;
                            return;
                        }
                    }

                    if (MyTarget != null && !MyTarget.IsKillable())
                    {
                        MyTarget = null;
                    }

                    if (MyAlly != null && MyAlly.IsDead)
                    {
                        MyAlly = null;
                    }
                }

                if (args.Message == WindowMessages.LeftButtonDoubleClick)
                {
                    if (target == null && !allym)
                    {
                        MyAlly = null;
                        MyTarget = null;
                    }
                }
            }
        }
    }
}
