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
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;
using SharpDX;

namespace KappaAIO.Champions
{
    // why are you looking at my codez ( ͡° ͜ʖ ͡°)
    internal class Yasuo : Base
    {
        public static bool BeforeImpact
        {
            get
            {
                return LowestKnockUpTime < 125 + Game.Ping && LowestKnockUpTime > 0;
            }
        }
        public static float LowestKnockUpTime
        {
            get
            {
                var buffs =
                    EntityManager.Heroes.Enemies.Where(e => e.IsKillable(R.Range) && e.IsAirborne())
                        .Select(a => a.Buffs.FirstOrDefault(b => b.Type == BuffType.Knockback || b.Type == BuffType.Knockup))
                        .OrderBy(b => b.EndTime - Game.Time);
                var buff = buffs.FirstOrDefault();
                return buff != null ? (buff.EndTime - Game.Time) * 1000 : 1000;
            }
        }

        public static Geometry.Polygon.Sector ESector(Obj_AI_Base target)
        {
            return new Geometry.Polygon.Sector(user.ServerPosition, target.PredPos(250).To3D(), (float)(60 * Math.PI / 180), 475);
        }

        public static Geometry.Polygon.Sector ESector(Vector3 target)
        {
            return new Geometry.Polygon.Sector(user.ServerPosition, target, (float)(60 * Math.PI / 180), 475);
        }

        public static uint QRange
        {
            get
            {
                return user.IsDashing() ? 375 : (uint)(Q3 ? 1000 : 475);
            }
        }
        public static int QWidth
        {
            get
            {
                return Q3 ? 100 : 50;
            }
        }
        private static bool Q3
        {
            get
            {
                return user.HasBuff("YasuoQ3W");
            }
        }
        private static readonly Spell.Skillshot Q;
        private static readonly Spell.Skillshot W;
        private static readonly Spell.Targeted E;
        private static readonly Spell.Active R;
        private static readonly Dictionary<Vector3, string> JumpSpots = new Dictionary<Vector3, string>();

        static Yasuo()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                //Blue team
                //Inside
                JumpSpots.Add(new Vector3(8299, 2662, 51), "SRU_KrugMini5.1.1"); //Krugsmall front
                JumpSpots.Add(new Vector3(8541, 2679, 50), "SRU_Krug5.1.2"); //Krugbig front
                JumpSpots.Add(new Vector3(7620, 4120, 54), "SRU_RedMini4.1.2"); //Redsmall front
                JumpSpots.Add(new Vector3(7850, 3930, 54), "SRU_RedMini4.1.3"); //Redsmall2 front
                JumpSpots.Add(new Vector3(6896, 5530, 55), "SRU_RazorbeakMini3.1.2"); //Birdsmall front
                JumpSpots.Add(new Vector3(3740, 6538, 52), "SRU_MurkwolfMini2.1.3"); //Wolfsmall front
                JumpSpots.Add(new Vector3(3916, 6430, 52), "SRU_MurkwolfMini2.1.2"); //Wolfsmall2 front
                JumpSpots.Add(new Vector3(3728, 8078, 51), "SRU_BlueMini1.1.2"); //Bluesmall front
                JumpSpots.Add(new Vector3(3610, 7928, 53), "SRU_BlueMini21.1.3"); //Bluesmall2 front
                JumpSpots.Add(new Vector3(3854, 7928, 51), "SRU_Blue1.1.1"); //Bluebig front
                JumpSpots.Add(new Vector3(2260, 8404, 51), "SRU_Gromp13.1.1"); //Gromp front
                //Outside
                JumpSpots.Add(new Vector3(8172, 3158, 51), "SRU_KrugMini5.1.1"); //Krugsmall Back
                JumpSpots.Add(new Vector3(8272, 3608, 53), "SRU_RedMini4.1.3"); //Redsmall2 Back
                JumpSpots.Add(new Vector3(6424, 5258, 48), "SRU_Razorbeak3.1.1"); //Birdsbig back
                JumpSpots.Add(new Vector3(7224, 5958, 52), "SRU_RazorbeakMini3.1.2"); //Birdsmall3 Back
                JumpSpots.Add(new Vector3(3674, 7058, 50), "SRU_MurkwolfMini2.1.3"); //Wolfsmall Back
                JumpSpots.Add(new Vector3(4324, 6258, 51), "SRU_MurkwolfMini2.1.2"); //Wolfsmall2 Back
                JumpSpots.Add(new Vector3(3624, 7408, 51), "SRU_BlueMini21.1.3"); //Bluesmall2 Back
                JumpSpots.Add(new Vector3(1674, 8356, 52), "SRU_Gromp13.1.1"); //Gromp Back

