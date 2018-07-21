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
    public class AhriSeduce : IGameScript
    {
        public void OnActivate(Champion owner)
        {
        }

        public void OnDeactivate(Champion owner)
        {
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Charm_cas.troy", owner);
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            Vector2 currentPos = new Vector2(owner.X, owner.Y);
            Vector2 angleVector = Vector2.Normalize(new Vector2(spell.X, spell.Y) - currentPos);
            Vector2 range = angleVector * AhriConsts.E_RANGE;
            Vector2 spellPositionToGo = range + currentPos;

            spell.AddProjectile("AhriSeduceMissile", spellPositionToGo.X, spellPositionToGo.Y);
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            BuffGameScriptController debuff = ((ObjAiBase)target).AddBuffGameScript("AhriCharm", "AhriCharm", spell);
            Particle charmParticle = ApiFunctionManager.AddParticleTarget(owner, "Ahri_Charm_buf.troy", target);
            float apDamages = owner.Stats.AbilityPower.Total * AhriConsts.E_AP_RATIO;
            float damage = AhriConsts.E_BASE_DAMAGES + spell.Level * AhriConsts.E_DAMAGES_LEVEL_SCALING + apDamages;
            float time = AhriConsts.E_CC_TIME_BASE + AhriConsts.E_CC_TIME_SCALE * spell.Level;
            Buff seduceBuff = ApiFunctionManager.AddBuffHudVisual("AhriSeduce", time, 1, (ObjAiBase)target);
            Buff doomBuff = ApiFunctionManager.AddBuffHudVisual("AhriSeduceDoom", time, 1, (ObjAiBase)target);
            
            projectile.SetToRemove();
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_ATTACK, false);
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_Charm_tar.troy", target);
            ApiFunctionManager.CreateTimer(time, () =>
            {
                ApiFunctionManager.RemoveBuffHudVisual(seduceBuff);
                ApiFunctionManager.RemoveBuffHudVisual(doomBuff);
                ApiFunctionManager.RemoveParticle(charmParticle);
                ((ObjAiBase)target).RemoveBuffGameScript(debuff);
            });
        }

        public void OnUpdate(double diff)
        {

        }
    }
}