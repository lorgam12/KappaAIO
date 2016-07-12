using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;
using KappaAIO.Champions;
using KappaAIO.Core;

namespace KappaAIO
{
    internal class Program
    {
        public static Champion[] hero = { Champion.AurelionSol, Champion.Azir, Champion.Brand, Champion.Kindred, Champion.LeeSin, Champion.Malzahar, Champion.Xerath };

        public static string info = "[" + DateTime.Now.ToString("H:mm:ss") + " - Info]";
        public static string warn = "[" + DateTime.Now.ToString("H:mm:ss") + " - Warn]";

        private static void Main(string[] args)
        {
            Console.WriteLine("Loading KappaAIO");
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (hero.Contains(Player.Instance.Hero))
            {
                Chat.Print("<font color='#FFFFFF'><b>KappaAIO Loaded</b></font>");
                CheckVersion.Init();
                var Instance = (Base)Activator.CreateInstance(null, "KappaAIO.Champions." + Player.Instance.Hero).Unwrap();
                DamageLib.DamageDatabase();
                kCore.Execute();
                Console.WriteLine(info + " KappaAIO: " + Player.Instance.Hero + " Loaded !");
                Console.WriteLine(info + " Have Fun !");
                Common.ShowNotification(Player.Instance.ChampionName + " - Loaded", 5000);
                Console.WriteLine(info + " ----------------------------------");
            }
            else
            {
                Common.Logger.Warn("KappaAIO: Failed To Load ! ");
                Common.Logger.Warn("Case: " + Player.Instance.Hero + " Not Supported ");
            }
        }
    }
}
