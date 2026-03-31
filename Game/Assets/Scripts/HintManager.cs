using UnityEngine;

public class HintManager : MonoBehaviour
{
    public GameObject hintObject;
    public TMPro.TMP_Text text;

    public bool firstNonDefaultHintShown = false;

    public void ShowHint(string hintText)
    {
        text.text = hintText;
        hintObject.SetActive(true);
        firstNonDefaultHintShown = true;
    }

    public void HideHint()
    {
        if (firstNonDefaultHintShown)
        {
            hintObject.SetActive(false);
        }
    }
}
