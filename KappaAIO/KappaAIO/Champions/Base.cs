using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Spells;
using KappaAIO.Core;
using KappaAIO.Core.CommonStuff;
using KappaAIO.Core.Managers;
using SharpDX;

namespace KappaAIO.Champions
{
    public abstract class Base
    {
        public static AIHeroClient user = Player.Instance;

        public static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        public static Menu Menuini, RMenu, EvadeMenu, AutoMenu, JumperMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, MiscMenu, DrawMenu, ColorMenu;

        public static Spell.Skillshot Flash;

        public static Spell.Targeted Smite;

        public abstract void Active();

        public abstract void Combo();

        public abstract void Harass();

        public abstract void LaneClear();

        public abstract void JungleClear();

        public abstract void KillSteal();

        public abstract void Draw();

        public delegate void InComingDamage(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args, float IncDamage);

        public static event InComingDamage OnIncDmg;

        protected Base()
        {
            this.Initialize();
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || user.IsDead)
            {
                return;
            }

            var caster = sender;
            var target = (AIHeroClient)args.Target;

            if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || target == null || caster == null || !target.IsAlly || !target.IsKillable())
            {
                return;
            }

            var aaprecent = (caster.GetAutoAttackDamage(target, true) / target.TotalShieldHealth()) * 100;
            var death = caster.GetAutoAttackDamage(target, true) >= target.TotalShieldHealth() || aaprecent >= target.HealthPercent;

            OnIncDmg?.Invoke(caster, target, args, caster.GetAutoAttackDamage(target, true));
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(args.Target is AIHeroClient) || !sender.IsEnemy || user.IsDead)
            {
                return;
            }

            var caster = sender;
            var enemy = sender as AIHeroClient;
            var target = (AIHeroClient)args.Target;
            Common.ally = EntityManager.Heroes.Allies.FirstOrDefault(a => a.IsInRange(args.End, 100) && a.IsKillable() && !a.IsMe);
            var hitally = Common.ally != null && args.End != Vector3.Zero && args.End.Distance(Common.ally) < 100;
            var hitme = args.End != Vector3.Zero && args.End.Distance(user) < 100;

            if (!(caster is AIHeroClient || caster is Obj_AI_Turret) || !caster.IsEnemy || enemy == null || caster == null)
            {
                return;
            }

            if (((target.IsAlly && !target.IsMe) || hitally) && Common.ally != null && Common.ally.IsValidTarget())
            {
                var spelldamageally = enemy.GetSpellDamage(Common.ally, args.Slot);
                var damagepercentally = (spelldamageally / Common.ally.TotalShieldHealth()) * 100;
                var deathally = damagepercentally >= Common.ally.HealthPercent || spelldamageally >= Common.ally.TotalShieldHealth()
                                || caster.GetAutoAttackDamage(Common.ally, true) >= Common.ally.TotalShieldHealth()
                                || enemy.GetAutoAttackDamage(Common.ally, true) >= Common.ally.TotalShieldHealth();
                
                OnIncDmg?.Invoke(caster, Common.ally, args, spelldamageally);
            }

            if (target.IsMe || hitme)
            {
                var spelldamageme = enemy.GetSpellDamage(user, args.Slot);
                var damagepercentme = (spelldamageme / user.TotalShieldHealth()) * 100;
                var deathme = damagepercentme >= user.HealthPercent || spelldamageme >= user.TotalShieldHealth() || caster.GetAutoAttackDamage(user, true) >= user.TotalShieldHealth()
                              || enemy.GetAutoAttackDamage(user, true) >= user.TotalShieldHealth();

                OnIncDmg?.Invoke(caster, user, args, spelldamageme);
            }
        }

        public void Initialize()
        {
            if (Player.Spells.FirstOrDefault(o => o.SData.Name.Contains("SummonerFlash")) != null)
            {
                Flash = SummonerSpells.Flash;
            }

            if (Player.Spells.FirstOrDefault(o => o.SData.Name.ToLower().Contains("smite")) != null)
            {
                Smite = SummonerSpells.Smite;
            }

            Game.OnTick += this.Game_OnTick;
            Drawing.OnDraw += this.Drawing_OnDraw;
        }

        public virtual void Drawing_OnDraw(EventArgs args)
        {
            this.Draw();

            // Spells Drawings
            foreach (var spell in SpellList)
            {
                var color = ColorMenu.Color(spell.Slot.ToString());
                spell.SpellRange(color, DrawMenu.checkbox(spell.Slot.ToString()));
            }

            // Damage
            DrawingsManager.DrawTotalDamage(SpellList, DrawMenu.checkbox("damage"));
        }

        public virtual void Game_OnTick(EventArgs args)
        {
            if (user.IsDead)
            {
                return;
            }

            this.Active();
            this.KillSteal();
            if (Common.orbmode(Orbwalker.ActiveModes.Combo))
            {
                this.Combo();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.Harass))
            {
                this.Harass();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.LaneClear))
            {
                this.LaneClear();
            }

            if (Common.orbmode(Orbwalker.ActiveModes.JungleClear))
            {
                this.JungleClear();
            }
        }
    }
}
