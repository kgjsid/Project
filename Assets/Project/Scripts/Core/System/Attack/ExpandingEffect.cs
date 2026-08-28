using UnityEngine;

namespace Core.System
{
    public class ExpandingEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float duration = 0.3f;   
        [SerializeField] private float startScale = 0.2f; 
        [SerializeField]
        private AnimationCurve scaleCurve
            = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private float timer;
        private float targetScale;
        private Color baseColor;

        private void Awake()
        {
            if(spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(float radius, Color color)
        {
            targetScale = radius * 2f;
            baseColor = color;
            timer = 0f;

            transform.localScale = Vector3.one * (targetScale * startScale);
            spriteRenderer.color = baseColor;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            if (t >= 1f)
            {
                Destroy(gameObject);
                return;
            }

            float scaleT = Mathf.Lerp(startScale, 1f, scaleCurve.Evaluate(t));
            transform.localScale = Vector3.one * (targetScale * scaleT);

            Color c = baseColor;
            c.a = baseColor.a * (1f - t);
            spriteRenderer.color = c;
        }
    }
}