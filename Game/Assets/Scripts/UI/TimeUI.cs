using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            timeText.text = GameManager.Instance.GetFormattedTime();
        }
    }
}
