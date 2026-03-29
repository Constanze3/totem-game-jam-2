using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject pointA;
        [SerializeField] private GameObject pointB;

        [SerializeField] private float velocity = 1f;
        [SerializeField] private float velocityIncreasePerFreeze = 0.5f;

        [SerializeField] private List<GameObject> slicePrefabs = new List<GameObject>();

        [SerializeField] private float spawnCooldown = 0.5f;
        private float spawnCooldownTimer = 0f;

        [SerializeField] private int slicesToWin = 10;
        private int frozenSliceCount = 0;
        private bool hasWon = false;

        private Rigidbody rb;
        private readonly List<Slice> spawnedSlices = new List<Slice>();

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            SetHorizontalVelocity(velocity);
        }

        private void Update()
        {
            if (hasWon)
                return;

            spawnCooldownTimer -= Time.deltaTime;

            MoveBetweenPoints();

            if (Input.GetKeyDown(KeyCode.Space) && spawnCooldownTimer <= 0f)
            {
                SpawnSlice();
                spawnCooldownTimer = spawnCooldown;
            }
        }

        private void MoveBetweenPoints()
        {
            if (transform.localPosition.x < pointA.transform.localPosition.x)
            {
                SetHorizontalVelocity(Mathf.Abs(velocity));
            }
            else if (transform.localPosition.x > pointB.transform.localPosition.x)
            {
                SetHorizontalVelocity(-Mathf.Abs(velocity));
            }
        }

        private void SetHorizontalVelocity(float xVelocity)
        {
            rb.linearVelocity = new Vector3(xVelocity, 0f, 0f);
        }

        private void SpawnSlice()
        {
            GameObject randomPrefab = GetRandomSlicePrefab();

            GameObject newObject = Instantiate(
                randomPrefab,
                transform.position,
                Quaternion.Euler(90f, 0f, 0f)
            );

            Slice slice = newObject.GetComponent<Slice>();

            if (slice != null)
            {
                slice.SetSpawner(this);
                spawnedSlices.Add(slice);
            }

        }

        private GameObject GetRandomSlicePrefab()
        {
            if (slicePrefabs == null || slicePrefabs.Count == 0)
                return null;

            int randomIndex = UnityEngine.Random.Range(0, slicePrefabs.Count);
            return slicePrefabs[randomIndex];
        }

        public void OnSliceFrozen(Slice slice)
        {
            if (hasWon)
                return;

            frozenSliceCount++;

            if (frozenSliceCount >= slicesToWin)
            {
                WinGame();
                return;
            }

            velocity += velocityIncreasePerFreeze;

            float direction = Mathf.Sign(rb.linearVelocity.x);
            if (direction == 0f)
                direction = 1f;

            SetHorizontalVelocity(direction * velocity);
        }

        private void WinGame()
        {
            hasWon = true;
            rb.linearVelocity = Vector3.zero;

            Debug.Log("You win!");
        }

        public void UnregisterSlice(Slice slice)
        {
            if (spawnedSlices.Contains(slice))
            {
                spawnedSlices.Remove(slice);
            }
        }
    }
}
