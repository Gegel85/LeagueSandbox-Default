using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.Champions.Ahri;
using System;
using System.Numerics;

namespace Spells
{
    public class AhriFoxFire : IGameScript
    {
        AttackableUnit[] hitTargets = new AttackableUnit[3];
        int nbrOftargetHit = 0;
        double newAngle = 0;
        Champion owner;
        Spell spell;
        bool isActive = false;

        public void OnActivate(Champion owner)
        {
        }

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            this.spell = spell;
            this.owner = owner;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_FoxFire_cas.troy", owner);
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Fire_Idle.troy", owner);
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_FoxFire_weapon_cas.troy", owner);
            for (int i = 0; i < hitTargets.Length; i++)
                hitTargets[i] = null;
            nbrOftargetHit = 0;
        }

        public Vector2 GetCirclePosition(float x, float y, float radius, double angle)
        {
            Vector2 position = new Vector2(0, 0)
            {
                X = (float)Math.Cos(angle) * radius + x,
                Y = (float)Math.Sin(angle) * radius + y
            };
            return (position);
        }

        public void AddFoxFire(Spell spell, Champion owner, double angle)
        {
            Vector2 pos = GetCirclePosition(owner.X, owner.Y, 300, angle);

            spell.AddProjectile("AhriFoxFireMissile", pos.X, pos.Y);
        }

        public void SpawnFoxFire()
        {
            AddFoxFire(spell, owner, 2 * Math.PI / 3 + newAngle);
            AddFoxFire(spell, owner, 4 * Math.PI / 3 + newAngle);
            AddFoxFire(spell, owner, 6 * Math.PI / 3 + newAngle);
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            ApiFunctionManager.AddBuffHUDVisual("AhriFoxFire", AhriConsts.W_EXPIRATION_TIME, 2, owner, AhriConsts.W_EXPIRATION_TIME);
            newAngle = 0;
            isActive = true;
            ApiFunctionManager.CreateTimer(10f, () => isActive = false);
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            float bonusDamages = owner.GetStats().AbilityPower.Total * AhriConsts.W_AP_RATIO;
            float baseDamage = AhriConsts.W_BASE_DAMAGES + AhriConsts.W_DAMAGES_LEVEL_SCALING * spell.Level;
            float totalDamages = baseDamage + bonusDamages;

            if (target == owner)
                return;
            for (int i = 0; i < hitTargets.Length; i++)
                if (hitTargets[i] == target)
                {
                    totalDamages *= 0.3f;
                    break;
                }
            //hitTargets[nbrOftargetHit++] = target;
            if (((ObjAiBase)target).HasBuffGameScriptActive("AhriCharm", "AhriCharm"))
                totalDamages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_FoxFire_tar.troy", owner);
            target.TakeDamage(owner, totalDamages, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            projectile.setToRemove();
        }

        public void OnUpdate(double diff)
        {
            if (isActive)
            {
                newAngle += diff / 1000f;
                SpawnFoxFire();
            }
        }
    }
}