using LeagueSandbox.GameServer.Logic.API;
using LeagueSandbox.GameServer.Logic.Scripting.CSharp;
using LeagueSandbox.GameServer.Logic.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.Logic.GameObjects.Spells;
using LeagueSandbox.GameServer.Logic.GameObjects.Stats;

namespace AhriCharm
{
    internal class AhriCharm : IBuffGameScript
    {
        private StatsModifier _statMod;
        private UnitCrowdControl _silence = new UnitCrowdControl(CrowdControlType.SILENCE);
        private UnitCrowdControl _disarm = new UnitCrowdControl(CrowdControlType.DISARM);
        private ObjAiBase unit;
        private Champion owner;

        public void OnActivate(ObjAiBase unit, Spell ownerSpell)
        {
            this.unit = unit;
            owner = ownerSpell.Owner;
            _statMod = new StatsModifier();
            _statMod.MoveSpeed.PercentBonus = _statMod.MoveSpeed.PercentBonus - 0.60f;
            unit.AddStatModifier(_statMod);
            unit.ApplyCrowdControl(_silence);
            unit.ApplyCrowdControl(_disarm);
            ApiFunctionManager.DashToUnit(unit, ownerSpell.Owner, unit.Stats.MoveSpeed.Total, false);
        }

        public void OnDeactivate(ObjAiBase unit)
        {
            unit.RemoveStatModifier(_statMod);
            unit.RemoveCrowdControl(_silence);
            unit.RemoveCrowdControl(_disarm);
            ApiFunctionManager.CancelDash(unit);
        }

        public void OnUpdate(double diff)
        {
        }
    }
}
