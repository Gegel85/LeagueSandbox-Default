﻿using System.Numerics;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logic.GameObjects.Spells;
using LeagueSandbox.GameServer.Logic.GameObjects.Missiles;

namespace Spells
{
    public class GravesChargeShot : IGameScript
    {
        public void OnActivate(Champion owner)
        {

        }

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            spell.SpellAnimation("SPELL4", owner);
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            var current = new Vector2(owner.X, owner.Y);
            var to = Vector2.Normalize(new Vector2(spell.X, spell.Y) - current);
            var range = to * 1000;
            var trueCoords = current + range;

            spell.AddProjectile("GravesChargeShotShot", trueCoords.X, trueCoords.Y);
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            var bonusAD = owner.Stats.AttackDamage.Total - owner.Stats.AttackDamage.BaseValue;
            var ad = bonusAD * 0.150f;
            var damage = new[] { 250, 400, 550 }[spell.Level - 1] + ad;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
        }

        public void OnUpdate(double diff)
        {
        }
    }
}