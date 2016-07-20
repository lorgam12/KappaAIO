using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using KappaAIO.Core.CommonStuff;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaAIO.Core.Managers
{
    internal static class DrawingsManager
    {
        public static void SpellRange(this Spell.SpellBase spell, Color Colors, bool draw = false)
        {
            if (draw)
            {
                Circle.Draw(spell.IsReady() ? new ColorBGRA(Colors.R, Colors.G, Colors.B, Colors.A) : (ColorBGRA)SharpDX.Color.Red, spell.Range, Player.Instance);
            }
        }

        public static void DrawSpellDamage(this Spell.SpellBase spell, bool draw = false)
        {
            if (draw)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsHPBarRendered && e.IsValidTarget()))
                {
                    if (enemy != null)
                    {
                        var hpx = enemy.HPBarPosition.X;
                        var hpy = enemy.HPBarPosition.Y;
                        var dmg = (int)spell.GetDamage(enemy);
                        var c = Color.GreenYellow;
                        Drawing.DrawText(hpx + 145, hpy, c, dmg + " / " + (int)enemy.TotalShieldHealth(), 3);
                    }
                }
            }
        }

        public static void DrawTotalDamage(List<Spell.SpellBase> list, bool draw = false)
        {
            if (draw)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsHPBarRendered))
                {
                    if (enemy != null)
                    {
                        var totaldmg = (int)enemy.TotalDamage(list);
                        var hpx = enemy.HPBarPosition.X;
                        var hpy = enemy.HPBarPosition.Y;
                        string damage = null;
                        var c = Color.GreenYellow;

                        if (totaldmg >= enemy.TotalShieldHealth() / 2)
                        {
                            damage = "Harass for Kill: ";
                            c = Color.Orange;
                        }

                        if (totaldmg >= Prediction.Health.GetPrediction(enemy, 1000))
                        {
                            damage = "Killable: ";
                            c = Color.Red;
                        }

                        Drawing.DrawText(hpx + 160, hpy, c, damage + totaldmg + " / " + (int)enemy.TotalShieldHealth(), 3);
                    }
                }
            }
        }

        public static void DrawTotalDamage(this Obj_AI_Base target, List<Spell.SpellBase> list, bool draw = false)
        {
            if (draw)
            {
                if (target != null && target.IsHPBarRendered)
                {
                    var hpx = target.HPBarPosition.X;
                    var hpy = target.HPBarPosition.Y;
                    var dmg = (int)target.TotalDamage(list);
                    var c = Color.GreenYellow;
                    Drawing.DrawText(hpx + 145, hpy, c, dmg + " / " + (int)target.TotalShieldHealth(), 3);
                }
            }
        }

        public static void drawLine(Vector3 from, Vector3 to, int width, Color color)
        {
            var wts1 = Drawing.WorldToScreen(from);
            var wts2 = Drawing.WorldToScreen(to);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], width, color);
        }
    }
}
