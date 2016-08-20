using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK.Events;
using KappaAIO.Champions;
using KappaAIO.Core.CommonStuff;

namespace KappaAIO
{
    internal class Program
    {
        public static Champion[] hero = { Champion.AurelionSol, Champion.Azir, Champion.Brand, Champion.Kindred, Champion.LeeSin, Champion.Malzahar, Champion.Xerath, /*Champion.Yasuo*/ };

        private static void Main(string[] args)
        {
            Console.WriteLine("Loading KappaAIO");
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (hero.Contains(Player.Instance.Hero))
            {
                kCore.Execute();
                Chat.Print("<font color='#FFFFFF'><b>KappaAIO Loaded</b></font>");
                CheckVersion.Init();
                var Instance = (Base)Activator.CreateInstance(null, "KappaAIO.Champions." + Player.Instance.Hero).Unwrap();
                DamageLib.DamageDatabase();
                Common.Logger.Info(Player.Instance.Hero + " Loaded !");
                Common.Logger.Info("Have Fun !");
                Common.ShowNotification(Player.Instance.ChampionName + " - Loaded", 5000);
                Common.Logger.Info("----------------------------------");
            }
            else
            {
                Common.Logger.Warn("KappaAIO: Failed To Load ! ");
                Common.Logger.Warn("Case: " + Player.Instance.Hero + " Not Supported ");
            }
            Chat.OnInput += Chat_OnMessage;
        }

        private static void Chat_OnMessage(ChatInputEventArgs chatInputEventArgs)
        {
            if (chatInputEventArgs.Input.Equals("Load KappaEvade", StringComparison.CurrentCultureIgnoreCase))
            {
                chatInputEventArgs.Process = false;
                KappaEvade.KappaEvade.Init();
            }
        }
    }
}
