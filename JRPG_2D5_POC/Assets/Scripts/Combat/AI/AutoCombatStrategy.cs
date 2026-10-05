using JRPG2D5.Combat.Commands;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.Systems;
using JRPG2D5.Data;

namespace JRPG2D5.Combat.AI
{
    public enum AutoCombatMode
    {
        Off,
        Basic,
        Smart,
        AutoAf
    }

    public interface IAutoCombatStrategy
    {
        void Tick(CombatSession session);
    }

    public static class AutoCombatStrategyFactory
    {
        public static IAutoCombatStrategy Create(AutoCombatMode mode)
        {
            switch (mode)
            {
                case AutoCombatMode.Basic:
                    return new BasicAutoStrategy();
                case AutoCombatMode.Smart:
                    return new SmartAutoStrategy();
                case AutoCombatMode.AutoAf:
                    return new AutoAfStrategy();
                default:
                    return null;
            }
        }
    }

    public sealed class BasicAutoStrategy : IAutoCombatStrategy
    {
        public void Tick(CombatSession session)
        {
            AutoCombatUtility.AttackWithFirstReadySkill(session, preferWeakness: false);
        }
    }

    public sealed class SmartAutoStrategy : IAutoCombatStrategy
    {
        public void Tick(CombatSession session)
        {
            AutoCombatUtility.AttackWithFirstReadySkill(session, preferWeakness: true);
        }
    }

    public sealed class AutoAfStrategy : IAutoCombatStrategy
    {
        public void Tick(CombatSession session)
        {
            if (session.AnotherForce != null && session.AnotherForce.IsGaugeFull)
                session.AnotherForce.TryStart();

            AutoCombatUtility.AttackWithFirstReadySkill(session, preferWeakness: true);
        }
    }

    static class AutoCombatUtility
    {
        public static void AttackWithFirstReadySkill(CombatSession session, bool preferWeakness)
        {
            if (session == null || session.Party == null) return;
            var front = session.Party.Frontline;
            for (int i = 0; i < front.Count; i++)
            {
                var source = front[i];
                if (source == null || !source.IsAlive || source.IsStunned) continue;
                var skill = PickSkill(source, preferWeakness);
                if (skill == null) continue;

                Combatant target = preferWeakness
                    ? session.FindWeaknessTarget(skill.element)
                    : session.FindLivingEnemy();
                if (target == null) return;

                session.ExecuteCommand(new ExecuteSkillCommand(source, target, skill, session));
                return;
            }
        }

        static SkillData PickSkill(Combatant source, bool preferWeakness)
        {
            if (source.CharacterData == null || source.CharacterData.skills == null) return null;
            var skills = source.CharacterData.skills;
            SkillData fallback = null;
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                if (skill == null || skill.isFollowUp) continue;
                if (source.CurrentMP < skill.mpCost) continue;
                if (source.AfCooldownRemaining > 0f) continue;
                fallback ??= skill;
                if (preferWeakness && skill.element != JRPG2D5.Enums.Element.None)
                    return skill;
            }
            return fallback;
        }
    }
}
