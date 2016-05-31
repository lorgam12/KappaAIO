namespace KappaAIO
{
    using System;

    using EloBuddy;
    using EloBuddy.SDK.Events;

    using KappaAIO.Champions;

    using SharpDX;

    internal class Program
    {
        internal static bool loaded;

        private static void Main(string[] args)
        {
            Console.WriteLine("Loading KappaAIO...");
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            var info = "[" + DateTime.Now.ToString("h:mm:ss") + " - Info]";
            var warn = "[" + DateTime.Now.ToString("h:mm:ss") + " - Warn]";

            Chat.Print("<font color='#FFFFFF'><b>KappaAIO Loaded</b></font>");
            switch (Player.Instance.Hero)
            {
                case Champion.AurelionSol:
                    AurelionSol.Execute();
                    loaded = true;
                    break;
                case Champion.Azir:
                    Azir.Execute();
                    loaded = true;
                    break;
                case Champion.Brand:
                    Brand.Execute();
                    loaded = true;
                    break;
                case Champion.Kindred:
                    Kindred.Execute();
                    loaded = true;
                    break;
                case Champion.Malzahar:
                    Malzahar.Execute();
                    loaded = true;
                    break;
                case Champion.Xerath:
                    Xerath.Execute();
                    loaded = true;
                    break;
            }
            if (loaded)
            {
                Core.kCore.Execute();
                Console.WriteLine(info + " KappaAIO: " + Player.Instance.Hero + " Loaded !");
                Console.WriteLine(info + " Have Fun !");
            }
            if (!loaded)
            {
                Console.WriteLine(warn + " KappaAIO: Failed To Loaded ! ");
                Console.WriteLine(warn + " Case: " + Player.Instance.Hero + " Not Supported ");
            }
            Console.WriteLine(info + " ----------------------------------");
        }
    }
}