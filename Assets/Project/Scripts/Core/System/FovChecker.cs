using UnityEngine;
using System.Collections.Generic;

namespace Core.System
{
    public class FovChecker : MonoBehaviour
    {
        [SerializeField] private float viewDistance;
        [SerializeField] private float backDistanceRatio = 0.3f; 

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        private List<Transform> visibleTargets = new List<Transform>();
        private Collider2D[] colliders = new Collider2D[5];
        private ContactFilter2D contactFilter;

        private Vector2 facingDirection = Vector2.right;

        private const float ROTATION_THRESHOLD = 0.0001f;

        public float ViewDistance
        {
            get
            {
                return viewDistance;
            }
            set
            {
                viewDistance = value;
            }
        }

        public float BackDistanceRatio
        {
            get
            {
                return backDistanceRatio;
            }
            set
            {
                backDistanceRatio = value;
            }
        }

        public List<Transform> VisibleTargets
        {
            get
            {
                return visibleTargets;
            }
        }

        public LayerMask TargetMask
        {
            get
            {
                return targetMask;
            }
            set
            {
                targetMask = value;
                contactFilter.SetLayerMask(targetMask);
            }
        }

        public LayerMask ObstacleMask
        {
            get
            {
                return obstacleMask;
            }
            set
            {
                obstacleMask = value;
            }
        }

        public Vector2 FacingDirection
        {
            get
            {
                return facingDirection;
            }
        }

        private void Start()
        {
            contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;
        }

        public void SetFacingDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < ROTATION_THRESHOLD) return;
            facingDirection = direction.normalized;
        }

        public void FindVisibleTargets()
        {
            visibleTargets.Clear();

            int size = Physics2D.OverlapCircle(transform.position, viewDistance, contactFilter, colliders);

            for (int hitIndex = 0; hitIndex < size; hitIndex++)
            {
                Transform target = colliders[hitIndex].transform;
                Vector2 dirToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
                float dstToTarget = Vector2.Distance(transform.position, target.position);

                float dot = Vector2.Dot(dirToTarget, facingDirection);
                float t = (dot + 1f) * 0.5f;

                float allowedDistance = Mathf.Lerp(viewDistance * backDistanceRatio, viewDistance, t);

                if (dstToTarget <= allowedDistance)
                {

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask);
                    if (hit.collider == null)
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }
    }
}
