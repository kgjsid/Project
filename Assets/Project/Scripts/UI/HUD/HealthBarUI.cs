using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Core.System;

namespace UI.HUD
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthText;

        private Health targetHealth;

        private const float INITIAL_RATIO = 1f;

        private void OnDestroy()
        {
            if (targetHealth != null)
            {
                targetHealth.OnHpChanged -= HandleHpChanged;
                targetHealth = null;
            }
        }

        public void SetTarget(Health health)
        {
            if (targetHealth != null) targetHealth.OnHpChanged -= HandleHpChanged;

            targetHealth = health;
            targetHealth.OnHpChanged += HandleHpChanged;

            Refresh(targetHealth.MaxHp > 0f ? targetHealth.CurrentHp / targetHealth.MaxHp : INITIAL_RATIO);
        }

        private void HandleHpChanged(float ratio)
        {
            Refresh(ratio);
        }

        private void Refresh(float ratio)
        {
            float max = targetHealth.MaxHp;

            healthSlider.value = max > 0f ? targetHealth.CurrentHp / max : 0f;
            healthText.text = $"{Mathf.CeilToInt(targetHealth.CurrentHp)} / {Mathf.CeilToInt(max)}";
        }
    }
}