using JRPG2D5.Core.EventBus;
using JRPG2D5.Enums;
using Unity.Cinemachine;
using UnityEngine;

namespace JRPG2D5.Gamefeel
{
    public class CameraShakeTrigger : MonoBehaviour
    {
        [SerializeField] CinemachineImpulseSource impulseSource;
        [SerializeField] float critImpulse = 1.25f;
        [SerializeField] float normalImpulse = 0.45f;

        void OnEnable()
        {
            GameEvents.OnActionExecuted += HandleAction;
        }

        void OnDisable()
        {
            GameEvents.OnActionExecuted -= HandleAction;
        }

        void HandleAction(object source, object target, int damage, Element element, bool isCrit)
        {
            if (impulseSource == null || damage <= 0) return;
            impulseSource.GenerateImpulse(isCrit ? critImpulse : normalImpulse);
        }
    }
}
