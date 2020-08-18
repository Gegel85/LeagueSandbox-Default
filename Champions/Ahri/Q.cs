using System.Linq;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.GameObjects.Spells;
using LeagueSandbox.GameServer.Logic.GameObjects.Missiles;
using LeagueSandbox.Champions.Ahri;
using System.Numerics;

namespace Spells
{
    public class AhriOrbofDeception : IGameScript
    {
        private Champion owner = null;
        private Spell spell = null;
        private Vector2 spellPositionToGo;
        private Projectile projectile;
        private bool isLaunched = false;
        Projectile returnProj = null;

        public void OnActivate(Champion owner)
        {
        }

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            this.owner = owner;
            this.spell = spell;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_cas.troy ", owner);
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            Vector2 currentPos = new Vector2(owner.X, owner.Y);
            Vector2 angleVector = Vector2.Normalize(new Vector2(spell.X, spell.Y) - currentPos);
            Vector2 range = angleVector * AhriConsts.Q_RANGE;

            spellPositionToGo = range + currentPos;
            isLaunched = true;
            projectile = spell.AddProjectile("AhriOrbMissile", spellPositionToGo.X, spellPositionToGo.Y);
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_mis.troy", projectile);
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            float apDamages = owner.Stats.AbilityPower.Total * AhriConsts.Q_AP_RATIO;
            float damages = AhriConsts.Q_BASE_DAMAGES + spell.Level * AhriConsts.Q_DAMAGES_LEVEL_SCALING + apDamages;
            
            if (((ObjAiBase)target).HasBuffGameScriptActive("AhriCharm", "AhriCharm"))
            {
                damages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            }

            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_tar.troy", owner);
            if (target == owner)
                return;
            target.TakeDamage(owner, damages, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void OnUpdate(double diff)
        {
            if (isLaunched && projectile.Target == null)
            {
                returnProj = owner.GetSpellByName("AhriOrbReturn").AddProjectileHitAllTargets("AhriOrbReturn", owner, spellPositionToGo);
                ApiFunctionManager.DashToLocation(owner, owner.X, owner.Y, 1000, true);
                ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_mis_02.troy", returnProj);
                isLaunched = false;
            }
            if (returnProj != null && returnProj.Target != null && !returnProj.IsToRemove())
            {
                //Setting move speed manually because I can't get it to work with config file
                returnProj.SetMoveSpeed(returnProj.GetMoveSpeed() + 50);
                if (returnProj.GetMoveSpeed() > 2600)
                {
                    returnProj.SetMoveSpeed(2600);
                }
            }
        }
    }

    public class AhriOrbReturn : IGameScript
    {
        public Vector2 spellPositionToGo;

        public void OnActivate(Champion owner)
        {
        }

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
        }
        
        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            float apDamages = owner.Stats.AbilityPower.Total * AhriConsts.Q_AP_RATIO;
            float damages = AhriConsts.Q_BASE_DAMAGES + owner.GetSpellByName("AhriOrbofDeception").Level * AhriConsts.Q_DAMAGES_LEVEL_SCALING + apDamages;

            if (((ObjAiBase)target).HasBuffGameScriptActive("AhriCharm", "AhriCharm"))
            {
                damages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            }

            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_tar.troy", target);
            if (target == owner)
            {
                projectile.SetToRemove();
                projectile.Target = null;
                return;
            }
            target.TakeDamage(owner, damages, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void OnUpdate(double diff)
        {
        }
    }
}