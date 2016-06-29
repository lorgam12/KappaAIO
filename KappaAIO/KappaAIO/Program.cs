namespace KappaAIO
{
    using System;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK.Events;

    using KappaAIO.Champions;
    using KappaAIO.Core;

    internal class Program
    {
        public static Champion[] hero =
            {
                Champion.AurelionSol, Champion.Azir, Champion.Brand, Champion.Kindred, Champion.LeeSin, Champion.Malzahar,
                Champion.Xerath
            };

        private static void Main(string[] args)
        {
            Console.WriteLine("Loading KappaAIO");
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            var info = "[" + DateTime.Now.ToString("H:mm:ss") + " - Info]";
            var warn = "[" + DateTime.Now.ToString("H:mm:ss") + " - Warn]";

            Chat.Print("<font color='#FFFFFF'><b>KappaAIO Loaded</b></font>");
            if (hero.Contains(Player.Instance.Hero))
            {
                CheckVersion.Init();
                var Instance = (Base)Activator.CreateInstance(null, "KappaAIO.Champions." + Player.Instance.Hero).Unwrap();
                DamageLib.DamageDatabase();
                kCore.Execute();
                Console.WriteLine(info + " KappaAIO: " + Player.Instance.Hero + " Loaded !");
                Console.WriteLine(info + " Have Fun !");
                Common.ShowNotification(Player.Instance.ChampionName + " - Loaded", 5000);
            }
            else
            {
                Console.WriteLine(warn + " KappaAIO: Failed To Load ! ");
                Console.WriteLine(warn + " Case: " + Player.Instance.Hero + " Not Supported ");
            }
            Console.WriteLine(info + " ----------------------------------");
        }
    }
}