using JRPG2D5.Combat.Rules;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Enums;
using UnityEngine;

namespace JRPG2D5.Combat.Systems
{
    public class AnotherForceManager : MonoBehaviour
    {
        [SerializeField] float maxGauge = 100f;
        [SerializeField] float baseGaugePerAction = 8f;
        [SerializeField] float weaknessBonus = 6f;
        [SerializeField] float critBonus = 4f;
        [SerializeField] float decayFactor = 0.15f;
        [SerializeField] float maxDuration = 20f;

        IAnotherForceRule _bossRule;

        public bool IsActive { get; private set; }
        public float Gauge { get; private set; }
        public float ElapsedTime { get; private set; }
        public float DecayFactor => decayFactor;
        public bool IsGaugeFull => Gauge >= maxGauge;

        public void RegisterBossRule(IAnotherForceRule rule)
        {
            _bossRule = rule;
        }

        public float CalculateAFCooldown(float baseCooldown)
        {
            float decay = decayFactor;
            if (_bossRule != null)
                decay = _bossRule.ModifyDecayFactor(decay);
            return baseCooldown * (1f + (ElapsedTime * decay));
        }

        public void AddGaugeFromAction(Element element, bool weakness, bool crit)
        {
            if (IsActive) return;

            float amount = baseGaugePerAction;
            if (weakness) amount += weaknessBonus;
            if (crit) amount += critBonus;

            var stance = GetComponent<StanceManager>();
            if (stance != null)
                amount *= stance.GetAfFillMultiplier(element);

            Gauge = Mathf.Min(maxGauge, Gauge + amount);
            GameEvents.TriggerAFGaugeChanged(Gauge / maxGauge * 100f);
        }

        public bool TryStart()
        {
            if (IsActive || Gauge < maxGauge) return false;

            if (_bossRule != null && _bossRule.ShouldInterruptAf())
            {
                Gauge = 0f;
                GameEvents.TriggerAFGaugeChanged(0f);
                GameEvents.TriggerAFInterrupted();
                return false;
            }

            IsActive = true;
            ElapsedTime = 0f;
            Gauge = 0f;
            _bossRule?.OnAFStarted(this);
            GameEvents.TriggerAFStarted();
            GameEvents.TriggerAFGaugeChanged(0f);
            return true;
        }

        public void ForceEnd()
        {
            if (!IsActive) return;
            IsActive = false;
            ElapsedTime = 0f;
            GameEvents.TriggerAFEnded();
        }

        public void NotifyDamageReceived(int damage, Combatant source)
        {
            if (!IsActive) return;
            _bossRule?.OnDamageReceived(damage, source);
        }

        public void Tick(float deltaTime)
        {
            if (!IsActive) return;
            ElapsedTime += deltaTime;
            if (ElapsedTime >= maxDuration)
                ForceEnd();
        }

        public void ResetForNewBattle()
        {
            IsActive = false;
            Gauge = 0f;
            ElapsedTime = 0f;
            GameEvents.TriggerAFGaugeChanged(0f);
        }
    }
}