                //Red team
                //Inside
                JumpSpots.Add(new Vector3(6548, 12236, 56), "SRU_KrugMini11.1.1"); //Krugsmall front
                JumpSpots.Add(new Vector3(6324, 12256, 56), "SRU_Krug11.1.2"); //Krugbig front
                JumpSpots.Add(new Vector3(6980, 10978, 56), "SRU_RedMini10.1.3"); //Redsmall front
                JumpSpots.Add(new Vector3(7238, 10844, 56), "SRU_RedMini10.1.2"); //Redsmall2 front
                JumpSpots.Add(new Vector3(7906, 9382, 52), "SRU_RazorbeakMini9.1.2"); //Birdsmall front
                JumpSpots.Add(new Vector3(10868, 8354, 62), "SRU_MurkwolfMini8.1.3"); //Wolfsmall front
                JumpSpots.Add(new Vector3(11052, 8334, 61), "SRU_MurkwolfMini8.1.2"); //Wolfsmall2 front
                JumpSpots.Add(new Vector3(11188, 6990, 51), "SRU_BlueMini7.1.2"); //Bluesmall front
                JumpSpots.Add(new Vector3(10974, 7010, 51), "SRU_Blue7.1.1"); //Bluebig front
                JumpSpots.Add(new Vector3(12578, 6436, 51), "SRU_Gromp14.1.1"); //Gromp front
                //Outside
                JumpSpots.Add(new Vector3(6624, 11746, 53), "SRU_KrugMini11.1.1"); //Krugsmall Back
                JumpSpots.Add(new Vector3(6540, 11242, 56), "SRU_RedMini10.1.3"); //Redsmall Back
                JumpSpots.Add(new Vector3(8372, 9606, 50), "SRU_Razorbeak9.1.1"); //Birdsbig back
                JumpSpots.Add(new Vector3(7672, 8906, 52), "SRU_RazorbeakMini9.1.2"); //Birdsmall Back
                JumpSpots.Add(new Vector3(10372, 8506, 63), "SRU_MurkwolfMini8.1.3"); //Wolfsmall Back
                JumpSpots.Add(new Vector3(11122, 7806, 52), "SRU_MurkwolfMini8.1.2"); //Wolfsmall2 Back
                JumpSpots.Add(new Vector3(11122, 7506, 52), "SRU_BlueMini7.1.2"); //Bluesmall Back
                JumpSpots.Add(new Vector3(13172, 6408, 54), "SRU_Gromp14.1.1"); //Gromp Back
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 475, SkillShotType.Linear, 250, int.MaxValue, 50);
            W = new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Linear, 250, int.MaxValue, 150);
            E = new Spell.Targeted(SpellSlot.E, 475);
            R = new Spell.Active(SpellSlot.R, 1200);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            Menuini = MainMenu.AddMenu("Yasuo", "Yasuo");
            AutoMenu = Menuini.AddSubMenu("Auto");
            ComboMenu = Menuini.AddSubMenu("Combo");
            JumperMenu = Menuini.AddSubMenu("Flee");
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("Color Picker");

            Menuini.Add("Qhit", new ComboBox("Q HitChance", 0, "High", "Medium", "Low"));

            AutoMenu.CreateCheckBox("Raoe", "Use AUTO R AOE");
            AutoMenu.CreateSlider("Rhits", "Auto AOE R Hits {0}", 3, 1, 6);

            ComboMenu.CreateCheckBox("Q", "Use Q");
            ComboMenu.CreateCheckBox("Q3", "Use Q3");
            ComboMenu.CreateCheckBox("E", "Use E");
            ComboMenu.CreateCheckBox("R", "Use R Finisher");
            ComboMenu.CreateCheckBox("RCombo", "Use R For Combo Damage");
            ComboMenu.CreateCheckBox("Raoe", "Use R AoE");
            ComboMenu.CreateSlider("RHits", "R Hit {0} Enemies", 2, 1, 6);
            ComboMenu.AddSeparator(0);
            ComboMenu.AddGroupLabel("Advanced Settings");
            ComboMenu.CreateCheckBox("EQ", "E > Q");
            ComboMenu.CreateCheckBox("EQ3", "E > Q3");
            ComboMenu.CreateCheckBox("Egap", "E GapClose To Selected Target");
            ComboMenu.CreateCheckBox("Edive", "E Dive Towers", false);

            JumperMenu.CreateKeyBind("flee", "Flee Across all units", false, KeyBind.BindTypes.HoldActive, 'A');
            JumperMenu.CreateKeyBind("wall", "Wall Jump", false, KeyBind.BindTypes.HoldActive, 'S');
            //JumperMenu.CreateKeyBind("wall2", "Wall Jump OUTSIDE Camp > IN Camp", false, KeyBind.BindTypes.HoldActive, 'Z');

            foreach (var spell in SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }
            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");

            KappaEvade.KappaEvade.Init();
            //Messages.OnMessage += Messages_OnMessage;
        }

        private static void Messages_OnMessage(Messages.WindowMessage args)
        {
            if (args.Message == WindowMessages.LeftButtonDown)
            {
                Chat.Print(user.ServerPosition);
            }
        }

        private static void WallJump()
        {
            foreach (var spot in JumpSpots.OrderBy(s => s.Key.Distance(Game.CursorPos)))
            {
                if (spot.Key.IsInRange(Game.CursorPos, 300))
                {
                    Chat.Print("IsInRange");
                    var mob = EntityManager.MinionsAndMonsters.GetJungleMonsters().FirstOrDefault(j => j.Name == spot.Value && j.CanDash() && j.IsValidTarget() && spot.Key.IsInRange(j, E.Range));
                    if (mob != null)
                    {
                        Chat.Print("mob != null");
                        Chat.Print(mob.IsValidTarget(E.Range) && !mob.EndPos().IsWall() && mob.ServerPosition.Extend(user, 200).To3D().IsWall());
                        if ((mob.IsValidTarget(225) && mob.EndPos().IsWall() && !mob.EndPos().Extend(user, -140).IsWall())
                            || (mob.IsValidTarget(E.Range) && !mob.EndPos().IsWall() && mob.ServerPosition.Extend(user, 200).To3D().IsWall()))
                        {
                            Chat.Print("E.Cast(mob);");
                            E.Cast(mob);
                            return;
                        }
                        Orbwalker.OrbwalkTo(spot.Key);
                    }
                }
                else
                {
                    Orbwalker.OrbwalkTo(Game.CursorPos);
                }
            }
        }

        public override void Active()
        {
            UpdateSpells();
            RAOE(AutoMenu.checkbox("Raoe"), AutoMenu.slider("Rhits"));

            foreach (var obj in ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.EndPos().Distance(Game.CursorPos)).Where(o => o.IsValidTarget(E.Range) && !o.isWard() && o.CanDash() && E.IsReady()))
            {
                if (JumperMenu.keybind("flee") && !obj.EndPos().IsWall() && new Geometry.Polygon.Sector(user.ServerPosition, Game.CursorPos, (float)(60 * Math.PI / 180), 475).IsInside(obj))
                {
                    E.Cast(obj);
                    return;
                }
            }
            if (JumperMenu.keybind("wall"))
            {
                WallJump();
            }
        }

        public override void Combo()
        {
            RAOE(ComboMenu.checkbox("Raoe"), ComboMenu.slider("Rhits"));

            var target = TargetSelector.GetTarget(Q.Range + 100, DamageType.Physical);
            var selected = TargetSelector.SelectedTarget;
            if (selected != null && selected.IsValidTarget())
            {
                target = selected;
            }

            if (ComboMenu.checkbox("E") && target != null && !target.IsValidTarget(user.GetAutoAttackRange()))
            {
                if (ComboMenu.checkbox("Egap"))
                {
                    foreach (var obj in ObjectManager.Get<Obj_AI_Base>().OrderBy(e => e.EndPos().Distance(target)).Where(e => e != null && ESector(target).IsInside(e) && e.IsValidTarget()))
                    {
                        if (obj == target)
                        {
                            if(obj.EndPos().IsInRange(target.PredPos(250), user.GetAutoAttackRange()))
                            EQ(obj, target, ComboMenu.checkbox("EQ") || (ComboMenu.checkbox("EQ3") && Q3), ComboMenu.checkbox("Edive"));
                        }
                        else
                        {
                            EQ(obj, target, ComboMenu.checkbox("EQ") || (ComboMenu.checkbox("EQ3") && Q3), ComboMenu.checkbox("Edive"));
                        }
                    }
                }
                else
                {
                    EQ(target, target, ComboMenu.checkbox("EQ") || ComboMenu.checkbox("EQ3") && Q3, ComboMenu.checkbox("Edive"));
                }
            }

            if (target != null && target.IsKillable(Q.Range + 25))
            {
                if (Q3 && Q.IsReady() && target.IsKillable(Q.Range))
                {
                    Q3AOE(target, 2);
                }

                if ((ComboMenu.checkbox("Q") || ComboMenu.checkbox("Q3") && Q3) && !user.IsDashing() && target.IsKillable(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target, Q.hitchance(Menuini));
                }

                if (ComboMenu.checkbox("E") && E.IsReady())
                {
                    EQ(target, target, ComboMenu.checkbox("EQ") || ComboMenu.checkbox("EQ3") && Q3, ComboMenu.checkbox("Edive"));
                }

                if (BeforeImpact && target.IsKillable(R.Range) && R.IsReady() && target.IsAirborne() && ((ComboMenu.checkbox("R") && BeforeImpact && R.GetDamage(target) >= target.Health) || (ComboMenu.checkbox("RCombo") && target.TotalDamage(SpellList) + user.GetAutoAttackDamage(target, true) >= target.Health)))
                {
                    R.Cast();
                }
            }
        }

        public override void Harass()
        {
        }

        public override void LaneClear()
        {
        }

        public override void JungleClear()
        {
        }

        public override void KillSteal()
        {
        }

        public override void Draw()
        {
            var target = TargetSelector.GetTarget(Q.Range + 1000, DamageType.Physical);
            if (target != null)
            {
                new Geometry.Polygon.Rectangle(user.ServerPosition, user.ServerPosition.Extend(Q.GetPrediction(target).CastPosition, Q.Range).To3D(), Q.Width).Draw(System.Drawing.Color.AliceBlue, 2);
            }
            foreach (var spot in JumpSpots.Select(s => s.Key))
            {
                Circle.Draw(Color.AliceBlue, 100, spot);
            }

            ESector(Game.CursorPos).Draw(System.Drawing.Color.AliceBlue, 2);

            foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(1000) && o.IsEnemy))
            {
                if (obj != null)
                {
                    Circle.Draw(Color.White, 100, obj.EndPos().IsWall() && !obj.EndPos().Extend(user, -135).IsWall() ? obj.EndPos().Extend(user, -145).To3D() : obj.EndPos());
                }
            }

            /*if (DashEnd != null)
                Circle.Draw(Color.White, 100, DashEnd);*/
        }

        public static void UpdateSpells()
        {
            Q.Range = QRange;
            Q.Width = QWidth;
            var attackspeed = Player.Instance.AttackSpeedMod / 2;
            var reduceddelay = 250 * (attackspeed * 0.017f) * 0.1f * 100;
            Q.CastDelay = (int)(250 - reduceddelay);
            Q.Speed = Q3 ? 1150 : int.MaxValue;
        }

        public static void Q3AOE(Obj_AI_Base target, int HitNumber)
        {
            if (!user.IsDashing() && Q3 && EntityManager.Heroes.Enemies.Count(
                e => e.IsKillable() && new Geometry.Polygon.Rectangle(user.ServerPosition, user.ServerPosition.Extend(Q.GetPrediction(target).CastPosition, Q.Range).To3D(), Q.Width).IsInside(Q.GetPrediction(e).CastPosition)) >= HitNumber)
            {
                Q.Cast(target);
            }
        }

        public static void EQ(Obj_AI_Base target1, Obj_AI_Base target2, bool Enabled, bool dive)
        {
            if (ECast(target1, dive) && E.IsReady() && target1.CanDash())
            {
                if (Enabled && target2 != null && Q.IsReady() && target2.IsInRange(target1.EndPos(), 375))
                {
                    Q.Cast(target2);
                }
            }
            if (Enabled && target2 != null && Q.IsReady() && user.IsDashing())
            {
                Q.Cast(target2);
            }
        }

        public static bool ECast(Obj_AI_Base target, bool dive, bool advDive = false)
        {
            if ((target.EndPos().UnderTurret() && dive) || !target.EndPos().UnderTurret())
            {
                return E.Cast(target);
            }
            return false;
        }

        public static void RAOE(bool Enable, int HitsNumber)
        {
            if (EntityManager.Heroes.Enemies.Count(e => e.IsAirborne() && e.IsKillable(R.Range)) >= HitsNumber && Enable && BeforeImpact && R.IsReady())
            {
                R.Cast();
            }
        }
    }
}
