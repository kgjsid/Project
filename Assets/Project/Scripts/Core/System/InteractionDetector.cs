using UnityEngine;

using Core.Interface;

namespace Core.System
{
    public class InteractionDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius = 1.5f;
        [SerializeField] private LayerMask interactableMask;

        private Collider2D[] results = new Collider2D[10];
        private ContactFilter2D contactFilter;

        private void Awake()
        {
            contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            contactFilter.SetLayerMask(interactableMask);
        }

        public void SetLayerMask(LayerMask mask)
        {
            interactableMask = mask;
            contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
            contactFilter.SetLayerMask(interactableMask);
        }

        public IInteractable GetClosestTarget()
        {
            int size = Physics2D.OverlapCircle(transform.position, detectRadius, contactFilter, results);

            IInteractable closest = null;
            float closestDist = float.MaxValue;

            for(int index = 0; index < size; index++)
            {
                if (!results[index].TryGetComponent(out IInteractable interactable)) continue;

                float dist = Vector2.Distance(transform.position, results[index].transform.position);
                if(dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                }
            }

            return closest;
        }
    }
}