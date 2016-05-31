namespace KappaAIO.Core
{
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class kCore
    {
        public static Menu CoreMenu, ks;

        public static readonly string[] Junglemobs =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder",
                "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue",
                "SRU_Red", "SRU_RiftHerald", "AscXerath"
            };

        public static void Execute()
        {
            CoreMenu = MainMenu.AddMenu("KappaCore", "KappaCore");
            ks = CoreMenu.AddSubMenu("Stealer");

            ks.AddGroupLabel("KillSteal Champions");
            foreach (var hero in EntityManager.Heroes.Enemies)
            {
                ks.Add(hero.ChampionName, new CheckBox(hero.ChampionName + " (" + hero.Name + ")"));
            }
            ks.AddSeparator(1);
            ks.AddGroupLabel("JungleSteal Mobs");
            foreach (var mob in Junglemobs)
            {
                ks.Add(mob, new CheckBox(mob));
            }
        }
    }
}