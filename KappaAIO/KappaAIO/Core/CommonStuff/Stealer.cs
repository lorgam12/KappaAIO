using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace KappaAIO.Core.CommonStuff
{
    public static class Stealer
    {
        public static Obj_AI_Base GetKStarget(this Spell.SpellBase spell)
        {
            return spell.IsReady()
                       ? EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsKillable(spell.Range) && kCore.ks.checkbox(e.ID()))
                             .FirstOrDefault(enemy => spell.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, (int)spell.TravelTime(enemy)))
                       : null;
        }

        public static Obj_AI_Base GetJStarget(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .Where(j => j.IsKillable() && kCore.Junglemobs.Contains(j.BaseSkinName) && j.IsKillable(spell.Range) && kCore.ks.checkbox(j.BaseSkinName))
                    .FirstOrDefault(jmob => spell.GetDamage(jmob) >= Prediction.Health.GetPrediction(jmob, (int)spell.TravelTime(jmob)));
        }
    }
}
