using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Data;
using JRPG2D5.Enums;

namespace JRPG2D5.Combat.Commands
{
    public class ExecuteSkillCommand : ICombatCommand
    {
        readonly Combatant _source;
        readonly Combatant _target;
        readonly SkillData _skill;
        readonly CombatSession _session;
        readonly bool _isFollowUp;

        public ExecuteSkillCommand(
            Combatant source,
            Combatant target,
            SkillData skill,
            CombatSession session,
            bool isFollowUp = false)
        {
            _source = source;
            _target = target;
            _skill = skill;
            _session = session;
            _isFollowUp = isFollowUp;
        }

        public bool CanExecute()
        {
            if (_source == null || _target == null || _skill == null || _session == null) return false;
            if (!_source.IsAlive || !_target.IsAlive || _source.IsStunned) return false;
            if (!_isFollowUp && !_source.TryPreviewMp(_skill.mpCost)) return false;
            if (!_isFollowUp && _session.AnotherForce != null && _session.AnotherForce.IsActive && !_source.CanUseSkillInAf())
                return false;
            return true;
        }

        public void Execute()
        {
            if (!CanExecute()) return;

            if (!_isFollowUp && !_source.TrySpendMp(_skill.mpCost)) return;

            int hits = System.Math.Max(1, _skill.hitCount);
            int totalDamage = 0;
            bool anyCrit = false;
            bool anyWeakness = false;
            Element element = _skill.element;

            for (int i = 0; i < hits; i++)
            {
                var result = DamageResolver.Resolve(_source, _target, _skill, _session.Stance);
                totalDamage += _target.ApplyDamage(result.Damage);
                anyCrit |= result.IsCritical;
                anyWeakness |= result.IsWeakness;
                _session.AnotherForce?.NotifyDamageReceived(result.Damage, _source);
            }

            if (!_isFollowUp && _session.AnotherForce != null && _session.AnotherForce.IsActive)
            {
                float cd = _session.AnotherForce.CalculateAFCooldown(_skill.baseCooldownAF);
                _source.SetAfCooldown(cd);
            }

            _session.AnotherForce?.AddGaugeFromAction(element, anyWeakness, anyCrit);

            GameEvents.TriggerActionExecuted(_source, _target, totalDamage, element, anyCrit);
            if (anyWeakness)
                GameEvents.TriggerWeaknessHit(_target, element);
        }
    }

    static class CombatantMpPreview
    {
        public static bool TryPreviewMp(this Combatant source, int cost)
        {
            return source != null && source.CurrentMP >= cost;
        }
    }
}
