using JRPG2D5.Core.EventBus;
using JRPG2D5.Data;
using JRPG2D5.Enums;
using UnityEngine;

namespace JRPG2D5.Combat.Systems
{
    public class StanceManager : MonoBehaviour
    {
        [SerializeField] StanceData currentStance;

        public StanceData CurrentStance => currentStance;

        public void SetStance(StanceData stance)
        {
            currentStance = stance;
            Element element = stance != null ? stance.boostedElement : Element.None;
            GameEvents.TriggerStanceChanged(element);
        }

        public float GetDamageMultiplier(Element skillElement)
        {
            if (currentStance == null || skillElement == Element.None) return 1f;
            if (skillElement == currentStance.boostedElement) return currentStance.damageMultiplier;
            if (skillElement == currentStance.weakenedElement) return 1f / currentStance.damageMultiplier;
            return 1f;
        }

        public float GetAfFillMultiplier(Element skillElement)
        {
            if (currentStance == null || skillElement == Element.None) return 1f;
            if (skillElement == currentStance.boostedElement) return currentStance.afFillMultiplier;
            return 1f;
        }
    }
}
