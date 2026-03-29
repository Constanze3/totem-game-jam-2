using System;
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

        [SerializeField] private AudioClip popSound;
        [SerializeField] private Interactable interactable;
        private AudioSource audioSource;

        private bool gameStarted = false;

        [SerializeField] private Transform cameraTeleportTarget;
        [SerializeField] private Transform cameraTransform;

        public Person person;

        private float initialVelocity;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.clip = popSound;

            rb = GetComponent<Rigidbody>();

            initialVelocity = velocity;
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            SetHorizontalVelocity(velocity);
        }

        private void OnEnable()
        {
            interactable.OnInteractionStart += StartGame;
            interactable.OnInteractionEnd += EndGame;
        }

        private void OnDisable()
        {
            interactable.OnInteractionStart -= StartGame;
            interactable.OnInteractionEnd -= EndGame;
        }

        private void StartGame()
        {
            ResetGame();

            gameStarted = true;
            cameraTransform.position = cameraTeleportTarget.position;
            cameraTransform.LookAt(interactable.interactionCameraTarget);
        }

        private void EndGame()
        {
            gameStarted = false;
            rb.linearVelocity = Vector3.zero;
        }

        private void Update()
        {
            if (!gameStarted || hasWon)
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
            if (slicePrefabs == null || slicePrefabs.Count == 0)
                return;

            audioSource.PlayOneShot(popSound);

            GameObject randomPrefab = GetRandomSlicePrefab();

            if (randomPrefab == null)
                return;

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
            gameStarted = false;
            rb.linearVelocity = Vector3.zero;

            interactable.EndInteraction();
            person.SetRage(0);

            Debug.Log("You win!");
        }

        public void UnregisterSlice(Slice slice)
        {
            if (spawnedSlices.Contains(slice))
            {
                spawnedSlices.Remove(slice);
            }
        }

        private void ResetGame()
        {
            hasWon = false;
            frozenSliceCount = 0;
            spawnCooldownTimer = 0f;
            velocity = initialVelocity;

            for (int i = spawnedSlices.Count - 1; i >= 0; i--)
            {
                if (spawnedSlices[i] != null)
                {
                    Destroy(spawnedSlices[i].gameObject);
                }
            }

            spawnedSlices.Clear();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.SetPositionAndRotation(initialPosition, initialRotation);

            SetHorizontalVelocity(velocity);
        }
    }
}
