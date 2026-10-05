using UnityEngine;
using JRPG2D5.Enums;

namespace JRPG2D5.Data
{
    [CreateAssetMenu(fileName = "NewStance", menuName = "JRPG2D5/Data/Stance Data")]
    public class StanceData : ScriptableObject
    {
        public string stanceName = "Fire Zone";
        public Element boostedElement = Element.Fire;
        public Element weakenedElement = Element.Water;

        [Range(1.0f, 2.0f)]
        public float damageMultiplier = 1.5f;

        [Range(1.0f, 3.0f)]
        public float afFillMultiplier = 2.0f;
    }
}
