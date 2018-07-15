using System.Linq;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.Logic.GameObjects.Spells;
using LeagueSandbox.GameServer.Logic.GameObjects.Missiles;

namespace Spells
{

    public class AkaliShadowSwipe : IGameScript
    {

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
            var ap = owner.Stats.AbilityPower.Total * 0.3f;
            var ad = owner.Stats.AttackDamage.Total * 0.6f;
            var damage = 5 + spell.Level * 25 + ap + ad;
            foreach (var enemyTarget in ApiFunctionManager.GetUnitsInRange(owner, 300, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(owner.Team)))
            {
                if (enemyTarget == null)
                    continue;
                if(enemyTarget is Champion || enemyTarget is Minion || enemyTarget is Monster)
                {
                    AkaliMota.OnProc(enemyTarget, false);
                    enemyTarget.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL,
                        false);
                }
            }
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
           
        }

        public void OnUpdate(double diff)
        {
        }
    }
}
