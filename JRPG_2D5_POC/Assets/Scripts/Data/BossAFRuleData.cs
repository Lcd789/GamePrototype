using UnityEngine;

namespace JRPG2D5.Data
{
    public enum AnotherForceRuleType
    {
        None,
        Absorber,
        HyperDecay,
        CounterStance,
        RelicRedirect
    }

    [CreateAssetMenu(fileName = "NewBossAFRule", menuName = "JRPG2D5/Data/Boss AF Rule")]
    public class BossAFRuleData : ScriptableObject
    {
        public AnotherForceRuleType ruleType = AnotherForceRuleType.HyperDecay;

        [Header("Hyper-Decay")]
        [Tooltip("Multiplies AF cooldown decay. GDD default for Hyper-Decay is 10.")]
        public float decayMultiplier = 10f;

        [Header("Absorber / Temporal Riposte")]
        [Range(0f, 2f)]
        public float absorbHealPercent = 1f;

        [Header("Counter Stance")]
        public bool startInCounterStance;

        [Header("Relic Redirect")]
        public int relicHitPoints = 300;
    }
}
