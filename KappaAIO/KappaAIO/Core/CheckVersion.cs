namespace KappaAIO.Core
{
    using System;
    using System.IO;
    using System.Net;

    using EloBuddy;
    using EloBuddy.SDK;

    using Version = System.Version;

    internal class CheckVersion
    {
        private static string UpdateMsg = string.Empty;

        private const string UpdateMsgPath = "https://raw.githubusercontent.com/plsfixrito/KappaAIO/master/KappaAIO/KappaAIO/UpdateMsg.txt";

        private const string WebVersionPath =
            "https://raw.githubusercontent.com/plsfixrito/KappaAIO/master/KappaAIO/KappaAIO/Properties/AssemblyInfo.cs";

        private static readonly Version CurrentVersionPath = typeof(CheckVersion).Assembly.GetName().Version;

        public static void Init()
        {
            try
            {
                GetResponse(
                    WebRequest.Create(UpdateMsgPath),
                    response => { UpdateMsg = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString(); });

                Core.DelayAction(
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
                                            Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + " Info]" + " KappaAIO: Your version is Updated !");
                                            Console.ResetColor();
                                        }
                                        else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Magenta;
                                            Console.WriteLine(
                                                DateTime.Now.ToString("[H:mm:ss - ")
                                                + " Warn] KappaAIO: There is a new Update Available for KappaAIO !");
                                            Console.ResetColor();
                                            Console.ForegroundColor = ConsoleColor.Cyan;
                                            Chat.Print("<b>KappaAIO: There is a new Update Available for KappaAIO !</b>");
                                            if (UpdateMsg != string.Empty)
                                            {
                                                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + " Info] KappaAIO: " + UpdateMsg);
                                                Console.ResetColor();
                                                Chat.Print("<b>KappaAIO: " + UpdateMsg + "</b>");
                                                Common.ShowNotification("There is a new Update Available for KappaAIO !", 10000, UpdateMsg);
                                            }
                                            else
                                            {
                                                Console.WriteLine(
                                                    DateTime.Now.ToString("[H:mm:ss - ") + " Info] KappaAIO: Failed to retrive update msg");
                                                Console.ResetColor();
                                                Chat.Print("<b>KappaAIO: Failed to retrive update msg</b>");
                                                Common.ShowNotification(
                                                    "There is a new Update Available for KappaAIO !",
                                                    10000,
                                                    "Failed to retrive update msg");
                                            }
                                        }
                                    });
                        },
                    500);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + " Warn] KappaAIO: Failed To Check for Updates !");
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + ex);
                Console.ResetColor();
            }
        }

        // Credits to MarioGK
        private static void GetResponse(WebRequest Request, Action<HttpWebResponse> ResponseAction)
        {
            try
            {
                Action wrapperAction = () =>
                    {
                        Request.BeginGetResponse(
                            iar =>
                                {
                                    var Response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                                    ResponseAction(Response);
                                },
                            Request);
                    };
                wrapperAction.BeginInvoke(
                    iar =>
                        {
                            var Action = (Action)iar.AsyncState;
                            Action.EndInvoke(iar);
                        },
                    wrapperAction);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss - ") + ex);
                Console.ResetColor();
            }
        }
    }
}