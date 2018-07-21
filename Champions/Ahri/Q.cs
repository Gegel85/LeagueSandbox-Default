using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.GameObjects.Spells;
using LeagueSandbox.GameServer.Logic.GameObjects.Missiles;
using System.Numerics;
using LeagueSandbox.Champions.Ahri;
using System.Linq;
using LeagueSandbox.GameServer;

namespace Spells
{
    public class AhriOrbofDeception : IGameScript
    {
        private Champion owner = null;
        private Spell spell = null;
        private Vector2 spellPositionToGo;
        private Projectile projectile;
        private bool isLaunched = false;

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
                damages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_tar.troy", owner);
            if (target == owner)
                return;
            target.TakeDamage(owner, damages, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        Projectile proj = null;
        public void OnUpdate(double diff)
        {

            if (isLaunched && projectile.Target == null)
            {
                //proj = owner.GetSpellByName("AhriOrbReturn").AddProjectile("AhriOrbReturn", owner.X, owner.Y, false, spellPositionToGo.X, spellPositionToGo.Y);
                proj = owner.GetSpellByName("AhriOrbReturn").AddProjectileTarget("AhriOrbReturn", owner, startX: spellPositionToGo.X, startY: spellPositionToGo.Y);
                //proj = spell.AddProjectileTarget("AhriOrbReturn", owner, x: spellPositionToGo.X, y: spellPositionToGo.Y);
                ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_mis_02.troy", proj);
                ApiFunctionManager.DashToLocation(owner, owner.X, owner.Y, 1000, true);
                isLaunched = false;
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
            ApiFunctionManager.LogInfo("Hit " + target);
            float apDamages = owner.Stats.AbilityPower.Total * AhriConsts.Q_AP_RATIO;
            float damages = AhriConsts.Q_BASE_DAMAGES + spell.Level * AhriConsts.Q_DAMAGES_LEVEL_SCALING + apDamages;

            if (((ObjAiBase)target).HasBuffGameScriptActive("AhriCharm", "AhriCharm"))
                damages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Orb_tar.troy", owner);
            if (target == owner)
            {
                projectile.SetToRemove();
                return;
            }
            target.TakeDamage(owner, damages, DamageType.DAMAGE_TYPE_TRUE, DamageSource.DAMAGE_SOURCE_ATTACK, false);
        }

        public void OnUpdate(double diff)
        {
        }
    }
}