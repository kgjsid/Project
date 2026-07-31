using System.Collections;
using UnityEngine;

namespace Core.System
{
    public class WeaponAimer : MonoBehaviour
    {
        [SerializeField] private Transform weaponPivot;
        [SerializeField] private SpriteRenderer weaponRenderer;

        [SerializeField] private float swingAngle = 70f;
        [SerializeField] private float swingDuration = 0.15f;

        private Vector2 currentAimDir = Vector2.right;
        private float swingOffset = 0f;
        private Coroutine swingRoutine;

        private Equipper equipper;
        private IAttacker subscribedAttacker;

        public void Init(Equipper equipper)
        {
            this.equipper = equipper;
            equipper.OnStatsChanged += HandleWeaponChanged;
            HandleWeaponChanged();
        }

        private void OnEnable()
        {
            equipper.OnStatsChanged += HandleWeaponChanged;
            HandleWeaponChanged();
        }

        private void OnDisable()
        {
            equipper.OnStatsChanged -= HandleWeaponChanged;
            Unsubscribe();
        }

        private void HandleWeaponChanged()
        {
            Unsubscribe();
            subscribedAttacker = equipper.GetCurrentAttacker();

            bool hasWeapon = subscribedAttacker != null;
            if (weaponRenderer != null) weaponRenderer.enabled = hasWeapon;

            if (hasWeapon)
            {
                subscribedAttacker.OnAimDirectionChanged += Rotate;
                subscribedAttacker.OnAttackPerformed += PlaySwing;
            }
        }

        private void Unsubscribe()
        {
            if (subscribedAttacker != null)
            {
                subscribedAttacker.OnAimDirectionChanged -= Rotate;
                subscribedAttacker.OnAttackPerformed -= PlaySwing;  
            }
        }

        private void Rotate(Vector2 dir)
        {
            currentAimDir = dir;
            ApplyRotation();
        }

        private void ApplyRotation()
        {
            float baseAngle = Mathf.Atan2(currentAimDir.y, currentAimDir.x) * Mathf.Rad2Deg;
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, baseAngle + swingOffset);
        }

        private void PlaySwing()
        {
            if (swingRoutine != null) StopCoroutine(swingRoutine);
            swingRoutine = StartCoroutine(SwingRoutine());
        }

        private IEnumerator SwingRoutine()
        {
            float elapsed = 0f;

            while (elapsed < swingDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / swingDuration;

                // +각도에서 -각도로 훑고 지나감 (위에서 아래로 베는 느낌)
                swingOffset = Mathf.Lerp(swingAngle, -swingAngle, t);
                ApplyRotation();

                yield return null;
            }

            swingOffset = 0f;
            ApplyRotation();
            swingRoutine = null;
        }
    }
}