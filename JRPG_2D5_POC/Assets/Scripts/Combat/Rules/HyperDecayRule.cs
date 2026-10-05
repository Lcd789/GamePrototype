using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;

namespace JRPG2D5.Combat.Rules
{
    public sealed class HyperDecayRule : IAnotherForceRule
    {
        readonly float _multiplier;

        public HyperDecayRule(float multiplier)
        {
            _multiplier = multiplier <= 0f ? 10f : multiplier;
        }

        public void OnAFStarted(AnotherForceManager manager) { }

        public void OnDamageReceived(int damage, Combatant source) { }

        public float ModifyDecayFactor(float currentDecayFactor)
        {
            return currentDecayFactor * _multiplier;
        }

        public bool ShouldInterruptAf() => false;
    }
}
