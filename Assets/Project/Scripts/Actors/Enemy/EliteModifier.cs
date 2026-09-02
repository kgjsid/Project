using UnityEngine;

namespace Actors.Enemy
{
    public class EliteModifier : MonoBehaviour
    {
        [SerializeField] private float scaleMultiplier = 1.3f;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private bool useAura = true;
        [SerializeField] private Sprite auraSprite;
        [SerializeField] private Color auraColor = new Color(1f, 0.15f, 0.1f, 0.5f);
        [SerializeField] private float auraScale = 1.4f;
        [SerializeField] private int auraSortingOffset = -1;
        [SerializeField] private float pulseAmplitude = 0.08f;
        [SerializeField] private float pulseSpeed = 3f;

        [SerializeField] private bool useColor = false;
        [SerializeField] private Color baseColor = new Color(1f, 0.6f, 0.6f, 1f);

        private Transform auraTransform;
        private float auraBaseScale;

        private void Start()
        {
            transform.localScale *= scaleMultiplier;

            if (useColor && spriteRenderer != null)
                spriteRenderer.color = baseColor;

            if (useAura && auraSprite != null)
                CreateAura();
        }

        private void CreateAura()
        {
            GameObject auraObject = new GameObject("EliteAura");
            auraObject.transform.SetParent(transform);
            auraObject.transform.localPosition = Vector3.zero;
            auraObject.transform.localRotation = Quaternion.identity;

            SpriteRenderer auraRenderer = auraObject.AddComponent<SpriteRenderer>();
            auraRenderer.sprite = auraSprite;
            auraRenderer.color = auraColor;

            if (spriteRenderer != null)
            {
                auraRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                auraRenderer.sortingOrder = spriteRenderer.sortingOrder + auraSortingOffset;
            }

            auraTransform = auraObject.transform;
            auraBaseScale = auraScale;
            auraTransform.localScale = Vector3.one * auraBaseScale;
        }

        private void Update()
        {
            if (auraTransform == null) return;

            float pulse = auraBaseScale + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            auraTransform.localScale = Vector3.one * pulse;
        }
    }
}