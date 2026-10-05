using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;

namespace JRPG2D5.Combat.Rules
{
    /// <summary>
    /// Interrupts AF and flags a stun if the party launches AF during the boss counter stance.
    /// </summary>
    public sealed class CounterStanceRule : IAnotherForceRule
    {
        public bool IsCounterStanceActive { get; set; }

        public CounterStanceRule(bool startActive)
        {
            IsCounterStanceActive = startActive;
        }

        public void OnAFStarted(AnotherForceManager manager) { }

        public void OnDamageReceived(int damage, Combatant source) { }

        public float ModifyDecayFactor(float currentDecayFactor) => currentDecayFactor;

        public bool ShouldInterruptAf() => IsCounterStanceActive;
    }
}
