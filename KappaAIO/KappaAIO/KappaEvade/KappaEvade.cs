﻿using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Rendering;
using KappaAIO.Core.CommonStuff;
using SharpDX;
using Color = System.Drawing.Color;

namespace KappaAIO.KappaEvade
{
    static class KappaEvade
    {
        public static void Init()
        {
            Common.Logger.Info("KappaEvade Loaded");
            SpellsDetector.Init();
            SpellsDetector.OnSkillShotDetected += SpellsDetector_OnSkillShotDetected;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnDelete += GameObject_OnDelete;
            Game.OnTick += Game_OnTick;
            //GameObject.OnCreate += GameObject_OnCreate;
            debug.Init();
        }

        public static bool IsInDanger(this Obj_AI_Base target, ActiveSpells spell)
        {
            return spell.ToPolygon().IsInside(target);
        }

        public static Geometry.Polygon.Sector CreateCone(Vector3 Center, Vector3 Direction, float Angle, float Range)
        {
            return new Geometry.Polygon.Sector(Center, Direction, (float)(Angle * Math.PI / 180), Range);
        }

        public static Geometry.Polygon ToPolygon(this ActiveSpells spell)
        {
            if (spell.spell.type == Database.SkillShotSpells.Type.LineMissile)
            {
                return new Geometry.Polygon.Rectangle(spell.Start, spell.End, spell.Width);
            }
            if (spell.spell.type == Database.SkillShotSpells.Type.CircleMissile)
            {
                return new Geometry.Polygon.Circle(spell.End, spell.Width);
            }
            if (spell.spell.type == Database.SkillShotSpells.Type.Cone)
            {
                return CreateCone(spell.Start, spell.End, spell.spell.Angle, spell.Range);
            }
            return new Geometry.Polygon();
        }

        public struct ActiveSpells
        {
            public Obj_AI_Base Caster;
            public Vector3 Start;
            public Vector3 End;
            public float Range;
            public float Width;
            public Database.SkillShotSpells.SSpell spell;
            public float EndTime;
            public GameObject Missile;
            public float ArriveTime;
        }

        public static List<ActiveSpells> DetectedSpells = new List<ActiveSpells>();

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (caster == null || missile == null || !caster.IsMe || !missile.IsValid)
            {
                return;
            }

            if (Database.SkillShotSpells.SkillShotsSpellsList.Any(s => caster != null && (s.hero == caster.Hero && s.MissileName.ToLower() == missile.SData.Name.ToLower())))
            {
                var spell = Database.SkillShotSpells.SkillShotsSpellsList.FirstOrDefault(s => caster != null && (s.hero == caster.Hero && s.MissileName.ToLower() == missile.SData.Name.ToLower()));

                if (DetectedSpells.All(s => spell.MissileName.ToLower() != s.spell.MissileName.ToLower() && caster.NetworkId != s.Caster.NetworkId))
                {
                    var endpos = missile.StartPosition.Extend(missile.EndPosition, spell.Range).To3D();

                    if (spell.type == Database.SkillShotSpells.Type.LineMissile || spell.type == Database.SkillShotSpells.Type.CircleMissile)
                    {
                        var rect = new Geometry.Polygon.Rectangle(missile.StartPosition, endpos, spell.Width);
                        var collide =
                            ObjectManager.Get<Obj_AI_Base>()
                                .OrderBy(m => m.Distance(sender))
                                .FirstOrDefault(
                                    s =>
                                    s.Team != sender.Team
                                    && (((s.IsMinion || s.IsMonster || s is Obj_AI_Minion) && !s.IsWard() && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Minions))
                                        || s is AIHeroClient && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Heros)) && rect.IsInside(s) && s.IsValidTarget());

                        if (collide != null)
                        {
                            endpos = collide.ServerPosition;
                        }
                    }

