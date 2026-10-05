using System.Collections.Generic;
using UnityEngine;

namespace JRPG2D5.Data
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "JRPG2D5/Data/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        public string characterName = "Hero";
        public Sprite portrait;

        [Header("Base Stats")]
        public int maxHP = 500;
        public int maxMP = 100;
        public int attack = 50;
        public int defense = 30;
        public int speed = 20;

        [Header("Skills & Valor Chant")]
        public SkillData valorChantSkill;
        public List<SkillData> skills = new List<SkillData>();

        [Header("Health-Based Posture Sprites")]
        public Sprite idleNormalSprite;   // HP > 60%
        public Sprite idleDamagedSprite;  // 25% < HP <= 60%
        public Sprite idleCriticalSprite; // HP <= 25%
    }
}
