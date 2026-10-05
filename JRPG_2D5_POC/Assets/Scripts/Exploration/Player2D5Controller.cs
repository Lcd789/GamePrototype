using UnityEngine;

namespace JRPG2D5.Exploration
{
    /// <summary>
    /// Y-axis sorting for 2.5D walkable strips. Movement input is wired later with Input System.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Player2D5Controller : MonoBehaviour
    {
        [SerializeField] int sortingScale = 100;
        [SerializeField] int sortingOffset;
        SpriteRenderer _sprite;

        void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (_sprite == null) return;
            _sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingScale) + sortingOffset;
        }
    }
}
