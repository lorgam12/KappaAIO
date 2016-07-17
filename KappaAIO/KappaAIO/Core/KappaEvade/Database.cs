using System.Collections.Generic;
using EloBuddy;

namespace KappaAIO.Core.KappaEvade
{
    public static class Database
    {
        internal static class SkillShotSpells
        {
            public static readonly List<SSpell> SkillShotsSpellsList = new List<SSpell>
            {
               new SSpell
                  {
                     type = Type.Cone,
                     hero = Champion.Aatrox,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1075,
                     Speed = 1200,
                     Width = 100,
                     MissileName = "AatroxE",
                     Collisions = new []{ Collision.YasuoWall }
                  },
               new SSpell
                  {
                     type = Type.LineMissile,
                     hero = Champion.Ahri,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 925,
                     Speed = 1750,
                     Width = 100,
                     MissileName = "AhriOrbMissile",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ahri,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1550,
                     Width = 60,
                     MissileName = "AhriSeduceMissile",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Amumu,
                     slot = SpellSlot.Q,
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 2000,
                     Width = 80,
                     MissileName = "SadMummyBandageToss",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Anivia,
                     slot = SpellSlot.Q,
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1250,
                     Speed = 850,
                     Width = 110,
                     MissileName = "FlashFrostSpell",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Annie,
                     slot = SpellSlot.W,
                     DangerLevel = 2,
                     Angle = 50,
                     CastDelay = 250,
                     Range = 625,
                     Speed = int.MaxValue,
                     Width = 80,
                     MissileName = "Incinerate",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Ashe,
                     slot = SpellSlot.W,
                     DangerLevel = 2,
                     Angle = 45,
                     CastDelay = 250,
                     Range = 1150,
                     Speed = 1500,
                     Width = 20,
                     MissileName = "VolleyAttack",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ashe,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = int.MaxValue,
                     Speed = 1600,
                     Width = 130,
                     MissileName = "EnchantedCrystalArrow",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros }
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.AurelionSol,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = int.MaxValue,
                     Speed = 850,
                     Width = 180,
                     MissileName = "AurelionSolQMissile",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.AurelionSol,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 300,
                     Range = 1420,
                     Speed = 4600,
                     Width = 120,
                     MissileName = "AurelionSolRBeamMissile",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Bard,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1600,
                     Width = 60,
                     MissileName = "BardQMissile",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Blitzcrank,
                     slot = SpellSlot.Q,
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1800,
                     Width = 70,
                     MissileName = "RocketGrabMissile",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Brand,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 2000,
                     Width = 60,
                     MissileName = "BrandQMissile",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Braum,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1200,
                     Width = 100,
                     MissileName = "BraumQMissile",
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Braum,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 1250,
                     Speed = 1125,
                     Width = 100,
                     MissileName = "BraumRWrapper",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Caitlyn,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 625,
                     Range = 1300,
                     Speed = 2200,
                     Width = 90,
                     MissileName = "CaitlynPiltoverPeacemaker",
                     Collisions = new []{ Collision.YasuoWall }
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Caitlyn,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 125,
                     Range = 950,
                     Speed = 2000,
                     Width = 80,
                     MissileName = "CaitlynEntrapmentMissile"
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Cassiopeia,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 825,
                     Speed = 2000,
                     Width = 20,
                     MissileName = "CassiopeiaR"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Corki,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 500,
                     Range = 825,
                     Speed = 1125,
                     Width = 270,
                     MissileName = "PhosphorusBombMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Corki,
                     slot = SpellSlot.R,
                     DangerLevel = 2,
                     CastDelay = 175,
                     Range = 1300,
                     Speed = 2000,
                     Width = 40,
                     MissileName = "MissileBarrageMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Corki,
                     slot = SpellSlot.R,
                     DangerLevel = 3,
                     CastDelay = 175,
                     Range = 1500,
                     Speed = 2000,
                     Width = 40,
                     MissileName = "MissileBarrageMissile2"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Diana,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 1400,
                     Width = 50,
                     MissileName = "DianaArc"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.DrMundo,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 2000,
                     Width = 60,
                     MissileName = "InfectedCleaverMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Draven,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1400,
                     Width = 130,
                     MissileName = "DravenDoubleShotMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Draven,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = int.MaxValue,
                     Speed = 2000,
                     Width = 160,
                     MissileName = "DravenR"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ekko,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1650,
                     Width = 60,
                     MissileName = "EkkoQMis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ekko,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 25000,
                     Width = 100,
                     MissileName = "EkkoQReturn"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Elise,
                     slot = SpellSlot.E,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1600,
                     Width = 70,
                     MissileName = "EliseHumanE"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ezreal,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 2000,
                     Width = 60,
                     MissileName = "EzrealMysticShotMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ezreal,
                     slot = SpellSlot.W,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1600,
                     Width = 80,
                     MissileName = "EzrealEssenceFluxMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ezreal,
                     slot = SpellSlot.R,
                     DangerLevel = 4,
                     CastDelay = 1000,
                     Range = int.MaxValue,
                     Speed = 2000,
                     Width = 80,
                     MissileName = "EzrealTrueshotBarrage"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Fizz,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1275,
                     Speed = 1350,
                     Width = 120,
                     MissileName = "FizzMarinerDoom"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Galio,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1040,
                     Speed = 1200,
                     Width = 235,
                     MissileName = "GalioResoluteSmite"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Galio,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1280,
                     Speed = 1300,
                     Width = 160,
                     MissileName = "GalioRighteousGust"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Gnar,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1185,
                     Speed = 2400,
                     Width = 60,
                     MissileName = "GnarQMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Gnar,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 500,
                     Range = 1150,
                     Speed = 2000,
                     Width = 90,
                     MissileName = "GnarBigQMissile"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Gragas,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 500,
                     Range = 975,
                     Speed = 1000,
                     Width = 250,
                     MissileName = "GragasQ"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Gragas,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1750,
                     Width = 350,
                     MissileName = "GragasExplosiveCask"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Graves,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 825,
                     Speed = 3000,
                     Width = 60,
                     MissileName = "GravesQLineMis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Graves,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 2100,
                     Width = 100,
                     MissileName = "GravesChargeShotShot"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Heimerdinger,
                     slot = SpellSlot.W,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1350,
                     Speed = 2500,
                     Width = 35,
                     MissileName = "HeimerdingerWAttack2"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Heimerdinger,
                     slot = SpellSlot.E,
                     DangerLevel = 4,
                     CastDelay = 325,
                     Range = 925,
                     Speed = 1750,
                     Width = 135,
                     MissileName = "HeimerdingerESpell"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Illaoi,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1900,
                     Width = 50,
                     MissileName = "Illaoiemis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Irelia,
                     slot = SpellSlot.R,
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1200,
                     Speed = 1600,
                     Width = 120,
                     MissileName = "ireliatranscendentbladesspell"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Janna,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1700,
                     Speed = 900,
                     Width = 120,
                     MissileName = "HowlingGaleSpell"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Jayce,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1570,
                     Speed = 2350,
                     Width = 70,
                     MissileName = "JayceShockBlastMis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Jhin,
                     slot = SpellSlot.W,
                     DangerLevel = 3,
                     CastDelay = 750,
                     Range = 2250,
                     Speed = 5000,
                     Width = 40,
                     MissileName = "JhinWMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Jhin,
                     slot = SpellSlot.R,
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 3500,
                     Speed = 5000,
                     Width = 80,
                     MissileName = "JhinRShotMis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Jinx,
                     slot = SpellSlot.W,
                     DangerLevel = 3,
                     CastDelay = 600,
                     Range = 1500,
                     Speed = 3300,
                     Width = 60,
                     MissileName = "JinxWMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Jinx,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 600,
                     Range = int.MaxValue,
                     Speed = 1700,
                     Width = 140,
                     MissileName = "JinxR"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Kalista,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 350,
                     Range = 1200,
                     Speed = 2000,
                     Width = 70,
                     MissileName = "kalistamysticshotmistrue"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Karma,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1700,
                     Width = 90,
                     MissileName = "KarmaQMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Karma,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 1700,
                     Width = 100,
                     MissileName = "KarmaQMissileMantra"
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Kassadin,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 700,
                     Speed = 1000,
                     Width = 20,
                     MissileName = "ForcePulse"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Kennen,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 180,
                     Range = 1175,
                     Speed = 1700,
                     Width = 50,
                     MissileName = "KennenShurikenHurlMissile1"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Khazix,
                     slot = SpellSlot.W,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1700,
                     Width = 70,
                     MissileName = "KhazixWMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.KogMaw,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1125,
                     Speed = 1650,
                     Width = 70,
                     MissileName = "KogMawQ"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.KogMaw,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1360,
                     Speed = 1400,
                     Width = 120,
                     MissileName = "KogMawVoidOozeMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Leblanc,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 960,
                     Speed = 1750,
                     Width = 55,
                     MissileName = "LeblancSoulShackle"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Leblanc,
                     slot = SpellSlot.R,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 960,
                     Speed = 1750,
                     Width = 55,
                     MissileName = "LeblancSoulShackleM"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.LeeSin,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1800,
                     Width = 60,
                     MissileName = "BlindMonkQOne"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Leona,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 200,
                     Range = 975,
                     Speed = 2000,
                     Width = 70,
                     MissileName = "LeonaZenithBladeMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lissandra,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 825,
                     Speed = 2250,
                     Width = 75,
                     MissileName = "LissandraQ"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lissandra,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1125,
                     Speed = 1750,
                     Width = 100,
                     MissileName = "LissandraE"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lucian,
                     slot = SpellSlot.W,
                     DangerLevel = 1,
                     CastDelay = 300,
                     Range = 1000,
                     Speed = 1600,
                     Width = 80,
                     MissileName = "LucianW"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lucian,
                     slot = SpellSlot.R,
                     DangerLevel = 1,
                     CastDelay = 300,
                     Range = 1000,
                     Speed = 1600,
                     Width = 80,
                     MissileName = "LucianR"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lulu,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 925,
                     Speed = 1450,
                     Width = 80,
                     MissileName = "LuluQMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lux,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1200,
                     Width = 70,
                     MissileName = "LuxLightBindingMis"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Lux,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 1000,
                     Range = 3500,
                     Speed = int.MaxValue,
                     Width = 110,
                     MissileName = "LuxMaliceCannon"
                   },
               new SSpell
                   {
                     hero = Champion.Maokai,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.MissFortune,
                     slot = SpellSlot.R,
                     DangerLevel = 3
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Morgana,
                     slot = SpellSlot.Q,
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1200,
                     Width = 80,
                     MissileName = "DarkBindingMissile"
                   },
               new SSpell
                   {
                     type = Type.CircleMissile,
                     hero = Champion.Nami,
                     slot = SpellSlot.Q,
                     DangerLevel = 4,
                     CastDelay = 1000,
                     Range = 875,
                     Speed = int.MaxValue,
                     Width = 200,
                     MissileName = "NamiQ"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Nami,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 2750,
                     Speed = 850,
                     Width = 250,
                     MissileName = "NamiRMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Nautilus,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1250,
                     Speed = 2000,
                     Width = 90,
                     MissileName = "NautilusAnchorDragMissile"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Nidalee,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1500,
                     Speed = 1300,
                     Width = 60,
                     MissileName = "JavelinToss"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Nocturne,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1125,
                     Speed = 1400,
                     Width = 60,
                     MissileName = "NocturneDuskbringer"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Olaf,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1600,
                     Width = 90,
                     MissileName = "OlafAxeThrowCast"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Orianna,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 2000,
                     Speed = 1200,
                     Width = 80,
                     MissileName = "OrianaIzunaCommand"
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Poppy,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Poppy,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Quinn,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.RekSai,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Rengar,
                     slot = SpellSlot.E,
                     DangerLevel = 3
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Riven,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Rumble,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Ryze,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Sejuani,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Shyvana,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Sion,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Sivir,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Skarner,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Sona,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                   },
               new SSpell
                   {
                     hero = Champion.Soraka,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.Cone,
                     hero = Champion.Syndra,
                     slot = SpellSlot.E,
                     DangerLevel = 3
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.TahmKench,
                     slot = SpellSlot.Q,
                     DangerLevel = 4
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Taliyah,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Taliyah,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     hero = Champion.Talon,
                     slot = SpellSlot.W,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Thresh,
                     slot = SpellSlot.Q,
                     DangerLevel = 4
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.TwistedFate,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Urgot,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Urgot,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Varus,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     hero = Champion.Varus,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Varus,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Veigar,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Velkoz,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Velkoz,
                     slot = SpellSlot.W,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Viktor,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Xerath,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Xerath,
                     slot = SpellSlot.E,
                     DangerLevel = 4
                   },
               new SSpell
                   {
                     hero = Champion.Xerath,
                     slot = SpellSlot.R,
                     DangerLevel = 3
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Zed,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Ziggs,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Ziggs,
                     slot = SpellSlot.W,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Ziggs,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     hero = Champion.Zilean,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                   },
               new SSpell
                   {
                     type = Type.LineMissile,
                     hero = Champion.Zyra,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                   }
            };

