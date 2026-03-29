using System;
using UnityEngine;

namespace Game
{
    public class SliceKiller : MonoBehaviour
    {
        [SerializeField] private float deathTimer = 1.5f;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("object fell");
            if (other.gameObject.GetComponent<Slice>() != null)
            {
                Destroy(other.gameObject, deathTimer);
            }
        }
    }
}
