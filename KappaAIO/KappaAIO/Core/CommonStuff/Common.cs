using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Notifications;
using KappaAIO.Core.Managers;
using SharpDX;

namespace KappaAIO.Core.CommonStuff
{
    public static class Common
    {
        #region numbers
        public static int lastNotification = 0;

        public static float TravelTime(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return ((target.Distance(Player.Instance) / spell.Handle.SData.MissileSpeed) * 1000) + spell.CastDelay + (Game.Ping / 2);
        }

        public static int CountEnemeis(this Obj_AI_Base target, float range)
        {
            try
            {
                return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.ServerPosition) && e.IsKillable());
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int CountEnemeis(this GameObject target, float range)
        {
            try
            {
                return EntityManager.Heroes.Enemies.Count(e => e.IsValidTarget(range, true, target.Position) && e.IsKillable());
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int CountAllies(this Obj_AI_Base target, float range)
        {
            try
            {
                return EntityManager.Heroes.Allies.Count(e => e.IsValidTarget(range, false, target.ServerPosition) && e.IsKillable());
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int CountEnemyMinions(this Obj_AI_Base target, float range)
        {
            try
            {
                return EntityManager.MinionsAndMonsters.EnemyMinions.Count(m => m.IsKillable() && m.IsInRange(target, range));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int CountAllyMinions(this Obj_AI_Base target, float range)
        {
            try
            {
                return EntityManager.MinionsAndMonsters.AlliedMinions.Count(m => m.IsKillable() && m.IsInRange(target, range));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int combobox(this Menu m, string id)
        {
            return m[id].Cast<ComboBox>().CurrentValue;
        }

        public static int slider(this Menu m, string id)
        {
            return m[id].Cast<Slider>().CurrentValue;
        }

        public static float Mana()
        {
            try
            {
                var mana = 0f;
                var Q = Player.GetSpell(SpellSlot.Q);
                var W = Player.GetSpell(SpellSlot.W);
                var E = Player.GetSpell(SpellSlot.E);
                var R = Player.GetSpell(SpellSlot.R);
                if (Q.IsReady)
                {
                    // Q mana
                    mana += Q.SData.Mana;
                }

                if (W.IsReady)
                {
                    // W mana
                    mana += W.SData.Mana;
                }

                if (E.IsReady)
                {
                    // E mana
                    mana += E.SData.Mana;
                }

                if (R.IsReady)
                {
                    // R mana
                    mana += R.SData.Mana;
                }

                return mana;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static float Mana(this Spell.SpellBase spell)
        {
            return spell.Handle.SData.Mana;
        }

        public static float PredHP(this Obj_AI_Base target, int time)
        {
            try
            {
                return Prediction.Health.GetPrediction(target, time);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }

        public static int countpassive(this Obj_AI_Base target)
        {
            try
            {
                return target.GetBuffCount("BrandAblaze");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return 0;
        }
        #endregion numbers

        #region bools
        public static bool UnderTurret(this Vector3 pos, bool EnemyTurret = true)
        {
            return EnemyTurret
                       ? EntityManager.Turrets.Enemies.Any(t => t.IsInRange(pos, t.GetAutoAttackRange() + 25) && !t.IsDead)
                       : EntityManager.Turrets.Allies.Any(t => t.IsInRange(pos, t.GetAutoAttackRange() + 25) && !t.IsDead);
        }

        public static bool checkbox(this Menu m, string id)
        {
            return m[id].Cast<CheckBox>().CurrentValue;
        }

        public static bool HasYasuoEBuff(this Obj_AI_Base target)
        {
            return target.HasBuff("YasuoDashWrapper");
        }

        public static bool keybind(this Menu m, string id)
        {
            return m[id].Cast<KeyBind>().CurrentValue;
        }

        public static bool Mana(this Spell.SpellBase spell, Menu m)
        {
            try
            {
                return ObjectManager.Player.ManaPercent > m.slider(spell.Slot + "mana");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool IsKillable(this Obj_AI_Base target)
        {
            try
            {
                return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")
                       && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                       && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity) && target.IsValidTarget();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool IsKillable(this Obj_AI_Base target, float range)
        {
            try
            {
                return !target.HasBuff("kindredrnodeathbuff") && !target.Buffs.Any(b => b.Name.ToLower().Contains("fioraw")) && !target.HasBuff("JudicatorIntervention")
                       && !target.HasBuff("ChronoShift") && !target.HasBuff("UndyingRage") && !target.IsInvulnerable && !target.IsZombie && !target.HasBuff("bansheesveil") && !target.IsDead
                       && !target.IsPhysicalImmune && target.Health > 0 && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.PhysicalImmunity)
                       && target.IsValidTarget(range);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool IsAirborne(this Obj_AI_Base target)
        {
            try
            {
                return target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool isWard(this GameObject obj)
        {
            try
            {
                return obj.Name.ToLower().Contains("ward") && !obj.Name.ToLower().Contains("wardcorpse");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool IsCC(this Obj_AI_Base target)
        {
            try
            {
                return target.Spellbook.IsChanneling || !target.CanMove || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Knockup)
                       || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Taunt);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool brandpassive(this Obj_AI_Base target)
        {
            try
            {
                return target.HasBuff("BrandAblaze");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        public static bool orbmode(Orbwalker.ActiveModes mode)
        {
            try
            {
                return Orbwalker.ActiveModesFlags.HasFlag(mode);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return false;
        }

        #endregion bools

        #region MenuStuff
        public static KeyBind CreateKeyBind(this Menu m, string id, string name, bool defaultvalue, KeyBind.BindTypes bindType, uint defaultvalue1 = 27U)
        {
            return m.Add(id, new KeyBind(name, defaultvalue, bindType, defaultvalue1));
        }

        public static CheckBox CreateCheckBox(this Menu m, string id, string name, bool defaultvalue = true)
        {
            return m.Add(id, new CheckBox(name, defaultvalue));
        }

        public static Slider CreateSlider(this Menu m, string id, string name, int defaultvalue = 0, int MinValue = 0, int MaxValue = 0)
        {
            return m.Add(id, new Slider(name, defaultvalue, MinValue, MaxValue));
        }
        #endregion MenuStuff
        
        public static AIHeroClient ally;

        public static Obj_AI_Turret tower;

        internal class Logger
        {
            public static void Error(string error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "KappaAIO] Error: " + error);
                Console.ResetColor();
            }

            public static void Error(Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "KappaAIO] Error: " + error);
                Console.ResetColor();
            }

            public static void Info(string info)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "KappaAIO] Info: " + info);
                Console.ResetColor();
            }

            public static void Warn(string Warn)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "KappaAIO] Warn: " + Warn);
                Console.ResetColor();
            }
        }
        
        public static DangerLevel danger(Menu m)
        {
            try
            {
                switch (m.combobox("Danger"))
                {
                    case 0:
                        {
                            return DangerLevel.High;
                        }

                    case 1:
                        {
                            return DangerLevel.Medium;
                        }

                    case 2:
                        {
                            return DangerLevel.Low;
                        }
                    default:
                        return DangerLevel.Low;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return DangerLevel.Low;
        }
        
        public static System.Drawing.Color Color(this Menu m, string id)
        {
            try
            {
                return m[id].Cast<ColorPicker>().CurrentValue;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return new System.Drawing.Color();
        }
        
        public static string ID(this Obj_AI_Base target)
        {
            try
            {
                return target.BaseSkinName + target.NetworkId;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return string.Empty;
        }
        
        public static Vector2 PredPos(this Obj_AI_Base target, int time)
        {
            try
            {
                return Prediction.Position.PredictUnitPosition(target, time);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            return new Vector2();
        }
        
        public static void ShowNotification(string message, int duration = -1, string message2 = "New Notification")
        {
            try
            {
                Notifications.Show(new SimpleNotification(message, message2), duration);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static void Log(string message, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0)
        {
            try
            {
                Console.WriteLine("{0} ({1}): {2}", Path.GetFileName(file), line, message);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static class LastCastedSpell
        {
            public static float Time;

            public static SpellSlot Spell;

            public static string Name;
        }
    }
}
