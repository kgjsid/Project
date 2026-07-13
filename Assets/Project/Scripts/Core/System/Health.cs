using System;
using UnityEngine;

namespace Core.System
{
    /// <summary>
    /// Health 컴포넌트
    /// 체력과 사망 시 실행할 이벤트 담당
    /// </summary>
    public class Health : MonoBehaviour
    {
        private float baseMaxHp;                    // 기본 값
        private float bonusMaxHp;                   // 추가 값(장비, 소모품)
        private float currentHp;                    // 현재 체력

        public event Action<float> OnHpChanged;     // 체력 변화 시 실행할 이벤트
        public event Action OnDie;                  // 사망 시 실행할 이벤트

        public float BaseMaxHp { get { return baseMaxHp; } set { baseMaxHp = value; RecalculateMaxHp(); } }
        public float BonusMaxHp { get { return bonusMaxHp; } set { bonusMaxHp = value; RecalculateMaxHp(); } }
        public float MaxHp { get { return baseMaxHp + bonusMaxHp; } }
        public float CurrentHp { get { return currentHp; } set { currentHp = value; } }

        public void TakeDamage(float damage)
        {
            if (IsDead()) return;

            currentHp -= damage;
            currentHp = Mathf.Max(currentHp, 0);

            OnHpChanged?.Invoke(currentHp / MaxHp);

            if (IsDead()) Die();
        }

        public bool IsDead()
        {
            return currentHp <= 0;
        }

        private void RecalculateMaxHp()
        {
            float ratio = MaxHp > 0 ? currentHp / MaxHp : 1f;
            currentHp = MaxHp * ratio;
        }

        private void Die()
        {
            OnDie?.Invoke();
        }
    }
}
