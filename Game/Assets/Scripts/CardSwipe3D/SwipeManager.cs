using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwipeManager : MonoBehaviour
{
    public Person person;

    [Header("Drag")]
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float minLocalX = 0f;

    [SerializeField]
    private float maxLocalX = 1.5f;

    [Header("Swipe Timing (seconds)")]
    [SerializeField]
    private float minTime = 0.2f;

    [SerializeField]
    private float maxTime = 1.0f;

    [Header("Checkpoint Failsafe")]
    [Tooltip(
        "Local X of the end checkpoint. If card passes this without the end trigger being hit, count as too fast."
    )]
    [SerializeField]
    private float endCheckpointLocalX = 1.2f;

    [Header("Feedback")]
    [SerializeField]
    private TextMeshProUGUI feedbackText;

    [Header("Audio")]
    [SerializeField]
    private AudioClip wrongsSound;

    [SerializeField]
    private AudioClip correctSound;

    [SerializeField]
    private AudioClip pickUpSound;

    [Header("Interaction")]
    [SerializeField]
    private Interactable interactable;

    private bool _dragging;
    private float _grabOffsetX;

    private Vector3 _startLocalPos;
    private float _swipeStartTime;
    private bool _swipeStarted;

    private bool _startSeen;
    private bool _endSeen;

    private float _screenZ;
    private AudioSource audioSource;

    private bool gameStarted = false;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        _startLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.OnInteractionStart += StartGame;
            interactable.OnInteractionEnd += EndGame;
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.OnInteractionStart -= StartGame;
            interactable.OnInteractionEnd -= EndGame;
        }
    }

    private void StartGame()
    {
        ResetCard();
        ClearFeedbackImmediate();

        gameStarted = true;
    }

    private void EndGame()
    {
        gameStarted = false;
        _dragging = false;
        ResetCard();
    }

    private void OnMouseDown()
    {
        if (!gameStarted || mainCamera == null)
            return;

        _dragging = true;
        audioSource.PlayOneShot(pickUpSound);

        Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        _screenZ = screenPoint.z;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = _screenZ;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector3 mouseLocal = transform.parent.InverseTransformPoint(mouseWorld);

        _grabOffsetX = transform.localPosition.x - mouseLocal.x;
    }

    private void OnMouseDrag()
    {
        if (!gameStarted || !_dragging || mainCamera == null)
            return;

        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = _screenZ;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector3 mouseLocal = transform.parent.InverseTransformPoint(mouseWorld);

        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Clamp(mouseLocal.x + _grabOffsetX, minLocalX, maxLocalX);
        transform.localPosition = localPos;
    }

    private void OnMouseUp()
    {
        if (!gameStarted)
            return;

        _dragging = false;

        if (_swipeStarted && !_endSeen)
        {
            ShowFeedback("Bad read!", Color.yellow);
            audioSource.PlayOneShot(wrongsSound);
            ResetCard();
        }
    }

    private void Update()
    {
        if (!gameStarted || !_swipeStarted || _endSeen)
            return;

        float currentX = transform.localPosition.x;

        if (_startSeen && !_endSeen && currentX > endCheckpointLocalX)
        {
            ShowFeedback("Too fast!", Color.red);
            audioSource.PlayOneShot(wrongsSound);
            ResetCard();
        }
    }

    public void StartSwipe()
    {
        if (!gameStarted || _swipeStarted)
            return;

        _swipeStarted = true;
        _startSeen = true;
        _endSeen = false;
        _swipeStartTime = Time.time;

        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void EndSwipe()
    {
        if (!gameStarted)
            return;

        _endSeen = true;

        if (!_swipeStarted)
        {
            ShowFeedback("Insert card properly!", Color.yellow);
            audioSource.PlayOneShot(wrongsSound);
            ResetCard();
            return;
        }

        float swipeTime = Time.time - _swipeStartTime;

        if (swipeTime < minTime)
        {
            ShowFeedback("Too fast!", Color.red);
            audioSource.PlayOneShot(wrongsSound);
            ResetCard();
        }
        else if (swipeTime > maxTime)
        {
            ShowFeedback("Too slow!", Color.red);
            audioSource.PlayOneShot(wrongsSound);
            ResetCard();
        }
        else
        {
            ShowFeedback("Access granted!", Color.white);
            audioSource.PlayOneShot(correctSound);
            person.SetRage(0);

            gameStarted = false;
            interactable.EndInteraction();
            ResetCard();
        }
    }

    public void ResetCard()
    {
        _dragging = false;
        _swipeStarted = false;
        _startSeen = false;
        _endSeen = false;
        transform.localPosition = _startLocalPos;
    }

    private void ShowFeedback(string msg, Color color)
    {
        if (feedbackText == null)
            return;

        feedbackText.text = msg;
        feedbackText.color = color;

        StopAllCoroutines();
        StartCoroutine(ClearFeedback());
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(2f);

        if (feedbackText != null)
            feedbackText.text = "";
    }

    private void ClearFeedbackImmediate()
    {
        StopAllCoroutines();

        if (feedbackText != null)
            feedbackText.text = "";
    }
}
