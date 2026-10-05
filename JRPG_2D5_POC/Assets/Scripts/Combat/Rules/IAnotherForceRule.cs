using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;

namespace JRPG2D5.Combat.Rules
{
    /// <summary>
    /// Strategy Pattern: inject boss-specific AF behaviour without changing AnotherForceManager.
    /// </summary>
    public interface IAnotherForceRule
    {
        void OnAFStarted(AnotherForceManager manager);
        void OnDamageReceived(int damage, Combatant source);
        float ModifyDecayFactor(float currentDecayFactor);
        bool ShouldInterruptAf();
    }
}
