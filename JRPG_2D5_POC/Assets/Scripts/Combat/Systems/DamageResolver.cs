using JRPG2D5.Combat.Runtime;
using JRPG2D5.Data;
using JRPG2D5.Enums;
using UnityEngine;

namespace JRPG2D5.Combat.Systems
{
    public readonly struct DamageResult
    {
        public readonly int Damage;
        public readonly bool IsCritical;
        public readonly bool IsWeakness;

        public DamageResult(int damage, bool isCritical, bool isWeakness)
        {
            Damage = damage;
            IsCritical = isCritical;
            IsWeakness = isWeakness;
        }
    }

    public static class DamageResolver
    {
        const float CritChance = 0.15f;
        const float CritMultiplier = 1.5f;
        const float WeaknessMultiplier = 1.5f;
        const float ResistMultiplier = 0.5f;

        public static DamageResult Resolve(Combatant source, Combatant target, SkillData skill, StanceManager stance)
        {
            float raw = skill.basePower * ((float)source.Attack / Mathf.Max(1, target.Defense));
            bool weakness = false;
            bool resist = false;

            if (target.EnemyData != null)
            {
                weakness = target.EnemyData.weaknesses != null && target.EnemyData.weaknesses.Contains(skill.element);
                resist = target.EnemyData.resistances != null && target.EnemyData.resistances.Contains(skill.element);
            }

            if (weakness) raw *= WeaknessMultiplier;
            else if (resist) raw *= ResistMultiplier;

            if (stance != null)
                raw *= stance.GetDamageMultiplier(skill.element);

            bool crit = Random.value < CritChance;
            if (crit) raw *= CritMultiplier;

            int damage = Mathf.Max(1, Mathf.RoundToInt(raw));
            return new DamageResult(damage, crit, weakness);
        }
    }
}
