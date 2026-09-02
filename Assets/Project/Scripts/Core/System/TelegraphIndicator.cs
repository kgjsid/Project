using UnityEngine;

namespace Core.System
{
    public class TelegraphIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer indicatorRenderer;
        [SerializeField] private Color startColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color endColor = new Color(1f, 0.2f, 0.2f, 0.9f);
        [SerializeField] private float baseRadius = 1f;

        private const int MAX_INDICATORS = 12;

        private SpriteRenderer[] indicators;
        private int activeCount = 1;

        private void Awake()
        {
            indicators = new SpriteRenderer[MAX_INDICATORS];
            for (int i = 0; i < MAX_INDICATORS; i++)
            {
                indicators[i] = Instantiate(indicatorRenderer, transform);
                indicators[i].enabled = false;
            }
            HideIndicator();
        }

        public void ShowIndicator()
        {
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].enabled = i < activeCount;
            }
            SetProgress(0f);
        }

        public void SetProgress(float time)
        {
            Color c = Color.Lerp(startColor, endColor, time);
            for (int i = 0; i < activeCount; i++)
            {
                indicators[i].color = c;
            }
        }

        public void HideIndicator()
        {
            indicatorRenderer.enabled = false;
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].enabled = false;
            }
        }

        public void SetRange(float range)
        {
            float scale = range / baseRadius;
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].transform.localScale = new Vector3(scale, scale, 1f);
            }
        }

        public void SetSprite(Sprite telegraphSprite)
        {
            for (int i = 0; i < indicators.Length; i++)
            {
                indicators[i].sprite = telegraphSprite;
            }
        }

        public void SetDirection(Vector2 dir, int count, float spreadAngle)
        {
            if (dir.sqrMagnitude < 0.0001f) return;

            activeCount = Mathf.Clamp(count, 1, MAX_INDICATORS);

            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float startAngle = -spreadAngle * 0.5f;
            float step = activeCount > 1 ? spreadAngle / (activeCount - 1) : 0f;

            for (int i = 0; i < activeCount; i++)
            {
                float angle = baseAngle + startAngle + step * i;
                float rad = angle * Mathf.Deg2Rad;
                indicators[i].transform.right = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
        }
    }
}