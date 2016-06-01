namespace KappaAIO.Core
{
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;

    public static class Stealer
    {
        public static Obj_AI_Base GetKStarget(this Spell.SpellBase spell)
        {
            return spell.IsReady()
                       ? EntityManager.Heroes.Enemies.Where(e => e.IsKillable() && e.IsValidTarget(spell.Range) && kCore.ks.checkbox(e.ChampionName))
                             .FirstOrDefault(enemy => spell.GetDamage(enemy) >= Prediction.Health.GetPrediction(enemy, spell.CastDelay))
                       : null;
        }

        public static Obj_AI_Base GetJStarget(this Spell.SpellBase spell)
        {
            return
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .Where(
                        j =>
                        j.IsKillable() && kCore.Junglemobs.Contains(j.BaseSkinName) && j.IsValidTarget(spell.Range)
                        && kCore.ks.checkbox(j.BaseSkinName))
                    .FirstOrDefault(jmob => spell.GetDamage(jmob) >= Prediction.Health.GetPrediction(jmob, spell.CastDelay));
        }
    }
}