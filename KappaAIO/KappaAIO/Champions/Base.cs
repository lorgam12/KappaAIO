namespace KappaAIO.Champions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Spells;

    using KappaAIO.Core;
    using KappaAIO.Core.Managers;

    public abstract class Base
    {
        public static AIHeroClient user = Player.Instance;

        public static readonly List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();

        public static Menu Menuini, RMenu, AutoMenu, JumperMenu, ComboMenu, HarassMenu, LaneClearMenu, JungleClearMenu, KillStealMenu, MiscMenu, DrawMenu, ColorMenu;

        public static Spell.Skillshot Flash;

        public static Spell.Targeted Smite;

        public abstract void Active();

        public abstract void Combo();

        public abstract void Harass();

        public abstract void LaneClear();

        public abstract void JungleClear();

        public abstract void KillSteal();

        public abstract void Draw();

        protected Base()
        {
            this.Initialize();
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
