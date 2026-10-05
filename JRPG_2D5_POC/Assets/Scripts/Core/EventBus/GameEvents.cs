using System;
using UnityEngine;
using JRPG2D5.Enums;

namespace JRPG2D5.Core.EventBus
{
    /// <summary>
    /// Event-Driven Architecture (Observer Pattern)
    /// Allows decoupled communication across Combat, UI, Exploration, and Gamefeel systems.
    /// </summary>
    public static class GameEvents
    {
        // Combat Events
        public static event Action<object, object, int, Element, bool> OnActionExecuted; // Source, Target, Damage, Element, IsCrit
        public static event Action<object> OnCriticalHit;
        public static event Action<object, Element> OnWeaknessHit;
        
        // Another Force Events
        public static event Action OnAFStarted;
        public static event Action OnAFEnded;
        public static event Action OnAFInterrupted;
        public static event Action<float> OnAFGaugeChanged; // Current percentage (0 to 100)

        // Stance / Zone Events
        public static event Action<Element> OnStanceChanged;

        // Party Events
        public static event Action<int, int> OnCharacterSwapped; // FrontlineIndex, ReserveIndex
        public static event Action<object> OnValorChantTriggered;

        // Exploration & State Events
        public static event Action OnCombatStarted;
        public static event Action OnCombatEnded;

        // Dispatcher helpers
        public static void TriggerActionExecuted(object source, object target, int damage, Element element, bool isCrit)
        {
            OnActionExecuted?.Invoke(source, target, damage, element, isCrit);
            if (isCrit) OnCriticalHit?.Invoke(source);
        }

        public static void TriggerWeaknessHit(object target, Element element)
        {
            OnWeaknessHit?.Invoke(target, element);
        }

        public static void TriggerAFStarted() => OnAFStarted?.Invoke();
        public static void TriggerAFEnded() => OnAFEnded?.Invoke();
        public static void TriggerAFInterrupted() => OnAFInterrupted?.Invoke();
        public static void TriggerAFGaugeChanged(float val) => OnAFGaugeChanged?.Invoke(val);
        public static void TriggerStanceChanged(Element element) => OnStanceChanged?.Invoke(element);
        public static void TriggerCharacterSwapped(int frontIndex, int reserveIndex) => OnCharacterSwapped?.Invoke(frontIndex, reserveIndex);
        public static void TriggerValorChant(object source) => OnValorChantTriggered?.Invoke(source);
        public static void TriggerCombatStarted() => OnCombatStarted?.Invoke();
        public static void TriggerCombatEnded() => OnCombatEnded?.Invoke();
    }
}
