using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using KappaAIO.Core.CommonStuff;
using SharpDX;

namespace KappaAIO.Core.Managers
{
    internal static class DashManager
    {
        public static float StartTick;
        public static float EndTick;
        public static Vector3 DashEnd;

        public static void Init()
        {
            Dash.OnDash += delegate(Obj_AI_Base sender, Dash.DashEventArgs args)
                {
                    if (!sender.IsMe || sender == null || args == null)
                        return;
                    StartTick = args.StartTick;
                    EndTick = args.EndTick;
                    DashEnd = args.EndPos;
                };
        }

        public static bool CanDash(this Obj_AI_Base target)
        {
            return !target.HasYasuoEBuff();
        }

        public static Vector3 EndPos(this Obj_AI_Base target)
        {
            return Player.Instance.Hero != Champion.Yasuo ? new Vector3() : Player.Instance.PredPos(100).Extend(target.PredPos(250), 475).To3D();
        }
    }
}
