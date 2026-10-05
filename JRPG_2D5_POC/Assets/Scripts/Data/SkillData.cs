using UnityEngine;
using JRPG2D5.Enums;

namespace JRPG2D5.Data
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "JRPG2D5/Data/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName = "New Skill";
        [TextArea] public string description;
        public Sprite icon;

        [Header("Costs & Cooldowns")]
        public int mpCost = 10;
        public float baseCooldownAF = 1.0f; // Base cooldown during Another Force

        [Header("Combat Properties")]
        public Element element = Element.None;
        public int basePower = 100;
        public int hitCount = 1;

        [Header("Follow-Up Skill")]
        public bool isFollowUp;
        public bool triggerOnCritical;
        public Element triggerOnWeakness = Element.None;
    }
}
