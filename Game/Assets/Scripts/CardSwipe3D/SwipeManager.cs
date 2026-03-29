using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SwipeManager : MonoBehaviour
{
    [Header("Drag")]
    public Camera mainCamera;
    public float minLocalX = 0f;
    public float maxLocalX = 1.5f;

    [Header("Swipe Timing (seconds)")]
    public float minTime = 0.2f;
    public float maxTime = 1.0f;

    [Header("Checkpoint Failsafe")]
    [Tooltip("Local X of the end checkpoint. If card passes this without the end trigger being hit, count as too fast.")]
    public float endCheckpointLocalX = 1.2f;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;

    private bool _dragging;
    private float _grabOffsetX;

    private Vector3 _startLocalPos;
    private float _swipeStartTime;
    private bool _swipeStarted;

    private bool _startSeen;
    private bool _endSeen;

    private float _screenZ;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        _startLocalPos = transform.localPosition;
    }

    private void OnMouseDown()
    {
        if (mainCamera == null) return;

        _dragging = true;

        // Cache the card's depth relative to the camera.
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        _screenZ = screenPoint.z;

        // Get mouse world position at the same screen depth as the card.
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = _screenZ;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector3 mouseLocal = transform.parent.InverseTransformPoint(mouseWorld);

        _grabOffsetX = transform.localPosition.x - mouseLocal.x;
    }

    private void OnMouseDrag()
    {
        if (!_dragging || mainCamera == null) return;

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
        _dragging = false;

        if (_swipeStarted && !_endSeen)
        {
            ShowFeedback("Bad read!", Color.yellow);
            ResetCard();
        }
    }

    private void Update()
    {
        if (!_swipeStarted || _endSeen)
            return;

        float currentX = transform.localPosition.x;

        if (_startSeen && !_endSeen && currentX > endCheckpointLocalX)
        {
            ShowFeedback("Too fast!", Color.red);
            ResetCard();
        }
    }

    public void StartSwipe()
    {
        if (_swipeStarted) return;

        _swipeStarted = true;
        _startSeen = true;
        _endSeen = false;
        _swipeStartTime = Time.time;

        if (feedbackText != null)
            feedbackText.text = "";
    }

    public void EndSwipe()
    {
        _endSeen = true;

        if (!_swipeStarted)
        {
            ShowFeedback("Insert card properly!", Color.yellow);
            ResetCard();
            return;
        }

        float swipeTime = Time.time - _swipeStartTime;

        if (swipeTime < minTime)
            ShowFeedback("Too fast!", Color.red);
        else if (swipeTime > maxTime)
            ShowFeedback("Too slow!", Color.red);
        else
            ShowFeedback("Access granted!", Color.white);

        ResetCard();
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
        if (feedbackText == null) return;

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
}
