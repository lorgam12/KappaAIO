using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappaAIO.Core.CommonStuff;

namespace KappaAIO.Core.Managers
{
    internal class ObjectsManager
    {
        public static List<Obj_AI_Minion> Wards = new List<Obj_AI_Minion>();

        public static Obj_AI_Turret EnemyTurret = EntityManager.Turrets.Enemies.OrderBy(t => t.Distance(Player.Instance)).FirstOrDefault(t => t.IsValidTarget() && !t.IsDead);

        public static float LastTurretAttackOnMe;

        public static float LastTurretAttack;

        public static bool SafeToDive
        {
            get
            {
                return EnemyTurret != null && Player.Instance.HealthPercent > 10 && EloBuddy.SDK.Core.GameTickCount - LastTurretAttackOnMe > 3000 && LastTurretAttack < 100
                       && (EnemyTurret.CountAllyMinions(EnemyTurret.GetAutoAttackRange()) > 2 || EnemyTurret.CountAlliesInRange(800) > 1);
            }
        }

        public static void Init()
        {
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    var turret = sender as Obj_AI_Turret;
                if (turret != null && EnemyTurret != null && turret.NetworkId == EnemyTurret.NetworkId)
                {
                    if (args.Target.IsMe)
                    {
                        LastTurretAttackOnMe = EloBuddy.SDK.Core.GameTickCount;
                    }
                    else
                    {
                        LastTurretAttack = EloBuddy.SDK.Core.GameTickCount;
                    }
                }
            };

            foreach (var ward in
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(w => w.Name.ToLower().Contains("ward") && w != null && w.IsAlly && w.IsValid && w.Health > 0 && !w.IsDead && !w.Name.ToLower().Contains("wardcorpse")))
            {
                Wards.Add(ward);
            }
            GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
            {
                if (sender.Name.ToLower().Contains("ward") && sender.IsAlly && !sender.Name.ToLower().Contains("wardcorpse"))
                {
                    Wards.Add((Obj_AI_Minion)sender);
                }
            };
            GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
            {
                if (sender.Name.ToLower().Contains("ward") && sender.IsAlly && !sender.Name.ToLower().Contains("wardcorpse"))
                {
                    Wards.Remove((Obj_AI_Minion)sender);
                }
            };
        }
    }
}
