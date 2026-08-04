using UnityEngine;

namespace Core.System
{
    public class TelegraphIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer indicatorRenderer;
        [SerializeField] private Color startColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color endColor = new Color(1f, 0.2f, 0.2f, 0.9f);
        [SerializeField] private float baseRadius = 1f;

        private void Awake()
        {
            HideIndicator();
        }

        public void ShowIndicator()
        {
            if (indicatorRenderer == null) return;

            indicatorRenderer.enabled = true;
            SetProgress(0f);
        }

        public void SetProgress(float time)
        {
            if (indicatorRenderer == null) return;

            indicatorRenderer.color = Color.Lerp(startColor, endColor, time);
        }

        public void HideIndicator()
        {
            if (indicatorRenderer == null) return;

            indicatorRenderer.enabled = false;
        }

        public void SetRange(float range)
        {
            float scale = range / baseRadius;
            indicatorRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void SetDirection(Vector2 dir)
        {
            if (indicatorRenderer == null || dir.sqrMagnitude < 0.0001f) return;
            indicatorRenderer.transform.right = dir;
        }
    }
}