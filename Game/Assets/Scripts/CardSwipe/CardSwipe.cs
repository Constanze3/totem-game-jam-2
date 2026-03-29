using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CardSwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Speed Thresholds")]
    public float minSpeed = 200f;
    public float maxSpeed = 800f;

    [Header("Slot Bounds")]
    public float slotMinX = -50f;
    public float slotMaxX = 50f;

    [Header("UI")]
    public TextMeshProUGUI feedbackText;

    private Vector2 _startPos;
    private float _velocity;
    private bool _inSlot = false;
    private RectTransform _rect;

    void Awake() => _rect = GetComponent<RectTransform>();

    public void OnBeginDrag(PointerEventData e)
    {
        _startPos = _rect.anchoredPosition;
        _inSlot = false;
        feedbackText.text = "";
    }

    public void OnDrag(PointerEventData e)
    {
        _rect.anchoredPosition += new Vector2(e.delta.x, 0);
        float clampedX = Mathf.Clamp(_rect.anchoredPosition.x, slotMinX, slotMaxX);
        _rect.anchoredPosition = new Vector2(clampedX, _rect.anchoredPosition.y);
        _velocity = Mathf.Abs(e.delta.x) / Time.deltaTime;
    }

    public void OnEndDrag(PointerEventData e)
    {
        bool completedSwipe = _rect.anchoredPosition.x >= slotMaxX - 5f;

        if (!_inSlot)
            ShowFeedback("Insert card into slot!", Color.yellow);
        else if (!completedSwipe)
            ShowFeedback("Swipe all the way through!", Color.yellow);
        else if (_velocity < minSpeed)
            ShowFeedback("Too slow!", Color.red);
        else if (_velocity > maxSpeed)
            ShowFeedback("Too fast!", Color.red);
        else
            ShowFeedback("Access granted!", Color.green);

        _rect.anchoredPosition = _startPos;
        _inSlot = false;
    }

    void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        StartCoroutine(ClearFeedback());
    }

    IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(10f);
        feedbackText.text = "";
    }

    public void EnterSlot() => _inSlot = true;
    public void ExitSlot() => _inSlot = false;
}