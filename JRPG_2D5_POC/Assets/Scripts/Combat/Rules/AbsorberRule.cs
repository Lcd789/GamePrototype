using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using UnityEngine;

namespace JRPG2D5.Combat.Rules
{
    /// <summary>
    /// Converts AF damage into boss healing (temporal absorber).
    /// </summary>
    public sealed class AbsorberRule : IAnotherForceRule
    {
        readonly Combatant _boss;
        readonly float _healPercent;
        bool _afActive;

        public AbsorberRule(Combatant boss, float healPercent)
        {
            _boss = boss;
            _healPercent = Mathf.Clamp(healPercent, 0f, 2f);
        }

        public void OnAFStarted(AnotherForceManager manager)
        {
            _afActive = true;
        }

        public void OnDamageReceived(int damage, Combatant source)
        {
            if (!_afActive || _boss == null || damage <= 0) return;
            int heal = Mathf.RoundToInt(damage * _healPercent);
            _boss.Heal(heal);
        }

        public float ModifyDecayFactor(float currentDecayFactor) => currentDecayFactor;

        public bool ShouldInterruptAf() => false;
    }
}
