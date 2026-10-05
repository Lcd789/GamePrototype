using System.Collections;
using JRPG2D5.Core.EventBus;
using UnityEngine;
using UnityEngine.AI;

namespace JRPG2D5.Core.Transitions
{
    /// <summary>
    /// Reverse Transition Orchestrator: fade, restore exploration pose, safety-check, unlock input.
    /// Time travel / world map is intentionally out of POC scope.
    /// </summary>
    public class ReverseTransitionOrchestrator : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] ExplorationAnchor anchor;
        [SerializeField] Collider2D[] explorationColliders;
        [SerializeField] CanvasGroup fadeOverlay;
        [SerializeField] float fadeDuration = 0.35f;
        [SerializeField] float navMeshSampleRadius = 1.5f;
        [SerializeField] GameObject defeatedEncounterGroup;

        bool _inputsLocked;

        public bool InputsLocked => _inputsLocked;

        void Reset()
        {
            if (player == null) player = transform;
            if (anchor == null) anchor = GetComponent<ExplorationAnchor>();
        }

        public void OnEncounterTriggered()
        {
            if (player == null) return;
            if (anchor == null) anchor = player.GetComponent<ExplorationAnchor>();
            if (anchor == null) anchor = player.gameObject.AddComponent<ExplorationAnchor>();

            anchor.SavePosition(player.position);
            SetExplorationPhysics(false);
            _inputsLocked = true;
            GameEvents.TriggerCombatStarted();
        }

        public void PlayReturnToExploration()
        {
            StartCoroutine(ReturnRoutine());
        }

        IEnumerator ReturnRoutine()
        {
            _inputsLocked = true;
            yield return Fade(1f);

            if (defeatedEncounterGroup != null)
                defeatedEncounterGroup.SetActive(false);

            RestorePlayerSafely();
            yield return Fade(0f);

            SetExplorationPhysics(true);
            _inputsLocked = false;
            GameEvents.TriggerCombatEnded();
        }

        void RestorePlayerSafely()
        {
            if (player == null || anchor == null || !anchor.HasSavedPosition) return;

            Vector3 target = anchor.SavedExplorationPosition;

            if (NavMesh.SamplePosition(target, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas))
                target = navHit.position;

            RaycastHit2D hit = Physics2D.Raycast(target + Vector3.up * 0.5f, Vector2.down, 2f);
            if (hit.collider != null)
                target = hit.point;

            player.position = target;
        }

        void SetExplorationPhysics(bool enabled)
        {
            if (explorationColliders == null) return;
            for (int i = 0; i < explorationColliders.Length; i++)
            {
                if (explorationColliders[i] != null)
                    explorationColliders[i].enabled = enabled;
            }
        }

        IEnumerator Fade(float targetAlpha)
        {
            if (fadeOverlay == null) yield break;
            fadeOverlay.blocksRaycasts = true;
            float start = fadeOverlay.alpha;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadeOverlay.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
                yield return null;
            }
            fadeOverlay.alpha = targetAlpha;
            fadeOverlay.blocksRaycasts = targetAlpha > 0.01f;
        }
    }
}
