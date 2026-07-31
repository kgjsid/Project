using System.Collections;
using UnityEngine;

namespace Core.System
{
    public class HitFlash : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color hitColor = new Color(255 / 255f, 120 / 255f, 120 / 255f);
        [SerializeField] private float flashDuration = 0.12f;

        private Health health;
        private Color originColor;
        private Coroutine flashRoutine;
        private WaitForSeconds flashSecond;

        public void Init(Health health)
        {
            flashSecond = new WaitForSeconds(flashDuration);
            this.health = health;
            health.OnDamaged += HandleDamaged;
        }

        private void Awake()
        {
            if (spriteRenderer == null) return;

            originColor = spriteRenderer.color;
        }

        private void OnDisable()
        {
            if(health != null)
            {
                health.OnDamaged -= HandleDamaged;
            }
        }

        private void HandleDamaged(Vector2 hitDirection, float knockbackForce)
        {
            if (spriteRenderer == null) return;

            if(flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            spriteRenderer.color = hitColor;
            yield return flashSecond;
            spriteRenderer.color = originColor;
            flashRoutine = null;
        }
    }
}