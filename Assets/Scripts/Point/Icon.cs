using UnityEngine;

namespace PointData
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Icon : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Vector3 Scale
        {
            get
            {
                return transform.localScale;
            }
            set
            {
                transform.localScale = value;
            }
        }

        public SpriteRenderer spriteRenderer
        {
            get
            {
                return _spriteRenderer;
            }
        }
    }
}