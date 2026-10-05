using System.Collections;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Enums;
using UnityEngine;

namespace JRPG2D5.Gamefeel
{
    public class FrameFreezeManager : MonoBehaviour
    {
        [SerializeField] float hitstopDuration = 0.03f;
        [SerializeField] float timeScaleDuringFreeze = 0.02f;

        bool _running;

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
            if (damage <= 0 || _running) return;
            StartCoroutine(FreezeRoutine(isCrit ? hitstopDuration * 1.5f : hitstopDuration));
        }

        IEnumerator FreezeRoutine(float duration)
        {
            _running = true;
            float previous = Time.timeScale;
            Time.timeScale = timeScaleDuringFreeze;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = previous <= 0f ? 1f : previous;
            _running = false;
        }
    }
}
