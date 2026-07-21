using UnityEngine;

namespace Core.System
{
    public class WeaponAimer : MonoBehaviour
    {
        [SerializeField] private Attacker attacker;
        [SerializeField] private Transform weaponPivot;

        private void OnEnable()
        {
            attacker.OnAimDirectionChanged += Rotate;
        }

        private void OnDisable()
        {
            attacker.OnAimDirectionChanged -= Rotate;
        }

        private void Rotate(Vector2 dir)
        {
            weaponPivot.right = dir;
        }
    }
}