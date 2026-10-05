using System.Collections.Generic;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using JRPG2D5.Data;

namespace JRPG2D5.Combat.Rules
{
    public sealed class CompositeAnotherForceRule : IAnotherForceRule
    {
        readonly List<IAnotherForceRule> _rules = new List<IAnotherForceRule>();

        public CompositeAnotherForceRule(IEnumerable<IAnotherForceRule> rules)
        {
            if (rules == null) return;
            foreach (var rule in rules)
            {
                if (rule != null) _rules.Add(rule);
            }
        }

        public void OnAFStarted(AnotherForceManager manager)
        {
            for (int i = 0; i < _rules.Count; i++)
                _rules[i].OnAFStarted(manager);
        }

        public void OnDamageReceived(int damage, Combatant source)
        {
            for (int i = 0; i < _rules.Count; i++)
                _rules[i].OnDamageReceived(damage, source);
        }

        public float ModifyDecayFactor(float currentDecayFactor)
        {
            float value = currentDecayFactor;
            for (int i = 0; i < _rules.Count; i++)
                value = _rules[i].ModifyDecayFactor(value);
            return value;
        }

        public bool ShouldInterruptAf()
        {
            for (int i = 0; i < _rules.Count; i++)
            {
                if (_rules[i].ShouldInterruptAf()) return true;
            }
            return false;
        }
    }

    public static class AnotherForceRuleFactory
    {
        public static IAnotherForceRule Create(IList<BossAFRuleData> datas, Combatant boss)
        {
            if (datas == null || datas.Count == 0) return null;

            var rules = new List<IAnotherForceRule>(datas.Count);
            for (int i = 0; i < datas.Count; i++)
            {
                var rule = Create(datas[i], boss);
                if (rule != null) rules.Add(rule);
            }

            if (rules.Count == 0) return null;
            if (rules.Count == 1) return rules[0];
            return new CompositeAnotherForceRule(rules);
        }

        public static IAnotherForceRule Create(BossAFRuleData data, Combatant boss)
        {
            if (data == null) return null;
            switch (data.ruleType)
            {
                case AnotherForceRuleType.Absorber:
                    return new AbsorberRule(boss, data.absorbHealPercent);
                case AnotherForceRuleType.HyperDecay:
                    return new HyperDecayRule(data.decayMultiplier);
                case AnotherForceRuleType.CounterStance:
                    return new CounterStanceRule(data.startInCounterStance);
                case AnotherForceRuleType.RelicRedirect:
                    return new RelicRedirectRule(boss, data.relicHitPoints);
                default:
                    return null;
            }
        }
    }
}
