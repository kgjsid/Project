using Core.System.Pooling;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class DamageNumber : MonoBehaviour, IPoolable
    {
        [SerializeField] private TMP_Text text;

        [SerializeField] private float riseDistance = 1f;
        [SerializeField] private float lifeTime = 0.7f;
        [SerializeField] private Vector2 randomSpread = new Vector2(0.3f, 0.2f);

        private Coroutine fadeRoutine;
        private Color baseColor;

        private void Awake()
        {
            if (text != null) baseColor = text.color;
        }

        public void OnSpawn()
        {

        }

        public void OnDespawn()
        {
            if(fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
                fadeRoutine = null;
            }
        }

        public void Show(float damage, Vector3 worldPos)
        {
            Vector3 offset = new Vector3(
                Random.Range(-randomSpread.x, randomSpread.x),
                Random.Range(0f, randomSpread.y),
                0f);
            transform.position = worldPos + offset;

            if (text != null) text.text = Mathf.RoundToInt(damage).ToString();

            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeRoutine());
        }

        private IEnumerator FadeRoutine()
        {
            Vector3 start = transform.position;
            Vector3 end = start + Vector3.up * riseDistance;

            float elapsed = 0f;
            while (elapsed < lifeTime)
            {
                float t = elapsed / lifeTime;

                transform.position = Vector3.Lerp(start, end, t);

                if (text != null)
                {
                    Color c = baseColor;
                    c.a = 1f - t;
                    text.color = c;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            fadeRoutine = null;
            PoolManager.Instance.Return(this); 
        }
    }
}