using JRPG2D5.Combat.Commands;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Data;
using JRPG2D5.Enums;

namespace JRPG2D5.Combat.Systems
{
    public class FollowUpSystem
    {
        readonly CombatSession _session;
        bool _running;

        public FollowUpSystem(CombatSession session)
        {
            _session = session;
        }

        public void Bind()
        {
            GameEvents.OnCriticalHit += HandleCrit;
            GameEvents.OnWeaknessHit += HandleWeakness;
        }

        public void Unbind()
        {
            GameEvents.OnCriticalHit -= HandleCrit;
            GameEvents.OnWeaknessHit -= HandleWeakness;
        }

        void HandleCrit(object source)
        {
            TryFollowUp(source as Combatant, triggerCrit: true, weaknessElement: Element.None);
        }

        void HandleWeakness(object target, Element element)
        {
            TryFollowUp(null, triggerCrit: false, weaknessElement: element);
        }

        void TryFollowUp(Combatant triggerSource, bool triggerCrit, Element weaknessElement)
        {
            if (_running) return;
            if (_session == null || _session.Party == null) return;
            _running = true;
            try
            {

            var front = _session.Party.Frontline;
            for (int i = 0; i < front.Count; i++)
            {
                var ally = front[i];
                if (ally == null || !ally.IsAlive || ally == triggerSource) continue;
                var skill = FindFollowUp(ally, triggerCrit, weaknessElement);
                if (skill == null) continue;

                var target = _session.FindLivingEnemy();
                if (target == null) return;

                var command = new ExecuteSkillCommand(ally, target, skill, _session, isFollowUp: true);
                command.Execute();
                return;
            }
            }
            finally
            {
                _running = false;
            }
        }

        static SkillData FindFollowUp(Combatant ally, bool triggerCrit, Element weaknessElement)
        {
            if (ally.CharacterData == null || ally.CharacterData.skills == null) return null;
            var skills = ally.CharacterData.skills;
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                if (skill == null || !skill.isFollowUp) continue;
                if (triggerCrit && skill.triggerOnCritical) return skill;
                if (weaknessElement != Element.None && skill.triggerOnWeakness == weaknessElement) return skill;
            }
            return null;
        }
    }
}
