using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using UnityEngine;

namespace JRPG2D5.Combat.Rules
{
    /// <summary>
    /// Redirects AF damage onto relics until the shield budget is depleted.
    /// </summary>
    public sealed class RelicRedirectRule : IAnotherForceRule
    {
        readonly Combatant _boss;
        int _relicHp;

        public int RemainingRelicHp => _relicHp;
        public bool ShieldActive => _relicHp > 0;

        public RelicRedirectRule(Combatant boss, int relicHitPoints)
        {
            _boss = boss;
            _relicHp = Mathf.Max(0, relicHitPoints);
        }

        public void OnAFStarted(AnotherForceManager manager) { }

        public void OnDamageReceived(int damage, Combatant source)
        {
            if (_relicHp <= 0 || damage <= 0) return;

            int absorbed = Mathf.Min(_relicHp, damage);
            _relicHp -= absorbed;

            if (_boss != null && absorbed > 0)
                _boss.Heal(absorbed);
        }

        public float ModifyDecayFactor(float currentDecayFactor) => currentDecayFactor;

        public bool ShouldInterruptAf() => false;
    }
}