            public struct SSpell
            {
                public Type type;

                public Champion hero;

                public SpellSlot slot;

                public int DangerLevel;

                public float Range;

                public float Angle;

                public float Width;

                public float Speed;

                public float CastDelay;

                public string MissileName;

                public Collision[] Collisions;
            }

            public enum Type
            {
                LineMissile,
                CircleMissile,
                Cone
            }

            public enum Collision
            {
                YasuoWall,
                Minions,
                Heros,
                Walls
            }
        }

        internal static class TargetedSpells
        {
            public static readonly List<TSpell> TargetedSpellsList = new List<TSpell>()
             {
              new TSpell
                 {
                     hero = Champion.Akali,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Anivia,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Annie,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Brand,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Caitlyn,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Cassiopeia,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Elise,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.FiddleSticks,
                     slot = SpellSlot.E,
                     DangerLevel = 3
                 },
              new TSpell
                 {
                     hero = Champion.Gangplank,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Janna,
                     slot = SpellSlot.W,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Jhin,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Kassadin,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Katarina,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Kayle,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Kindred,
                     slot = SpellSlot.E,
                     DangerLevel = 3
                 },
              new TSpell
                 {
                     hero = Champion.Leblanc,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Malphite,
                     slot = SpellSlot.Q,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Malzahar,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.MissFortune,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Nami,
                     slot = SpellSlot.W,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Nautilus,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Nunu,
                     slot = SpellSlot.E,
                     DangerLevel = 3
                 },
              new TSpell
                 {
                     hero = Champion.Pantheon,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Ryze,
                     slot = SpellSlot.E,
                     DangerLevel = 1
                 },
              new TSpell
                 {
                     hero = Champion.Shaco,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Syndra,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Teemo,
                     slot = SpellSlot.Q,
                     DangerLevel = 3
                 },
              new TSpell
                 {
                     hero = Champion.Tristana,
                     slot = SpellSlot.E,
                     DangerLevel = 2
                 },
              new TSpell
                 {
                     hero = Champion.Tristana,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Vayne,
                     slot = SpellSlot.E,
                     DangerLevel = 4
                 },
              new TSpell
                 {
                     hero = Champion.Veigar,
                     slot = SpellSlot.R,
                     DangerLevel = 5
                 },
              new TSpell
                 {
                     hero = Champion.Viktor,
                     slot = SpellSlot.Q,
                     DangerLevel = 1
                 }
             };

            public struct TSpell
            {
                public Champion hero;

                public SpellSlot slot;

                public int DangerLevel;
            }
        }
    }
}
