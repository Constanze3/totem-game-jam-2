using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class Slice : MonoBehaviour
    {
        private Rigidbody rb;
        private FoodSpawner spawner;

        private Vector3 lastPosition;
        private float stillTimer = 0f;

        [SerializeField]
        private float movementThreshold = 0.01f;

        [SerializeField]
        private float freezeTimer = 2f;

        private bool hasFrozen = false;

        [SerializeField]
        private AudioClip hitClip;
        private AudioSource audioSource;

        private bool hasHitGround = false;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody>();
            lastPosition = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasHitGround)
                return;
            hasHitGround = true;
            audioSource.PlayOneShot(hitClip);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("FoodCube"))
            {
                spawner.inFoodCubeCount += 1;
                Debug.Log("FoodCube Enter " + spawner.inFoodCubeCount);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("FoodCube"))
            {
                spawner.inFoodCubeCount -= 1;
                Debug.Log("FoodCube Exit " + spawner.inFoodCubeCount);
            }
        }

        public void SetSpawner(FoodSpawner foodSpawner)
        {
            spawner = foodSpawner;
        }

        private void Update()
        {
            if (rb == null || hasFrozen)
                return;

            float distanceMoved = Vector3.Distance(transform.position, lastPosition);

            if (distanceMoved < movementThreshold)
            {
                stillTimer += Time.deltaTime;

                if (stillTimer >= freezeTimer)
                {
                    Freeze();
                }
            }
            else
            {
                stillTimer = 0f;
                lastPosition = transform.position;
            }
        }

        private void Freeze()
        {
            hasFrozen = true;

            rb.isKinematic = true;
            rb.useGravity = false;

            if (spawner != null)
            {
                spawner.OnSliceFrozen(this);
            }
        }

        private void OnDestroy()
        {
            if (spawner != null)
            {
                spawner.UnregisterSlice(this);
            }
        }
    }
}
