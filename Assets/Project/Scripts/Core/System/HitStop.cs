using System.Collections;
using UnityEngine;

namespace Core.System
{
    public class HitStop : MonoBehaviour
    {
        [SerializeField] Animator animator;

        private Mover mover;
        private Coroutine freezeRoutine;

        public void Init(Mover mover)
        {
            this.mover = mover;
        }

        public void Freeze(float duration)
        {
            if (freezeRoutine != null) StopCoroutine(freezeRoutine);
            freezeRoutine = StartCoroutine(FreezeRoutine(duration));
        }

        private IEnumerator FreezeRoutine(float duration)
        {
            if (mover != null) mover.IsFrozen = true;
            if (animator != null) animator.speed = 0f;

            yield return new WaitForSeconds(duration);

            if (mover != null) mover.IsFrozen = false;
            if (animator != null) animator.speed = 1f;

            freezeRoutine = null;
        }

        private void OnDisable()
        {
            if (mover != null) mover.IsFrozen = false;
            if (animator != null) animator.speed = 1f;
            freezeRoutine = null;
        }
    }
}