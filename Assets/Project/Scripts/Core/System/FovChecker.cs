using UnityEngine;
using System.Collections.Generic;

using Core.Interface;

namespace Core.System
{
    public class FovChecker : MonoBehaviour
    {
        [SerializeField] private float viewAngle;
        [SerializeField] private float viewDistance;

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        private List<Transform> visibleTargets = new List<Transform>();
        private Collider[] colliders = new Collider[5];
        private float cosAngle = 0f;

        public float ViewAngle
        {
            get
            {
                return viewAngle;
            }
            set
            {
                viewAngle = value;
            }
        }

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

        private void Start()
        {
            cosAngle = Mathf.Cos(viewAngle * Mathf.Deg2Rad);
        }

        public void FindVisibleTargets()
        {
            visibleTargets.Clear();

            int size = Physics.OverlapSphereNonAlloc(transform.position, viewDistance, colliders, targetMask);

            for (int i = 0; i < size; i++)
            {
                Transform target = colliders[i].transform;
                Vector3 dirToPlayer = (target.position - transform.position).normalized;

                if (Vector3.Dot(dirToPlayer, transform.forward) > cosAngle)
                {
                    float dstToPlayer = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        public IInteractable GetClosestTarget()
        {
            if (visibleTargets.Count == 0) return null;

            IInteractable closestInteractable = null;
            float closestDist = float.MaxValue;

            foreach(var target in visibleTargets)
            {
                if(target.TryGetComponent(out IInteractable interactable))
                {
                    float dist = Vector3.Distance(transform.position, target.position);

                    if(dist < closestDist)
                    {
                        closestDist = dist;
                        closestInteractable = interactable;
                    }
                }
            }

            return closestInteractable;
        }
    }
}
