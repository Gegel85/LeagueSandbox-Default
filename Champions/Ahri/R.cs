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
    public class AhriTumble : IGameScript
    {
        private Spell   spell;
        private Champion owner;
        private int used = 0;
        private bool isDashing = false;
        private bool needsToReduce = false;
        private bool hasBeenReduced = true;
        private Buff dashesBuff;
        private Particle dashParticule;
        
        public void OnActivate(Champion owner)
        {
        }

        public void OnDeactivate(Champion owner)
        {
        }

        private void DesactivateSpell()
        {
            if (hasBeenReduced)
            {
                spell.SetCooldown(spell.Slot, AhriConsts.DASH_BASE_CD - spell.Level * AhriConsts.DASH_CD_SCALING);
                hasBeenReduced = false;
                used = 0;
                ApiFunctionManager.RemoveBuffHudVisual(dashesBuff);
                ApiFunctionManager.RemoveParticle(dashParticule);
            }
        }

        public void OnStartCasting(Champion owner, Spell spell, AttackableUnit target)
        {
            Vector2 currentPos = new Vector2(owner.X, owner.Y);
            Vector2 angleVector = Vector2.Normalize(new Vector2(spell.X, spell.Y) - currentPos);
            Vector2 range = angleVector * AhriConsts.DASH_RANGE;
            Vector2 dashLocation = currentPos + range;
            
            this.spell = spell;
            this.owner = owner;
            needsToReduce = true;
            isDashing = true;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_SpiritRush_cas.troy", owner);
            if (used == 0)
            {
                dashParticule = ApiFunctionManager.AddParticleTarget(owner, "AhriSpiritRushReady_tar.troy", owner);
                dashesBuff = ApiFunctionManager.AddBuffHudVisual("AhriTumble", AhriConsts.DASH_EXPIRATION_TIME, 2, owner);
                ApiFunctionManager.CreateTimer(10, DesactivateSpell);
                used = used + 1;
            }
            else if (used == 1)
            {
                ApiFunctionManager.EditBuff(dashesBuff, 1);
                used = used + 1;
                owner.Stats.CurrentMana += AhriConsts.DASH_MANA_COST;
            }
            else
            {
                needsToReduce = false;
                DesactivateSpell();
                owner.Stats.CurrentMana += AhriConsts.DASH_MANA_COST;
            }
            ApiFunctionManager.DashToLocation(owner, dashLocation.X, dashLocation.Y, AhriConsts.DASH_SPEED, false, "Spell4");
        }

        public void OnFinishCasting(Champion owner, Spell spell, AttackableUnit target)
        {
        }

        public void ApplyDashDamages()
        {
            AttackableUnit[] targets = new AttackableUnit[3];
            bool foundUnit = false;
            
            foreach (AttackableUnit enemyTarget in ApiFunctionManager.GetUnitsInRange(owner, 300, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(owner.Team)))
            {
                if (enemyTarget == null)
                    continue;
                if (enemyTarget is Champion || enemyTarget is Minion || enemyTarget is Monster)
                {
                    foundUnit = false;
                    for (int i = 0; !foundUnit && i < targets.Length; i++)
                    {
                        if (targets[i] == null)
                        {
                            targets[i] = enemyTarget;
                            foundUnit = true;
                        }
                    }
                    for (int i = 0; !foundUnit && i < targets.Length; i++)
                    {
                        if (owner.GetDistanceTo(enemyTarget) < owner.GetDistanceTo(targets[i]) ||
                            (enemyTarget is Champion && !(targets[i] is Champion)))
                        {
                            targets[i] = enemyTarget;
                            foundUnit = true;
                        }
                    }
                }
            }
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                    spell.AddProjectileTarget("AhriTumbleMissile", targets[i]);
            }
        }

        public void ApplyEffects(Champion owner, AttackableUnit target, Spell spell, Projectile projectile)
        {
            float bonusDamages = owner.Stats.AbilityPower.Total * AhriConsts.DASH_AP_RATIO;
            float baseDamage = AhriConsts.DASH_BASE_DAMAGES + AhriConsts.DASH_DAMAGES_LEVEL_SCALING * spell.Level;
            float totalDamages = baseDamage + bonusDamages;

            if (((ObjAiBase)target).HasBuffGameScriptActive("AhriCharm", "AhriCharm"))
                totalDamages *= AhriConsts.E_AMPLIFIED_DAMAGES;
            ApiFunctionManager.AddParticleTarget(owner, "Ahri_SpiritRush_tar.troy", owner);
            target.TakeDamage(owner, totalDamages, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            projectile.SetToRemove();
        }

        public void OnUpdate(double diff)
        {
            if (spell != null && needsToReduce)
            {
                hasBeenReduced = true;
                needsToReduce = false;
                spell.LowerCooldown(spell.Slot, spell.CurrentCooldown - AhriConsts.COOLDOWN_BETWEEN_DASHES);
            }
            if (isDashing && !owner.IsDashing)
            {
                isDashing = false;
                ApplyDashDamages();
            }
        }
    }
}