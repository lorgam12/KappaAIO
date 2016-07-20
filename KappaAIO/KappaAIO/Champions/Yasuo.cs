using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using KappaAIO.Core.CommonStuff;
using KappaAIO.KappaEvade;
using KappaAIO.Core.Managers;
using SharpDX;
using static KappaAIO.Core.Managers.DashManager;

namespace KappaAIO.Champions
{
    // why are you looking at my codez ( ͡° ͜ʖ ͡°)
    internal class Yasuo : Base
    {
        public static uint QRange
        {
            get
            {
                return (uint)(Q3 ? 1000 : 475);
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
        private static Spell.Skillshot Q;
        private static Spell.Skillshot W;
        private static Spell.Targeted E;
        private static Spell.Active R;
        private static Dictionary<Vector3, string> JumpSpots = new Dictionary<Vector3, string>();
        /*private static List<Vector3> JumpSpots = new List<Vector3>()
                                                     {
                                                         //Blue team
                                                         new Vector3(8299, 2662, 51), //Krugsmall front
                                                         new Vector3(8541, 2679, 50),  //Krugsmall front
                                                         //new Vector3(8220, 3160, 52), //Krugsmall back
                                                         new Vector3(7620, 4120, 54), //Redsmall front
                                                         new Vector3(7850, 3930, 54), //Redsmall2 front
                                                         new Vector3(6836, 5488, 54), //Birdsmall front
                                                         new Vector3(3897, 6479, 52), //Wolfsmall front
                                                         new Vector3(3721, 6542, 52), //Wolfsmall2 front
                                                         new Vector3(3714, 8073, 51), //Bluesmall front
                                                         new Vector3(3833, 7931, 52), //Bluebig front
                                                         new Vector3(2249, 8425, 51), //Gromp front
                                                         //Red team
                                                         new Vector3(6528, 12250, 56), //Krugsmall front
                                                         new Vector3(6325, 12253, 56), //Krugbig front
                                                         new Vector3(6980, 10978, 56), //Redsmall front
                                                         new Vector3(7244, 10878, 56), //Redsmall2 front
                                                         new Vector3(7913, 9412, 52), //Birdsmall front
                                                         new Vector3(10867, 8363, 62), //Wolfsmall front
                                                         new Vector3(11081, 8312, 61), //Wolfsmall2 front
                                                         new Vector3(10974, 7010, 51), //Bluebig front
                                                         new Vector3(12575, 6380, 51), //Gromp front
                                                     };*/

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
            DrawMenu = Menuini.AddSubMenu("Drawings Settings");
            ColorMenu = Menuini.AddSubMenu("Color Picker");

            Menuini.Add("orb", new CheckBox("Orbwalker While Holding flee key"));
            Menuini.Add("spot", new CheckBox("Orbwalk To Closest Jump Spot"));
            Menuini.Add("spots", new CheckBox("Only Accurate Wall jumps"));
            Menuini.CreateKeyBind("flee", "Flee Across all units", false, KeyBind.BindTypes.HoldActive, 'A');
            Menuini.CreateKeyBind("wall", "Wall Jump INISDE Camp > OUT Camp", false, KeyBind.BindTypes.HoldActive, 'S');
           // Menuini.CreateKeyBind("wall2", "Wall Jump OUTSIDE Camp > IN Camp", false, KeyBind.BindTypes.HoldActive, 'Z');

            foreach (var spell in SpellList)
            {
                DrawMenu.Add(spell.Slot.ToString(), new CheckBox(spell.Slot + " Range"));
                ColorMenu.Add(spell.Slot.ToString(), new ColorPicker(spell.Slot + " Color", System.Drawing.Color.Chartreuse));
            }
            DrawMenu.Add("damage", new CheckBox("Draw Combo Damage"));
            DrawMenu.AddLabel("Draws = ComboDamage / Enemy Current Health");

            //KappaEvade.KappaEvade.Init();
            Messages.OnMessage += Messages_OnMessage;
        }

        private static void Messages_OnMessage(Messages.WindowMessage args)
        {
            if (args.Message == WindowMessages.LeftButtonDown)
            {
                Chat.Print(user.ServerPosition);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            var hero = args.Target as AIHeroClient;
            if (caster == null || !caster.IsEnemy || hero == null || !hero.IsMe || !W.IsReady()) return;

            if (Database.TargetedSpells.TargetedSpellsList.Any(s => s.hero == caster.Hero && s.slot == args.Slot))
            {
                var spell = Database.TargetedSpells.TargetedSpellsList.FirstOrDefault(s => s.hero == caster.Hero && s.slot == args.Slot);
                if (EvadeMenu.slider(caster.ID() + spell.slot + "dl") >= EvadeMenu.slider("dl"))
                {
                    var impact = (args.Start.Distance(user) / args.SData.MissileSpeed) + (250 - Game.Ping);
                    var delay = EvadeMenu.checkbox("impact") ? impact : EvadeMenu.checkbox("rnd") ? new Random().Next(EvadeMenu.slider("min"), EvadeMenu.slider("max")) : EvadeMenu.slider("max");
                    Chat.Print(delay);
                    EloBuddy.SDK.Core.DelayAction(() => W.Cast(user.ServerPosition.Extend(args.Start, 200).To3D()), (int)delay);
                }
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
                        if ((mob.IsValidTarget(225) && mob.EndPos().IsWall() && !mob.EndPos().Extend(user, -140).IsWall()) || (mob.IsValidTarget(E.Range) && !mob.EndPos().IsWall() && mob.ServerPosition.Extend(user, 200).To3D().IsWall()))
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
            /*
                var spot = JumpSpots.OrderBy(s => s.Distance(Game.CursorPos)).FirstOrDefault(s => s.Distance(Game.CursorPos) <= 1000 && ObjectManager.Get<Obj_AI_Minion>().Count(m => m.IsValidTarget() && m.IsInRange(s, 100)) > 0);
                if (Menuini.keybind("wall") || Menuini.keybind("flee"))
                {
                    if (spot != null && spot.Distance(user) < 2500 && Menuini.checkbox("spot") && EloBuddy.SDK.Core.GameTickCount - laste > 1000 && Menuini.keybind("wall"))
                    {
                        Orbwalker.OrbwalkTo(spot.To2D().To3DWorld());
                    }
                    else
                    {
                        if (Menuini.checkbox("orb"))
                        {
                            Orbwalker.OrbwalkTo(Game.CursorPos);
                        }
                    }
                }
                foreach (var obj in ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.EndPos().Distance(Game.CursorPos)).Where(o => o.IsValidTarget(E.Range) && !o.isWard() && o.CanDash() && E.IsReady()))
                {
                    if (Menuini.keybind("wall") && EloBuddy.SDK.Core.GameTickCount - laste > 1000)
                    {
                        if ((obj.EndPos().IsWall() && !obj.EndPos().Extend(user, -135).IsWall())/* || (!obj.EndPos().IsWall() && obj.ServerPosition.Extend(user, 250).IsWall()))
                        {
                            if (Menuini.checkbox("spots"))
                            {
                                if (user.IsInRange(spot, 50))
                                {
                                    E.Cast(obj);
                                    laste = EloBuddy.SDK.Core.GameTickCount;
                                    return;
                                }
                            }
                            E.Cast(obj);
                            laste = EloBuddy.SDK.Core.GameTickCount;
                            return;
                        }
                    }
                }*/
        }

        public override void Active()
        {
            Q.Range = QRange;
            Q.Width = QWidth;

            foreach (var obj in ObjectManager.Get<Obj_AI_Base>().OrderBy(o => o.EndPos().Distance(Game.CursorPos)).Where(o => o.IsValidTarget(E.Range) && !o.isWard() && o.CanDash() && E.IsReady()))
            {
                if (Menuini.keybind("flee") && !obj.EndPos().IsWall() && new Geometry.Polygon.Sector(user.ServerPosition, Game.CursorPos, (float)(60 * Math.PI / 180), 475).IsInside(obj))
                {
                    E.Cast(obj);
                    return;
                }
            }
            if (Menuini.keybind("wall"))
            {
                WallJump();
            }
        }

        public override void Combo()
        {
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
            foreach (var spot in JumpSpots.Select(s => s.Key))
            {
                Circle.Draw(Color.AliceBlue, 100, spot);
            }

            new Geometry.Polygon.Sector(user.ServerPosition, Game.CursorPos, (float)(60 * Math.PI / 180), 475).Draw(System.Drawing.Color.AliceBlue, 2);

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
    }
}
