using System;
using System.Net;
using EloBuddy;
using Version = System.Version;

namespace KappaAIO.Core.CommonStuff
{
    internal class CheckVersion
    {
        private static string UpdateMsg = string.Empty;

        private const string UpdateMsgPath = "https://raw.githubusercontent.com/plsfixrito/KappaAIO/master/KappaAIO/KappaAIO/UpdateMsg.txt";

        private const string WebVersionPath = "https://raw.githubusercontent.com/plsfixrito/KappaAIO/master/KappaAIO/KappaAIO/Properties/AssemblyInfo.cs";

        private static readonly Version CurrentVersion = typeof(CheckVersion).Assembly.GetName().Version;

        public static void Init()
        {
            try
            {
                var WebClient = new WebClient();
                WebClient.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args) { UpdateMsg = args.Result; };
                WebClient.DownloadStringTaskAsync(UpdateMsgPath);

                var WebClient2 = new WebClient();
                WebClient2.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs args)
                    {
                        if (!args.Result.Contains(CurrentVersion.ToString()))
                        {
                            Common.Logger.Warn("There is a new Update Available for KappaAIO !");
                            Chat.Print("<b>KappaAIO: There is a new Update Available for KappaAIO !</b>");
                            Common.Logger.Info(UpdateMsg);
                            Common.ShowNotification("There is a new Update Available for KappaAIO !", 10000, UpdateMsg);
                            Chat.Print("<b>KappaAIO: " + UpdateMsg + "</b>");
                        }
                        else
                        {
                            Common.Logger.Info("Your AIO is updated !");
                        }
                    };
                WebClient2.DownloadStringTaskAsync(WebVersionPath);

                /*
                GetResponse(WebRequest.Create(UpdateMsgPath), response => { UpdateMsg = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString(); });

                EloBuddy.SDK.Core.DelayAction(
                    () =>
                        {
                            GetResponse(
                                WebRequest.Create(WebVersionPath),
                                response =>
                                    {
                                        var data = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
                                        if (data.Contains(CurrentVersionPath.ToString()))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Cyan;
                                            Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "Info]" + " KappaAIO: Your version is Updated !");
                                            Console.ResetColor();
                                        }
                                        else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Magenta;
                                            Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "Warn] KappaAIO: There is a new Update Available for KappaAIO !");
                                            Console.ResetColor();
                                            Console.ForegroundColor = ConsoleColor.Cyan;
                                            Chat.Print("<b>KappaAIO: There is a new Update Available for KappaAIO !</b>");
                                            if (UpdateMsg != string.Empty)
                                            {
                                                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "Info] KappaAIO: " + UpdateMsg);
                                                Console.ResetColor();
                                                Chat.Print("<b>KappaAIO: " + UpdateMsg + "</b>");
                                                Common.ShowNotification("There is a new Update Available for KappaAIO !", 10000, UpdateMsg);
                                            }
                                            else
                                            {
                                                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + "Info] KappaAIO: Failed to retrive update msg");
                                                Console.ResetColor();
                                                Chat.Print("<b>KappaAIO: Failed to Retrieve Update msg</b>");
                                                Common.ShowNotification("There is a new Update Available for KappaAIO !", 10000, "Failed to retrive update msg");
                                            }
                                        }
                                    });
                        },
                    500);
                    */
            }
            catch (Exception ex)
            {
                Common.Logger.Error("Failed To Check for Updates !");
                Common.Logger.Error(ex.ToString());
            }
        }
    }
}