                    if (spell.type == Database.SkillShotSpells.Type.Cone)
                    {
                        var sector = new Geometry.Polygon.Sector(missile.StartPosition, endpos, spell.Angle, spell.Range);
                        var collide =
                            ObjectManager.Get<Obj_AI_Base>()
                                .OrderBy(m => m.Distance(sender))
                                .FirstOrDefault(
                                    s =>
                                    s.Team != sender.Team
                                    && (((s.IsMinion || s.IsMonster || s is Obj_AI_Minion) && !s.IsWard() && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Minions))
                                        || s is AIHeroClient && spell.Collisions.Contains(Database.SkillShotSpells.Collision.Heros)) && sector.IsInside(s) && s.IsValidTarget());

                        if (collide != null)
                        {
                            endpos = collide.ServerPosition;
                        }
                    }
                    Chat.Print("OnCreate " + missile.SData.Name);
                    DetectedSpells.Add(
                        new ActiveSpells
                            {
                                spell = spell, Start = missile.StartPosition, End = missile.EndPosition, Width = spell.Width,
                                EndTime = (endpos.Distance(missile.StartPosition) / spell.Speed) + (spell.CastDelay / 1000) + Game.Time, Missile = missile, Caster = caster,
                                ArriveTime = (missile.StartPosition.Distance(Player.Instance) / spell.Speed) - (spell.CastDelay / 1000)
                            });
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            foreach (var spell in DetectedSpells /*.Where(s => s.spell.ForceRemove)*/)
            {
                //Chat.Print(Game.Time - spell.EndTime);
                //Chat.Print("OnTick remove " + spell.spell.MissileName);
                DetectedSpells.RemoveAll(s => Game.Time - s.EndTime >= 0);
                return;
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender == null)
                return;
            var missile = sender as MissileClient;
            var caster = missile?.SpellCaster as AIHeroClient;
            if (missile == null || caster == null || missile.IsAutoAttack() || !missile.IsValid)
                return;
            Chat.Print("OnDelete Detected " + missile.SData.Name);
            if (DetectedSpells.Any(s => s.Caster != null && s.spell.MissileName.ToLower() == missile.SData.Name.ToLower() && caster.NetworkId == s.Caster.NetworkId))
            {
                Chat.Print("OnDelete Remove " + missile.SData.Name);
                DetectedSpells.RemoveAll(s => s.Caster != null && s.spell.MissileName.ToLower() == missile.SData.Name.ToLower() && caster.NetworkId == s.Caster.NetworkId);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in DetectedSpells /*.Where(s => debug.SkillShots.checkbox(s.Caster.ID() + s.spell.slot + "draw"))*/)
            {
                if (spell.Missile != null)
                {
                    Circle.Draw(SharpDX.Color.GreenYellow, spell.Width, spell.Missile);
                }

                var c = Player.Instance.IsInDanger(spell) ? Color.Red : Color.AliceBlue;
                spell.ToPolygon().Draw(c, 2);
            }
        }

        private static void SpellsDetector_OnSkillShotDetected(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args, Database.SkillShotSpells.SSpell spell, Vector3 Start, Vector3 End, float Range, float Width, MissileClient missile)
        {
            var spellrange = Range + debug.SkillShots.slider("range");
            var endpos = spell.IsFixedRange ? Start.Extend(End, spellrange).To3D() : End;

            if (spell.type == Database.SkillShotSpells.Type.CircleMissile && End.Distance(Start) < Range)
            {
                endpos = End;
            }

            var newactive = new ActiveSpells
                                {
                Start = Start, End = endpos, Range = spellrange, Width = Width + debug.SkillShots.slider("width"), spell = spell, EndTime = (endpos.Distance(Start) / spell.Speed) + (spell.CastDelay / 1000) + Game.Time, Caster = sender, Missile = missile, ArriveTime = (Start.Distance(Player.Instance) / spell.Speed) - (spell.CastDelay / 1000)
                                };

            if (!DetectedSpells.Contains(newactive))
            {
                DetectedSpells.Add(newactive);
            }
        }
    }
}