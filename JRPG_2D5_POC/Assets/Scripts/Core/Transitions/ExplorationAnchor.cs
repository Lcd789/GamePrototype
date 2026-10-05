using UnityEngine;

namespace JRPG2D5.Core.Transitions
{
    /// <summary>
    /// Return-to-Origin anchor. Capture exploration position before combat, restore after victory.
    /// </summary>
    public class ExplorationAnchor : MonoBehaviour
    {
        public Vector3 SavedExplorationPosition { get; private set; }
        public bool HasSavedPosition { get; private set; }

        public void SavePosition(Vector3 currentPos)
        {
            SavedExplorationPosition = currentPos;
            HasSavedPosition = true;
        }

        public void RestorePosition(GameObject player)
        {
            if (player == null || !HasSavedPosition) return;
            player.transform.position = SavedExplorationPosition;
        }
    }
}
