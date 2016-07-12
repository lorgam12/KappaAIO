using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace KappaAIO.Core
{
    internal class kCore
    {
        public static Menu CoreMenu, GapMenu, ks;

        public static readonly string[] Junglemobs =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug",
                "SRU_Razorbeak", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "SRU_RiftHerald", "TT_NWraith", "TT_NWolf", "TT_NGolem", "TT_Spiderboss",
                "AscXerath"
            };

        public static void Execute()
        {
            foreach (var ward in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(w => w.Name.ToLower().Contains("ward") && w != null && w.IsAlly && w.IsValid && w.Health > 0 && !w.IsDead && !w.Name.ToLower().Contains("wardcorpse")))
            {
                Managers.ObjectsManager.Wards.Add(ward);
            }

            CoreMenu = MainMenu.AddMenu("KappaCore", "KappaCore");
            GapMenu = CoreMenu.AddSubMenu("Anti-GapCloser Settings");
            ks = CoreMenu.AddSubMenu("Stealer");

            GapMenu.AddGroupLabel("Anti GapCloser Champions");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                foreach (var spell in Gapcloser.GapCloserList.Where(e => enemy.ChampionName == e.ChampName))
                {
                    GapMenu.Add(spell.SpellName + enemy.ID(), new CheckBox(spell.ChampName + " " + spell.SpellSlot));
                }
            }

            ks.AddGroupLabel("KillSteal Champions");
            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                ks.Add(hero.ID(), new CheckBox(hero.ChampionName + " (" + hero.Name + ")"));
            }

            ks.AddSeparator(1);
            ks.AddGroupLabel("JungleSteal Mobs");
            foreach (var mob in Junglemobs)
            {
                ks.Add(mob, new CheckBox(mob));
            }

            GameObject.OnCreate += delegate(GameObject sender, EventArgs args)
                {
                    if (sender.Name.ToLower().Contains("ward") && sender.IsAlly && !sender.Name.ToLower().Contains("wardcorpse"))
                    {
                        Managers.ObjectsManager.Wards.Add((Obj_AI_Minion)sender);
                    }
                };
            GameObject.OnDelete += delegate(GameObject sender, EventArgs args)
                {
                    if (sender.Name.ToLower().Contains("ward") && sender.IsAlly && !sender.Name.ToLower().Contains("wardcorpse"))
                    {
                        Managers.ObjectsManager.Wards.Remove((Obj_AI_Minion)sender);
                    }
                };
        }
    }
}
