using UnityEngine;

namespace Core.System
{
    public class WeaponAimer : MonoBehaviour
    {
        [SerializeField] private Equipper equipper;
        [SerializeField] private Transform weaponPivot;

        private IAttacker subscribedAttacker;

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
            if (subscribedAttacker != null)
            {
                subscribedAttacker.OnAimDirectionChanged += Rotate;
            }
        }

        private void Unsubscribe()
        {
            if (subscribedAttacker != null)
                subscribedAttacker.OnAimDirectionChanged -= Rotate;
        }

        private void Rotate(Vector2 dir)
        {
            weaponPivot.right = dir;
        }
    }
}