using System.Collections.Generic;
using UnityEngine;
using JRPG2D5.Enums;

namespace JRPG2D5.Data
{
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "JRPG2D5/Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string enemyName = "Monster";
        public Sprite combatSprite;
        public bool isFEAR = false;
        public bool isBoss = false;

        [Header("Stats")]
        public int maxHP = 1000;
        public int attack = 40;
        public int defense = 20;

        [Header("Elemental Affinities")]
        public List<Element> weaknesses = new List<Element>();
        public List<Element> resistances = new List<Element>();

        [Header("Skills")]
        public List<SkillData> skills = new List<SkillData>();

        [Header("Boss Another Force Rules")]
        public List<BossAFRuleData> anotherForceRules = new List<BossAFRuleData>();
    }
}
