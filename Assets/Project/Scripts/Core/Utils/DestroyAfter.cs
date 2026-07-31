using UnityEngine;

namespace Core.Utils
{
    public class DestroyAfter : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.25f;
        private void OnEnable() 
        { 
            Destroy(gameObject, lifetime); 
        }
    }
}